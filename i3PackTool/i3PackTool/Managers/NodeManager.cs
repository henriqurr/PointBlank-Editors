using i3PackTool.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace i3PackTool.Managers
{
    public class NodeManager
    {
		public static List<CSingleNode> m_pvPackNodes = new List<CSingleNode>();

		public static bool GetNodeInfos(Reader reader, CPackHeader header)
        {
			try
			{
				for (int i = 0; i < header.NodeSize; i++)
				{
					byte[] pNodeBuffer = new byte[0x001C];
					long ulDirOffs = 0;
					if (!GetDirectoryInfoById(reader, ref pNodeBuffer, i, ref ulDirOffs))
					{
						throw new Exception("CollectNodesInfo: GetDirectoryInfoById failed.");
					}

					reader._offset = (int)ulDirOffs;
					CNodeInfo pNode = new CNodeInfo
					{
						Type = reader.ReadInt(),
						Index = reader.ReadLong(),
						Offset = reader.ReadLong(),
						Size = reader.ReadLong()
					};
					Console.WriteLine(JsonConvert.SerializeObject(pNode, Formatting.Indented));
					byte[] pBufferNode = new byte[pNode.Size];
					Array.Copy(reader._buffer, pNode.Offset, pBufferNode, 0, pNode.Size);
					int ulNameSize = (byte)BitConverter.ToInt32(reader._buffer, (int)pNode.Offset);
					string csNodeName = Encoding.GetEncoding(1252).GetString(reader._buffer, (int)pNode.Offset + 1, ulNameSize);
					int ulBaseOffs = ulNameSize + 1 + 76;
					List<int> vHasChild = new List<int>();
					ulong ulPaddedOffs = 0;
					
					for (int j = 0; ; j++)
					{
						uint ulChkChild = BitConverter.ToUInt32(reader._buffer, (int)pNode.Offset + (int)ulBaseOffs + (j * 4));
						Console.WriteLine("ulChkChild:" + ulChkChild + " dwNodeSize:" + header.NodeSize);
						if (ulChkChild <= header.NodeSize)
						{
							vHasChild.Add((int)ulChkChild);
							ulPaddedOffs += 4;
						}
						else
						{
							break;
						}
					}
				
					// copy encrypted info to buffer, then decrypt it to get file count
					byte[] ucFileInfo = new byte[8];
					Array.Copy(reader._buffer, (long)((ulong)pNode.Offset + (ulong)ulBaseOffs + (ulong)ulPaddedOffs), ucFileInfo, 0, 8);
					BitRotate.Unshift(ucFileInfo, 8, 3);
					ulong ulFileCount = (ulong)BitConverter.ToInt32(ucFileInfo, 4);
					CSingleNode Node = new CSingleNode(csNodeName, (ulong)pNode.Index, (ulong)pNode.Offset, (ulong)pNode.Size, ulFileCount);
					if (vHasChild.Count != 0)
						Node.ChildId = vHasChild;
					// file process
					int ucDecInfo = BitConverter.ToInt32(ucFileInfo, 0);

					if (ucDecInfo != 860767824) // old decrypt
					{
						bool bPadded = false;
						for (int j = 0; j < (int)ulFileCount; j++)
						{
							uint ulRawOffs = (uint)(ulBaseOffs + (int)ulPaddedOffs + 8 + (j * (bPadded ? 0x005C : 0x005C - 16)));
							//uint ulOffs = BitConverter.ToUInt32(reader._buffer, (int)pNode.Offset + (int)ulRawOffs); //(int)pNode.Offset + (int)ulRawOffs
							//Console.WriteLine("ulOffs " + ulOffs); //só falta corrigir daqui pra baixo

							byte[] pFileInfoBuffer = new byte[0x005C];
							Array.Copy(reader._buffer, (long)((ulong)pNode.Offset + (ulong)ulRawOffs), pFileInfoBuffer, 0, (long)pFileInfoBuffer.Length);
							Console.WriteLine("" + (long)((ulong)pNode.Offset + (ulong)ulRawOffs), pFileInfoBuffer);
							//fixar novos arquivos daqui pra baixo
							uint Ended = BitConverter.ToUInt32(pFileInfoBuffer, 88);
							if (Ended == 0x01000000)
								bPadded = true;
							else
								bPadded = false;

							BitRotate.Unshift(pFileInfoBuffer, (bPadded ? 0x005C : 0x005C - 16), 2);
							Reader t = new Reader(pFileInfoBuffer);
							CPackFileInfo pFileInfo = new CPackFileInfo()
							{
								Filename = t.ReadString(32),
								_0x0020 = t.ReadBytes(20),
								N0193C181 = t.ReadUShort(),
								SizeOr_1 = t.ReadUShort(),
								OffsShift_1 = t.ReadUShort(),
								SizeShift_1 = t.ReadUShort(),
								_0x003C = t.ReadInt(),
								OffsOr_1 = t.ReadUShort(),
								N01970D81 = t.ReadInt(),
								SizeOr_2 = t.ReadUShort(),
								OffsShift_2 = t.ReadUShort(),
								SizeShift_2 = t.ReadUShort(),
								N01A26723 = t.ReadInt(),
								OffsOr_2 = t.ReadUShort(),
								N019E3220 = t.ReadBytes(3),
								Ended = t.ReadUInt(),
							}; // f7 3f95b7 // 3f9603

							Console.WriteLine(JsonConvert.SerializeObject(pFileInfo, Formatting.Indented));

							ulong ulPushOffset = (ulong)(bPadded ? (pFileInfo.OffsShift_2 << 0x10) | pFileInfo.OffsOr_2 : (pFileInfo.OffsShift_1 << 0x10) | pFileInfo.OffsOr_1);
							ulong ulPushSize = (ulong)(bPadded ? (pFileInfo.SizeShift_2 << 0x10) | pFileInfo.SizeOr_2 : (pFileInfo.SizeShift_1 << 0x10) | pFileInfo.SizeOr_1);

							Node.Files.Add(new CNodeFileInfo(pFileInfo.Filename,
													   ulPushOffset,
													   ulPushSize,
													   ulRawOffs,
													   bPadded,
													   -1,
													   0
													  ));
						}
					}
					else // new decrypt
					{
						reader._offset = (int)((ulong)pNode.Offset + (ulong)ulBaseOffs + (ulong)ulPaddedOffs) + 8;
						ushort ucFileDecInfo = reader.ReadUShort(); // new shift

						for (int j = 0; j < (int)ulFileCount; j++)
						{
							uint ulRawOffs = (uint)(ulBaseOffs + (int)ulPaddedOffs + 8 + 2 + (j * 0x005C));
							//uint ulOffs = BitConverter.ToUInt32(reader._buffer, (int)pNode.Offset + (int)ulRawOffs); //(int)pNode.Offset + (int)ulRawOffs
							//Console.WriteLine("ulOffs " + ulOffs); //só falta corrigir daqui pra baixo

							byte[] pFileInfoBuffer = new byte[0x005C];
							Array.Copy(reader._buffer, (long)((ulong)pNode.Offset + (ulong)ulRawOffs), pFileInfoBuffer, 0, (long)pFileInfoBuffer.Length);

							Console.WriteLine("" + (long)((ulong)pNode.Offset + (ulong)ulRawOffs), pFileInfoBuffer);

							i3ResourceFile.Decrypt(pFileInfoBuffer, pFileInfoBuffer, ucFileDecInfo, pFileInfoBuffer.Length);
							Reader t = new Reader(pFileInfoBuffer);
							CPackFileInfo pFileInfo = new CPackFileInfo()
							{
								Filename = t.ReadString(32),
								_0x0020 = t.ReadBytes(20),
								N0193C181 = t.ReadUShort(),
								SizeOr_1 = t.ReadUShort(),
								OffsShift_1 = t.ReadUShort(),
								SizeShift_1 = t.ReadUShort(),
								_0x003C = t.ReadInt(),
								OffsOr_1 = t.ReadUShort(),
								N01970D81 = t.ReadInt(),
								SizeOr_2 = t.ReadUShort(),
								OffsShift_2 = t.ReadUShort(),
								SizeShift_2 = t.ReadUShort(),
								N01A26723 = t.ReadInt(),
								OffsOr_2 = t.ReadUShort(),
								N019E3220 = t.ReadBytes(3),
								Ended = t.ReadUInt()
							}; // f7 3f95b7 // 3f9603

							Console.WriteLine(JsonConvert.SerializeObject(pFileInfo, Formatting.Indented));

							ulong ulPushOffset = (ulong)pFileInfo.OffsShift_2 << 0x10 | pFileInfo.OffsOr_2;
							ulong ulPushSize = (ulong)pFileInfo.SizeShift_2 << 0x10 | pFileInfo.SizeOr_2;

							Node.Files.Add(new CNodeFileInfo(pFileInfo.Filename,
													   ulPushOffset,
													   ulPushSize,
													   ulRawOffs,
													   false,
													   ucDecInfo,
													   ucFileDecInfo
													  ));
						}
					}
					Node.DirTableOffs = (ulong)ulDirOffs;
					m_pvPackNodes.Add(Node);
					HeaderManager.m_pvHeaderDirInfo.Add(new CHeaderNodeInfo((ulong)ulDirOffs, (ulong)pNode.Index, (ulong)pNode.Offset, (ulong)pNode.Size));
					Console.WriteLine(JsonConvert.SerializeObject(Node, Formatting.Indented));
				}
			}
			catch( Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return false;
			}
			return true;
        }

		public static CSingleNode GetNodeById(ulong id)
        {
			for(int i = 0; i < m_pvPackNodes.Count; i++)
            {
				CSingleNode node = m_pvPackNodes[i];
				if (node.Index == id)
				{
					return node;
				}
			}
			return null;
		}

        public static bool GetDirectoryInfoById(Reader reader, ref byte[] pNodeBuffer, int index, ref long offset)
        {

			try
			{
				long ulOff = HeaderManager._header.DirTableOffset + (index * 0x001C); // 0x001C CNodeInfo Size
				reader._offset = (int)ulOff;
				pNodeBuffer = reader.ReadBytes(0x001C);
				offset = ulOff;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return false;
			}
			return true;
		}
    }
}
