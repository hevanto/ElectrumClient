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
        public IBitSize BitSize { get; }

        public IHash Hash(byte[] data);
    }
}
