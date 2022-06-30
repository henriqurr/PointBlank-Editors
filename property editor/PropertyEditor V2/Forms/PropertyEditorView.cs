using PropertyEditor.Managers;
using PropertyEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace PropertyEditor
{
    public partial class PropertyEditorView : Form
    {
        private bool _encryptedPef = false;
        private OpenFileDialog ofdPefSelector = new OpenFileDialog();
        List<TreeNode> oldNodes = new List<TreeNode>();
        public PropertyEditorView()
        {
            InitializeComponent();
        }

        private void PropertyEditor_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Waiting for load file...");
            lbNation.Text = "Nation:" + Settings.Nation;
            Console.WriteLine("Nation Selected: {0} idx: {1}", Settings.Nation, (int)Settings.Nation);
        }



        private void PropertyEditorView_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                lvDataItems.Columns[0].Width = 120;
                lvDataItems.Columns[1].Width = 200;
                lvDataItems.Columns[2].Width = 130;
                lvDataItems.Columns[3].Width = 150;
                lvDataItems.Columns[4].Width = 400;
            }
            if (WindowState == FormWindowState.Normal)
            {
                lvDataItems.Columns[0].Width = 98;
                lvDataItems.Columns[1].Width = 113;
                lvDataItems.Columns[2].Width = 124;
                lvDataItems.Columns[3].Width = 107;
                lvDataItems.Columns[4].Width = 121;
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdPefSelector.ShowDialog() == DialogResult.OK)
            {
                FullClear(); //reset all infos
                lbPath.Text = ofdPefSelector.FileName;
                Console.WriteLine("Path file " + ofdPefSelector.FileName);
                Console.WriteLine("Loading file...");
                byte[] buffer = File.ReadAllBytes(ofdPefSelector.FileName);
                byte[] fileFormat = new byte[4];
                Array.Copy(buffer, 0, fileFormat, 0, 4);
                if (Encoding.GetEncoding(Settings.Encoding).GetString(fileFormat) != "I3R2")
                {
                    _encryptedPef = true;
                    for (var i = 0; i < buffer.Length; i += 2048)
                    {
                        BitRotate.Unshift(buffer, i, 2048, 3); //descriptografa em blocos de 2048 bytes (FIX)
                    }
                }
                lbEncrypted.Text = "Encrypted: " + _encryptedPef;
                Console.WriteLine("File encrypted: " + _encryptedPef);
                using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
                {
                    HeaderManager.GetPefHeader(reader);
                    StringTablesManager.GetStringTables(reader, HeaderManager._header);
                    ObjectsManager.GetObjects(reader, HeaderManager._header);
                    ObjectsManager.GetObjectsValues(reader);
                    ShowNodes();
                }
                //using (FileStream fs = new FileStream("Objects.json", FileMode.Create))
                //{
                //    using (StreamWriter sw = new StreamWriter(fs))
                //    {
                //        sw.WriteLine(JsonConvert.SerializeObject(ObjectManager._objects, Formatting.Indented));
                //    }
                //}
            }
        }

        public void ShowNodes()
        {
            pbTotal.Value = 0;
            var registryRoot = ObjectsManager.GetRegistryRoot();
            pbTotal.Maximum = (int)(ObjectsManager._objects.Count);
            tvFolders.BeginUpdate();
            TreeNode tds = tvFolders.Nodes.Add(registryRoot.Id.ToString(), registryRoot.Keys.Name);
            LoadFiles(registryRoot, tds);
            LoadSubDirectories(registryRoot, tds);
        }

        public void LoadFiles(Objects obj, TreeNode td)
        {
            foreach (var id in obj.Keys.Items)
            {
                Task.Run(() =>
                {
                    var file = ObjectsManager.GetObjectById(id);
                    if (tvFolders.InvokeRequired)
                    {
                        tvFolders.Invoke(new Action(() =>
                        { 
                            TreeNode tds = td.Nodes.Add(file.Id.ToString(), file.Keys.Name);
                            UpdateProgress();
                        }));
                    }
                    else
                    {
                        TreeNode tds = td.Nodes.Add(file.Id.ToString(), file.Keys.Name);
                        UpdateProgress();
                    }
                });
            }
        }

        public void LoadSubDirectories(Objects obj, TreeNode td)
        {
            foreach (var id in obj.Keys.Folders)
            {
                Task.Run(() => {
                    var folder = ObjectsManager.GetObjectById(id);
                    if (tvFolders.InvokeRequired)
                    {
                        tvFolders.Invoke(new Action(() =>
                        {
                            TreeNode tds = td.Nodes.Add(folder.Id.ToString(), folder.Keys.Name);
                            LoadFiles(folder, tds);
                            LoadSubDirectories(folder, tds);
                            UpdateProgress();
                        }));
                    }
                    else
                    {
                        TreeNode tds = td.Nodes.Add(folder.Id.ToString(), folder.Keys.Name);
                        LoadFiles(folder, tds);
                        LoadSubDirectories(folder, tds);
                        UpdateProgress();
                    }
                });
            }
        }

        private void UpdateProgress()
        {
            if ((pbTotal.Value + 2) >= pbTotal.Maximum)
            {
                pbTotal.Value += 2;
                tvFolders.Sort();
                tvFolders.EndUpdate();
                tvFolders.Enabled = true;
                lvDataItems.Enabled = true;
                button1.Enabled = true;
                textBox1.Enabled = true;
                MessageBox.Show("Sucess!");
                return;
            }
            if (pbTotal.Value < pbTotal.Maximum)
            {
                pbTotal.Value++;
                int percent = (int)(((double)pbTotal.Value / (double)pbTotal.Maximum) * 100);
                //pbTotal.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));  
                //Application.DoEvents();
            }
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Node.Name))
            {
                return;
            }
            lvDataItems.BeginUpdate();
            lvDataItems.Items.Clear();
            //MessageBox.Show(e.Node.Name);
            Objects obj = ObjectsManager.GetObjectById(ulong.Parse(e.Node.Name));
            if (obj != null)
            {
                if (obj.Keys.IsFolder)
                {
                    List<Objects> items = new List<Objects>();
                    foreach (ulong id in obj.Keys.Items)
                    {
                        Objects file = ObjectsManager.GetObjectById(id);
                        items.Add(file);
                    }
                    var ascending = items.OrderBy(x => x.Keys.Name); //list in crescent order
                    foreach (var file in ascending)
                    {
                        ListViewItem objIdView = new ListViewItem();
                        objIdView.Text = file.Id.ToString();
                        ListViewSubItem objNameView = new ListViewSubItem();
                        objNameView.Text = file.Keys.Name;
                        ListViewSubItem objTypeView = new ListViewSubItem();
                        objTypeView.Text = file.Type.ToString();
                        ListViewSubItem objValueTypeView = new ListViewSubItem();
                        objValueTypeView.Text = ((Models.Enums.ValueType)file.Keys.ValueType).ToString();
                        ListViewSubItem objValueView = new ListViewSubItem();
                        objValueView.Text = file.Keys.Type == 9 ? file.Keys.Nations[(int)Settings.Nation].ToString() : file.Keys.Nations[0].ToString();
                        objIdView.SubItems.Add(objNameView);
                        objIdView.SubItems.Add(objTypeView);
                        objIdView.SubItems.Add(objValueTypeView);
                        objIdView.SubItems.Add(objValueView);
                        lvDataItems.Items.Add(objIdView);
                    }
                }
                else
                {
                    ListViewItem objIdView = new ListViewItem();
                    objIdView.Text = obj.Id.ToString();
                    ListViewSubItem objNameView = new ListViewSubItem();
                    objNameView.Text = obj.Keys.Name;
                    ListViewSubItem objTypeView = new ListViewSubItem();
                    objTypeView.Text = obj.Type.ToString();
                    ListViewSubItem objValueTypeView = new ListViewSubItem();
                    objValueTypeView.Text = ((Models.Enums.ValueType)obj.Keys.ValueType).ToString();
                    ListViewSubItem objValueView = new ListViewSubItem();
                    objValueView.Text = obj.Keys.Type == 9 ? obj.Keys.Nations[(int)Settings.Nation].ToString() : obj.Keys.Nations[0].ToString();
                    objIdView.SubItems.Add(objNameView);
                    objIdView.SubItems.Add(objTypeView);
                    objIdView.SubItems.Add(objValueTypeView);
                    objIdView.SubItems.Add(objValueView);
                    lvDataItems.Items.Add(objIdView);
                }
            }
            lvDataItems.EndUpdate();
        }

        public void SetProgressBarValue(long receive, long total)
        {
            pbTotal.Value = (int)(receive * 100 / total);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ofdPefSelector.FileName))
                return;
            using (FileStream fs = new FileStream(ofdPefSelector.FileName, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    HeaderManager.WriteHeader(bw);
                    StringTablesManager.WriteStringTables(bw);
                    ObjectsManager.WriteObjects(bw);
                    ObjectsManager.WriteObjectsKeys(bw);
                    ObjectsManager.SetOffsets(bw);
                }
                MessageBox.Show("Sucess!");
            }
            if (_encryptedPef)
            {
                byte[] buffer = File.ReadAllBytes(ofdPefSelector.FileName);
                for (var i = 0; i < buffer.Length; i += 2048)
                {
                    BitRotate.Shift(buffer, i, 2048, 3); //descriptografa em blocos de 2048 bytes (FIX)
                }
                using (FileStream fs = new FileStream(ofdPefSelector.FileName, FileMode.Create))
                {
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Objects obj = ObjectsManager.GetObjectById(ulong.Parse(lvDataItems.FocusedItem.Text));
            if (obj != null)
            {
                if (obj.Keys.IsFolder)
                    return;
                using (EditView edit = new EditView(obj, (int)Settings.Nation))
                {
                    if (edit.ShowDialog() == DialogResult.OK)
                    {
                        lvDataItems.BeginUpdate();
                        lvDataItems.Items.Clear();
                        //MessageBox.Show(e.Node.Name);
                        Objects objNode = ObjectsManager.GetObjectById(ulong.Parse(tvFolders.SelectedNode.Name));
                        if (objNode != null)
                        {
                            if (objNode.Keys.IsFolder)
                            {
                                List<Objects> items = new List<Objects>();
                                foreach (ulong id in objNode.Keys.Items)
                                {
                                    Objects file = ObjectsManager.GetObjectById(id);
                                    items.Add(file);
                                }
                                var ascending = items.OrderBy(x => x.Keys.Name); //list in crescent order
                                foreach (var file in ascending)
                                {
                                    ListViewItem objIdView = new ListViewItem();
                                    objIdView.Text = file.Id.ToString();
                                    ListViewSubItem objNameView = new ListViewSubItem();
                                    objNameView.Text = file.Keys.Name;
                                    ListViewSubItem objTypeView = new ListViewSubItem();
                                    objTypeView.Text = file.Type.ToString();
                                    ListViewSubItem objValueTypeView = new ListViewSubItem();
                                    objValueTypeView.Text = ((Models.Enums.ValueType)file.Keys.ValueType).ToString();
                                    ListViewSubItem objValueView = new ListViewSubItem();
                                    objValueView.Text = file.Keys.Type == 9 ? file.Keys.Nations[(int)Settings.Nation].ToString() : file.Keys.Nations[0].ToString();
                                    objIdView.SubItems.Add(objNameView);
                                    objIdView.SubItems.Add(objTypeView);
                                    objIdView.SubItems.Add(objValueTypeView);
                                    objIdView.SubItems.Add(objValueView);
                                    lvDataItems.Items.Add(objIdView);
                                }
                            }
                            else
                            {
                                ListViewItem objIdView = new ListViewItem();
                                objIdView.Text = objNode.Id.ToString();
                                ListViewSubItem objNameView = new ListViewSubItem();
                                objNameView.Text = objNode.Keys.Name;
                                ListViewSubItem objTypeView = new ListViewSubItem();
                                objTypeView.Text = objNode.Type.ToString();
                                ListViewSubItem objValueTypeView = new ListViewSubItem();
                                objValueTypeView.Text = ((Models.Enums.ValueType)objNode.Keys.ValueType).ToString();
                                ListViewSubItem objValueView = new ListViewSubItem();
                                objValueView.Text = objNode.Keys.Type == 9 ? objNode.Keys.Nations[(int)Settings.Nation].ToString() : objNode.Keys.Nations[0].ToString();
                                objIdView.SubItems.Add(objNameView);
                                objIdView.SubItems.Add(objTypeView);
                                objIdView.SubItems.Add(objValueTypeView);
                                objIdView.SubItems.Add(objValueView);
                                lvDataItems.Items.Add(objIdView);
                            }
                        }
                        lvDataItems.EndUpdate();
                        MessageBox.Show((obj.Keys.Type == 9 ? obj.Keys.Nations[(int)Settings.Nation] : obj.Keys.Nations[0]) + "");
                    }
                }
            }
        }

        private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon...", Application.ProductName);
            //Objects objNode = ObjectManager.GetObjectById(ulong.Parse(treeView1.SelectedNode.Name));
            //if (objNode.Keys.IsFolder)
            //{
            //    // dump items, folders and subitems
            //}
        }

        private void infosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Objects obj = ObjectsManager.GetObjectById(ulong.Parse(tvFolders.SelectedNode.Name));
            if (obj == null)
            {
                MessageBox.Show("It was not possible to get the information for this object.", Application.ProductName);
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Type: " + obj.Type);
            stringBuilder.AppendLine("Id: " + obj.Id);
            stringBuilder.AppendLine("Offset: " + obj.Offset);
            stringBuilder.AppendLine("Size: " + obj.Size);
            stringBuilder.AppendLine("IsFolder: " + obj.Keys.IsFolder);
            if (obj.Keys.IsFolder)
            {
                stringBuilder.AppendLine("isRegistryRoot: " + obj.Keys.IsRegistryRoot);
                stringBuilder.AppendLine("Folders: " + obj.Keys.Folders.Count);
                stringBuilder.AppendLine("Items: " + obj.Keys.Items.Count);
            }
            else
            {
                stringBuilder.AppendLine("Nations: " + obj.Keys.NationsCount);
                stringBuilder.AppendLine("ValueType: " + (Models.Enums.ValueType)obj.Keys.ValueType);
                if (obj.Keys.Type == 9)
                {
                    stringBuilder.AppendLine("***** [NATIONS] *****");
                    for (int i = 0; i < obj.Keys.NationsCount; i++)
                    {
                        stringBuilder.AppendLine(string.Format(" {1} Value: {2}", i, ((Models.Enums.Nation)i).ToString(), obj.Keys.Nations[i]));
                    }
                    stringBuilder.AppendLine("***** [NATIONS] *****");
                }
                else
                {
                    stringBuilder.AppendLine("Value:" + obj.Keys.Nations[0]);
                }
            }
            MessageBox.Show(stringBuilder.ToString(), string.Format("{0} - Infos", obj.Keys.Name));
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsView settings = new SettingsView())
            {
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Changes made successfully.", Application.ProductName);
                }
            }
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Copyright © Exploit Network 2021\nDevelopers: Coyote, PISTOLA", Application.ProductName);
        }

        private void FullClear()
        {
            Console.WriteLine("Cleaning all old infos...");
            lbPath.Text = "None";
            lvDataItems.Enabled = false;
            tvFolders.Enabled = false;
            lbEncrypted.Text = "None";
            _encryptedPef = false;
            tvFolders.Nodes.Clear();
            lvDataItems.Items.Clear();
            pbTotal.Value = 0;
            HeaderManager._header = new Header();
            StringTablesManager._stringTables = new List<StringTable>();
            ObjectsManager._changeOffsets = new Dictionary<ulong, ulong>();
            ObjectsManager._objects = new List<Objects>();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text.Trim().ToLower();
            List<TreeNode> foundNodes = new List<TreeNode>();
            button1.Enabled = false;
            tvFolders.BeginUpdate();
            for (int i = 0; i < tvFolders.Nodes.Count; i++)
            {
                var item = tvFolders.Nodes[i];
                oldNodes.Add(item);
                if (item.Text.ToLower().Contains(text))
                    foundNodes.Add(item);
                LoadSubChidrenNodes(item, foundNodes, text);
            }
            tvFolders.Nodes.Clear();
            TreeNode lastItem = new TreeNode();
            foreach(var item in foundNodes)
            {
                if (lastItem.Nodes.Count > 0 && lastItem.Nodes.Contains(item))
                {
                    continue;
                }
                tvFolders.Nodes.Add(item);
                lastItem = item;
                //item.EnsureVisible();
                //item.Expand();
            }
            if(foundNodes.Count > 0)
                tvFolders.SelectedNode = foundNodes[0];
            tvFolders.Sort();
            tvFolders.EndUpdate();
            button2.Enabled = true;
            
            Console.WriteLine("Items Found: {0}", foundNodes.Count);
            lbFoundNodes.Text = string.Format("Search: {0} items found.", foundNodes.Count);
        }

        private void LoadSubChidrenNodes(TreeNode treeNode, List<TreeNode> foundNodes, string text)
        {
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                var item = treeNode.Nodes[i];
                if (item.Text.ToLower().Contains(text))
                    foundNodes.Add(item);
                LoadSubChidrenNodes(item, foundNodes, text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tvFolders.Nodes.Clear();
            tvFolders.BeginUpdate();
            for (int i = 0; i < oldNodes.Count; i++)
            {
                var item = oldNodes[i];
                tvFolders.Nodes.Add(item);
            }
            tvFolders.EndUpdate();
            oldNodes.Clear();
            textBox1.Text = "";
            lbFoundNodes.Text = "";
            button2.Enabled = false;
            button1.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdSaveFile = new SaveFileDialog();
            var oldFileName = ofdPefSelector.FileName.Substring(ofdPefSelector.FileName.LastIndexOf("\\") + 1);
            sfdSaveFile.Filter = "|" + oldFileName;
            sfdSaveFile.FileName = oldFileName;
            if (sfdSaveFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(sfdSaveFile.FileName, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        HeaderManager.WriteHeader(bw);
                        StringTablesManager.WriteStringTables(bw);
                        ObjectsManager.WriteObjects(bw);
                        ObjectsManager.WriteObjectsKeys(bw);
                        ObjectsManager.SetOffsets(bw);
                    }
                    MessageBox.Show("Sucess!");
                }
                if (_encryptedPef)
                {
                    byte[] buffer = File.ReadAllBytes(sfdSaveFile.FileName);
                    for (var i = 0; i < buffer.Length; i += 2048)
                    {
                        BitRotate.Shift(buffer, i, 2048, 3); //descriptografa em blocos de 2048 bytes (FIX)
                    }
                    using (FileStream fs = new FileStream(sfdSaveFile.FileName, FileMode.Create))
                    {
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
