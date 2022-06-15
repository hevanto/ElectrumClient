using NBitcoin;
using System.Text;

namespace ElectrumClient.Hashing
{
    public interface IHash
    {
        public bool IsEmpty { get; }
        public bool IsReversed { get; }
        public IHash Reversed { get; }
        public IHex Hex { get; }

        public string ToString();

        public string ToScriptHashString();
    }

    public class Hash : IHash
    {
        private static readonly Hash _empty = new Hash();

        private byte[] _bytes;
        private bool _isReversed;

        public static Hash Empty => _empty;

        public Hash() : this(new byte[0], false)
        {
        }

        public Hash(byte[] bytes, bool isReversed = false)
        {
            _bytes = bytes;
            _isReversed = isReversed;
        }

        public Hash(IHex hex, bool isReversed = false)
        {
            _bytes = hex.ToBytes();
            _isReversed= isReversed;
        }

        public bool IsEmpty { get { return _bytes.Length == 0; } }

        public bool IsReversed { get { return _isReversed; } }

        public IHash Reversed
        {
            get
            {
                return new Hash(_bytes.Reverse().ToArray(), !_isReversed);
            }
        }

        public IHex Hex
        {
            get
            {
                return new Hashing.Hex(_bytes);
            }
        }

        public override string ToString()
        {
            return Hex.ToString();
        }

        public string ToScriptHashString()
        {
            return IsReversed ? ToString() : Reversed.ToString();
        }

        public static implicit operator Hash (byte[] bytes)
        {
            return new Hash(bytes);
        }

        public static implicit operator Hash (string value)
        {
            return new Hash((IHex)new Hex(value));
        }

        public static implicit operator Hash (Hex hex)
        {
            return new Hash(hex.ToBytes());
        }

        public static implicit operator Hex (Hash value)
        {
            return new Hashing.Hex(value._bytes);
        }
    }

    public static class HashExtensions
    {
        public static IHash ToIHash(this uint256 hash)
        {
            return new Hash(new Hex(hash.ToString()), false);
        }
    }
}