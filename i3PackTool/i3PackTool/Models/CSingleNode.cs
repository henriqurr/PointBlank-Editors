using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i3PackTool.Models
{
	public class CSingleNode
	{
		public string NodeName;
		public ulong Index;
		public ulong Offset;
		public ulong Size;
		public ulong FileCount;
		public ulong DirTableOffs;

		// tree node info
		public List<int> ChildId = new List<int>();

		// file info
		public List<CNodeFileInfo> Files = new List<CNodeFileInfo>();

		public CSingleNode(string nn, ulong idx, ulong off, ulong sz, ulong fc)
		{
			Index = idx;
			NodeName = nn;
			Offset = off;
			Size = sz;
			FileCount = fc;
		}

		public bool HasChild()
		{
			return ChildId.Count != 0;
		}

		public bool IsLeaf()
		{
			return ChildId.Count == 0;
		}

		public bool IsRoot()
		{
			return NodeName == "/";
		}
	}
}