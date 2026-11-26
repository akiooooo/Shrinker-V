namespace GameFormats
{
	public abstract class Cacheable<TKey>
	{
		public TKey Key;

		public long MemoryUsage;
	}
}
