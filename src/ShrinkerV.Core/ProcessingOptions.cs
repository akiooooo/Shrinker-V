using System;
using System.Threading;

namespace ShrinkerV.Core
{
    public sealed class ProcessingOptions
    {
        public string InputFolder       { get; set; }
        public string OutputFolder      { get; set; }
        public bool   ProcessInPlace    { get; set; }
        public int    CompressionFactor { get; set; }
        public bool   UseGrayscale      { get; set; }
        public bool   SkipWeapons       { get; set; }
        public int    MinTextureSize    { get; set; }

        // Separate settings for clothing/peds
        public int    ClothingCompressionFactor { get; set; }
        public bool   ClothingUseGrayscale      { get; set; }
        public int    PedsCompressionFactor     { get; set; }
        public bool   PedsUseGrayscale          { get; set; }

        // Map removal
        public bool   RemoveMap                 { get; set; }
        public bool ProtectCharacters { get; set; }
        public bool SkipPeds { get; set; }
        public bool KeepAudio { get; set; }
        public bool KeepVideo { get; set; }
        public bool KeepEffects { get; set; }

        // Подробный лог на каждую текстуру. Выключен = в разы быстрее
        // (при 100k текстур это 200k строк с форматированием + запись в файл).
        public bool   VerboseLog        { get; set; }
        // Степень параллелизма для обработки текстур внутри одного словаря.
        public int    MaxThreads        { get; set; }
        public CancellationToken Cancel { get; set; }

        internal ProcessingOptions Copy() { return (ProcessingOptions)MemberwiseClone(); }

        public ProcessingOptions()
        {
            ProtectCharacters = KeepAudio = KeepVideo = KeepEffects = SkipWeapons = true;
            CompressionFactor = 8;
            ClothingCompressionFactor = 8;
            PedsCompressionFactor = 8;
            MinTextureSize    = 4;
            VerboseLog   = false;
            MaxThreads   = Math.Max(1, Environment.ProcessorCount);
            Cancel = CancellationToken.None;
        }
    }
}
