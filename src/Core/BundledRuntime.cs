using System;
using System.IO;
using System.IO.Compression;

namespace ShrinkerV.Core
{
    internal static class RuntimeFiles
    {
        internal static byte[] Read(string name)
        {
            string external = Environment.GetEnvironmentVariable("SHRINKER_V_RUNTIME_DIR");
            if (!string.IsNullOrWhiteSpace(external))
                return File.ReadAllBytes(Path.Combine(external, name));
            using (var resource = typeof(RuntimeFiles).Assembly.GetManifestResourceStream("runtime.zip"))
            using (var zip = new ZipArchive(resource, ZipArchiveMode.Read))
            {
                var entry = zip.GetEntry(name);
                if (entry == null) throw new FileNotFoundException("Missing runtime data", name);
                using (var reader = new BinaryReader(entry.Open()))
                {
                    int length = checked((int)entry.Length);
                    var data = reader.ReadBytes(length);
                    if (data.Length != length || length == 0) throw new InvalidDataException(name);
                    return data;
                }
            }
        }
    }
}
