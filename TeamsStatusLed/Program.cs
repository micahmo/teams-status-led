using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamsStatusLed
{
    class Program
    {
        #region Main

        static void Main()
        {
            Task.Run(StartMainLoop);

            InitSystemTray();

            // This is needed to fully support the Notify Icon / WinForms functionality.
            Application.Run();
        }

        #endregion

        #region Methods

        private static void InitSystemTray()
        {
            ToolStripMenuItem exit = new ToolStripMenuItem {Text = "Exit"};
            exit.Click += (_, __) =>
            {
                _notifyIcon.Visible = false;
                Environment.Exit(0);
            };

            ToolStripMenuItem settings = new ToolStripMenuItem { Text = "Settings" };
            settings.Click += (_, __) =>
            {
                new Process {StartInfo = new ProcessStartInfo($"{new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath}.config") {UseShellExecute = true}}.Start();
            };

            ToolStripMenuItem restart = new ToolStripMenuItem {Text = "Restart"};
            restart.Click += (_, __) =>
            {
                new Process { StartInfo = new ProcessStartInfo(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath.Replace("dll", "exe")) { UseShellExecute = true } }.Start();
                _notifyIcon.Visible = false;
                Environment.Exit(0);
            };

            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "Startup", "TeamsStatusLed.url");
            string executablePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath.Replace("dll", "exe");
            ToolStripMenuItem startAutomatically = new ToolStripMenuItem {Text = "Start automatically"};
            startAutomatically.Click += (_, __) =>
            {
                startAutomatically.Checked = !startAutomatically.Checked;

                if (startAutomatically.Checked && !File.Exists(shortcutPath))
                {
                    using StreamWriter writer = new StreamWriter(shortcutPath);
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine($"URL=file:///{executablePath}");
                    writer.WriteLine("IconIndex=0");
                }
                else if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
            };

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(CurrentStatus);
            contextMenuStrip.Items.Add(ArduinoStatus);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(startAutomatically);
            contextMenuStrip.Items.Add(settings);
            contextMenuStrip.Items.Add(restart);
            contextMenuStrip.Items.Add(exit);

            _notifyIcon = new NotifyIcon
            {
                Text = "Teams Status LED",
                ContextMenuStrip = contextMenuStrip,
                Icon = new Icon(SystemIcons.Application, 40, 40),
                Visible = true
            };

            // Have to do this after the menu is created
            startAutomatically.Checked = File.Exists(shortcutPath);
        }

        private static void StartMainLoop()
        {
            long lastLength = 0;

            while (true)
            {
                try
                {
                    using FileStream file = File.Open(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Microsoft\Teams\logs.txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    file.Seek(lastLength - file.Length, SeekOrigin.End);

                    using StreamReader streamReader = new StreamReader(file);

                    string text = streamReader.ReadToEnd();

                    foreach (string line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.Contains("StatusIndicatorStateService: emit"))
                        {
                            if (line.Trim().Split().LastOrDefault() is { } newStatus)
                            {
                                // NewActivity is not really a status change
                                if (newStatus == "NewActivity") continue;

                                ApplyStatus(Status.GetStatus(newStatus));
                            }
                        }
                        else if (line.Contains("StatusIndicatorStateService: Added"))
                        {
                            string newStatus = null;

                            bool next = false;
                            foreach (string word in line.Trim().Split())
                            {
                                if (next)
                                {
                                    newStatus = word.Trim();
                                    break;
                                }
                                else if (word == "Added")
                                {
                                    next = true;
                                }
                            }

                            if (newStatus == "NewActivity") continue;

                            // If we go to InAMeeting when we're already in a "red" state, ignore it.
                            if (newStatus == "InAMeeting" && _currentStatus.Color == Color.Red) continue;

                            if (!string.IsNullOrEmpty(newStatus))
                            {
                                ApplyStatus(Status.GetStatus(newStatus));
                            }
                        }
                        else if (line.Contains("Main window closed"))
                        {
                            ApplyStatus(Status.GetStatus("closed"));
                        }
                    }

                    lastLength = file.Length;

                    Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                }
                catch
                {
                    // Live. Die. Repeat.
                }
            }
        }

        private static void ApplyStatus(Status status)
        {
            _currentStatus = status;
            CurrentStatus.Text = $"Teams Status: {status.FriendlyName}";
            _notifyIcon.Icon = status.Icon;
            _notifyIcon.Text = $"Teams Status: {status.FriendlyName}";
            Updaters.ForEach(u => u.Update(status));
        }

        #endregion

        #region Private fields

        private static readonly ToolStripMenuItem CurrentStatus = new ToolStripMenuItem
        {
            Enabled = false,
            Text = "Teams Status: Unknown"
        };

        private static readonly ToolStripMenuItem ArduinoStatus = new ToolStripMenuItem
        {
            Enabled = false
        };

        private static NotifyIcon _notifyIcon;

        private static readonly List<IUpdater> Updaters = new List<IUpdater> {new ArduinoUpdater(ArduinoStatus)};

        private static Status _currentStatus;

        #endregion
    }
}
