using RSC_Editor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSC_Editor.Managers
{
    public class ItemsManager
    {
        public static List<Items> _items = new List<Items>();
        public static void GetItems(BinaryReader reader, Header header)
        {
            for(int i = 0; i < header.ItemsCount; i++)
            {
                int rsc1 = reader.ReadInt32();
                int type = reader.ReadInt32();
                int fileSize = reader.ReadInt32();
                int fullFileSize = reader.ReadInt32();
                reader.ReadInt32();
                string filename = Encoding.GetEncoding(1252).GetString(reader.ReadBytes(fileSize));
                reader.ReadByte();
                string fullFileName = Encoding.GetEncoding(1252).GetString(reader.ReadBytes(fullFileSize));
                if (type != 7)
                    reader.ReadBytes(133);
                else
                    reader.ReadByte();

                //Console.WriteLine("Size:{1} Filename:{2} Fullfile:{3}", rsc1, type, filename, fullFileName);
                Items item = new Items
                {
                    Type = type,
                    FileSize = fileSize,
                    FullFileSize = fullFileSize,
                    Filename = filename,
                    Fullfilename = fullFileName
                };
                _items.Add(item);
            }
        }
    }
}
