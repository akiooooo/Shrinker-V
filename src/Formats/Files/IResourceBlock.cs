namespace GameFormats.Files
{
	public interface IResourceBlock
	{
		long FilePosition { get; set; }

		long BlockLength { get; }

		long BlockLength_Gen9 { get; }

		void Read(ResourceDataReader reader, params object[] parameters);

		void Write(ResourceDataWriter writer, params object[] parameters);
	}
}
