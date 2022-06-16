using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient
{
    public interface IBitSize : IComparable<IBitSize>, IEquatable<IBitSize>, IComparable
    {
        public int Size { get; }
        public int NumBytes { get; }
    }

    public class BitSize : IBitSize
    {
        public static readonly BitSize BitSize256 = new BitSize256();
        public static readonly BitSize BitSize384 = new BitSize384();
        public static readonly BitSize BitSize512 = new BitSize512();

        private int _bitsize;

        public BitSize() : this(0)
        {
        }

        internal BitSize(int size)
        {
            _bitsize = size;
        }
        public int Size { get { return _bitsize; } }
        public int NumBytes { get { return _bitsize / 8; } }

        public int CompareTo(IBitSize? other)
        {
            if (other is null) return -1;
            return Size - other.Size;
        }

        public int CompareTo(object? obj)
        {
            return CompareTo(obj as IBitSize);
        }

        public bool Equals(IBitSize? other)
        {
            if (other is null) return false;
            return Size == other.Size;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IBitSize);
        }

        public override int GetHashCode()
        {
            return Size;
        }

        public static bool operator == (BitSize left, IBitSize right)
        {
            return left.Equals(right);
        }

        public static bool operator != (BitSize left, IBitSize right)
        {
            return !left.Equals(right);
        }
    }

    public class BitSize256 : BitSize, IBitSize
    {
        public static readonly int BitSize = 256;
        public BitSize256() : base(BitSize) { }
    }

    public class BitSize384 : BitSize, IBitSize
    {
        public static readonly int BitSize = 384;
        public BitSize384() : base(BitSize) { }
    }

    public class BitSize512 : BitSize, IBitSize
    {
        public static readonly int BitSize = 512;
        public BitSize512() : base(BitSize) { }
    }
}
