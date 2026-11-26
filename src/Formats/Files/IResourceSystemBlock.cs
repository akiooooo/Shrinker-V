using System;

namespace GameFormats.Files
{
	public interface IResourceSystemBlock : IResourceBlock
	{
		Tuple<long, IResourceBlock>[] GetParts();

		IResourceBlock[] GetReferences();
	}
}
