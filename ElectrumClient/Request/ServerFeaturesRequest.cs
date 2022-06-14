namespace ElectrumClient.Request
{
    internal class ServerFeaturesRequest : RequestBase
    {
        private static readonly string METHOD = "server.features";
        internal ServerFeaturesRequest()
            : base(METHOD)
        {
        }
    }
}
