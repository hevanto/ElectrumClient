using NBitcoin;

namespace ElectrumClient.Hashing
{
    internal class SHA384 : AbstractHashFunction, IHashFunction
    {
        public SHA384() : base() { }

        public override IBitSize BitSize { get { return ElectrumClient.BitSize.BitSize384; } }

        public override IHash Hash(byte[] data)
        {
            using (System.Security.Cryptography.SHA384 hasher = System.Security.Cryptography.SHA384.Create())
            {
                byte[] bytes = hasher.ComputeHash(data);
                return new Hash<BitSize384>(bytes);
            }    
        }
    }
}
