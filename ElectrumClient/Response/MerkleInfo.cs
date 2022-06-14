using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IMerkleInfo
    {
        public long BlockHeight { get; }
        public IList<string> Merkle { get; }
        public long Pos { get; }
    }

    internal class MerkleInfo : ResponseBase, IMerkleInfo
    {
        public MerkleInfo()
        {
            Result = new MerkleInfoResult();
        }

        [JsonProperty("result")]
        internal MerkleInfoResult Result { get; set; }

        public long BlockHeight { get { return Result.BlockHeight; } }
        public IList<string> Merkle { get { return Result.Merkle; } }
        public long Pos { get { return Result.Pos; } }

        internal class MerkleInfoResult
        {
            internal MerkleInfoResult()
            {
                Merkle = new List<string>();
            }

            [JsonProperty("block_height")]
            public long BlockHeight { get; set; }

            [JsonProperty("merkle")]
            public List<string> Merkle { get; set; }

            [JsonProperty("pos")]
            public long Pos { get; set; }
        }
    }
}
