﻿using CoinStealer.Actions;
using System;
using System.Diagnostics;
using System.Linq;

namespace CoinStealer
{
    public class Program
    {
        static void Main(string[] args)
        {

#if DEBUG
            if (Debugger.IsAttached)
            {
                //args = new string[] { "--generate_all" };
                //args = new string[] { "--check_balances", "./Uniques/Addresses.txt" };
                //args = new string[] { "--lotery" };
                //args = new string[] { "--payload", "https://github.com/shargon/CoinStealer/blob/master/CoinStealer/Samples/Addresses.txt?raw=true" };
            }
#endif

            string toDo = args.FirstOrDefault();

            if (string.IsNullOrEmpty(toDo))
            {
                // Print help

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--generate_all");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t\tGenerate all address LIKE [??]*[??]");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--check_balances [File]");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t\tCheck balances from a file");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--payload [File]");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t\tGet powershell payload");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--lotery");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t\tLotery time :)");
                return;
            }

            switch (toDo.ToLowerInvariant())
            {
                // Generate all possible addresses LIKE [??]*[??]
                case "--generate_all":
                    {
                        GenerateUniqueAddress.Run();
                        break;
                    }

                // https://github.com/ethereum/wiki/wiki/JSON-RPC
                // geth.exe --datadir "V:\Ethereum" --rpc --rpcaddr 127.0.0.1 --rpcport 8545
                // .\geth.exe --fast --cache=1024 --datadir "V:\Ethereum" --rpc --rpcaddr 127.0.0.1 --rpcport 8545

                case "--check_balances":
                    {
                        CheckBalances.Run(args.Skip(1).FirstOrDefault());
                        break;
                    }

                // Lotery time :)
                case "--lotery":
                    {
                        LoteryTicket.Run();
                        break;
                    }

                // Get PS payload
                case "--payload":
                    {
                        GeneratePayload.Run(args.Skip(1).FirstOrDefault());
                        break;
                    }
            }

#if DEBUG
            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                Console.WriteLine("**** PRESS ENTER FOR EXIT ****");
                Console.ReadLine();
            }
#endif
        }
    }
}