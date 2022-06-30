using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i3PackTool.Models
{
    public class CHeaderNodeInfo
    {
		public ulong BaseAddr;
		public ulong Index;
		public ulong Offset;
		public ulong Size;

		public CHeaderNodeInfo(ulong ba, ulong idx, ulong off, ulong sz)
		{
			BaseAddr = ba;
			Index = idx;
			Offset = off;
			Size = sz;
		}
	}
}
