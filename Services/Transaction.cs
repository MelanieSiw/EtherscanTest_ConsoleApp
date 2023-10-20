using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest_ConsoleApp.Entities.Services
{
    public class TransactionService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        string currentMethodName = GetCurrentMethodName();
        DateTime currentDateTime = DateTime.Now;
       
        public async Task<int> Insert (Transaction transaction, string connectionString)
        {
            var connection = new MySqlConnection(connectionString);
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                connection.Open();
                var query = @"
                    INSERT INTO transactions  (blockID, hash, `fromAddress`, `toAddress`, value, gas, gasPrice, transactionIndex)
                    VALUES (@blockId, @hash, @from, @to, @value, @gas, @gasPrice, @transactionIndex);";

                MySqlCommand command = new MySqlCommand(query);
                command.Connection = connection;
                command.Parameters.AddWithValue("@blockId", transaction.BlockId);
                command.Parameters.AddWithValue("@hash", transaction.Hash);
                command.Parameters.AddWithValue("@from", transaction.From);
                command.Parameters.AddWithValue("@to", transaction.To);
                command.Parameters.AddWithValue("@value", transaction.Value);
                command.Parameters.AddWithValue("@gas", transaction.Gas);
                command.Parameters.AddWithValue("@gasPrice", transaction.GasPrice);
                command.Parameters.AddWithValue("@transactionIndex", transaction.TransactionIndex);

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
