using EthereumRpc;
using System;
using System.IO;
using System.Numerics;
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

            EthereumService ethereumService = new EthereumService(connectionOptions);

            Parallel.ForEach(File.ReadAllLines(file), (address) =>
            {
                if (!address.StartsWith("0x"))
                    address = "0x" + address;

                string sammout;
                try
                {
                    BigInteger ammout = ethereumService.GetBalance(address, BlockTag.Latest);
                    if (ammout == 0) return;

                    sammout = ammout.ToString();
                }
                catch
                {
                    sammout = "ERROR";
                }

                lock (console)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[" + address + "] ");
                    Console.ForegroundColor = sammout == "ERROR" ? ConsoleColor.Red : ConsoleColor.Yellow;
                    Console.WriteLine(sammout.ToString());
                }
            });
        }
    }
}