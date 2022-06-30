using PropertyEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PropertyEditor.Managers
{
    public class StringTablesManager
    {
        public static List<StringTable> _stringTables = new List<StringTable>();
        public static string lastReg;

        public static void GetStringTables(BinaryReader br, Header header)
        {
            br.BaseStream.Position = (long)header.StringTableOffset;
            byte[] stringTables = br.ReadBytes((int)header.StringTableSizes);
            using(StreamReader reader = new StreamReader(new MemoryStream(stringTables)))
            {
                for (int i = 0; i < header.StringTableCount; i++)
                {
                    string str = reader.ReadLine();
                    if (!string.IsNullOrEmpty(str))
                    {
                        if(str.Length > 4  && str.Substring(0, 2) == "i3") //old i3Reg
                        {
                            lastReg = str;
                            _stringTables.Add(new StringTable
                            {
                                Name = str,
                                Values = new List<string>()
                            }); 
                        }
                        else
                        {
                            for(int j = 0; j < _stringTables.Count; j++)
                            {
                                StringTable item = _stringTables[j];
                                if(item.Name == lastReg)
                                {
                                    item.Values.Add(str);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void WriteStringTables(BinaryWriter bw) 
        {
            Console.WriteLine("Writting StringTables...");
            for(int i = 0; i < _stringTables.Count; i++)
            {
                var stringTable = _stringTables[i];
                bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes(stringTable.Name));
                if ((i + 1) == _stringTables.Count && Program._propertyEditor.isEnvSet)
                {
                    break;
                }
                bw.Write((ushort)2573);
                for (int j = 0; j < stringTable.Values.Count; j++)
                {
                    var item = stringTable.Values[j];
                    bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes(item));
                    if ((i + 1) == _stringTables.Count && (j + 1) == stringTable.Values.Count)
                    {
                        continue;
                    }
                    bw.Write((ushort)2573);
                }
                Application.DoEvents();
            }
        }
    }
}
