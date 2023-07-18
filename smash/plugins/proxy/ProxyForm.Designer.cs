namespace smash.plugins.proxy
{
    partial class ProxyForm
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
            labelProxy = new Label();
            cmbProxy = new ComboBox();
            mainMenu = new MenuStrip();
            mainMenuOptions = new ToolStripMenuItem();
            mainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // labelProxy
            // 
            labelProxy.Location = new Point(49, 107);
            labelProxy.Name = "labelProxy";
            labelProxy.Size = new Size(417, 38);
            labelProxy.TabIndex = 1;
            labelProxy.Text = "代理地址";
            labelProxy.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmbProxy
            // 
            cmbProxy.FormattingEnabled = true;
            cmbProxy.Location = new Point(136, 60);
            cmbProxy.Name = "cmbProxy";
            cmbProxy.Size = new Size(230, 39);
            cmbProxy.TabIndex = 0;
            cmbProxy.SelectedIndexChanged += cmbProxy_SelectedIndexChanged;
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { mainMenuOptions });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(514, 39);
            mainMenu.TabIndex = 2;
            mainMenu.Text = "menuStrip1";
            // 
            // mainMenuOptions
            // 
            mainMenuOptions.Name = "mainMenuOptions";
            mainMenuOptions.Size = new Size(130, 35);
            mainMenuOptions.Text = "配置选项";
            mainMenuOptions.Click += OnMainMenuOptionsClick;
            // 
            // ProxyForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(514, 171);
            Controls.Add(labelProxy);
            Controls.Add(cmbProxy);
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            Name = "ProxyForm";
            Text = "代理配置";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label labelProxy;
        private ComboBox cmbProxy;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mainMenuOptions;
    }
}