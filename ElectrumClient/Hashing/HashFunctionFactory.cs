using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectrumClient.Hashing
{
    internal static class HashFunctionFactory
    {
        private const string _default_hash_func = "sha256";

        private static Dictionary<string, Type> _registry = new Dictionary<string, Type>()
        {
            { "sha256", typeof(SHA256) },
            { "sha384", typeof(SHA384) },
            { "sha512", typeof(SHA512) },
        };

        private static Dictionary<int, string> _bitSizesToName = new Dictionary<int, string>()
        {
            { BitSize256.BitSize, "sha256" },
            { BitSize384.BitSize, "sha384" },
            { BitSize512.BitSize, "sha512" },
        };

        private static Dictionary<string, BitSize> _hashFuncToBitSize = new Dictionary<string, BitSize>()
        {
            { "sha256", BitSize.BitSize256 },
            { "sha384", BitSize.BitSize384 },
            { "sha512", BitSize.BitSize512 },
        };

        public static IHashFunction GetHashFunction(string name = _default_hash_func)
        {
            var key = name.ToLower();
            var type = _registry[key];

            if (type == null)
                throw new Exception($"No hash function found with name: {name}");

            var hashFunc = Activator.CreateInstance(type);
            if (hashFunc == null)
                throw new Exception($"Unable to create hash function with name: {name}");

            return (IHashFunction)hashFunc;
        }

        public static IHashFunction GetHashFunction(IBitSize? bs)
        {
            if (bs == null) { bs = new BitSize256(); }
            if (!_bitSizesToName.ContainsKey(bs.Size)) throw new Exception($"Unable to create has function for bitsize: {bs.Size}");
            return GetHashFunction(_bitSizesToName[bs.Size]);
        }

        public static BitSize GetHashFunctionBitSize(string name = _default_hash_func)
        {
            var key = name.ToLower();
            return _hashFuncToBitSize[key];
        }
    }
}
