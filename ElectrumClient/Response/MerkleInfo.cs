using ElectrumClient.Hashing;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IMerkleInfo
    {
        public long BlockHeight { get; }
        public IList<IHash> Merkle { get; }
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
        public IList<IHash> Merkle
        {
            get
            {
                var lst = new List<IHash>();
                foreach (var item in Result.Merkle)
                    lst.Add(item);
                return lst;
            }
        }
        public long Pos { get { return Result.Pos; } }

        internal class MerkleInfoResult
        {
            internal MerkleInfoResult()
            {
                Merkle = new List<Hash>();
            }

            [JsonProperty("block_height")]
            public long BlockHeight { get; set; }

            [JsonProperty("merkle")]
            public List<Hash> Merkle { get; set; }

            [JsonProperty("pos")]
            public long Pos { get; set; }
        }
    }
}
