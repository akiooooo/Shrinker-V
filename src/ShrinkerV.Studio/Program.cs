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
        private static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += LoadCore;
            StartStudio();
        }

        private static Assembly LoadCore(object sender, ResolveEventArgs args)
        {
            if (new AssemblyName(args.Name).Name != "ShrinkerV.Core") return null;
            using (var stream = typeof(Program).Assembly.GetManifestResourceStream("ShrinkerV.Core.dll"))
            using (var bytes = new MemoryStream())
            {
                stream.CopyTo(bytes);
                return Assembly.Load(bytes.ToArray());
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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
