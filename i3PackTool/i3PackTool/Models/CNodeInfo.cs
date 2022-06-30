using System.Runtime.InteropServices;

namespace i3PackTool.Models
{
	public class CNodeInfo
    {
		public int Type;
		public long Index;
		public long Offset;
		public long Size;

		//char _0x0000[4];
		//DWORD dwIndex; //0x0004 
		//char _0x0008[4];
		//DWORD dwOffset; //0x000C 
		//char _0x0010[4];
		//DWORD dwSize; //0x0014 
		//char _0x0018[4];

	} // Size = 0x001C
}
