namespace GameFormats.Files
{
	public class ResourceSystemDataBlock : ResourceSystemBlock
	{
		public byte[] Data { get; set; }

		public int DataLength { get; set; }

		public override long BlockLength => (Data != null) ? Data.Length : DataLength;

		public ResourceSystemDataBlock(byte[] data)
		{
			Data = data;
			DataLength = ((Data != null) ? Data.Length : 0);
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Data = reader.ReadBytes(DataLength);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Data);
		}
	}
}
