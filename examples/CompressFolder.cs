using System;
using ShrinkerV.Core;

internal static class CompressFolder
{
    private static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: CompressFolder <input folder> <output folder>");
            return 2;
        }
        try
        {
            var options = Shrinker.CreateDefaultOptions();
            options.InputFolder = args[0];
            options.OutputFolder = args[1];
            options.UseGrayscale = true;
            var stats = Shrinker.Run(options, Console.WriteLine);
            Console.WriteLine("Archives modified: {0}; textures processed: {1}",
                stats.ArchivesModified, stats.TexturesShrunken);
            return stats.ErrorsEncountered == 0 && stats.TexturesFailed == 0 && stats.ArchivesFailed == 0 ? 0 : 1;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }
}
