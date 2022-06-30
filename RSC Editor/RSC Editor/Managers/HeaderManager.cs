using RSC_Editor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSC_Editor.Managers
{
    public class HeaderManager
    {
        public static Header _header = new Header();

        public static void GetHeader(BinaryReader reader)
        {
            _header.FileType = Encoding.GetEncoding(1252).GetString(reader.ReadBytes(4));
            _header.ItemsCount = reader.ReadInt32();
			Console.WriteLine("Header:\nFileType: {0}\nItemsCount:{1}", _header.FileType, _header.ItemsCount);
        }
    }
}
