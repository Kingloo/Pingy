using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Pingy.Model
{
    public abstract class PingBase : INotifyPropertyChanged
    {
        private const int timeout = 2500;

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;

            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
        
        #region Properties
        private string _displayName = string.Empty;
        public string DisplayName
        {
            get => _displayName;
            protected set => SetProperty(ref _displayName, value, nameof(DisplayName));
        }

        public string DisplayText => Status.ToString();

        private PingStatus _status = PingStatus.None;
        public PingStatus Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, value, nameof(Status));

                RaisePropertyChanged(nameof(DisplayText));
            }
        }

        private IPAddress _address = IPAddress.None;
        public IPAddress Address
        {
            get => _address;
            protected set => SetProperty(ref _address, value, nameof(Address));
        }
        
        private Int64 _roundtripTime = 0;
        public Int64 RoundtripTime
        {
            get => _roundtripTime;
            protected set => SetProperty(ref _roundtripTime, value, nameof(RoundtripTime));
        }
        #endregion

        public static bool TryCreate(string line, out PingBase? pingBase)
        {
            if (IPAddress.TryParse(line, out IPAddress address))
            {
                pingBase = new IP(address);
                return true;
            }
            else if (Uri.TryCreate(string.Format(CultureInfo.CurrentCulture, "http://{0}", line), UriKind.Absolute, out Uri? uri))
            {
                IPHostEntry hostEntry = new IPHostEntry
                {
                    HostName = uri.DnsSafeHost
                };

                pingBase = new Domain(hostEntry);
                return true;
            }

            pingBase = null;
            return false;
        }

        public Task PingAsync() => PingAsync(CancellationToken.None);

        public virtual async Task PingAsync(CancellationToken token)
        {
            Status = PingStatus.Updating;

            PingReply? reply = null;

            try
            {
                Task<PingReply?> task = Task.Run(() => PingIPAddressAsync(Address), token);

                reply = await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                Status = PingStatus.Cancelled;
            }

            ParsePingReply(reply);

            RoundtripTime = (reply is PingReply) ? reply.RoundtripTime : -1;
        }
        
        protected static async Task<PingReply?> PingIPAddressAsync(IPAddress ip)
        {
            try
            {
                Ping ping = new Ping();

                return await ping.SendPingAsync(ip, timeout).ConfigureAwait(false);
            }
            catch (PingException)
            {
                return null;
            }
        }

        protected virtual void ParsePingReply(PingReply? reply)
        {
            if (reply is PingReply)
            {
                Status = (reply.Status == IPStatus.Success)
                    ? PingStatus.Success
                    : PingStatus.Failure;
            }
            else
            {
                Status = PingStatus.Failure;
            }
        }
    }
}