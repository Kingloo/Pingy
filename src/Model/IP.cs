using System.Net;

namespace Pingy.Model
{
    public class IP : PingBase
    {
        public IP(IPAddress ip)
        {
            Address = ip;

            DisplayName = Address.ToString();
        }
    }
}