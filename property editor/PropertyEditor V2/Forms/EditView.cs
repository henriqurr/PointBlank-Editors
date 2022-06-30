using PropertyEditor.Managers;
using PropertyEditor.Models;
using PropertyEditor.Models.Enums;
using System;
using System.Windows.Forms;

namespace PropertyEditor
{
    public partial class EditView : Form
    {
        public Objects obj;
        public int nation;
        public EditView(Objects obj, int nation)
        {
            InitializeComponent();
            this.obj = obj;
            this.nation = nation;
        }

        private void EditView_Load(object sender, EventArgs e)
        {
            Text = string.Format("Edit - {0}", obj.Keys.Name);
            cbType.DataSource = Enum.GetValues(typeof(Models.Enums.ValueType));
            txtValue.Text = obj.Keys.Nations[obj.Keys.Type == 9 ? nation : 0].ToString();
            cbType.Text = ((Models.Enums.ValueType)obj.Keys.ValueType).ToString();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            btSave.Enabled = false;
            var lastValue = obj.Keys.Nations[obj.Keys.Type == 9 ? nation : 0];
            if (lastValue.Equals(txtValue.Text))
            {
                Close();
                return;
            }
            obj.Keys.ValueType = cbType.SelectedIndex; //change new value type
            var oldSize = obj.Size;
            if(obj.Keys.ValueType == 2) //STRING UNICODE MULTIPLIES FOR 2
            {
                ulong diference = (ulong)((lastValue.ToString().Length * 2) - (txtValue.Text.ToString().Length * 2));
                if (diference < 0)
                {
                    obj.Size += diference;
                }
                else if (diference > 0)
                {
                    obj.Size -= diference;
                }
            }
            else
            {
                ulong diference = (ulong)((lastValue.ToString().Length) - (txtValue.Text.ToString().Length));
                if (diference < 0)
                {
                    obj.Size += diference;
                }
                else if (diference > 0)
                {
                    obj.Size -= diference;
                }
            }
            Console.WriteLine("Old Size:{0} New Size:{1}", oldSize, obj.Size);
            obj.Keys.Nations[obj.Keys.Type == 9 ? nation : 0] = txtValue.Text; // change new value
            DialogResult = DialogResult.OK;
            Close();
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (txtValue.Text == obj.Keys.Nations[obj.Keys.Type == 9 ? nation : 0].ToString())
            {
                btSave.Enabled = false;
                return;
            }
            btSave.Enabled = true;
        }
    }
}
