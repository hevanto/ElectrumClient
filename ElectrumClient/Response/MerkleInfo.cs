using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IMerkleInfo : IAsyncResponseResult
    {
        public long BlockHeight { get; }
        public IList<Hash<BitSize256>> Merkle { get; }
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
        public IList<Hash<BitSize256>> Merkle
        {
            get
            {
                var lst = new List<Hash<BitSize256>>();
                foreach (var item in Result.Merkle)
                    lst.Add(HashFactory.Create256(item));
                return lst;
            }
        }
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
