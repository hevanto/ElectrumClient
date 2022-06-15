using NBitcoin;

namespace ElectrumClient.Hashing
{
    internal class SHA384 : AbstractHashFunction, IHashFunction
    {
        public SHA384(bool reverseBytes = false) : base(reverseBytes)
        {
        }

        public override IHash Hash(byte[] data)
        {
            using (System.Security.Cryptography.SHA384 hasher = System.Security.Cryptography.SHA384.Create())
            {
                byte[] bytes = hasher.ComputeHash(data);
                if (_reverseBytes) bytes = bytes.Reverse().ToArray();
                return new Hash(bytes, _reverseBytes);
            }    
        }
    }
}
