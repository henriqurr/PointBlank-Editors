using RSC_Editor.Managers;
using RSC_Editor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSC_Editor
{
    public partial class RSCEditorViewer : Form
    {
        public RSCEditorViewer()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lbFilePath.Text = "Path: " + openFileDialog1.FileName;
                byte[] buffer = File.ReadAllBytes(openFileDialog1.FileName);
                using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
                {
                    HeaderManager.GetHeader(reader);
                    ItemsManager.GetItems(reader, HeaderManager._header);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < ItemsManager._items.Count; i++)
                    {
                        Items item = ItemsManager._items[i];
                        sb.AppendLine(item.Fullfilename + "     " + item.Filename);
                    }
                    richTextBox1.Text = sb.ToString();
                    lbFilesCount.Text = "Files Count: " + HeaderManager._header.ItemsCount;
                }
            }
        }

        private void RSCEditorViewer_Load(object sender, EventArgs e)
        {

        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RSC Editor v1.0\n\nAll Credits to Exploit Network\n\nDevelopers:\nCoyote", "Credits");
        }
    }
}
