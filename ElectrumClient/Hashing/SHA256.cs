using NBitcoin;

namespace ElectrumClient.Hashing
{
    internal class SHA256 : AbstractHashFunction, IHashFunction
    {
        public SHA256(bool reverseBytes = false) : base(reverseBytes)
        {
        }

        public override IHash Hash(byte[] data)
        {
            using (System.Security.Cryptography.SHA256 hasher = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = hasher.ComputeHash(data);
                if (_reverseBytes) bytes = bytes.Reverse().ToArray();
                return new Hash(bytes, _reverseBytes);
            }
        }
    }
}
