using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CoinStealer
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, EthAddress> list = new Dictionary<string, EthAddress>();

            Action a = () =>
            {
                while (list.Count != ushort.MaxValue)
                {
                    using (CngKey hkey = CngKey.Create(CngAlgorithm.ECDsaP256, null, new CngKeyCreationParameters()
                    {
                        KeyCreationOptions = CngKeyCreationOptions.None,
                        KeyUsage = CngKeyUsages.AllUsages,
                        ExportPolicy = CngExportPolicies.AllowExport | CngExportPolicies.AllowArchiving | CngExportPolicies.AllowPlaintextArchiving | CngExportPolicies.AllowPlaintextExport,
                        Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider,
                        UIPolicy = new CngUIPolicy(CngUIProtectionLevels.None),
                    }))
                    {
                        EthAddress e = new EthAddress
                            (
                            hkey.Export(CngKeyBlobFormat.EccPrivateBlob).Skip(72).ToArray(),
                            hkey.Export(CngKeyBlobFormat.EccPublicBlob).Skip(8).ToArray()
                            );

                        string sub = e.Address.Substring(0, 2) + e.Address.Substring(e.Address.Length - 2, 2);

                        lock (list)
                        {
                            if (list.ContainsKey(sub)) continue;
                            list.Add(sub, e);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(e.PrivateKey.ToHex());
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(" [" + e.Address + "]");
                            Console.Title = list.Count.ToString();
                        }
                    }
                }
            };

            Task ta = new Task(a), tb = new Task(a);

            ta.Start();
            tb.Start();

            Task.WaitAll(ta, tb);

            File.WriteAllLines
                (
                "C:\\Temporal\\Keys.txt",
                list.Values
                .OrderBy(u => u.Address)
                .Select(u => u.PrivateKey.ToHex() + " [" + u.Address + "]")
                );
            File.WriteAllLines
                (
                "C:\\Temporal\\Addresses.txt",
                list.Values
                .Select(u => u.Address)
                .OrderBy(u => u)
                );
        }
    }
}