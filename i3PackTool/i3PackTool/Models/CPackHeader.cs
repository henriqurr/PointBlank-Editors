namespace i3PackTool.Models
{
	public class CPackHeader
    {
		public long HeaderID;
		public short VersionMajor;
		public short VersionMinor;
		public int StringTableCount;
		public long StringTableOffset;
		public long StringTableSize;
		public int NodeCount;
		public long DirTableOffset;
		public long DirTableSize;
		public long NodeSize;
		public byte[] N00D72CC0; //0x003C 
		public byte[] _0x004C; 

		//char szHeaderID[8]; //0x0000 
		//DWORD N00CE995A; //0x0008 
		//DWORD N00CE995B; //0x000C 
		//DWORD dwStringTableOff; //0x0010 
		//char _0x0014[4];
		//DWORD dwStringTableSize; //0x0018 
		//char _0x001C[4];
		//DWORD dwNodeCount; //0x0020 
		//DWORD dwDirTableOff; //0x0024 
		//char _0x0028[4];
		//DWORD dwDirTableSize; //0x002C 
		//char _0x0030[4];
		//DWORD dwNodeSize; //0x0034 
		//char _0x0038[4];
		//char N00D72CC0[16]; //0x003C 
		//char _0x004C[108];
	}
}
