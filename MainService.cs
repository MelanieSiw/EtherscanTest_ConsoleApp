using EtherscanTest_ConsoleApp.Entities;
using EtherscanTest_ConsoleApp.Entities.Services;
using EtherscanTest_ConsoleApp.Extension;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static EtherscanTest_ConsoleApp.Entities.Response;
using GetBlockByNumberResp = EtherscanTest_ConsoleApp.Entities.Response.GetBlockByNumberResp;

namespace EtherscanTest_ConsoleApp
{
    public class MainService
    {
        private readonly BlockService _blockService = new BlockService();
        private readonly TransactionService _transactionService = new TransactionService();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        int startBlock = int.Parse(ConfigurationManager.AppSettings["startBlock"]);
        int endBlock = int.Parse(ConfigurationManager.AppSettings["endBlock"]);
        string connectionString = ConfigurationManager.ConnectionStrings["localhost"].ConnectionString;
        string apiKey = ConfigurationManager.AppSettings["ApiKey"];
        string apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

        string currentMethodName = GetCurrentMethodName();
        DateTime currentDateTime = DateTime.Now;


        public async Task StartTask()
        {
            for (int blockNo = startBlock; blockNo < endBlock; blockNo++)
            {
                var block = await GetBlockByBlockNumber(blockNo);
                if (block == null) continue;

                await InsertBlock(block);
                var blockId = _blockService.GetIdByBlockNumber(blockNo, connectionString);
                var transCount = await GetTransactionCount(blockNo);

                if (transCount <= 0) continue;

                for (int i = 0; i < transCount; i++)
                {
                    var tran = await GetTransaction(blockNo, i);
                    if (tran == null) continue;
                    tran.BlockId = blockId;
                    await InsertTransaction(tran);
                }
            }
        }

        public async Task<Block> GetBlockByBlockNumber (int blockNo)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var client = new HttpClient();
            var path = $"{apiUrl}?module=proxy&action=eth_getBlockByNumber&tag={blockNo.ToHex()}&boolean=false&apikey={apiKey}";
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var jsonResp = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<CommonResp<GetBlockByNumberResp>>(jsonResp);
                var block = ConvertBlock(resp.result);
                block.BlockNumber = blockNo;

                stopwatch.Stop();
                TimeSpan processingTime = stopwatch.Elapsed;
                logger.Info("Method Name: " + currentMethodName, "Current Time Stamp: " + currentDateTime, "Time taken to process: " + processingTime);

                return block;
            }
            return null;
        }

        public async Task<int> GetTransactionCount(int blockNo)
        {
            Stopwatch stopwatch = Stopwatch.StartNew(); 

            var client = new HttpClient();
            var path = $"{apiUrl}?module=proxy&action=eth_getBlockTransactionCountByNumber&tag={blockNo.ToHex()}&apikey={apiKey}";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var jsonResp = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<CommonResp<string>>(jsonResp);

                stopwatch.Stop();
                TimeSpan processingTime = stopwatch.Elapsed;
                logger.Info("Method Name: " + currentMethodName, "Current Time Stamp: " + currentDateTime, "Time taken to process: " + processingTime);

                return resp.result.FromHex();
            }

            return 0;
        }

        public async Task<Transaction> GetTransaction(int blockNo, int index)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var client = new HttpClient();
            var path = $"{apiUrl}?module=proxy&action=eth_getTransactionByBlockNumberAndIndex&tag={blockNo.ToHex()}&index={index.ToHex()}&apikey={apiKey}";
            HttpResponseMessage response = await client.GetAsync(path);
            Transaction tran = null;
            if (response.IsSuccessStatusCode)
            {
                var jsonResp = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<CommonResp<Response.GetTransactionFromBlockNumberIndexResp>>(jsonResp);
                tran = ConvertTransaction(resp.result);
            }

            stopwatch.Stop();
            TimeSpan processingTime = stopwatch.Elapsed;
            logger.Info("Method Name: " + currentMethodName + "Current Time Stamp: " + currentDateTime + "Time taken to process: " + processingTime);

            return tran;
        }

        public Block ConvertBlock(GetBlockByNumberResp model)
        {
            return new Block
            {
                Hash = model.hash,
                ParentHash = model.parentHash,
                BlockReward = model.number.FromHexDecimal(),
                GasLimit = model.gasLimit.FromHexDecimal(),
                GasUsed = model.gasUsed.FromHexDecimal(),
                Miner = model.miner
            };
        }

        public Transaction ConvertTransaction(Response.GetTransactionFromBlockNumberIndexResp model)
        {
            return new Transaction
            {
                Hash = model.hash,
                From = model.from,
                To = model.to,
                Value = model.value.FromHexDecimal(),
                Gas = model.gas.FromHexDecimal(),
                GasPrice = model.gasPrice.FromHexDecimal(),
                TransactionIndex = model.transactionIndex.FromHex()

            };
        }

        private async Task<int> InsertBlock(Block block)
        {
            return await _blockService.Insert(block, connectionString);
        }

        private async Task<int> InsertTransaction(Transaction tran)
        {
            return await _transactionService.Insert(tran, connectionString);
        }

        static string GetCurrentMethodName()
        {
            // Create a stack trace that captures the current call stack.
            StackTrace stackTrace = new StackTrace();

            // Get the top stack frame (the current method).
            StackFrame stackFrame = stackTrace.GetFrame(1);

            // Get the method name from the stack frame.
            string methodName = stackFrame.GetMethod().Name;

            return methodName;
        }
    }
}
