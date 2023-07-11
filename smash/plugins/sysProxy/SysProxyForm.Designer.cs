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
            lbPac = new Label();
            cbIsEnv = new CheckBox();
            cbIsPac = new CheckBox();
            cbUseSysProxy = new CheckBox();
            cmbSysProxy = new ComboBox();
            SuspendLayout();
            // 
            // lbPac
            // 
            lbPac.Location = new Point(24, 127);
            lbPac.Name = "lbPac";
            lbPac.Size = new Size(406, 31);
            lbPac.TabIndex = 8;
            lbPac.Text = "pac地址";
            lbPac.TextAlign = ContentAlignment.TopCenter;
            // 
            // cbIsEnv
            // 
            cbIsEnv.AutoSize = true;
            cbIsEnv.Location = new Point(59, 79);
            cbIsEnv.Name = "cbIsEnv";
            cbIsEnv.Size = new Size(190, 35);
            cbIsEnv.TabIndex = 7;
            cbIsEnv.Text = "环境变量代理";
            cbIsEnv.UseVisualStyleBackColor = true;
            cbIsEnv.CheckedChanged += CheckedChanged;
            // 
            // cbIsPac
            // 
            cbIsPac.AutoSize = true;
            cbIsPac.Location = new Point(265, 79);
            cbIsPac.Name = "cbIsPac";
            cbIsPac.Size = new Size(142, 35);
            cbIsPac.TabIndex = 6;
            cbIsPac.Text = "PAC代理";
            cbIsPac.UseVisualStyleBackColor = true;
            cbIsPac.CheckedChanged += CheckedChanged;
            // 
            // cbUseSysProxy
            // 
            cbUseSysProxy.AutoSize = true;
            cbUseSysProxy.Location = new Point(184, 183);
            cbUseSysProxy.Name = "cbUseSysProxy";
            cbUseSysProxy.Size = new Size(94, 35);
            cbUseSysProxy.TabIndex = 5;
            cbUseSysProxy.Text = "启用";
            cbUseSysProxy.UseVisualStyleBackColor = true;
            cbUseSysProxy.CheckedChanged += CheckedChanged;
            // 
            // cmbSysProxy
            // 
            cmbSysProxy.FormattingEnabled = true;
            cmbSysProxy.Location = new Point(100, 24);
            cmbSysProxy.Name = "cmbSysProxy";
            cmbSysProxy.Size = new Size(230, 39);
            cmbSysProxy.TabIndex = 4;
            cmbSysProxy.SelectedIndexChanged += cmbSysProxy_SelectedIndexChanged;
            // 
            // SysProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(451, 251);
            Controls.Add(lbPac);
            Controls.Add(cbIsEnv);
            Controls.Add(cmbSysProxy);
            Controls.Add(cbIsPac);
            Controls.Add(cbUseSysProxy);
            Name = "SysProxyForm";
            Text = "系统代理";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lbPac;
        private CheckBox cbIsEnv;
        private CheckBox cbIsPac;
        private CheckBox cbUseSysProxy;
        private ComboBox cmbSysProxy;
    }
}