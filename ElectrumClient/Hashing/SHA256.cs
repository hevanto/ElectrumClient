namespace ElectrumClient.Hashing
{
    internal class SHA256 : AbstractHashFunction, IHashFunction
    {
        public SHA256() : base() { }

        public override IBitSize BitSize { get { return ElectrumClient.BitSize.BitSize256; } }

        public override IHash Hash(byte[] data)
        {
            using (System.Security.Cryptography.SHA256 hasher = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = hasher.ComputeHash(data);
                return new Hash<BitSize256>(bytes);
            }
        }
    }
}
