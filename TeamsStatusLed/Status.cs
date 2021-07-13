using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace TeamsStatusLed
{
    internal class Status
    {
        #region Public static methods

        public static Status GetStatus(string status)
        {
            Status result;

            if (statuses.TryGetValue(status, out Status existingStatus))
            {
                result = existingStatus;
            }
            else
            {
                result = new Status();

                switch (status)
                {
                    case "Available":
                        result.FriendlyName = "Available";
                        result.Color = Color.Green;
                        break;
                    case "OnThePhone":
                        result.FriendlyName = "In a call";
                        result.Color = Color.Red;
                        break;
                    case "Presenting":
                        result.FriendlyName = "Presenting";
                        result.Color = Color.Red;
                        break;
                    case "InAMeeting":
                        result.FriendlyName = "In a meeting";
                        result.Color = bool.TryParse(ConfigurationManager.AppSettings["TreatMeetingAsBusy"], out bool parsedValue) && parsedValue ? Color.Red : Color.Green;
                        break;
                    case "Busy":
                        result.FriendlyName = "Busy";
                        result.Color = Color.Red;
                        break;
                    case "DoNotDisturb":
                        result.FriendlyName = "Do not disturb";
                        result.Color = Color.Red;
                        break;
                    case "BeRightBack":
                        result.FriendlyName = "Be right back";
                        result.Color = Color.Yellow;
                        break;
                    case "Away":
                        result.FriendlyName = "Away";
                        result.Color = Color.Yellow;
                        break;
                    case "Offline":
                        result.FriendlyName = "Offline";
                        result.Color = Color.Gray;
                        break;
                    case "closed":
                        result.FriendlyName = "Not running";
                        result.Color = Color.Gray;
                        break;
                    default:
                        result.FriendlyName = $"Unknown ({status})";
                        result.Color = Color.Gray;
                        break;
                }
            }

            return result;
        }

        #endregion

        #region Public properties

        public string FriendlyName { get; private set; }

        public Icon Icon => Color switch
        {
            Color.Red => RedIcon,
            Color.Green => GreenIcon,
            Color.Yellow => YellowIcon,
            Color.Gray => GrayIcon,
            _ => GrayIcon
        };


        public Color Color { get; private set; }

        #endregion

        #region Private static fields

        private static Dictionary<string, Status> statuses = new Dictionary<string, Status>();

        private static readonly Icon RedIcon = Icon.FromHandle(Resources.red.GetHicon());
        private static readonly Icon GreenIcon = Icon.FromHandle(Resources.green.GetHicon());
        private static readonly Icon YellowIcon = Icon.FromHandle(Resources.yellow.GetHicon());
        private static readonly Icon GrayIcon = Icon.FromHandle(Resources.gray.GetHicon());

        #endregion
    }

    #region Color enum

    internal enum Color
    {
        Red,
        Green,
        Yellow,
        Gray
    }

    #endregion
}
