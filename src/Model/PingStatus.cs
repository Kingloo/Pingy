namespace Pingy.Model
{
    public enum PingStatus
    {
        None,
        Updating,
        Success,
        DnsResolutionError,
        IPv6GatewayMissing,
        Failure,
        Cancelled
    }
}