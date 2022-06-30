using PropertyEditor.Models;
using System;
using System.IO;
using System.Text;

namespace PropertyEditor.Managers
{
    public class HeaderManager
    {
        public static Header _header = new Header();
        public static void GetPefHeader(BinaryReader reader)
        {
            Console.WriteLine("Loading header...");
            _header.ResourceType = Encoding.GetEncoding(Settings.Encoding).GetString(reader.ReadBytes(4));
            _header.Unk = reader.ReadInt32();
            _header.VersionMajor = reader.ReadUInt16();
            _header.VersionMinor = reader.ReadUInt16();
            _header.StringTableCount = reader.ReadInt32();
            _header.StringTableOffset = reader.ReadUInt64();
            _header.StringTableSizes = reader.ReadUInt64();
            _header.ObjectInfoCount = reader.ReadInt32();
            _header.ObjectInfoOffset = reader.ReadUInt64();
            _header.ObjectInfoSize = reader.ReadUInt64();
        }

        public static void WriteHeader(BinaryWriter bw)
        {
            Console.WriteLine("Writting header...");
            bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes(_header.ResourceType));
            bw.Write((int)_header.Unk);
            bw.Write((ushort)_header.VersionMajor);
            bw.Write((ushort)_header.VersionMinor);
            bw.Write((int)_header.StringTableCount);
            bw.Write((ulong)_header.StringTableOffset);
            bw.Write((ulong)_header.StringTableSizes);
            bw.Write((int)_header.ObjectInfoCount);
            bw.Write((ulong)_header.ObjectInfoOffset);
            bw.Write((ulong)_header.ObjectInfoSize);
            bw.Write(new byte[132]); // credits
        }
    }
}
