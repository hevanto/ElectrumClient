namespace ElectrumClient.Hashing
{
    internal class SHA512 : AbstractHashFunction, IHashFunction
    {
        public SHA512() : base() { }

        public override IBitSize BitSize { get { return ElectrumClient.BitSize.BitSize512; } }
        public override IHash Hash(byte[] data)
        {
            using (System.Security.Cryptography.SHA512 hasher = System.Security.Cryptography.SHA512.Create())
            {
                byte[] bytes = hasher.ComputeHash(data);
                return new Hash<BitSize512>(bytes);
            }
        }
    }
}
