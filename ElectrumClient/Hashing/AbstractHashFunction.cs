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
        public AbstractHashFunction()
        {
        }

        public abstract IBitSize BitSize { get; }

        public abstract IHash Hash(byte[] data);
    }
}
