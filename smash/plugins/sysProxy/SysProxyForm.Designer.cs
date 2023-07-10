namespace smash.plugins.sysProxy
{
    partial class SysProxyForm
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
            groupBox3 = new GroupBox();
            lbPac = new Label();
            cbIsEnv = new CheckBox();
            cbIsPac = new CheckBox();
            cbUseSysProxy = new CheckBox();
            cmbSysProxy = new ComboBox();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lbPac);
            groupBox3.Controls.Add(cbIsEnv);
            groupBox3.Controls.Add(cbIsPac);
            groupBox3.Controls.Add(cbUseSysProxy);
            groupBox3.Controls.Add(cmbSysProxy);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(0, 0);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(434, 194);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "系统代理";
            // 
            // lbPac
            // 
            lbPac.Location = new Point(15, 140);
            lbPac.Name = "lbPac";
            lbPac.Size = new Size(406, 31);
            lbPac.TabIndex = 8;
            lbPac.Text = "pac地址";
            lbPac.TextAlign = ContentAlignment.TopCenter;
            // 
            // cbIsEnv
            // 
            cbIsEnv.AutoSize = true;
            cbIsEnv.Location = new Point(12, 97);
            cbIsEnv.Name = "cbIsEnv";
            cbIsEnv.Size = new Size(190, 35);
            cbIsEnv.TabIndex = 7;
            cbIsEnv.Text = "环境变量代理";
            cbIsEnv.UseVisualStyleBackColor = true;
            // 
            // cbIsPac
            // 
            cbIsPac.AutoSize = true;
            cbIsPac.Location = new Point(218, 97);
            cbIsPac.Name = "cbIsPac";
            cbIsPac.Size = new Size(142, 35);
            cbIsPac.TabIndex = 6;
            cbIsPac.Text = "PAC代理";
            cbIsPac.UseVisualStyleBackColor = true;
            // 
            // cbUseSysProxy
            // 
            cbUseSysProxy.AutoSize = true;
            cbUseSysProxy.Location = new Point(330, 43);
            cbUseSysProxy.Name = "cbUseSysProxy";
            cbUseSysProxy.Size = new Size(94, 35);
            cbUseSysProxy.TabIndex = 5;
            cbUseSysProxy.Text = "启用";
            cbUseSysProxy.UseVisualStyleBackColor = true;
            // 
            // cmbSysProxy
            // 
            cmbSysProxy.FormattingEnabled = true;
            cmbSysProxy.Location = new Point(12, 43);
            cmbSysProxy.Name = "cmbSysProxy";
            cmbSysProxy.Size = new Size(230, 39);
            cmbSysProxy.TabIndex = 4;
            // 
            // SysProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 194);
            Controls.Add(groupBox3);
            Name = "SysProxyForm";
            Text = "系统代理";
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox3;
        private Label lbPac;
        private CheckBox cbIsEnv;
        private CheckBox cbIsPac;
        private CheckBox cbUseSysProxy;
        private ComboBox cmbSysProxy;
    }
}