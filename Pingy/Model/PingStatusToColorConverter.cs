﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pingy.Model
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

            switch (status)
            {
                case PingStatus.None:
                    return None;
                case PingStatus.Updating:
                    return Updating;
                case PingStatus.Success:
                    return Success;
                case PingStatus.DnsResolutionError:
                    return DnsResolutionError;
                case PingStatus.IPv6GatewayMissing:
                    return IPv6GatewayMissing;
                case PingStatus.Failure:
                    return Failure;
                case PingStatus.Cancelled:
                    return Cancelled;
                default:
                    return None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("conversion from Brush to PingStatus is not supported");
    }
}