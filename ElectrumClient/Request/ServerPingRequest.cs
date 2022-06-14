namespace ElectrumClient.Request
{
    internal class ServerPingRequest : RequestBase
    {
        private static readonly string METHOD = "server.ping";
        internal ServerPingRequest()
            : base(METHOD)
        {
        }
    }
}
