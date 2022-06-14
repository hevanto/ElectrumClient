using Newtonsoft.Json;

namespace ElectrumClient.Response
{
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
        public void UnmarshallResult(string json);
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

        public void UnmarshallResult(string json)
        {
            _result = JsonConvert.DeserializeObject<R>(json);
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
    }
}
