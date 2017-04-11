using System;
using System.Globalization;
using System.Linq;
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
        public string Address => isIpAddress ? ipAddress.ToString() : hostName;

        public string Tooltip
        {
            get
            {
                if (Status == PingStatus.Success)
                {
                    return string.Format(CultureInfo.CurrentCulture,
                        "{0} in {1} ms",
                        Status.ToString(),
                        RoundtripTime.ToString());
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
                if (_status != value)
                {
                    _status = value;

                    RaisePropertyChanged(nameof(Status));
                    RaisePropertyChanged(nameof(Tooltip));
                }
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
                if (_roundtripTime != value)
                {
                    _roundtripTime = value;

                    RaisePropertyChanged(nameof(RoundtripTime));
                }
            }
        }
        #endregion

        private Ping(string address)
        {
            isIpAddress = false;

            hostName = address;
        }

        private Ping(IPAddress ip)
        {
            isIpAddress = true;

            ipAddress = ip;
        }

        public static bool TryCreate(string line, out Ping ping)
        {
            if (String.IsNullOrWhiteSpace(line))
            {
                ping = null;
                return false;
            }

            if (IPAddress.TryParse(line, out IPAddress ip))
            {
                ping = new Ping(ip);
            }
            else
            {
                ping = new Ping(line);
            }

            return true;
        }

        public async Task PingAsync()
        {
            Status = PingStatus.Updating;
            System.Net.NetworkInformation.PingReply reply = null;

            if (isIpAddress)
            {
                reply = await PingIpAddress(ipAddress)
                    .ConfigureAwait(false);
            }
            else
            {
                bool canResolveHostName = await TryResolveHostName(hostName)
                    .ConfigureAwait(false);

                if (canResolveHostName)
                {
                    reply = await PingHostName(hostName)
                        .ConfigureAwait(false);
                }
            }

            ParsePingReply(reply);
        }

        private static async Task<System.Net.NetworkInformation.PingReply> PingIpAddress(IPAddress ipAddress)
        {
            var ping = new System.Net.NetworkInformation.Ping();

            return await ping.SendPingAsync(ipAddress, timeout)
                .ConfigureAwait(false);
        }

        private static async Task<bool> TryResolveHostName(string hostName)
        {
            IPAddress[] ipAddresses = null;

            try
            {
                ipAddresses = await Dns.GetHostAddressesAsync(hostName)
                    .ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                Utils.LogException(ex);

                return false;
            }
            
            return ipAddresses.Any();
        }

        private static async Task<System.Net.NetworkInformation.PingReply> PingHostName(string hostName)
        {
            var ping = new System.Net.NetworkInformation.Ping();

            return await ping.SendPingAsync(hostName, timeout)
                .ConfigureAwait(false);
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
            if (other == null) { return false; }
            
            return isIpAddress
                ? ipAddress.Equals(other.ipAddress)
                : hostName.Equals(other.hostName);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().Name);
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
