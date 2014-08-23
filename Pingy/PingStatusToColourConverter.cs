using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Pingy
{
    [ValueConversion(typeof(Ping.PingStatus), typeof(Brush))]
    class PingStatusToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush toReturn = Brushes.White;

            if (value == null)
            {
                toReturn = Brushes.LightBlue;
            }
            else
            {
                Ping.PingStatus pingStatus = (Ping.PingStatus)value;
                
                switch (pingStatus)
                {
                    case Ping.PingStatus.None:
                        toReturn = (Brush)Application.Current.Resources["PingStatusNone"];
                        break;
                    case Ping.PingStatus.Updating:
                        toReturn = (Brush)Application.Current.Resources["PingStatusUpdating"];
                        break;
                    case Ping.PingStatus.Success:
                        toReturn = (Brush)Application.Current.Resources["PingStatusSuccess"];
                        break;
                    case Ping.PingStatus.Failure:
                        toReturn = (Brush)Application.Current.Resources["PingStatusFailure"];
                        break;
                    case Ping.PingStatus.DnsResolutionError:
                        toReturn = (Brush)Application.Current.Resources["PingStatusDnsResolutionError"];
                        break;
                    default:
                        toReturn = Brushes.White;
                        break;
                }
            }

            return toReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Ping.PingStatus.None;
        }
    }
}
