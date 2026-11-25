using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Waka.Core
{
    internal static class BundledRuntime
    {
        private static readonly Assembly Library = typeof(BundledRuntime).Assembly;
        private static readonly Dictionary<string, Lazy<Assembly>> Assemblies =
            new Dictionary<string, Lazy<Assembly>>(StringComparer.OrdinalIgnoreCase);

        static BundledRuntime()
        {
            foreach (var name in new[] { "CodeWalker.Core", "SharpDX", "SharpDX.Mathematics" })
            {
                string resource = name + ".dll";
                Assemblies.Add(name, new Lazy<Assembly>(() =>
                {
                    using (var stream = Open(resource))
                    using (var memory = new MemoryStream())
                    {
                        stream.CopyTo(memory);
                        return Assembly.Load(memory.ToArray());
                    }
                }));
            }
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        internal static void Initialize() { }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            Lazy<Assembly> candidate;
            var requested = new AssemblyName(args.Name);
            if (!Assemblies.TryGetValue(requested.Name, out candidate)) return null;
            var found = candidate.Value;
            return String.Equals(found.FullName, requested.FullName, StringComparison.OrdinalIgnoreCase) ? found : null;
        }

        private static Stream Open(string name)
        {
            var stream = Library.GetManifestResourceStream(name);
            if (stream == null) throw new FileNotFoundException("Missing embedded resource: " + name);
            return stream;
        }

        // CodeWalker opens its lookup tables by path. Give each process its own directory.
        private static readonly Lazy<string> Extracted = new Lazy<string>(Extract);

        internal static string DirectoryPath
        {
            get
            {
                var external = Environment.GetEnvironmentVariable("WAKA_RUNTIME_DIR");
                return String.IsNullOrWhiteSpace(external) ? Extracted.Value : Path.GetFullPath(external);
            }
        }

        private static string Extract()
        {
            string folder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(),
                "shrinker-v-" + Guid.NewGuid().ToString("N"))).FullName;
            try
            {
                using (var stream = Open("runtime.zip"))
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.Name != entry.FullName || String.IsNullOrEmpty(entry.Name))
                            throw new InvalidDataException("Unexpected runtime entry.");
                        using (var input = entry.Open())
                        using (var output = new FileStream(Path.Combine(folder, entry.Name), FileMode.CreateNew))
                            input.CopyTo(output);
                    }
                }
                AppDomain.CurrentDomain.DomainUnload += (sender, args) => Remove(folder);
                AppDomain.CurrentDomain.ProcessExit += (sender, args) => Remove(folder);
                return folder;
            }
            catch { Remove(folder); throw; }
        }

        private static void Remove(string folder)
        {
            try { Directory.Delete(folder, true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
