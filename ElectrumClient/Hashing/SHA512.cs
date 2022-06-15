using NBitcoin;

namespace ElectrumClient.Hashing
{
    internal class SHA512 : AbstractHashFunction, IHashFunction
    {
        public SHA512(bool reverseBytes = false) : base(reverseBytes)
        {
        }

        public override IHash Hash(byte[] data)
        {
            using (System.Security.Cryptography.SHA512 hasher = System.Security.Cryptography.SHA512.Create())
            {
                byte[] bytes = hasher.ComputeHash(data);
                if (_reverseBytes) bytes = bytes.Reverse().ToArray();
                return new Hash(bytes, _reverseBytes);
            }
        }
    }
}
