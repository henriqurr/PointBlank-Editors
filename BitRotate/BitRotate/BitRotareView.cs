using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BitRotate
{
    public partial class BitRotareView : Form
    {
        public BitRotareView()
        {
            InitializeComponent();        
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    string file = openFileDialog1.FileNames[i];
                    byte[] Strings = File.ReadAllBytes(file);
                    for (var j = 0; j < Strings.Length; j += 2048)
                    { 
                        BitRotate.Shift(Strings, j, int.Parse(numericUpDown1.Text));
                    }
                    using (FileStream fs = new FileStream(file + ".enc", FileMode.Create))
                    {
                        fs.Write(Strings, 0, Strings.Length);
                    }
                }
                label1.Text = "File encrypted sucessfully.";
                textBox2.Text = $"{openFileDialog1.FileName}.enc";
                MessageBox.Show("File encrypted sucessfully.");
            }
            else if (radioButton2.Checked)
            {
                for(int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {

                    string file = openFileDialog1.FileNames[i];
                    byte[] Strings = File.ReadAllBytes(file);
                    for (var j = 0; j < Strings.Length; j += 2048)
                    {
                        BitRotate.Unshift(Strings, j, int.Parse(numericUpDown1.Text)); //descriptografa em blocos de 2048 bytes (FIX)
                    }
                    using (FileStream fs = new FileStream(file + ".dec", FileMode.Create))
                    {
                        using (BinaryWriter writter = new BinaryWriter(fs))
                        {
                            writter.Write(Strings);
                        } 
                    }
                }
                label1.Text = "File decrypted sucessfully.";
                textBox2.Text = $"{openFileDialog1.FileName}.dec";
                MessageBox.Show("File decrypted sucessfully.");
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.UTF8.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button1.Text = "Decrypt";
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Text = "Encrypt";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                button1.Enabled = true;
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void BitRotareView_Load(object sender, EventArgs e)
        {
            
        }
    }
}
