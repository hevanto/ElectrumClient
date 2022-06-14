namespace ElectrumClient.Request
{
    internal class ServerDonationAddressRequest : RequestBase
    {
        private static readonly string METHOD = "server.donation_address";
        internal ServerDonationAddressRequest()
            : base(METHOD)
        {
        }
    }
}
