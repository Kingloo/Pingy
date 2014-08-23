using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Pingy
{
    [ValueConversion(typeof(Ping.PingStatus), typeof(Brush))]
    class PingStatusToColourConverter : IValueConverter
    {
        private Brush _none = Brushes.White;
        public Brush None
        {
            get { return this._none; }
            set { this._none = value; }
        }

        private Brush _updating = Brushes.White;
        public Brush Updating
        {
            get { return this._updating; }
            set { this._updating = value; }
        }

        private Brush _success = Brushes.White;
        public Brush Success
        {
            get { return this._success; }
            set { this._success = value; }
        }

        private Brush _failure = Brushes.White;
        public Brush Failure
        {
            get { return this._failure; }
            set { this._failure = value; }
        }

        private Brush _dnsResolutionError = Brushes.White;
        public Brush DnsResolutionError
        {
            get { return this._dnsResolutionError; }
            set { this._dnsResolutionError = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush toReturn = this.None;

            if (value == null)
            {
                return this.None;
            }
            else
            {
                Ping.PingStatus pingStatus = (Ping.PingStatus)value;
                
                switch (pingStatus)
                {
                    case Ping.PingStatus.None:
                        toReturn = this.None;
                        break;
                    case Ping.PingStatus.Updating:
                        toReturn = this.Updating;
                        break;
                    case Ping.PingStatus.Success:
                        toReturn = this.Success;
                        break;
                    case Ping.PingStatus.Failure:
                        toReturn = this.Failure;
                        break;
                    case Ping.PingStatus.DnsResolutionError:
                        toReturn = this.DnsResolutionError;
                        break;
                    default:
                        toReturn = this.None;
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
