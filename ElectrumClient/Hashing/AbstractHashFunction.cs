using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient.Hashing
{
    internal abstract class AbstractHashFunction : IHashFunction
    {
        protected bool _reverseBytes;

        public AbstractHashFunction(bool reverseBytes)
        {
            _reverseBytes = reverseBytes;
        }

        public abstract IHash Hash(byte[] data);
        public IHash Hash(Script script) => Hash(script.ToBytes());
        public IHex HashHex(byte[] data) => Hash(data).Hex;
        public IHex HashHex(Script script) => Hash(script).Hex;
    }
}
