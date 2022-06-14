namespace ElectrumClient.Request
{
    internal class ServerPeersSubscribeRequest : RequestBase
    {
        private static readonly string METHOD = "server.peers.subscribe";
        internal ServerPeersSubscribeRequest()
            : base(METHOD)
        {
        }
    }
}
