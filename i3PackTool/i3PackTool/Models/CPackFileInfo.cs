namespace i3PackTool.Models
{
	public class CPackFileInfo
    {
		public string Filename; //32
		public byte[] _0x0020;
		public ushort N0193C181;
		public ushort SizeOr_1;
		public ushort OffsShift_1;
		public ushort SizeShift_1;
		public int _0x003C;
		public ushort OffsOr_1;
		public int N01970D81;
		public ushort SizeOr_2;
		public ushort OffsShift_2;
		public ushort SizeShift_2;
		public int N01A26723;
		public ushort OffsOr_2;
		public byte[] N019E3220;
		public uint Ended;

		//char szFilename[32]; //0x0000 
		//char _0x0020[20];
		//char N0193C181[2]; //0x0034 
		//WORD SizeOr_1; //0x0036 52
		//WORD OffsShift_1; //0x0038 54
		//WORD SizeShift_1; //0x003A 56
		//char _0x003C[4];
		//WORD OffsOr_1; //0x0040 5c
		//char N01970D81[4]; //0x0042 
		//WORD SizeOr_2; //0x0046 52
		//WORD OffsShift_2; //0x0048 54
		//WORD SizeShift_2; //0x004A 56
		//char N01A26723[4]; //0x004C 
		//WORD OffsOr_2; //0x0050 5c
		//char N019E3220[3]; //0x0052 
		//DWORD Ended; //0x0055

	} //Size= 0x005D
}
