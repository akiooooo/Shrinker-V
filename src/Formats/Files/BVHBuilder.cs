using System.Collections.Generic;
using System.Linq;
using GameMath;

namespace GameFormats.Files
{
	public class BVHBuilder
	{
		public static int MaxNodeItemCount = 4;

		public static int MaxTreeNodeCount = 127;

		public static BVH Build(List<BVHBuilderItem> items, int itemThreshold)
		{
			if (items == null)
			{
				return null;
			}
			BVH bVH = new BVH();
			Vector3 vector = new Vector3(float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue);
			List<BVHBuilderNode> list = new List<BVHBuilderNode>();
			List<BVHBuilderNode> list2 = new List<BVHBuilderNode>();
			List<BVHBuilderItem> list3 = new List<BVHBuilderItem>();
			for (int i = 0; i < items.Count; i++)
			{
				BVHBuilderItem bVHBuilderItem = items[i];
				if (bVHBuilderItem != null)
				{
					list3.Add(bVHBuilderItem);
					vector = Vector3.Min(vector, bVHBuilderItem.Min);
					vector2 = Vector3.Max(vector2, bVHBuilderItem.Max);
				}
			}
			Vector3 vector3 = (vector + vector2) * 0.5f;
			bVH.BoundingBoxMin = new Vector4(vector, float.NaN);
			bVH.BoundingBoxMax = new Vector4(vector2, float.NaN);
			bVH.BoundingBoxCenter = new Vector4(vector3, float.NaN);
			bVH.Quantum = new Vector4(Vector3.Max((vector - vector3).Abs(), (vector2 - vector3).Abs()) / 32767f, float.NaN);
			bVH.QuantumInverse = new Vector4(1f / bVH.Quantum.XYZ(), float.NaN);
			BVHBuilderNode bVHBuilderNode = new BVHBuilderNode();
			bVHBuilderNode.Items = list3.ToList();
			bVHBuilderNode.Build(itemThreshold);
			bVHBuilderNode.GatherNodes(list);
			bVHBuilderNode.GatherTrees(list2);
			if (itemThreshold > 1)
			{
				items.Clear();
				foreach (BVHBuilderNode item2 in list)
				{
					if (item2.Items == null)
					{
						continue;
					}
					foreach (BVHBuilderItem item3 in item2.Items)
					{
						item3.Index = items.Count;
						items.Add(item3);
					}
				}
			}
			List<BVHTreeInfo_s> list4 = new List<BVHTreeInfo_s>();
			List<BVHNode_s> list5 = new List<BVHNode_s>();
			Vector3 vector4 = bVH.QuantumInverse.XYZ();
			Vector3 vector5 = bVH.BoundingBoxCenter.XYZ();
			for (int j = 0; j < list.Count; j++)
			{
				BVHBuilderNode bVHBuilderNode2 = list[j];
				int num = (((bVHBuilderNode2.Items?.Count ?? 0) > 0) ? bVHBuilderNode2.Items[0].Index : 0);
				int totalNodes = bVHBuilderNode2.TotalNodes;
				list5.Add(new BVHNode_s
				{
					Min = (bVHBuilderNode2.Min - vector5) * vector4,
					Max = (bVHBuilderNode2.Max - vector5) * vector4,
					ItemCount = (short)((totalNodes <= 1) ? bVHBuilderNode2.TotalItems : 0),
					ItemId = (short)((totalNodes <= 1) ? num : bVHBuilderNode2.TotalNodes)
				});
			}
			for (int k = 0; k < list2.Count; k++)
			{
				BVHBuilderNode bVHBuilderNode3 = list2[k];
				list4.Add(new BVHTreeInfo_s
				{
					Min = (bVHBuilderNode3.Min - vector5) * vector4,
					Max = (bVHBuilderNode3.Max - vector5) * vector4,
					NodeIndex1 = (short)bVHBuilderNode3.Index,
					NodeIndex2 = (short)(bVHBuilderNode3.Index + bVHBuilderNode3.TotalNodes)
				});
			}
			int count = list5.Count;
			if (itemThreshold <= 1)
			{
				int num2 = items.Count * 2 + 1;
				BVHNode_s item = new BVHNode_s
				{
					ItemId = 1
				};
				while (list5.Count < num2)
				{
					list5.Add(item);
				}
			}
			bVH.Nodes = new ResourceSimpleList64b_s<BVHNode_s>();
			bVH.Nodes.data_items = list5.ToArray();
			bVH.Nodes.EntriesCount = (uint)count;
			bVH.Trees = new ResourceSimpleList64_s<BVHTreeInfo_s>();
			bVH.Trees.data_items = list4.ToArray();
			return bVH;
		}
	}
}
