namespace ElectrumClient.Request
{
    internal class ServerBannerRequest : RequestBase
    {
        private static readonly string METHOD = "server.banner";
        internal ServerBannerRequest()
            : base(METHOD)
        {
        }
    }
}
