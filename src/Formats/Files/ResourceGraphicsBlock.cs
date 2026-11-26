namespace GameFormats.Files
{
	public abstract class ResourceGraphicsBlock : IResourceGraphicsBlock, IResourceBlock
	{
		public virtual long FilePosition { get; set; }

		public abstract long BlockLength { get; }

		public virtual long BlockLength_Gen9 => BlockLength;

		public abstract void Read(ResourceDataReader reader, params object[] parameters);

		public abstract void Write(ResourceDataWriter writer, params object[] parameters);
	}
}
