using System;
using System.Net;

namespace Pingy.Model
{
    public abstract class IP : PingBase
    {
        public IP(IPAddress ip)
        {
            Address = ip ?? throw new ArgumentNullException(nameof(ip));

            DisplayName = Address.ToString();
        }
    }
}
