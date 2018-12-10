using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pingy.Model
{
    public abstract class PingBase : IPingable, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

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
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        public static bool TryCreate(string line, out PingBase pingBase)
        {
            if (IPAddress.TryParse(line, out IPAddress address))
            {
                switch (address.AddressFamily)
                {
                    case AddressFamily.InterNetwork: // IPv4
                        pingBase = new IPv4(address);
                        return true;
                    case AddressFamily.InterNetworkV6: // IPv6
                        pingBase = new IPv6(address);
                        return true;
                    default:
                        pingBase = null;
                        return false;
                }
            }
            else if (Uri.TryCreate(string.Format(CultureInfo.CurrentCulture, "http://{0}", line), UriKind.Absolute, out Uri uri))
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

            PingReply reply = default;

            try
            {
                Task<PingReply> task = Task.Run(() => PingIPAddressAsync(Address));

                while (!task.IsCompleted)
                {
                    await Task.Delay(250).ConfigureAwait(false);

                    token.ThrowIfCancellationRequested();
                }
                
                reply = await task.ConfigureAwait(false);
                
                ParsePingReply(reply);
            }
            catch (OperationCanceledException)
            {
                Status = PingStatus.Cancelled;
            }
            
            RoundtripTime = reply?.RoundtripTime ?? 0;
        }
        
        protected static async Task<PingReply> PingIPAddressAsync(IPAddress ip)
        {
            try
            {
                Ping ping = new Ping();

                return await ping.SendPingAsync(ip, 2000).ConfigureAwait(false);
            }
            catch (PingException)
            {
                return null;
            }
        }

        protected virtual void ParsePingReply(PingReply reply)
        {
            if (reply == null)
            {
                Status = PingStatus.Failure;
            }
            else
            {
                Status = (reply.Status == IPStatus.Success)
                    ? PingStatus.Success
                    : PingStatus.Failure;
            }
        }
    }
}
