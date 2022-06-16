using ElectrumClient.Hashing;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient
{
    public interface IHash : IComparable<IHash>, IEquatable<IHash>, IComparable
    {
        public int Size { get; }
        public IHexString Hex { get; }
        public bool IsEmpty { get; }

        public string ToString();
        public byte[] ToBytes();
    }

    public static class HashFactory
    {
        public static IHash Create(IBitSize size, byte[] bytes)
        {
            var type = typeof(Hash<>).MakeGenericType(size.GetType());
            var instance = (IHash?)Activator.CreateInstance(type, new object?[] { bytes });
            return instance ?? new Hash<BitSize256>(bytes);
        }

        public static IHash Create(IBitSize size, Script script)
        {
            var hasher = HashFunctionFactory.GetHashFunction(size);
            return hasher.Hash(script.ToBytes());
        }

        public static Hash<BitSize256> Create256(string hex)
        {
            return Create256(new HexString(hex));
        }

        public static Hash<BitSize256> Create256(HexString hex)
        {
            if (!hex.BitSize.Equals(BitSize.BitSize256)) throw new ArgumentException("Hex string must be 256 bit long (32 characters)");
            return new Hash<BitSize256>(hex.ToBytes());
        }

        public static Hash<BitSize256> Create256(uint256 u)
        {
            return new Hash<BitSize256>(u.ToBytes());
        }
    }

    public class Hash<BS> : IHash where BS : IBitSize, new()
    {
        private static readonly Hash<BS> _empty = new Hash<BS>();

        static private BS _bs = new BS();
        private byte[] _bytes = new byte[new BS().NumBytes];

        public Hash() { }

        public Hash(byte[] bytes)
        {
            if (bytes.Length != _bytes.Length) throw new ArgumentException($"Hash should contain {_bs.Size} bytes");
            _bytes = bytes;
        }

        public Hash(Script script) : this(script.ToBytes()) { }

        public int Size { get { return _bs.Size; } }

        public IHexString Hex
        {
            get
            {
                return new HexString(_bytes);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _bytes.SequenceEqual(_empty._bytes);
            }
        }

        public override string ToString()
        {
            return Hex.ToString();
        }

        public byte[] ToBytes()
        {
            return _bytes;
        }

        public int CompareTo(IHash? other)
        {
            if (other is null) return -1;

            var otherBytes = other.ToBytes();
            var leftLen = _bytes.Length;
            var rightLen = otherBytes.Length;
            var minLen = leftLen < rightLen ? leftLen : rightLen;

            for (var i=0; i<minLen; i++)
            {
                var result = _bytes[i].CompareTo(otherBytes[i]);
                if (result != 0) return result;
            }
            if (leftLen == rightLen) return 0;
            return leftLen < rightLen ? -1 : 1;
        }

        public int CompareTo(object? obj)
        {
            return CompareTo(obj as IHash);
        }

        public bool Equals(IHash? other)
        {
            if (other is null) return false;
            
            var otherBytes = other.ToBytes();
            return _bytes.SequenceEqual(otherBytes);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IHash);
        }

        public override int GetHashCode()
        {
            var result = 0;
            var numInts = _bytes.Length / 4;
            for (var i = 0; i < numInts; i++)
                result ^= ((((int)_bytes[i * 4]) << 24) | (((int)_bytes[i * 4 + 1]) << 16) | (((int)_bytes[i * 4 + 2]) << 8) | ((int)_bytes[i * 4 + 3]));
            if (_bytes.Length % 4 != 0)
            {
                int lastInt = 0;
                for (var i = 0; i < _bytes.Length % 4; i++)
                {
                    lastInt |= (((int)_bytes[numInts * 4 + i]) << (24 - (i * 8)));
                }
                result^= lastInt;
            }
            return result;
        }
    }

    public static class HashExtensions
    {
        public static uint256 ToUint256(this Hash<BitSize256> hash)
        {
            return new uint256(hash.ToBytes());
        }
    }
}
