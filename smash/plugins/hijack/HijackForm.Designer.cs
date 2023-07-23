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
            mainMenu = new MenuStrip();
            mainMenuOptions = new ToolStripMenuItem();
            mainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // cbFilterDns
            // 
            cbFilterDns.AutoSize = true;
            cbFilterDns.Location = new Point(310, 114);
            cbFilterDns.Name = "cbFilterDns";
            cbFilterDns.Size = new Size(98, 35);
            cbFilterDns.TabIndex = 6;
            cbFilterDns.Text = "DNS";
            cbFilterDns.UseVisualStyleBackColor = true;
            cbFilterDns.Click += CheckedChanged;
            // 
            // cbFilterUdp
            // 
            cbFilterUdp.AutoSize = true;
            cbFilterUdp.Location = new Point(207, 114);
            cbFilterUdp.Name = "cbFilterUdp";
            cbFilterUdp.Size = new Size(97, 35);
            cbFilterUdp.TabIndex = 5;
            cbFilterUdp.Text = "UDP";
            cbFilterUdp.UseVisualStyleBackColor = true;
            cbFilterUdp.Click += CheckedChanged;
            // 
            // cbFilterTcp
            // 
            cbFilterTcp.AutoSize = true;
            cbFilterTcp.Location = new Point(110, 114);
            cbFilterTcp.Name = "cbFilterTcp";
            cbFilterTcp.Size = new Size(91, 35);
            cbFilterTcp.TabIndex = 4;
            cbFilterTcp.Text = "TCP";
            cbFilterTcp.UseVisualStyleBackColor = true;
            cbFilterTcp.Click += CheckedChanged;
            // 
            // cbUseHijack
            // 
            cbUseHijack.AutoSize = true;
            cbUseHijack.Location = new Point(207, 174);
            cbUseHijack.Name = "cbUseHijack";
            cbUseHijack.Size = new Size(94, 35);
            cbUseHijack.TabIndex = 3;
            cbUseHijack.Text = "启用";
            cbUseHijack.UseVisualStyleBackColor = true;
            cbUseHijack.Click += CheckedChanged;
            // 
            // cmbGroup
            // 
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(136, 60);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(230, 39);
            cmbGroup.TabIndex = 1;
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { mainMenuOptions });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(514, 39);
            mainMenu.TabIndex = 7;
            mainMenu.Text = "menuStrip1";
            // 
            // mainMenuOptions
            // 
            mainMenuOptions.Name = "mainMenuOptions";
            mainMenuOptions.Size = new Size(130, 35);
            mainMenuOptions.Text = "进程选项";
            mainMenuOptions.Click += OnMainMenuOptionsClick;
            // 
            // HijackForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(514, 230);
            Controls.Add(cbFilterDns);
            Controls.Add(cbFilterUdp);
            Controls.Add(cbFilterTcp);
            Controls.Add(cmbGroup);
            Controls.Add(cbUseHijack);
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            Name = "HijackForm";
            Text = "进程劫持";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox cbUseHijack;
        private ComboBox cmbGroup;
        private CheckBox cbFilterTcp;
        private CheckBox cbFilterUdp;
        private CheckBox cbFilterDns;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mainMenuOptions;
    }
}