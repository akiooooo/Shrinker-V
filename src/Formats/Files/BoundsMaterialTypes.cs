using System.Collections.Generic;

namespace GameFormats.Files
{
	public static class BoundsMaterialTypes
	{
		public static List<BoundsMaterialData> Materials;

		public static BoundsMaterialData GetMaterial(BoundsMaterialType type)
		{
			if (Materials == null)
			{
				return null;
			}
			if (type.Index >= Materials.Count)
			{
				return null;
			}
			return Materials[type.Index];
		}

		public static string GetMaterialName(BoundsMaterialType type)
		{
			BoundsMaterialData material = GetMaterial(type);
			if (material == null)
			{
				return string.Empty;
			}
			return material.Name;
		}
	}
}
