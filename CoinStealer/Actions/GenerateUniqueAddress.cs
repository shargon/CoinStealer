using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoinStealer.Actions
{
    /// <summary>
    /// Generate all address LIKE [??]*[??]
    /// </summary>
    public class GenerateUniqueAddress
    {
        public static void Run()
        {
            object console = new object();
            Dictionary<string, EthAddress> list = new Dictionary<string, EthAddress>();

            Action a = () =>
            {
                Random r = new Random();

                while (list.Count != ushort.MaxValue)
                {
                    byte[] privKey = new byte[32];
                    r.NextBytes(privKey);

                    EthAddress e = new EthAddress(privKey);
                    string sub = e.Address.Substring(0, 2) + e.Address.Substring(e.Address.Length - 2, 2);

                    lock (list)
                    {
                        if (list.ContainsKey(sub)) continue;
                        list.Add(sub, e);
                    }

                    lock (console)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(e.PrivateKey.ToHex());
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" [" + e.Address + "]");
                        Console.Title = list.Count.ToString();
                    }
                }
            };

            Task ta = new Task(a), tb = new Task(a), tc = new Task(a);

            ta.Start();
            tb.Start();
            tc.Start();

            Task.WaitAll(ta, tb, tc);

            File.WriteAllLines
                (
                ".\\Keys.txt",
                list.Values
                .OrderBy(u => u.Address)
                .Select(u => u.PrivateKey.ToHex() + " [" + u.Address + "]")
                );
            File.WriteAllLines
                (
                ".\\Addresses.txt",
                list.Values
                .Select(u => u.Address)
                .OrderBy(u => u)
                );
        }
    }
}