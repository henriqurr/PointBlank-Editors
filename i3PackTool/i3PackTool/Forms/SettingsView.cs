using i3PackTool.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace i3PackTool.Forms
{
    public partial class SettingsView : Form
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void SettingsView_Load(object sender, EventArgs e)
        {
            if (!SettingsManager._settings.ValuesInHex)
                rbShowInDecimal.Checked = true;
            else if (SettingsManager._settings.ValuesInHex)
                rbShowInHex.Checked = true;
            else
                rbShowInDecimal.Checked = true;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rbShowInDecimal.Checked)
                SettingsManager._settings.ValuesInHex = false;
            else if(rbShowInHex.Checked)
                SettingsManager._settings.ValuesInHex = true;
            else
                SettingsManager._settings.ValuesInHex = false;

            DialogResult = DialogResult.OK;
            this.Close();
        }

       
    }
}
