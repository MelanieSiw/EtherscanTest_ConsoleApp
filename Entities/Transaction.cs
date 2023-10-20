using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest_ConsoleApp.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int BlockId { get; set; }
        public string Hash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Value { get; set; }
        public decimal Gas { get; set; }
        public decimal GasPrice { get; set; }
        public int TransactionIndex { get; set; }
    }
    public class GetTransactionFromBlockNumberIndexResp
    {
        public string blockNumber { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string gas { get; set; }
        public string gasPrice { get; set; }
        public string hash { get; set; }
        public string value { get; set; }
        public string transactionIndex { get; set; }
    }

}
