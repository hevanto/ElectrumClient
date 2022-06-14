using ElectrumClient.Converters;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IError
    {
        public int Code { get; }
        public string Message { get; }
    }
    internal class Error : ResponseBase, IError
    {
        public Error()
        {
            Result = new ErrorResult();
        }

        [JsonProperty("error")]
        [JsonConverter(typeof(EmbeddedErrorConverter))]
        internal ErrorResult Result { get; set; }

        public int Code { get { return Result.Code; } }
        public string Message { get { return Result.Message; } }

        internal class ErrorResult
        {
            internal ErrorResult()
            {
                Code = 0;
                Message = "";
            }

            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
