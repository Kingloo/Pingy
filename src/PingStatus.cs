namespace Pingy
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