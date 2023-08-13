namespace smash.plugins
{
    partial class SysProxySettingForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SysProxySettingForm));
            sysProxysView = new DataGridView();
            contextMenu = new ContextMenuStrip(components);
            mainMenuAddProxy = new ToolStripMenuItem();
            mainMenuDelProxy = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)sysProxysView).BeginInit();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // sysProxysView
            // 
            sysProxysView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            sysProxysView.Dock = DockStyle.Fill;
            sysProxysView.Location = new Point(0, 0);
            sysProxysView.Name = "sysProxysView";
            sysProxysView.RowHeadersWidth = 82;
            sysProxysView.RowTemplate.Height = 40;
            sysProxysView.Size = new Size(800, 403);
            sysProxysView.TabIndex = 0;
            // 
            // contextMenu
            // 
            contextMenu.ImageScalingSize = new Size(32, 32);
            contextMenu.Items.AddRange(new ToolStripItem[] { mainMenuAddProxy, mainMenuDelProxy });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(185, 80);
            // 
            // mainMenuAddProxy
            // 
            mainMenuAddProxy.Name = "mainMenuAddProxy";
            mainMenuAddProxy.Size = new Size(184, 38);
            mainMenuAddProxy.Text = "添加新项";
            mainMenuAddProxy.Click += OnMainMenuAddProxyClick;
            // 
            // mainMenuDelProxy
            // 
            mainMenuDelProxy.Name = "mainMenuDelProxy";
            mainMenuDelProxy.Size = new Size(184, 38);
            mainMenuDelProxy.Text = "删除选中";
            mainMenuDelProxy.Click += MainMenuDelProxy_Click;
            // 
            // SysProxySettingForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 403);
            Controls.Add(sysProxysView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SysProxySettingForm";
            Text = "系统代理";
            ((System.ComponentModel.ISupportInitialize)sysProxysView).EndInit();
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView sysProxysView;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem mainMenuDelProxy;
        private ToolStripMenuItem mainMenuAddProxy;
    }
}