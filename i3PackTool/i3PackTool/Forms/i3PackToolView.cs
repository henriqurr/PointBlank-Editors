using i3PackTool.Forms;
using i3PackTool.Managers;
using i3PackTool.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace i3PackTool
{
    public partial class i3PackToolView : Form
    {
        public Reader _reader;
        public string filePath;
        public i3PackToolView()
        {
            InitializeComponent();
        }

        private void i3PackToolView_Load(object sender, EventArgs e)
        {
            //Console.WriteLine("" + Marshal.SizeOf(c.Index));
            
        }

        //public T ByteArrayToObject<T>(byte[] buffer)
        //{
        //    if (buffer == null)
        //        return default(T);

        //    using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
        //    {
        //        foreach (var item in typeof(CNodeInfo).GetFields())
        //        {
        //            br.ReadBytes(Marshal.SizeOf(item.FieldType));
        //            Console.WriteLine();
        //        }
        //    }
        //    return (T)default(T);
        //}

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdPath.ShowDialog() == DialogResult.OK)
            {
                OpenFile(ofdPath.FileName);
            }
        }

        public void OpenFile(string path)
        {
            FullClear();

            filePath = path;
            lbPath.Text = "Path: " + path;
            cmsPathFile.Items[0].Enabled = true;
            cmsPathFile.Items[1].Enabled = true;

            byte[] buffer = File.ReadAllBytes(path);
            _reader = new Reader(buffer);
            HeaderManager.GetHeader(_reader);
            Console.WriteLine(JsonConvert.SerializeObject(HeaderManager._header, Formatting.Indented));
            StringTableManager.GetStringTables(_reader, HeaderManager._header);
            if (NodeManager.GetNodeInfos(_reader, HeaderManager._header))
            {
                treeView1.Nodes.Clear();
                TreeNode hti = new TreeNode();
                ulong ulTreeIndex = 0;
                bool bRoot = false;
                for (int i = NodeManager.m_pvPackNodes.Count - 1; i >= 0; i--)
                {
                    CSingleNode pNode = NodeManager.m_pvPackNodes[i];
                    ulTreeIndex = (ulong)i;

                    if (pNode.IsRoot())
                    {
                        Console.WriteLine("Root:" + pNode.NodeName);
                        hti = treeView1.Nodes.Add(pNode.Index.ToString(), pNode.NodeName);
                        bRoot = true;
                    }
                    else
                    {
                        if (pNode.IsLeaf())
                        {
                            Console.WriteLine("IsLeaf:" + pNode.NodeName);
                            TreeNode _node = hti.Nodes.Add(pNode.Index.ToString(), pNode.NodeName);
                            treeView1.SelectedNode = _node;
                            //if (pNode.FileCount > 0)
                            //treeView1.SetItemState(_hti, TVIS_BOLD, TVIS_BOLD);
                        }
                        else
                        {
                            Console.WriteLine("Folder:" + pNode.NodeName);
                            if (!bRoot)
                            {
                                hti = treeView1.Nodes.Add(pNode.Index.ToString(), pNode.NodeName);
                                bRoot = true;
                            }
                            else
                            {
                                hti = hti.Nodes.Add(pNode.Index.ToString(), pNode.NodeName);
                            }
                            //if (rit->FileCount)
                            //    m_treeNode.SetItemState(hti, TVIS_BOLD, TVIS_BOLD);
                        }
                    }
                }
                treeView1.ExpandAll();
            }
            //Console.WriteLine(JsonConvert.SerializeObject(StringTableManager._stringTables, Formatting.Indented));
        }

        public void FullClear()
        {
            filePath = null;
            HeaderManager._header = new CPackHeader();
            HeaderManager.m_pvHeaderDirInfo.Clear();
            StringTableManager.lastReg = null;
            StringTableManager._stringTables = new List<StringTable>();
            NodeManager.m_pvPackNodes.Clear();
            treeView1.Nodes.Clear();
            listView1.Items.Clear();
            cmsPathFile.Items[0].Enabled = false;
            cmsPathFile.Items[1].Enabled = false;
            lbPath.Text = "Path: None";
            lbFilesCount.Text = "Files Count: None";
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            listView1.Items.Clear();
            var node = NodeManager.GetNodeById(ulong.Parse(e.Node.Name));
            if (node == null)
                return;

            for (int i = 0; i < node.Files.Count; i++)
            {
                var file = node.Files[i];
                ListViewItem lvFile = new ListViewItem(file.Filename);
                ListViewSubItem lvFileOffset = new ListViewSubItem();
                lvFileOffset.Text = SettingsManager._settings.ValuesInHex ? Utils.DecimalToHex((int)file.Offset) : file.Offset.ToString();

                ListViewSubItem lvFileSize = new ListViewSubItem();
                lvFileSize.Text = SettingsManager._settings.ValuesInHex ? Utils.DecimalToHex((int)file.Size) : file.Size.ToString();

                ListViewSubItem lvFileCRC32 = new ListViewSubItem();
                try
                {
                    byte[] data = new byte[file.Size];
                    Array.Copy(_reader._buffer, (int)file.Offset, data, 0, data.Length);
                    var crc = new CrcStream(new MemoryStream(data));
                    crc.Read(data, 0, data.Length);
                    lvFileCRC32.Text = crc.ReadCrc.ToString("X8");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                lvFile.SubItems.Add(lvFileOffset);
                lvFile.SubItems.Add(lvFileSize);
                lvFile.SubItems.Add(lvFileCRC32);
                listView1.Items.Add(lvFile);
            }
            lbFilesCount.Text = "Files Count: " + node.Files.Count;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = listView1.FocusedItem;
            if (item == null)
                return;
            SaveFileDialog sfd = new SaveFileDialog();
            string fileName = item.SubItems[0].Text;
            sfd.Filter = "|" + fileName;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                
                long fileOffset = int.Parse(item.SubItems[1].Text);  //quando estiver em hexadecimal converter pra decimal 
                int fileSize = int.Parse(item.SubItems[2].Text);  //quando estiver em hexadecimal converter pra decimal  
                byte[] fileData = new byte[fileSize];  //quando estiver em hexadecimal converter pra decimal 
                Array.Copy(_reader._buffer, fileOffset, fileData, 0, fileData.Length);
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    fs.Write(fileData, 0, fileData.Length);
                    fs.Close();
                }
                MessageBox.Show($"{fileName} dumped sucessfuly", "i3PackTool");
            }
        }

        private void dumpAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string nodeName = treeView1.SelectedNode.Text;
                string dirFilePath = folderBrowserDialog1.SelectedPath + "\\_" + nodeName;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    var item = listView1.Items[i];
                    string fileName = item.SubItems[0].Text;
                    long fileOffset = int.Parse(item.SubItems[1].Text);  //quando estiver em hexadecimal converter pra decimal 
                    int fileSize = int.Parse(item.SubItems[2].Text);  //quando estiver em hexadecimal converter pra decimal  

                    string itemPath = dirFilePath + "\\" + fileName;
                    
                    Directory.CreateDirectory(dirFilePath);

                    byte[] fileData = new byte[fileSize];  //quando estiver em hexadecimal converter pra decimal 
                    
                    Array.Copy(_reader._buffer, fileOffset, fileData, 0, fileData.Length);
                    
                    using (FileStream fs = new FileStream(itemPath, FileMode.Create))
                    {
                        fs.Write(fileData, 0, fileData.Length);
                        fs.Close();
                    }
                }
                MessageBox.Show($"{listView1.Items.Count} files dumped sucessfuly", "i3PackTool");
                Process.Start("explorer.exe", dirFilePath);
            }
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("i3PackTool v1.0\n\nAll Credits to Exploit Network\n\nDevelopers:\nCoyote\nPISTOLA\nSpecial Thanks to Abujafar for c++ source", "Credits");
        }

        private void openPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/select," + filePath);
        }

        private void copyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDirectory = filePath.Replace(filePath.Substring(filePath.LastIndexOf(@"\") + 1), "");
            Clipboard.SetText(fileDirectory);
            MessageBox.Show("Directory copied successfully.", "i3PackTool");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsView settings = new SettingsView())
            {
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    listView1.Items.Clear();
                    if (treeView1.SelectedNode == null)
                        return;
                    var node = NodeManager.GetNodeById(ulong.Parse(treeView1.SelectedNode.Name));
                    if (node == null)
                        return;

                    for (int i = 0; i < node.Files.Count; i++)
                    {
                        var file = node.Files[i];
                        ListViewItem lvFile = new ListViewItem(file.Filename);
                        ListViewSubItem lvFileOffset = new ListViewSubItem();
                        lvFileOffset.Text = SettingsManager._settings.ValuesInHex ? Utils.DecimalToHex((int)file.Offset) : file.Offset.ToString();

                        ListViewSubItem lvFileSize = new ListViewSubItem();
                        lvFileSize.Text = SettingsManager._settings.ValuesInHex ? Utils.DecimalToHex((int)file.Size) : file.Size.ToString();

                        ListViewSubItem lvFileCRC32 = new ListViewSubItem();
                        try
                        {
                            byte[] data = new byte[file.Size];
                            Array.Copy(_reader._buffer, (int)file.Offset, data, 0, data.Length);
                            var crc = new CrcStream(new MemoryStream(data));
                            crc.Read(data, 0, data.Length);
                            lvFileCRC32.Text = crc.ReadCrc.ToString("X8");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        lvFile.SubItems.Add(lvFileOffset);
                        lvFile.SubItems.Add(lvFileSize);
                        lvFile.SubItems.Add(lvFileCRC32);
                        listView1.Items.Add(lvFile);
                    }
                    lbFilesCount.Text = "Files Count: " + node.Files.Count;
                }
            }
        }

        private void replaceThisFileWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = listView1.FocusedItem;
            if (item == null)
                return;
            string fileName = item.SubItems[0].Text;
            openFileDialog1.Filter = fileName + "|" + fileName;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] fileData = File.ReadAllBytes(openFileDialog1.FileName);
                long fileOffset = int.Parse(item.SubItems[1].Text);
                int fileSize;
                if (SettingsManager._settings.ValuesInHex)
                {
                    fileSize = Utils.HexToDecimal(item.SubItems[2].Text);
                }
                else
                {
                    fileSize = int.Parse(item.SubItems[2].Text);
                }

                //if(fileData.Length != fileSize) // comentar esse if futuramente para adicionar novos conteudos na script.i3pack e etc
                //{
                //    MessageBox.Show("Could not read target file.");
                //    return;
                //}

                var it = NodeManager.m_pvPackNodes.Where(x => x.NodeName == treeView1.SelectedNode.Text).FirstOrDefault();

                if(it == null)
                {
                    MessageBox.Show("ChangeFile: node iterator not found.");
                    return;
                }

                var fit = it.Files.Where(x => x.Filename == fileName).FirstOrDefault();

                if(fit == null)
                {
                    MessageBox.Show("ChangeFile: files iterator not found.");
                    return;
                }

                ulong ulFirstOffs = fit.Offset;

                var pWriteBuffer = new byte[ulFirstOffs];

                Array.Copy(_reader._buffer, (int)0, pWriteBuffer, 0, (int)ulFirstOffs);
               
                Array.Resize<byte>(ref pWriteBuffer, (int)ulFirstOffs + fileData.Length);

                Array.Copy(fileData, 0, pWriteBuffer, (int)ulFirstOffs, fileData.Length);

                ulong ulLastSectionSize = (ulong)_reader._buffer.Length - (fit.Offset + fit.Size);

                Array.Resize<byte>(ref pWriteBuffer, (int)ulFirstOffs + fileData.Length + (int)ulLastSectionSize);

                //pLastBuffer
                Array.Copy(_reader._buffer, (int)ulFirstOffs + fileData.Length, pWriteBuffer, (int)fit.Offset + (int)fit.Size, (int)ulLastSectionSize);

                ulong ulTotalSize = ulFirstOffs + (ulong)fileData.Length + ulLastSectionSize;
                ulong ulDiff = (ulong)fileData.Length - fit.Size;

                for (int _it = 0; _it < NodeManager.m_pvPackNodes.Count; _it++)
                {
                    var m_pvPackNode = NodeManager.m_pvPackNodes[_it];
                    
                    for (int _fit = 0; _fit < m_pvPackNode.Files.Count; _fit++)
                    {
                        var file = NodeManager.m_pvPackNodes[_it].Files[_fit];
                        if (file.DecType != 860767824)
                        {
                            if (file.Offset >= ulFirstOffs)
                            {
                                bool ChangeSize = file.Offset == ulFirstOffs;

                                ulong ulOffsNew = file.Offset + ulDiff;

                                ulong ulFilePackOffs = m_pvPackNode.Offset + file.RawOffset;

                                ulong ulFileInfoSz = file.Padded ? 92 : (ulong)(92 - 16);

                                var pFIBuffer = new byte[ulFileInfoSz];

                                Array.Copy(pWriteBuffer, (int)ulFilePackOffs, pFIBuffer, 0, (int)ulFileInfoSz);

                                BitRotate.Unshift(pFIBuffer, (int)ulFileInfoSz, 2);

                                Reader t = new Reader(pFIBuffer);
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

                                if (file.Padded)
                                {
                                    if (ChangeSize)
                                    {
                                        ulong ulNewFsz = (ulong)((pFileInfo.SizeShift_2 << 0x10) | pFileInfo.SizeOr_2) + ulDiff;
                                        pFileInfo.SizeShift_2 = (ushort)(ulNewFsz >> 0x10);
                                        pFileInfo.SizeOr_2 = (ushort)((pFileInfo.SizeShift_2 << 0x10) ^ (int)ulNewFsz);
                                    }

                                    if (!ChangeSize)
                                    {
                                        pFileInfo.OffsShift_2 = (ushort)(ulOffsNew >> 0x10);
                                        pFileInfo.OffsOr_2 = (ushort)((pFileInfo.OffsShift_2 << 0x10) ^ (int)ulOffsNew);
                                    }
                                }
                                else
                                {
                                    if (ChangeSize)
                                    {
                                        ulong ulNewFsz = (ulong)((pFileInfo.SizeShift_1 << 0x10) | pFileInfo.SizeOr_1) + ulDiff;
                                        pFileInfo.SizeShift_1 = (ushort)(ulNewFsz >> 0x10);
                                        pFileInfo.SizeOr_1 = (ushort)((pFileInfo.SizeShift_1 << 0x10) ^ (int)ulNewFsz);
                                    }

                                    if (!ChangeSize)
                                    {
                                        pFileInfo.OffsShift_1 = (ushort)(ulOffsNew >> 0x10);
                                        pFileInfo.OffsOr_1 = (ushort)((pFileInfo.OffsShift_1 << 0x10) ^ (int)ulOffsNew);
                                    }
                                }

                                var pFINewBuffer = new byte[ulFileInfoSz];

                                using (BinaryWriter br = new BinaryWriter(new MemoryStream(pFINewBuffer)))
                                {
                                    br.Write(Encoding.GetEncoding(1252).GetBytes(pFileInfo.Filename));
                                    br.Write(new byte[52 - pFileInfo.Filename.Length]);
                                    br.Write(pFileInfo.N0193C181);
                                    br.Write(pFileInfo.SizeOr_1);
                                    br.Write(pFileInfo.OffsShift_1);
                                    br.Write(pFileInfo.SizeShift_1);
                                    br.Write(pFileInfo._0x003C);
                                    br.Write(pFileInfo.OffsOr_1);
                                    br.Write(pFileInfo.N01970D81);
                                    br.Write(pFileInfo.SizeOr_2);
                                    br.Write(pFileInfo.OffsShift_2);
                                    br.Write(pFileInfo.SizeShift_2);
                                    br.Write(pFileInfo.N01A26723);
                                    br.Write(pFileInfo.OffsOr_2);
                                    br.Write(pFileInfo.N019E3220);
                                    br.Write(pFileInfo.Ended);
                                    br.Close();
                                }

                                BitRotate.Shift(pFINewBuffer, 2);

                                Array.Copy(pFINewBuffer, 0, pWriteBuffer, (int)ulFilePackOffs, (int)ulFileInfoSz);

                                if (file.Offset == ulFirstOffs)
                                {
                                    ulong ulNewSize = (ulong)fileData.Length;
                                }
                            }
                        }
                        else
                        {
                            //fixar
                            if (file.Offset >= ulFirstOffs)
                            {
                                bool ChangeSize = file.Offset == ulFirstOffs;

                                ulong ulOffsNew = file.Offset + ulDiff;

                                ulong ulFilePackOffs = m_pvPackNode.Offset + file.RawOffset;

                                ulong ulFileInfoSz = 92;

                                var pFIBuffer = new byte[ulFileInfoSz];

                                Array.Copy(pWriteBuffer, (int)ulFilePackOffs, pFIBuffer, 0, (int)ulFileInfoSz);

                                i3ResourceFile.Decrypt(pFIBuffer, pFIBuffer, file.FileDecInfo, pFIBuffer.Length);

                                Reader t = new Reader(pFIBuffer);
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

                                if (ChangeSize)
                                {
                                    ulong ulNewFsz = (ulong)((pFileInfo.SizeShift_2 << 0x10) | pFileInfo.SizeOr_2) + ulDiff;
                                    pFileInfo.SizeShift_2 = (ushort)(ulNewFsz >> 0x10);
                                    pFileInfo.SizeOr_2 = (ushort)((pFileInfo.SizeShift_2 << 0x10) ^ (int)ulNewFsz);
                                }
                                else
                                {
                                    pFileInfo.OffsShift_2 = (ushort)(ulOffsNew >> 0x10);
                                    pFileInfo.OffsOr_2 = (ushort)((pFileInfo.OffsShift_2 << 0x10) ^ (int)ulOffsNew);
                                }

                                var pFINewBuffer = new byte[ulFileInfoSz];

                                using (BinaryWriter br = new BinaryWriter(new MemoryStream(pFINewBuffer)))
                                {
                                    br.Write(Encoding.GetEncoding(1252).GetBytes(pFileInfo.Filename));
                                    br.Write(new byte[52 - pFileInfo.Filename.Length]);
                                    br.Write(pFileInfo.N0193C181);
                                    br.Write(pFileInfo.SizeOr_1);
                                    br.Write(pFileInfo.OffsShift_1);
                                    br.Write(pFileInfo.SizeShift_1);
                                    br.Write(pFileInfo._0x003C);
                                    br.Write(pFileInfo.OffsOr_1);
                                    br.Write(pFileInfo.N01970D81);
                                    br.Write(pFileInfo.SizeOr_2);
                                    br.Write(pFileInfo.OffsShift_2);
                                    br.Write(pFileInfo.SizeShift_2);
                                    br.Write(pFileInfo.N01A26723);
                                    br.Write(pFileInfo.OffsOr_2);
                                    br.Write(pFileInfo.N019E3220);
                                    br.Write(pFileInfo.Ended);
                                    br.Close();
                                }

                                i3ResourceFile.Encrypt(pFINewBuffer, pFINewBuffer, file.FileDecInfo, pFINewBuffer.Length);

                                Array.Copy(pFINewBuffer, 0, pWriteBuffer, (int)ulFilePackOffs, (int)ulFileInfoSz);

                                if (file.Offset == ulFirstOffs)
                                {
                                    ulong ulNewSize = (ulong)fileData.Length;
                                }
                            }
                        }
                    }

                    //pDir
                    byte[] pDir = new byte[0x001C];
                    Array.Copy(pWriteBuffer, (int)m_pvPackNode.DirTableOffs, pDir, 0, pDir.Length);
                    Reader ta = new Reader(pDir);
                    CNodeInfo pNode = new CNodeInfo
                    {
                        Type = ta.ReadInt(),
                        Index = ta.ReadLong(),
                        Offset = ta.ReadLong(),
                        Size = ta.ReadLong()
                    };

                    if ((ulong)pNode.Offset > ulFirstOffs)
                    {
                        pNode.Offset += (long)ulDiff;
                    }

                    Array.Copy(pDir, 0, pWriteBuffer, (int)m_pvPackNode.DirTableOffs, pDir.Length);
                }

                SaveFileDialog sfd = new SaveFileDialog();
                var filePathSave = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                sfd.Filter = "|" + filePathSave;
                sfd.FileName = filePathSave;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                    fs.Write(pWriteBuffer, 0, pWriteBuffer.Length);
                    fs.Close();
                    MessageBox.Show($"{fileName} replaced sucessfuly", "i3PackTool");
                    if(MessageBox.Show($"Your modified file has been saved into new location: \n\n>>> {sfd.FileName} <<<\n\nDo you want to open your modified i3Pack right now?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        OpenFile(sfd.FileName);
                    }
                }
                
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //using (FileStream fs = new FileStream(ofdPath.FileName, FileMode.Create))
            //{
            //    fs.Write(_reader._buffer, 0, _reader._buffer.Length);
            //    fs.Close();
            //}
            //MessageBox.Show($"{ofdPath.FileName} save sucessfuly", "i3PackTool");
        }

        private void cmsFileView_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var file = listView1.FocusedItem;
                var fileName = file.Text;
                var fileFormat = fileName.Substring(fileName.IndexOf('.') + 1);
                EnableOpenFileByFormat(fileFormat);
            }
            catch
            {

            }
        }

        private void EnableOpenFileByFormat(string formatType)
        {
            switch (formatType.ToLower())
            {
                case "nlf":
                case "pef":
                    openToolStripMenuItem1.Enabled = true;
                    break;
                default:
                    openToolStripMenuItem1.Enabled = false;
                    break;
            }
        }

        private void OpenFileByFormat(string formatType, ListViewItem file)
        {
            switch (formatType.ToLower())
            {
                case "pef":
                    //open with new PefEditor
                    break;
                case "nlf":
                case "txt":
                    //dump file in temp and open with default program
                    break;
                default:
                    openToolStripMenuItem1.Enabled = false;
                    break;
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                var file = listView1.FocusedItem;
                var fileName = file.Text;
                var fileFormat = fileName.Substring(fileName.IndexOf('.') + 1);
                OpenFileByFormat(fileFormat, file);
            }
            catch
            {

            }
        }
    }
}
