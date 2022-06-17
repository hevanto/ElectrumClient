using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IAsyncResponseResult
    {
        public Network Network { get; }
        internal void SetNetwork(Network network);
    }
    public interface IAsyncResponse<IR,IE>
    {
        public Type ResultType { get; }
        public Type ErrorType { get; }
        public bool IsError { get; }
        public bool IsSuccess { get; }
        public IR? Result { get; }
        public IE? Error { get; }

        public IR? ToSyncResponse(out IE? error);
    }

    internal interface IIAsyncResponse
    {
        public void UnmarshallResult(string json, Network network);
        public void UnmarshallError(string json);
    }

    internal class AsyncResponse<R,E,IR,IE> : IAsyncResponse<IR,IE>, IIAsyncResponse
        where R : IR
        where E : IError, IE
    {
        private R? _result;
        private E? _error;

        public AsyncResponse()
        {
        }

        public AsyncResponse(R? result)
        {
            _result = result;
        }

        public AsyncResponse(E? error)
        {
            _error = error;
        }

        public AsyncResponse(R? result, E? error)
        {
            _result = result;
            _error = error;
        }

        public void UnmarshallResult(string json, Network network)
        {
            _result = JsonConvert.DeserializeObject<R>(json);
            if (_result != null)
            {
                Type rType = typeof(R);

                if (typeof(IAsyncResponseResult).IsAssignableFrom(rType))
                    ((IAsyncResponseResult)_result).SetNetwork(network);
                else if (typeof(IList<>).IsAssignableFrom(rType))
                {
                    Type? propType = GetCollectionElementType(rType);
                    if (propType != null)
                    {
                        if (typeof(IAsyncResponseResult).IsAssignableFrom(propType))
                        {
                            SetNetworkOnList((IList<IAsyncResponseResult>)_result, network);
                        }
                    }
                }
                
            }
            
        }

        public void UnmarshallError(string json)
        {
            _error = JsonConvert.DeserializeObject<E>(json);
        }

        public Type ResultType { get { return typeof(R); } }
        public Type ErrorType { get { return typeof(E);  } }
        public bool IsError { get { return _error != null; } }
        public bool IsSuccess{ get { return _result != null;  } }
        public IR? Result { get { return _result; } } 
        public R? ResultValue { get { return _result; } set { _result = value; } }
        public IE? Error { get { return _error; } }
        public E? ErrorValue { get { return _error; } set { _error = value; } }

        public IR? ToSyncResponse(out IE? error)
        {
            error = default(E);
            if (_error != null) error = _error;
            if (_result != null) return _result;
            return default(R);
        }

        private static void SetNetworkOnList(IList<IAsyncResponseResult> lst, Network network)
        {
            foreach (var elem in lst)
                elem.SetNetwork(network);
        }

        private static Type? GetCollectionElementType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // Try the generic way
            var etype = typeof(IEnumerable<>);
            foreach (var bt in type.GetInterfaces())
                if (bt.IsGenericType && bt.GetGenericTypeDefinition() == etype)
                    return bt.GetGenericArguments()[0];

            // Try the non-generic way

            // If it's a dictionary we always return DictionaryEntry
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
                return typeof(System.Collections.DictionaryEntry);

            // If it's a list, we look for an Item property with an int index parameter
            // where the property type is anything but object
            if (typeof(System.Collections.IList).IsAssignableFrom(type))
            {
                foreach (var prop in type.GetProperties())
                {
                    if ("Item" == prop.Name && typeof(object) != prop.PropertyType)
                    {
                        var ipa = prop.GetIndexParameters();
                        if (1 == ipa.Length && typeof(int) == ipa[0].ParameterType)
                        {
                            return prop.PropertyType;
                        }
                    }
                }
            }

            // If it's a colleciton, we look for an Add() method whose parameter is
            // anything but object
            if (typeof(System.Collections.ICollection).IsAssignableFrom(type))
            {
                foreach (var meth in type.GetMethods())
                {
                    if ("Add" == meth.Name)
                    {
                        var pa = meth.GetParameters();
                        if (1 == pa.Length && typeof(object) != pa[0].ParameterType)
                        {
                            return pa[0].ParameterType;
                        }
                    }
                }
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return typeof(object);

            return null;
        }
    }
}
