using System.Net;
using System.Net.NetworkInformation;

namespace Pingy.Model
{
    public class IPv6 : IP
    {
        public IPv6(IPAddress ip)
            : base(ip) { }
        
        protected override void ParsePingReply(PingReply reply)
        {
            if (reply == null)
            {
                Status = PingStatus.IPv6GatewayMissing;
            }
            else
            {
                base.ParsePingReply(reply);
            }
        }
    }
}
