using System;
using System.Windows.Forms;

namespace Waka.Studio
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (s, e) => ShowFatal(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                ShowFatal(e.ExceptionObject as Exception);
            Application.Run(new MainForm());
        }

        private static void ShowFatal(Exception ex)
        {
            try { MessageBox.Show(ex != null ? ex.ToString() : "Unknown error",
                "Shrinker V by akiooo — Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch { }
        }
    }
}
