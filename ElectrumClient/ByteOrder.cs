namespace ElectrumClient
{
    public class ByteOrder : IComparable<ByteOrder>, IEquatable<ByteOrder>, IComparable
    {
        public static readonly ByteOrder LittleEndian = new ByteOrder(_ByteOrder._LittleEndian);
        public static readonly ByteOrder BigEndian = new ByteOrder(_ByteOrder._BigEndian);

        public static readonly ByteOrder NetworkByteOrder = BigEndian;
        public static readonly ByteOrder HostByteOrder = BitConverter.IsLittleEndian ? LittleEndian : BigEndian;

        private _ByteOrder _order;

        private ByteOrder(_ByteOrder bo) {  _order = bo; }

        private enum _ByteOrder
        {
            _LittleEndian = 0,
            _BigEndian = 1,
        }

        public int CompareTo(ByteOrder? other)
        {
            if (other is null) return - 1;
            return (int)_order - (int)other._order;
        }

        public int CompareTo(object? obj)
        {
            return CompareTo(obj as ByteOrder);
        }

        public bool Equals(ByteOrder? other)
        {
            if (other is null) return false;
            return _order == other._order;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ByteOrder);
        }

        public override int GetHashCode()
        {
            return (int)_order;
        }

        public static bool operator == (ByteOrder left, ByteOrder right)
        {
            return left.Equals(right);
        }

        public static bool operator != (ByteOrder left, ByteOrder right)
        {
            return !left.Equals(right);
        }

        
    }
}