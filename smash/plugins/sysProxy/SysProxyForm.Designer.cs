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
            mainMenu = new MenuStrip();
            mainMenuOptions = new ToolStripMenuItem();
            mainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // lbPac
            // 
            lbPac.Location = new Point(129, 174);
            lbPac.Name = "lbPac";
            lbPac.Size = new Size(262, 31);
            lbPac.TabIndex = 8;
            lbPac.Text = "pac文件";
            lbPac.TextAlign = ContentAlignment.TopCenter;
            // 
            // cbIsEnv
            // 
            cbIsEnv.AutoSize = true;
            cbIsEnv.Location = new Point(129, 121);
            cbIsEnv.Name = "cbIsEnv";
            cbIsEnv.Size = new Size(142, 35);
            cbIsEnv.TabIndex = 7;
            cbIsEnv.Text = "环境变量";
            cbIsEnv.UseVisualStyleBackColor = true;
            cbIsEnv.Click += CheckedChanged;
            // 
            // cbIsPac
            // 
            cbIsPac.AutoSize = true;
            cbIsPac.Location = new Point(297, 121);
            cbIsPac.Name = "cbIsPac";
            cbIsPac.Size = new Size(94, 35);
            cbIsPac.TabIndex = 6;
            cbIsPac.Text = "PAC";
            cbIsPac.UseVisualStyleBackColor = true;
            cbIsPac.Click += CheckedChanged;
            // 
            // cbUseSysProxy
            // 
            cbUseSysProxy.AutoSize = true;
            cbUseSysProxy.Location = new Point(212, 231);
            cbUseSysProxy.Name = "cbUseSysProxy";
            cbUseSysProxy.Size = new Size(94, 35);
            cbUseSysProxy.TabIndex = 5;
            cbUseSysProxy.Text = "启用";
            cbUseSysProxy.UseVisualStyleBackColor = true;
            cbUseSysProxy.Click += CheckedChanged;
            // 
            // cmbSysProxy
            // 
            cmbSysProxy.FormattingEnabled = true;
            cmbSysProxy.Location = new Point(136, 60);
            cmbSysProxy.Name = "cmbSysProxy";
            cmbSysProxy.Size = new Size(230, 39);
            cmbSysProxy.TabIndex = 4;
            cmbSysProxy.SelectedIndexChanged += cmbSysProxy_SelectedIndexChanged;
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { mainMenuOptions });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(514, 39);
            mainMenu.TabIndex = 11;
            mainMenu.Text = "menuStrip1";
            // 
            // mainMenuOptions
            // 
            mainMenuOptions.Name = "mainMenuOptions";
            mainMenuOptions.Size = new Size(178, 35);
            mainMenuOptions.Text = "系统代理选项";
            mainMenuOptions.Click += OnMainMenuOptionsClick;
            // 
            // SysProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(514, 305);
            Controls.Add(cmbSysProxy);
            Controls.Add(lbPac);
            Controls.Add(cbUseSysProxy);
            Controls.Add(mainMenu);
            Controls.Add(cbIsEnv);
            Controls.Add(cbIsPac);
            MainMenuStrip = mainMenu;
            Name = "SysProxyForm";
            Text = "系统代理";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lbPac;
        private CheckBox cbIsEnv;
        private CheckBox cbIsPac;
        private CheckBox cbUseSysProxy;
        private ComboBox cmbSysProxy;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mainMenuOptions;
    }
}