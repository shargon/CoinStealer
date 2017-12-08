using EthereumRpc;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace CoinStealer.Actions
{
    /// <summary>
    /// Check Balances from file
    ///     Thanks to https://github.com/LawrenceBotley/EthereumRpc-NET
    /// </summary>
    public class CheckBalances
    {
        public static void Run(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Console.WriteLine("File required");
                return;
            }

            object console = new object();
            ConnectionOptions connectionOptions = new ConnectionOptions()
            {
                Port = "8545",
                Url = "http://127.0.0.1"
            };

            long count = 0, have = 0;
            EthereumService ethereumService = new EthereumService(connectionOptions);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Start checking accounts");
            Console.WriteLine("");

            Parallel.ForEach(File.ReadAllLines(file), (address) =>
            //foreach(string address in File.ReadAllLines(file))
            {
                string endAddress = address;

                if (!address.StartsWith("0x"))
                    endAddress = "0x" + address;

                Interlocked.Increment(ref count);

                string sammout;
                try
                {
                    long tx = ethereumService.GetTransactionCount(endAddress, BlockTag.Latest);
                    if (tx == 0) return;

                    BigInteger ammout = ethereumService.GetBalance(endAddress, BlockTag.Latest);

                    sammout = ammout.ToString();
                    Interlocked.Increment(ref have);
                }
                catch (Exception ex)
                {
                    sammout = "ERROR " + ex.Message;
                }

                lock (console)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[" + endAddress + "] ");
                    Console.ForegroundColor = sammout.StartsWith("ERROR ") ? ConsoleColor.Red : ConsoleColor.Yellow;
                    Console.WriteLine(sammout);
                }
            });

            // END

            lock (console)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("*****************************************");
                Console.WriteLine("Checked accounts [" + count + "] ");
                Console.WriteLine("Accounts with balance [" + have + "] ");
            }
        }
    }
}