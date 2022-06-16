using ElectrumClient.Converters;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IError
    {
        public string Description { get; }
        public int Code { get; }
        public string Message { get; }
    }
    internal class Error : ResponseBase, IError
    {
        public Error()
        {
            Result = new ErrorResult();
        }

        public override string ToString()
        {
            if (Message != "")
                return $"{Description} [{Code} - {Message}]";
            else
                return $"{Description}";
        }

        [JsonProperty("error")]
        [JsonConverter(typeof(EmbeddedErrorConverter))]
        internal ErrorResult Result { get; set; }

        public string Description { get { return Result.Description; } }
        public int Code { get { return Result.Code; } }
        public string Message { get { return Result.Message; } }

        internal class ErrorResult
        {
            internal ErrorResult()
            {
                Description = "";
                Code = 0;
                Message = "";
            }

            internal ErrorResult(string description)
            {
                Description = description;
                Code = 0;
                Message = "";
            }

            public string Description { get; set; }

            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
