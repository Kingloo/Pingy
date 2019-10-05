using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Pingy.Model;

namespace Pingy
{
    public class Domain : PingBase
    {
        private readonly IPHostEntry ipHostEntry = null;

        public Domain(IPHostEntry ipHostEntry)
        {
            this.ipHostEntry = ipHostEntry;
        }

        public override async Task PingAsync(CancellationToken token)
        {
            Status = PingStatus.Updating;

            IPAddress ip = await ResolveHostNameAsync(ipHostEntry.HostName).ConfigureAwait(false);

            if (ip == IPAddress.None)
            {
                Status = PingStatus.DnsResolutionError;
            }
            else
            {
                Address = ip;

                await base.PingAsync(token).ConfigureAwait(false);
            }
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
            if (reply is PingReply)
            {
                base.ParsePingReply(reply);
            }
            else
            {
                Status = PingStatus.DnsResolutionError;
            }
        }
    }
}