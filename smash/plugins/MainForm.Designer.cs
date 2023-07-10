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
            mainPanel = new Panel();
            SuspendLayout();
            // 
            // startBtn
            // 
            startBtn.Location = new Point(156, 560);
            startBtn.Margin = new Padding(6, 5, 6, 5);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(188, 78);
            startBtn.TabIndex = 2;
            startBtn.Text = "启动驱动";
            startBtn.UseVisualStyleBackColor = true;
            startBtn.Click += startBtn_Click;
            // 
            // mainPanel
            // 
            mainPanel.AutoScroll = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.Location = new Point(12, 12);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(450, 530);
            mainPanel.TabIndex = 8;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 659);
            Controls.Add(mainPanel);
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
        private Panel mainPanel;
    }
}