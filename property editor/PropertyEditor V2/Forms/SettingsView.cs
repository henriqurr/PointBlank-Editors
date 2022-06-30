using PropertyEditor.Models;
using PropertyEditor.Models.Enums;
using System;
using System.IO;
using System.Windows.Forms;

namespace PropertyEditor
{
    public partial class SettingsView : Form
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            cbDebugConsole.Checked = Settings.ShowConsole;
            comboBox1.DataSource = Enum.GetValues(typeof(Nation));
            comboBox1.SelectedIndex = (int)Settings.Nation;
        }

        private void cbDebugConsole_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDebugConsole.Checked)
            {
                Settings.ShowConsole = true;
            }
            else
            {
                Settings.ShowConsole = false;
            }
        }

        private void SaveNewInfos()
        {
            try
            {
                using (FileStream fs = new FileStream(string.Format("{0}/config.data", Application.StartupPath), FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(Settings.ShowConsole);
                        bw.Write((int)Settings.Nation);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Utils.ShowConsole(Settings.ShowConsole);
            Settings.Nation = (Nation)comboBox1.SelectedIndex;
            SaveNewInfos();
            DialogResult = DialogResult.OK;
        }
    }
}
