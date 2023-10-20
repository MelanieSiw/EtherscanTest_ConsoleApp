using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace EtherscanTest_ConsoleApp.Entities.Services
{
    public class BlockService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        string currentMethodName = GetCurrentMethodName();
        DateTime currentDateTime = DateTime.Now;
        public async Task<int> Insert(Block block, string connectionString)
        {
            var connection = new MySqlConnection(connectionString);

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                connection.Open();
                var query = @"
                    INSERT INTO blocks (blockNumber, hash, parentHash, miner, blockReward, gasLimit, gasUsed)
                    VALUES (@blockNumber, @hash, @parentHash, @miner, @blockReward, @gasLimit, @gasUsed);";

                MySqlCommand command = new MySqlCommand(query);
                command.Connection = connection;
                command.Parameters.AddWithValue("@blockNumber", block.BlockNumber);
                command.Parameters.AddWithValue("@hash", block.Hash);
                command.Parameters.AddWithValue("@parentHash", block.ParentHash);
                command.Parameters.AddWithValue("@miner", block.Miner);
                command.Parameters.AddWithValue("@blockReward", block.BlockReward);
                command.Parameters.AddWithValue("@gasLimit", block.GasLimit);
                command.Parameters.AddWithValue("@gasUsed", block.GasUsed);

                stopwatch.Stop();
                TimeSpan processingTime = stopwatch.Elapsed;
                logger.Info("Method Name: " + currentMethodName, "Current Time Stamp: " + currentDateTime, "Time taken to process: " + processingTime);


                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "FATAL ERROR: Database connection unsuccessful: {Message}", ex.Message);
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public int GetIdByBlockNumber(int blockNumber, string connString)
        {
            var connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                var query = $@"
                    SELECT blockId FROM blocks WHERE blockNumber = {blockNumber};";
                MySqlCommand command = new MySqlCommand(query);
                command.Connection = connection;
                logger.Info(currentMethodName, currentDateTime);


                return connection.QueryFirstOrDefault<int>(query);
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "FATAL ERROR: Database connection unsuccessful: {Message}", ex.Message);
                throw;
            }
            finally
            {
                connection.CloseAsync();
            }
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
