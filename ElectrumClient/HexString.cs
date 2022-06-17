using ElectrumClient.Response;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient
{
    public interface IHexString
    {
        public bool IsEmpty { get; }
        public IBitSize BitSize { get; }
        public IHash? Hash { get; }

        public string ToString();
        public byte[] ToBytes();
        public byte[] ToBytes(ByteOrder byteorder);
    }

    public class HexString : IHexString
    {
        // This is always in network byte order!
        private string _hexString = "";

        public HexString() { }

        public HexString(string hex)
        {
            if (hex.Length % 2 != 0) throw new ArgumentException("Argument is not a Hex string");
            _hexString = hex;
        }

        public HexString(byte[] bytes) : this(bytes, ByteOrder.HostByteOrder) { }

        public HexString(byte[] bytes, ByteOrder byteOrder)
        {
            if (bytes == null || bytes.Length == 0) return;
            _hexString = BytesToHex(bytes, byteOrder);
        }

        public HexString(IHash hash) : this(hash.ToBytes()) { }

        public HexString(Transaction tx) : this(tx.ToHex()) { }

        public bool IsEmpty { get { return _hexString == ""; } }

        public IBitSize BitSize
        {
            get
            {
                var numBytes = _hexString.Length / 2;
                return new BitSize(numBytes * 8);
            }
        }

        public IHash? Hash
        {
            get
            {
                if (BitSize == ElectrumClient.BitSize.BitSize256)
                    return new Hash<BitSize256>(ToBytes());

                if (BitSize == ElectrumClient.BitSize.BitSize384)
                    return new Hash<BitSize384>(ToBytes());

                if (BitSize == ElectrumClient.BitSize.BitSize512)
                    return new Hash<BitSize512>(ToBytes());

                return null;
            }
        }

        public override string ToString()
        {
            return _hexString;
        }

        public byte[] ToBytes()
        {
            return ToBytes(ByteOrder.HostByteOrder);
        }

        public byte[] ToBytes(ByteOrder byteOrde)
        {
            return HexToBytes(_hexString, byteOrde);
        }

        private static string BytesToHex(byte[] bytes, ByteOrder byteorder)
        {
            if (byteorder != ByteOrder.NetworkByteOrder) bytes = bytes.Reverse().ToArray();
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        private static byte[] HexToBytes(string hex, ByteOrder byteOrder)
        {
            if (hex.Length % 2 != 0) throw new ArgumentException("Argument is not a Hex string");

            var numBytes = hex.Length / 2;
            var bytes = new byte[numBytes];
            for (var i=0; i<numBytes; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i*2, 2), 16);
            if (byteOrder != ByteOrder.NetworkByteOrder) bytes = bytes.Reverse().ToArray();
            return bytes;
        }
    }
}
