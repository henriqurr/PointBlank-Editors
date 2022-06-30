namespace i3PackTool.Forms
{
    partial class SettingsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbShowInHex = new System.Windows.Forms.RadioButton();
            this.rbShowInDecimal = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbShowInHex
            // 
            this.rbShowInHex.AutoSize = true;
            this.rbShowInHex.Location = new System.Drawing.Point(12, 35);
            this.rbShowInHex.Name = "rbShowInHex";
            this.rbShowInHex.Size = new System.Drawing.Size(150, 17);
            this.rbShowInHex.TabIndex = 0;
            this.rbShowInHex.Text = "Show infos in hexadecimal";
            this.rbShowInHex.UseVisualStyleBackColor = true;
            // 
            // rbShowInDecimal
            // 
            this.rbShowInDecimal.AutoSize = true;
            this.rbShowInDecimal.Checked = true;
            this.rbShowInDecimal.Location = new System.Drawing.Point(12, 12);
            this.rbShowInDecimal.Name = "rbShowInDecimal";
            this.rbShowInDecimal.Size = new System.Drawing.Size(127, 17);
            this.rbShowInDecimal.TabIndex = 1;
            this.rbShowInDecimal.TabStop = true;
            this.rbShowInDecimal.Text = "Show infos in decimal";
            this.rbShowInDecimal.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 67);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Show Console";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(109, 112);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(190, 112);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 147);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.rbShowInDecimal);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rbShowInHex);
            this.Controls.Add(this.button2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton rbShowInDecimal;
        private System.Windows.Forms.RadioButton rbShowInHex;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}