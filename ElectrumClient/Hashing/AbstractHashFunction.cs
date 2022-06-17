namespace ElectrumClient.Hashing
{
    internal abstract class AbstractHashFunction : IHashFunction
    {
        public AbstractHashFunction()
        {
        }

        public abstract IBitSize BitSize { get; }

        public abstract IHash Hash(byte[] data);
    }
}
