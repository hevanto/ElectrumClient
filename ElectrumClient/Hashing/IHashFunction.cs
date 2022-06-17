namespace ElectrumClient.Hashing
{
    internal interface IHashFunction
    {
        public IBitSize BitSize { get; }

        public IHash Hash(byte[] data);
    }
}
