using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i3PackTool.Models
{
    public class CNodeFileInfo
    {
		public string Filename;
		public ulong Offset;
		public ulong Size;
		public ulong RawOffset;
		public bool Padded;
		public int DecType;
		public ushort FileDecInfo;

		public CNodeFileInfo(string fn, ulong off, ulong sz)
		{
			Filename = fn;
			Offset = off;
			Size = sz;
		}

		public CNodeFileInfo(string fn, ulong off, ulong sz, ulong ro, bool pad, int decType, ushort fileDecInfo)
		{
			Filename = fn;
			Offset = off;
			Size = sz;
			RawOffset = ro;
			Padded = pad;
			DecType = decType;
			FileDecInfo = fileDecInfo;
		}
	}
}
