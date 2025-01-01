using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Pingy.Model;

namespace Pingy.Converters
{
	[ValueConversion(typeof(PingStatus), typeof(Brush))]
	public sealed class PingStatusToColorConverter : IValueConverter
	{
		public Brush None { get; set; } = Brushes.White;
		public Brush Updating { get; set; } = Brushes.White;
		public Brush Success { get; set; } = Brushes.White;
		public Brush DnsResolutionError { get; set; } = Brushes.White;
		public Brush IPv6GatewayMissing { get; set; } = Brushes.White;
		public Brush Failure { get; set; } = Brushes.White;
		public Brush Cancelled { get; set; } = Brushes.White;

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
