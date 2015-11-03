using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pingy
{
    public class Ping : ViewModelBase, IEquatable<Ping>
    {
        public enum PingStatus
        {
            None,
            Updating,
            Success,
            Failure,
            DnsResolutionError
        };

        #region Fields
        private bool isIpAddress = false;
        private IPAddress ipAddress = null;
        private string hostName = string.Empty;
        private const int timeout = 1250;
        #endregion

        #region Properties
        public string Address
        {
            get
            {
                if (isIpAddress)
                {
                    return ipAddress.ToString();
                }
                else
                {
                    return hostName;
                }
            }
        }

        public string Tooltip
        {
            get
            {
                if (Status == PingStatus.Success)
                {
                    return string.Format("{0} in {1} ms", Status.ToString(), RoundtripTime.ToString());
                }
                else
                {
                    return Status.ToString();
                }
            }
        }

        private PingStatus _status = PingStatus.None;
        public PingStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;

                OnPropertyChanged();
                OnPropertyChanged("Tooltip");
            }
        }

        private long _roundtripTime = 0;
        public long RoundtripTime
        {
            get
            {
                return _roundtripTime;
            }
            set
            {
                _roundtripTime = value;

                OnPropertyChanged();
            }
        }
        #endregion

        public Ping(string address)
        {
            if (IPAddress.TryParse(address, out ipAddress))
            {
                this.isIpAddress = true;
            }
            else
            {
                this.hostName = address;
            }
        }

        public async Task PingAsync()
        {
            Status = PingStatus.Updating;
            System.Net.NetworkInformation.PingReply reply = null;

            if (isIpAddress)
            {
                reply = await PingIpAddress(ipAddress).ConfigureAwait(false);
            }
            else
            {
                bool canResolveHostName = await TryResolveHostName(hostName).ConfigureAwait(false);

                if (canResolveHostName)
                {
                    reply = await PingHostName(hostName).ConfigureAwait(false);
                }
            }

            ParsePingReply(reply);
        }

        private async Task<System.Net.NetworkInformation.PingReply> PingIpAddress(IPAddress ipAddress)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

            return await ping.SendPingAsync(ipAddress, timeout).ConfigureAwait(false);
        }

        private async Task<bool> TryResolveHostName(string hostName)
        {
            IPAddress[] ipAddresses = null;

            try
            {
                ipAddresses = await Dns.GetHostAddressesAsync(hostName).ConfigureAwait(false);
            }
            catch (SocketException)
            {
                return false;
            }

            return (ipAddresses.Length > 0);
        }

        private async Task<System.Net.NetworkInformation.PingReply> PingHostName(string hostName)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

            return await ping.SendPingAsync(hostName, timeout).ConfigureAwait(false);
        }

        private void ParsePingReply(System.Net.NetworkInformation.PingReply reply)
        {
            if (reply == null)
            {
                Status = PingStatus.DnsResolutionError;
            }
            else
            {
                RoundtripTime = reply.RoundtripTime;

                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    Status = PingStatus.Success;
                }
                else
                {
                    Status = PingStatus.Failure;
                }
            }
        }

        public bool Equals(Ping other)
        {
            if (isIpAddress)
            {
                return ipAddress.Equals(other.ipAddress);
            }
            else
            {
                return hostName.Equals(other.hostName);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(this.GetType().ToString());
            sb.AppendLine(hostName);
            sb.AppendLine(isIpAddress.ToString());

            if (isIpAddress)
            {
                sb.AppendLine(ipAddress.ToString());
            }

            sb.AppendLine(timeout.ToString());

            return sb.ToString();
        }
    }
}
