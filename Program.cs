using NLog;
using System.Threading.Tasks;

namespace EtherscanTest_ConsoleApp
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            var service = new MainService();
            await service.StartTask();
        }

       
    }
}
