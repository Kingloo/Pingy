using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pingy
{
    public class Ping : ViewModelBase
    {
        public enum PingStatus { None, Updating, Success, Failure, DnsResolutionError };

        #region Fields
        private bool isIpAddress = false;
        private IPAddress ipAddress = null;
        private string hostName = string.Empty;
        private const int timeout = 500;
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
                if (this.Status == PingStatus.Success)
                {
                    return string.Format("{0} in {1} ms", this.Status.ToString(), this.RoundtripTime.ToString());
                }
                else
                {
                    return this.Status.ToString();
                }
            }
        }

        private PingStatus _status = PingStatus.None;
        public PingStatus Status
        {
            get { return this._status; }
            set
            {
                this._status = value;
                OnPropertyChanged();
                OnPropertyChanged("Tooltip");
            }
        }

        private long _roundtripTime = 0;
        public long RoundtripTime
        {
            get { return this._roundtripTime; }
            set
            {
                this._roundtripTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public Ping(string address)
        {
            if (IPAddress.TryParse(address, out this.ipAddress))
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

            if (this.isIpAddress)
            {
                reply = await PingIpAddress(ipAddress);
            }
            else
            {
                bool canResolveHostName = await TryResolveHostName(hostName);

                if (canResolveHostName)
                {
                    reply = await PingHostName(hostName);
                }
            }

            ParsePingReply(reply);
        }

        private async Task<System.Net.NetworkInformation.PingReply> PingIpAddress(IPAddress ipAddress)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

            return await ping.SendPingAsync(ipAddress, timeout);
        }

        private async Task<bool> TryResolveHostName(string hostName)
        {
            IPAddress[] ipAddresses = null;

            try
            {
                ipAddresses = await Dns.GetHostAddressesAsync(hostName);
            }
            catch (SocketException)
            {
                return false;
            }

            if (ipAddresses.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<System.Net.NetworkInformation.PingReply> PingHostName(string hostName)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

            return await ping.SendPingAsync(hostName, timeout);
        }

        private void ParsePingReply(System.Net.NetworkInformation.PingReply reply)
        {
            if (reply == null)
            {
                this.Status = PingStatus.DnsResolutionError;
            }
            else
            {
                this.RoundtripTime = reply.RoundtripTime;

                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    this.Status = PingStatus.Success;
                }
                else
                {
                    this.Status = PingStatus.Failure;
                }
            }
        }
    }
}
