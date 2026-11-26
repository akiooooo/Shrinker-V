using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	public struct GameFileCacheKey
	{


		public uint Hash { get; }
        public GameFileType Type { get; }
	}
}
