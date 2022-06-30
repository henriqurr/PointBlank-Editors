using System.Globalization;

namespace i3PackTool
{
    public class Utils
    {
        public static string DecimalToHex(int dec)
        {
            return dec.ToString("X");
        } 

        public static int HexToDecimal(string hex)
        {
            return int.Parse(hex, NumberStyles.HexNumber);
        }
	}
}
