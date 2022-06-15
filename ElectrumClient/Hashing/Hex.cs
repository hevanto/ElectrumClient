using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient.Hashing
{
    public interface IHex
    {
        public bool IsEmpty { get; }
        public byte[] ToBytes();
        public string ToString();
    }

    public class Hex : IHex
    {
        private static readonly Hex _empty = new Hex();

        private string _hex;

        public static Hex Empty => _empty;

        public Hex() : this("")
        {
        }

        public Hex(string value)
        {
            _hex = value;
        }

        public Hex(byte[] bytes)
        {
            _hex = BytesToHex(bytes);
        }

        public bool IsEmpty { get { return _hex == ""; } }

        public byte[] ToBytes() => HexToBytes(_hex);
        public override string ToString() => _hex;

        internal static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) throw new ArgumentException("Argument is not a valid hex string");

            var numBytes = hex.Length / 2;
            var bytes = new byte[numBytes];
            for (var i = 0; i < numBytes; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        internal string BytesToHex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        public static implicit operator Hex(string value)
        {
            return new Hex(value);
        }

        public static implicit operator Hex(byte[] bytes)
        {
            return new Hex(bytes);
        }
    }

    public static class HexExtensions
    {
        public static IHex ToIHex(this Transaction tx)
        {
            return new Hex(tx.ToHex());
        }
    }
}
