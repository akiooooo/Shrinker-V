using System;
using System.Collections.Generic;
using GameMath;

namespace GameFormats.Files
{
	public class BVHBuilderNode
	{
		public List<BVHBuilderNode> Children;

		public List<BVHBuilderItem> Items;

		public Vector3 Min;

		public Vector3 Max;

		public int Index;

		public int TotalNodes
		{
			get
			{
				int num = 1;
				if (Children != null)
				{
					foreach (BVHBuilderNode child in Children)
					{
						num += child.TotalNodes;
					}
				}
				return num;
			}
		}

		public int TotalItems
		{
			get
			{
				int num = Items?.Count ?? 0;
				if (Children != null)
				{
					foreach (BVHBuilderNode child in Children)
					{
						num += child.TotalItems;
					}
				}
				return num;
			}
		}

		public void Build(int itemThreshold)
		{
			UpdateMinMax();
			if (Items == null || Items.Count <= itemThreshold)
			{
				return;
			}
			Vector3 zero = Vector3.Zero;
			foreach (BVHBuilderItem item in Items)
			{
				zero += item.Min;
				zero += item.Max;
			}
			Vector3 vector = zero * (0.5f / (float)Items.Count);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (BVHBuilderItem item2 in Items)
			{
				Vector3 vector2 = (item2.Min + item2.Max) * 0.5f;
				if (vector2.X < vector.X)
				{
					num++;
				}
				if (vector2.Y < vector.Y)
				{
					num2++;
				}
				if (vector2.Z < vector.Z)
				{
					num3++;
				}
			}
			float num4 = (float)Items.Count / 2f;
			float num5 = Math.Abs(num4 - (float)num);
			float num6 = Math.Abs(num4 - (float)num2);
			float num7 = Math.Abs(num4 - (float)num3);
			int num8 = -1;
			num8 = ((!(num5 <= num6) || !(num5 <= num7)) ? ((num6 <= num7) ? 1 : 2) : 0);
			List<BVHBuilderItem> list = new List<BVHBuilderItem>();
			List<BVHBuilderItem> list2 = new List<BVHBuilderItem>();
			foreach (BVHBuilderItem item3 in Items)
			{
				Vector3 vector3 = (item3.Min + item3.Max) * 0.5f;
				bool flag = false;
				switch (num8)
				{
				default:
					flag = vector3.X > vector.X;
					break;
				case 1:
					flag = vector3.Y > vector.Y;
					break;
				case 2:
					flag = vector3.Z > vector.Z;
					break;
				}
				if (flag)
				{
					list.Add(item3);
				}
				else
				{
					list2.Add(item3);
				}
			}
			if (list.Count == 0 || list2.Count == 0)
			{
				List<BVHBuilderItem> list3 = new List<BVHBuilderItem>();
				list3.AddRange(list);
				list3.AddRange(list2);
				if (list3.Count <= 0)
				{
					return;
				}
				list3.Sort(delegate(BVHBuilderItem a, BVHBuilderItem b)
				{
					int num12 = a.Min.CompareTo(b.Min);
					return (num12 != 0) ? num12 : a.Max.CompareTo(b.Max);
				});
				list.Clear();
				list2.Clear();
				int num9 = list3.Count / 2;
				for (int num10 = 0; num10 < num9; num10++)
				{
					list.Add(list3[num10]);
				}
				for (int num11 = num9; num11 < list3.Count; num11++)
				{
					list2.Add(list3[num11]);
				}
			}
			Items = null;
			Children = new List<BVHBuilderNode>();
			BVHBuilderNode bVHBuilderNode = new BVHBuilderNode();
			bVHBuilderNode.Items = list;
			bVHBuilderNode.Build(itemThreshold);
			Children.Add(bVHBuilderNode);
			BVHBuilderNode bVHBuilderNode2 = new BVHBuilderNode();
			bVHBuilderNode2.Items = list2;
			bVHBuilderNode2.Build(itemThreshold);
			Children.Add(bVHBuilderNode2);
			Children.Sort((BVHBuilderNode a, BVHBuilderNode b) => b.TotalItems.CompareTo(a.TotalItems));
		}

		public void UpdateMinMax()
		{
			Vector3 vector = new Vector3(float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue);
			if (Items != null)
			{
				foreach (BVHBuilderItem item in Items)
				{
					vector = Vector3.Min(vector, item.Min);
					vector2 = Vector3.Max(vector2, item.Max);
				}
			}
			if (Children != null)
			{
				foreach (BVHBuilderNode child in Children)
				{
					child.UpdateMinMax();
					vector = Vector3.Min(vector, child.Min);
					vector2 = Vector3.Max(vector2, child.Max);
				}
			}
			Min = vector;
			Max = vector2;
		}

		public void GatherNodes(List<BVHBuilderNode> nodes)
		{
			Index = nodes.Count;
			nodes.Add(this);
			if (Children == null)
			{
				return;
			}
			foreach (BVHBuilderNode child in Children)
			{
				child.GatherNodes(nodes);
			}
		}

		public void GatherTrees(List<BVHBuilderNode> trees)
		{
			if (TotalNodes > BVHBuilder.MaxTreeNodeCount && (Children?.Count ?? 0) > 0)
			{
				foreach (BVHBuilderNode child in Children)
				{
					child.GatherTrees(trees);
				}
				return;
			}
			trees.Add(this);
		}

		public override string ToString()
		{
			string text = ((Children != null) ? (TotalNodes + ", 0 - ") : ((Items != null) ? ("i, " + TotalItems + " - ") : "error!"));
			string text2 = ((Children != null) ? (Children.Count + " children") : "");
			string text3 = ((Items != null) ? (Items.Count + " items") : "");
			if (string.IsNullOrEmpty(text2))
			{
				return text + text3;
			}
			if (string.IsNullOrEmpty(text3))
			{
				return text + text2;
			}
			return text2 + ", " + text3;
		}
	}
}
