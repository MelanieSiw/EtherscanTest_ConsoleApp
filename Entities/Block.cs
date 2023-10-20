using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EtherscanTest_ConsoleApp.Entities
{
    public class Block
    {
        public int BlockId { get; set; }
        public int BlockNumber { get; set; }
        public string Hash { get; set; }
        public string ParentHash { get; set; }
        public string Miner { get; set; }
        public decimal BlockReward { get; set; }
        public decimal GasLimit { get; set; }
        public decimal GasUsed { get; set; }

    }
    public class GetBlockByNumberResp
    {
        public string hash { get; set; }
        public string parentHash { get; set; }
        public string gasLimit { get; set; }
        public string gasUsed { get; set; }
        public string miner { get; set; }
        public string number { get; set; }
    }

}
