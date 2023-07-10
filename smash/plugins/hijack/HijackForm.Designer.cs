namespace smash.plugins.hijack
{
    partial class HijackForm
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
            groupBox1 = new GroupBox();
            cbUseHijack = new CheckBox();
            cmbGroup = new ComboBox();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox3);
            groupBox1.Controls.Add(checkBox2);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(cbUseHijack);
            groupBox1.Controls.Add(cmbGroup);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(6, 5, 6, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 5, 6, 5);
            groupBox1.Size = new Size(434, 149);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "进程劫持";
            // 
            // cbUseHijack
            // 
            cbUseHijack.AutoSize = true;
            cbUseHijack.Location = new Point(330, 43);
            cbUseHijack.Name = "cbUseHijack";
            cbUseHijack.Size = new Size(94, 35);
            cbUseHijack.TabIndex = 3;
            cbUseHijack.Text = "启用";
            cbUseHijack.UseVisualStyleBackColor = true;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(12, 43);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(230, 39);
            cmbGroup.TabIndex = 1;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 100);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(91, 35);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "TCP";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(109, 100);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(97, 35);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "UDP";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(212, 100);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(194, 35);
            checkBox3.TabIndex = 6;
            checkBox3.Text = "代理DNS解析";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // HijackForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 149);
            Controls.Add(groupBox1);
            Name = "HijackForm";
            Text = "进程劫持";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private CheckBox cbUseHijack;
        private ComboBox cmbGroup;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
    }
}