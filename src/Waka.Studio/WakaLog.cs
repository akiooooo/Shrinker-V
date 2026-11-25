using System;
using System.IO;
using System.Text;

namespace Waka.Studio
{
    internal static class WakaLog
    {
        private static readonly object _sync = new object();
        private static StreamWriter _writer;
        // PERF: раньше AutoFlush=true + FileOptions.WriteThrough означало
        // физический sync на диск на КАЖДУЮ строку. При ~2 строках на текстуру
        // и 100k текстур это 200k синхронных дисковых операций — главный тормоз.
        // Теперь: буфер 64 KB + фоновый флаш раз в 2 сек.
        private static System.Threading.Timer _flushTimer;

        public static void Init(string folder)
        {
            lock (_sync)
            {
                try
                {
                    Directory.CreateDirectory(folder);
                    var path = Path.Combine(folder, "waka_shrinker.log");
                    _writer = new StreamWriter(
                        new FileStream(path, FileMode.Append, FileAccess.Write,
                            FileShare.ReadWrite, 65536, FileOptions.SequentialScan),
                        new UTF8Encoding(false)) { AutoFlush = false };
                    _writer.WriteLine("=== session " + DateTime.Now + " ===");
                    _flushTimer = new System.Threading.Timer(
                        _ => Flush(), null, 2000, 2000);
                }
                catch { }
            }
        }

        public static void Write(string line)
        {
            lock (_sync) { try { if (_writer != null) _writer.WriteLine(line); } catch { } }
        }

        public static void Flush()
        {
            lock (_sync) { try { if (_writer != null) _writer.Flush(); } catch { } }
        }

        public static void Close()
        {
            lock (_sync)
            {
                if (_flushTimer != null) _flushTimer.Dispose();
                if (_writer != null) _writer.Dispose();
                _flushTimer = null;
                _writer = null;
            }
        }
    }
}
