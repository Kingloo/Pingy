using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pingy.Converters
{
    [ValueConversion(typeof(PingStatus), typeof(Brush))]
    public class PingStatusToColorConverter : IValueConverter
    {
        public Brush None { get; set; }
        public Brush Updating { get; set; }
        public Brush Success { get; set; }
        public Brush DnsResolutionError { get; set; }
        public Brush IPv6GatewayMissing { get; set; }
        public Brush Failure { get; set; }
        public Brush Cancelled { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PingStatus status = (PingStatus)value;

            return status switch
            {
                PingStatus.None => None,
                PingStatus.Updating => Updating,
                PingStatus.Success => Success,
                PingStatus.DnsResolutionError => DnsResolutionError,
                PingStatus.IPv6GatewayMissing => IPv6GatewayMissing,
                PingStatus.Failure => Failure,
                PingStatus.Cancelled => Cancelled,
                _ => None
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("conversion from Brush to PingStatus is not supported");
    }
}