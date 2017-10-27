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
        private const int timeout = 1800;
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
                        RoundtripTime.ToString(CultureInfo.CurrentCulture));
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
            get => _status;
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
            get => _roundtripTime;
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
                reply = await PingIpAddressAsync(ipAddress).ConfigureAwait(false);
            }
            else
            {
                reply = await PingHostNameAsync(hostName).ConfigureAwait(false);
            }

            ParsePingReply(reply);
        }

        private static async Task<System.Net.NetworkInformation.PingReply> PingIpAddressAsync(IPAddress ipAddress)
        {
            System.Net.NetworkInformation.PingReply reply = null;
            
            try
            {
                var ping = new System.Net.NetworkInformation.Ping();

                reply = await ping.SendPingAsync(ipAddress, timeout).ConfigureAwait(false);
            }
            catch (System.Net.NetworkInformation.PingException ex)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "{0}: {1}", ipAddress, ex.Message);

                await Log.LogMessageAsync(message).ConfigureAwait(false);
            }

            return reply;
        }
        
        private static async Task<System.Net.NetworkInformation.PingReply> PingHostNameAsync(string hostName)
        {
            System.Net.NetworkInformation.PingReply reply = null;

            bool canResolveHostName = await TryResolveHostNameAsync(hostName).ConfigureAwait(false);

            if (canResolveHostName)
            {
                var ping = new System.Net.NetworkInformation.Ping();

                try
                {
                    reply = await ping.SendPingAsync(hostName, timeout).ConfigureAwait(false);
                }
                catch (System.Net.NetworkInformation.PingException ex)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, "{0}: {1}", hostName, ex.Message);

                    await Log.LogMessageAsync(message).ConfigureAwait(false);
                }
            }

            return reply;
        }

        private static async Task<bool> TryResolveHostNameAsync(string hostName)
        {
            IPAddress[] ipAddresses = null;

            try
            {
                ipAddresses = await Dns.GetHostAddressesAsync(hostName).ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                string errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} couldn't be resolved: {1}",
                    hostName,
                    ex.SocketErrorCode.ToString());

                await Log.LogMessageAsync(errorMessage)
                    .ConfigureAwait(false);

                return false;
            }

            return ipAddresses.Any();
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

            sb.AppendLine(timeout.ToString(CultureInfo.CurrentCulture));

            return sb.ToString();
        }
    }
}
