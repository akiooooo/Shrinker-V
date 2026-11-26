namespace GameFormats.Files
{
	public interface IResourceXXSystemBlock : IResourceSystemBlock, IResourceBlock
	{
		IResourceSystemBlock GetType(ResourceDataReader reader, params object[] parameters);
	}
}
