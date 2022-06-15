using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient.Hashing
{
    internal interface IHashFunction
    {
        public IHash Hash(byte[] data);
        public IHash Hash(Script script);
        public IHex HashHex(byte[] data);
        public IHex HashHex(Script script);
    }
}
