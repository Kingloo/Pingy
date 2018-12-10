using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pingy.Model
{
    public class Domain : PingBase
    {
        public IPHostEntry Host { get; protected set; }

        public Domain(IPHostEntry hostEntry)
        {
            Host = hostEntry ?? throw new ArgumentNullException(nameof(hostEntry));

            DisplayName = Host.HostName;
        }
        
        public override async Task PingAsync(CancellationToken token)
        {
            Status = PingStatus.Updating;

            IPAddress ip = await ResolveHostNameAsync(Host.HostName).ConfigureAwait(false);

            if (ip == IPAddress.None)
            {
                Status = PingStatus.DnsResolutionError;

                return;
            }
            else
            {
                Address = ip;
            }

            await base.PingAsync(token).ConfigureAwait(false);
        }

        private static async Task<IPAddress> ResolveHostNameAsync(string hostName)
        {
            try
            {
                IPAddress[] ips = await Dns.GetHostAddressesAsync(hostName).ConfigureAwait(false);

                return ips.First() ?? IPAddress.None;
            }
            catch (SocketException)
            {
                return IPAddress.None;
            }
        }

        protected override void ParsePingReply(PingReply reply)
        {
            if (reply is null)
            {
                Status = PingStatus.DnsResolutionError;
            }
            else
            {
                base.ParsePingReply(reply);
            }
        }
    }
}
