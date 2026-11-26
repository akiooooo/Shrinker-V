using System;
using System.IO;
using System.Threading;

namespace ShrinkerV.Core
{
    public enum LogLevel { Info, Ok, Warn, Error }

    public delegate void ProcessingProgress(string message, LogLevel level, ProcessingStats stats,
        int completedArchives, int totalArchives);

    public static class Shrinker
    {


        private static int active;

        public static ProcessingOptions CreateDefaultOptions()
        {
            return new ProcessingOptions();
        }

        public static ProcessingStats Run(ProcessingOptions options, Action<string> log = null)
        {
            ProcessingProgress progress = null;
            if (log != null) progress = (message, level, stats, completed, total) => log(message);
            return RunWithProgress(options, progress);
        }

        public static ProcessingStats RunWithProgress(ProcessingOptions options, ProcessingProgress progress)
        {
            if (options == null) throw new ArgumentNullException("options");
            var job = options.Copy();
            job.Cancel.ThrowIfCancellationRequested();
            job.InputFolder = FullFolder(job.InputFolder, "InputFolder");
            if (!Directory.Exists(job.InputFolder)) throw new DirectoryNotFoundException(job.InputFolder);
            job.OutputFolder = job.ProcessInPlace ? job.InputFolder : FullFolder(job.OutputFolder, "OutputFolder");
            if (!job.ProcessInPlace && (Contains(job.InputFolder, job.OutputFolder) || Contains(job.OutputFolder, job.InputFolder)))
                throw new ArgumentException("Input and output folders must be separate.");
            if (job.CompressionFactor < 1 || job.ClothingCompressionFactor < 1 || job.PedsCompressionFactor < 1 ||
                job.MinTextureSize < 1 || job.MinTextureSize > ushort.MaxValue || job.MaxThreads < 1)
                throw new ArgumentOutOfRangeException("options", "Check compression factors, minimum size and thread count.");

            // GameFormats shares mutable archive and key state throughout the process.
            if (Interlocked.CompareExchange(ref active, 1, 0) != 0)
                throw new InvalidOperationException("Another Shrinker V by akiooo job is running.");
            try
            {
                GtaProcessor engine = null;
                engine = new GtaProcessor(job, (message, level) =>
                {
                    if (progress != null) progress(message, level, engine.Stats, engine.ProgressValue, engine.ProgressMax);
                });
                engine.LoadReplacementAssets();
                Directory.CreateDirectory(job.OutputFolder);
                return engine.Run();
            }
            finally { Interlocked.Exchange(ref active, 0); }
        }

        private static string FullFolder(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A folder is required.", name);
            return Path.GetFullPath(value).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        }

        private static bool Contains(string parent, string child)
        {
            return child.StartsWith(parent, StringComparison.OrdinalIgnoreCase);
        }
    }

}
