using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Pingy.Model
{
    public interface IPingable
    {
        string DisplayName { get; }
        string DisplayText { get; }
        PingStatus Status { get; set; }
        IPAddress Address { get; }
        Int64 RoundtripTime { get; }

        Task PingAsync();
        Task PingAsync(CancellationToken token);
    }
}
