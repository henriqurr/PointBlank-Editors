using i3PackTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i3PackTool.Managers
{
    public class HeaderManager
    {
        public static CPackHeader _header = new CPackHeader();
        public static List<CHeaderNodeInfo> m_pvHeaderDirInfo = new List<CHeaderNodeInfo>();

        public static void GetHeader(Reader reader)
        {
            _header.HeaderID = reader.ReadLong();
            _header.VersionMajor = reader.ReadShort();
            _header.VersionMinor = reader.ReadShort();
            _header.StringTableCount = reader.ReadInt();
            _header.StringTableOffset = reader.ReadLong();
            _header.StringTableSize = reader.ReadLong();
            _header.NodeCount = reader.ReadInt();
            _header.DirTableOffset = reader.ReadLong();
            _header.DirTableSize = reader.ReadLong();
            _header.NodeSize = reader.ReadLong();
            _header.N00D72CC0 = reader.ReadBytes(16);
            _header._0x004C = reader.ReadBytes(108);
        }
    }
}
