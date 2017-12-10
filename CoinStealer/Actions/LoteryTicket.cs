using EthereumRpc;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace CoinStealer.Actions
{
    /// <summary>
    /// Lotery Time
    /// </summary>
    public class LoteryTicket
    {
        static bool Cancel = false;

        public static void Run()
        {
            Console.WriteLine("Press Control+C for cancel");
            Console.CancelKeyPress += Console_CancelKeyPress;

            object console = new object();
            ConnectionOptions connectionOptions = new ConnectionOptions()
            {
                Port = "8545",
                Url = "http://127.0.0.1"
            };

            long have = 0;
            Random r = new Random();
            EthereumService ethereumService = new EthereumService(connectionOptions);

            while (!Cancel)
            {
                Parallel.For(0, 1000, (i) =>
                {
                    if (Cancel) return;

                    byte[] privKey = new byte[32];
                    r.NextBytes(privKey);

                    EthAddress e = new EthAddress(privKey);
                    string endAddress = e.Address;

                    if (!endAddress.StartsWith("0x"))
                        endAddress = "0x" + endAddress;

                    string sammout;
                    try
                    {
                        //Thread.Sleep(5);
                        Interlocked.Increment(ref have);

                        long tx = ethereumService.GetTransactionCount(endAddress, BlockTag.Latest);
                        if (tx == 0) return;

                        BigInteger ammout = ethereumService.GetBalance(endAddress, BlockTag.Latest);
                        sammout = ammout.ToString();
                    }
                    catch (Exception ex)
                    {
                        sammout = "ERROR " + ex.Message;
                    }
                    finally
                    {
                        lock (console) Console.Title = have.ToString();
                    }

                    lock (console)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[" + privKey.ToHex() + "] ");
                        Console.ForegroundColor = sammout.StartsWith("ERROR ") ? ConsoleColor.Red : ConsoleColor.Yellow;
                        Console.WriteLine(sammout);

                        File.AppendAllText("./Lotery.txt", privKey.ToHex() + " " + sammout + Environment.NewLine);
                    }
                });
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Cancel = true;
        }
    }
}