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
            cbFilterDns = new CheckBox();
            cbFilterUdp = new CheckBox();
            cbFilterTcp = new CheckBox();
            cbUseHijack = new CheckBox();
            cmbGroup = new ComboBox();
            SuspendLayout();
            // 
            // cbFilterDns
            // 
            cbFilterDns.AutoSize = true;
            cbFilterDns.Location = new Point(218, 81);
            cbFilterDns.Name = "cbFilterDns";
            cbFilterDns.Size = new Size(194, 35);
            cbFilterDns.TabIndex = 6;
            cbFilterDns.Text = "代理DNS解析";
            cbFilterDns.UseVisualStyleBackColor = true;
            cbFilterDns.CheckedChanged += CheckedChanged;
            // 
            // cbFilterUdp
            // 
            cbFilterUdp.AutoSize = true;
            cbFilterUdp.Location = new Point(115, 81);
            cbFilterUdp.Name = "cbFilterUdp";
            cbFilterUdp.Size = new Size(97, 35);
            cbFilterUdp.TabIndex = 5;
            cbFilterUdp.Text = "UDP";
            cbFilterUdp.UseVisualStyleBackColor = true;
            cbFilterUdp.CheckedChanged += CheckedChanged;
            // 
            // cbFilterTcp
            // 
            cbFilterTcp.AutoSize = true;
            cbFilterTcp.Location = new Point(18, 81);
            cbFilterTcp.Name = "cbFilterTcp";
            cbFilterTcp.Size = new Size(91, 35);
            cbFilterTcp.TabIndex = 4;
            cbFilterTcp.Text = "TCP";
            cbFilterTcp.UseVisualStyleBackColor = true;
            cbFilterTcp.CheckedChanged += CheckedChanged;
            // 
            // cbUseHijack
            // 
            cbUseHijack.AutoSize = true;
            cbUseHijack.Location = new Point(165, 141);
            cbUseHijack.Name = "cbUseHijack";
            cbUseHijack.Size = new Size(94, 35);
            cbUseHijack.TabIndex = 3;
            cbUseHijack.Text = "启用";
            cbUseHijack.UseVisualStyleBackColor = true;
            cbUseHijack.CheckedChanged += CheckedChanged;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(100, 24);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(230, 39);
            cmbGroup.TabIndex = 1;
            // 
            // HijackForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 201);
            Controls.Add(cbFilterDns);
            Controls.Add(cbFilterUdp);
            Controls.Add(cbFilterTcp);
            Controls.Add(cmbGroup);
            Controls.Add(cbUseHijack);
            Name = "HijackForm";
            Text = "进程劫持";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox cbUseHijack;
        private ComboBox cmbGroup;
        private CheckBox cbFilterTcp;
        private CheckBox cbFilterUdp;
        private CheckBox cbFilterDns;
    }
}