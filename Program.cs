using CoinStealer.Actions;
using System;
using System.Diagnostics;
using System.Linq;

namespace CoinStealer
{
    class Program
    {
        static void Main(string[] args)
        {

#if DEBUG
            if (Debugger.IsAttached)
            {
                //args = new string[] { "--generate_all" };
                args = new string[] { "--check_balances", "./Uniques/Addresses.txt" };
            }
#endif

            string toDo = args.FirstOrDefault();
            if (string.IsNullOrEmpty(toDo))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--generate_all");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t\tGenerate all address LIKE [??]*[??]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--check_balances [File]");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t\tCheck balances from a file");
                return;
            }

            switch (toDo.ToLowerInvariant())
            {
                case "--generate_all":
                    {
                        GenerateUniqueAddress.Run();
                        break;
                    }

                // https://github.com/ethereum/wiki/wiki/JSON-RPC
                // geth.exe --datadir "V:\Ethereum" --rpc --rpcaddr 127.0.0.1 --rpcport 8545

                case "--check_balances":
                    {
                        CheckBalances.Run(args.Skip(1).FirstOrDefault());
                        break;
                    }
            }
        }
    }
}