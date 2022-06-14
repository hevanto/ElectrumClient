namespace ElectrumClient.Request
{
    internal class ServerVersionRequest : RequestBase
    {
        private static readonly string METHOD = "server.version";
        internal ServerVersionRequest(string clientName, string protocolMin, string protocolMax)
            : base(METHOD, clientName, (protocolMin == protocolMax ? protocolMin : new object[2] { protocolMin, protocolMax}))
        {
        }
    }
}
