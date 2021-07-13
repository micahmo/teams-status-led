using System;
using System.Configuration;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamsStatusLed
{
    internal class ArduinoUpdater : IUpdater
    {
        #region Constructor

        public ArduinoUpdater(ToolStripMenuItem toolStripMenuItem)
        {
            _arduinoStatusToolStripMenuItem = toolStripMenuItem;
            _arduinoStatusToolStripMenuItem.Click += (_, __) =>
            {
                MessageBox.Show(errorString);
            };

            // Start main loop
            Task.Run(WriteLoop);
        }

        #endregion

        #region Private methods

        private void WriteLoop()
        {
            while (true)
            {
                string comPort = ConfigurationManager.AppSettings["ComPort"];
                int baudRate = int.TryParse(ConfigurationManager.AppSettings["BaudRate"], out int parsedBaudRate) ? parsedBaudRate : 9600;

                try
                {
                    var port = new SerialPort(comPort, baudRate);
                    port.Open();

                    while (comPort == ConfigurationManager.AppSettings["ComPort"])
                    {
                        _arduinoStatusToolStripMenuItem.Text = $"'{valueToWrite}' -> {comPort}";
                        _arduinoStatusToolStripMenuItem.Enabled = false;
                        port.Write(valueToWrite);
                        Thread.Sleep((int)TimeSpan.FromSeconds(.5).TotalMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    _arduinoStatusToolStripMenuItem.Text = $"Error communicating with {comPort} (click)";
                    _arduinoStatusToolStripMenuItem.Enabled = true;
                    errorString = $"Error communicating with Arduino on COM port {comPort} at baud rate {baudRate}: {ex}";
                }
                Thread.Sleep((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
            }
        }

        #endregion

        public void Update(Status status)
        {
            valueToWrite = status.Color == Color.Red ? "1" : "0";
        }

        private string valueToWrite = "0";
        private readonly ToolStripMenuItem _arduinoStatusToolStripMenuItem;
        private string errorString;
    }
}
