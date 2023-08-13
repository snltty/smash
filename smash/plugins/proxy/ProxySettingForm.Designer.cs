namespace smash.plugins
{
    partial class ProxySettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxySettingForm));
            proxysView = new DataGridView();
            contextMenu = new ContextMenuStrip(components);
            mainManuUseProxy = new ToolStripMenuItem();
            mainMenuDelProxy = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)proxysView).BeginInit();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // proxysView
            // 
            proxysView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            proxysView.Dock = DockStyle.Fill;
            proxysView.Location = new Point(0, 0);
            proxysView.Name = "proxysView";
            proxysView.RowHeadersWidth = 82;
            proxysView.RowTemplate.Height = 40;
            proxysView.Size = new Size(932, 543);
            proxysView.TabIndex = 3;
            // 
            // contextMenu
            // 
            contextMenu.ImageScalingSize = new Size(32, 32);
            contextMenu.Items.AddRange(new ToolStripItem[] { mainManuUseProxy, mainMenuDelProxy });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(185, 80);
            // 
            // mainManuUseProxy
            // 
            mainManuUseProxy.Name = "mainManuUseProxy";
            mainManuUseProxy.Size = new Size(184, 38);
            mainManuUseProxy.Text = "添加新项";
            mainManuUseProxy.Click += MainManuUseProxy_Click;
            // 
            // mainMenuDelProxy
            // 
            mainMenuDelProxy.Name = "mainMenuDelProxy";
            mainMenuDelProxy.Size = new Size(184, 38);
            mainMenuDelProxy.Text = "删除选中";
            mainMenuDelProxy.Click += MainMenuDelProxy_Click;
            // 
            // ProxySettingForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(932, 543);
            Controls.Add(proxysView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 5, 6, 5);
            Name = "ProxySettingForm";
            Text = "代理";
            FormClosing += OnFormClosing;
            ((System.ComponentModel.ISupportInitialize)proxysView).EndInit();
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private DataGridView proxysView;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem mainManuUseProxy;
        private ToolStripMenuItem mainMenuDelProxy;
    }
}