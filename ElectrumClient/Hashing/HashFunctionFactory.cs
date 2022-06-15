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
        public static IHashFunction GetHashFunction(string name = _default_hash_func, bool reverseBytes = true)
        {
            var key = name.ToLower();
            var type = _registry[key];
            
            if (type == null)
                throw new Exception($"No hash function found with name: {name}");

            var hashFunc = Activator.CreateInstance(type, new object[1] {reverseBytes});
            if (hashFunc == null)
                throw new Exception($"Unable to create hash function with name: {name}");

            return (IHashFunction)hashFunc;
        }
    }
}
