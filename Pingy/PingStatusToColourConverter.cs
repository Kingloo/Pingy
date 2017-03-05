using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Pingy
{
    [ValueConversion(typeof(Ping.PingStatus), typeof(Brush))]
    public class PingStatusToColourConverter : IValueConverter
    {
        public Brush None { get; set; }
        public Brush Updating { get; set; }
        public Brush Success { get; set; }
        public Brush Failure { get; set; }
        public Brush DnsResolutionError { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Ping.PingStatus pingStatus = (Ping.PingStatus)value;

            switch (pingStatus)
            {
                case Ping.PingStatus.None:
                    return None;
                case Ping.PingStatus.Updating:
                    return Updating;
                case Ping.PingStatus.Success:
                    return Success;
                case Ping.PingStatus.Failure:
                    return Failure;
                case Ping.PingStatus.DnsResolutionError:
                    return DnsResolutionError;
                default:
                    return None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Ping.PingStatus.None;
        }
    }
}
