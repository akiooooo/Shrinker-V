namespace ShrinkerV.Core
{
    public sealed class ProcessingStats
    {
        public int  ArchivesProcessed;
        public int  ArchivesModified;
        public int  ArchivesFailed;
        public int  TexturesShrunken;
        public int  TexturesFailed;
        public int  TexturesSkipped;
        public int  AwcReplaced;
        public int  ErrorsEncountered;
        public long BytesBefore;
        public long BytesAfter;
    }
}
