using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ShrinkerV.Studio
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                StartStudio();
                return 0;
            }

            AttachConsole(0xffffffff);
            if (args[0] == "--run" && args.Length >= 3)
                return CommandLine.Run(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Skip(args, 1)));
            Console.WriteLine("Shrinker V by akiooo\n--run INPUT OUTPUT [--gray] [--factor N] [--threads N]");
            return args.Length == 1 && args[0] == "--help" ? 0 : 2;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(uint processId);
        private static void StartStudio()
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
