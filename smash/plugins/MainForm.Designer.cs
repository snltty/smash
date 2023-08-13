namespace smash.plugins
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            startBtn = new Button();
            mainTab = new TabControl();
            SuspendLayout();
            // 
            // startBtn
            // 
            startBtn.Location = new Point(349, 367);
            startBtn.Margin = new Padding(6, 5, 6, 5);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(188, 78);
            startBtn.TabIndex = 2;
            startBtn.Text = "启动驱动";
            startBtn.UseVisualStyleBackColor = true;
            startBtn.Click += startBtn_Click;
            // 
            // mainTab
            // 
            mainTab.Location = new Point(12, 9);
            mainTab.Name = "mainTab";
            mainTab.SelectedIndex = 0;
            mainTab.Size = new Size(850, 347);
            mainTab.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(874, 459);
            Controls.Add(mainTab);
            Controls.Add(startBtn);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 5, 6, 5);
            Name = "MainForm";
            Text = "smash";
            FormClosing += Closing;
            FormClosed += OnMainFormClosed;
            Load += OnWinLoad;
            ResumeLayout(false);
        }

        #endregion
        private Button startBtn;
        private TabControl mainTab;
    }
}