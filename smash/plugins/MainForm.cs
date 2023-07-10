using System.Diagnostics;
using System.Reflection;
using common.libs;
using smash.plugins.hijack;
using smash.plugins.sysProxy;
using smash.plugin;

namespace smash.plugins
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon = null;
        Image unright = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"smash.public.right-gray.png"));
        Image right = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"smash.public.right.png"));
        Icon icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"smash.public.icon.ico"));
        Icon iconGray = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"smash.public.icon-gray.ico"));
        string name = "smash进程劫持代理";

        private readonly Config config = new Config();
        private readonly HijackController hijackController;
        private readonly SysProxyController SysProxyController;

        private ToolStripItem startupMenu;
        private ToolStripItem startMenu;

        bool hideForm;

        public MainForm(StartUpArgInfo startUpArgInfo, HijackController hijackController, SysProxyController SysProxyController, HijackOptionsForm hijackOptionsForm, HijackProcessForm hijackProcessForm)
        {
            HideForm(startUpArgInfo.Args);
            config = Config.Load();
            this.hijackController = hijackController;
            this.SysProxyController = SysProxyController;

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();
            InitialTray();

            AddTablePage();

        }

        #region 托盘
        private void InitialTray()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.BalloonTipTitle = name;
            notifyIcon.BalloonTipText = name + "已启动";
            notifyIcon.Text = name;

            notifyIcon.Icon = iconGray;
            notifyIcon.Visible = true;

            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            startupMenu = notifyIcon.ContextMenuStrip.Items.Add("程序自启动", unright, StartUp);
            startMenu = notifyIcon.ContextMenuStrip.Items.Add("启动进程驱动", unright, Switch);
            notifyIcon.ContextMenuStrip.Items.Add("退出", null, Close);
            notifyIcon.DoubleClick += ContextMenuStrip_MouseDoubleClick;

            StartUp();
        }
        private void ContextMenuStrip_MouseDoubleClick(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            Show();
        }
        bool menuClose = false;
        private void Close(object sender, EventArgs e)
        {
            menuClose = true;
            this.Close();
        }
        private new void Closing(object sender, FormClosingEventArgs e)
        {
            HideForm();
            if (menuClose == false)
            {
                e.Cancel = true;
            }
            menuClose = false;
        }
        private void OnMainFormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }
        private void HideForm(string[] args)
        {
            hideForm = args.Length > 0 && args[0] == "/service";
        }
        private void HideForm()
        {
            ShowInTaskbar = false;
            Hide();
            GCHelper.FlushMemory();
        }
        #endregion

        #region 自启动
        class Model
        {
            public string Key { get; set; }
            public string Path { get; set; }
        }
        private Model GetInfo()
        {
            string currentPath = Application.StartupPath;
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            string keyName = exeName.Replace(".exe", "");
            return new Model
            {
                Key = keyName,
                Path = System.IO.Path.Combine(currentPath, exeName)
            };
        }
        bool isStartUp = false;
        private void StartUp(object sender, EventArgs e)
        {
            Model model = GetInfo();
            try
            {
                if (isStartUp == false)
                {
                    CommandHelper.Windows("schtasks.exe", new string[] {
                        "schtasks.exe /create /tn \""+model.Key+"\" /rl highest /sc ONSTART /delay 0000:30 /tr \""+model.Path+" /service\" /f"
                    });
                    notifyIcon.BalloonTipText = "已设置自启动";
                    notifyIcon.ShowBalloonTip(1000);
                }
                else
                {
                    CommandHelper.Windows("schtasks.exe", new string[] {
                        "schtasks /delete  /TN "+model.Key+" /f"
                    });
                    notifyIcon.BalloonTipText = "已取消自启动";
                    notifyIcon.ShowBalloonTip(1000);
                }
            }
            catch (Exception ex)
            {
                notifyIcon.BalloonTipText = ex.Message;
                notifyIcon.ShowBalloonTip(1000);
            }
            StartUp();
        }
        private void StartUp()
        {
            Model model = GetInfo();
            string res = CommandHelper.Windows("", new string[] {
                "schtasks.exe /query /fo TABLE|findstr \"" + model.Key + "\""
            });
            bool has = false;
            foreach (string item in res.Split('\n'))
            {
                if (item.StartsWith(model.Key))
                {
                    has = true;
                    break;
                }
            }

            if (has == false)
            {
                isStartUp = false;
                startupMenu.Image = unright;
            }
            else
            {
                startupMenu.Image = right;
                isStartUp = true;
            }
        }
        #endregion


        #region 标签页
        private void AddTablePage()
        {
            int top = 0;
            foreach (ITabForm item in PluginLoader.TabForms.OrderBy(c => c.Order))
            {
                Form form = item as Form;
                form.Top = top;
                form.Left = 10;
                form.TopLevel = false;
                form.Visible = true;
                form.FormBorderStyle = FormBorderStyle.None;
                mainPanel.Controls.Add(form);
                top += form.Height + 20;
            }
        }
        #endregion


        bool starting = false;
        bool running = false;
        private void SetButtonTest()
        {
            this.Invoke(() =>
            {
                if (starting)
                {
                    startBtn.Text = "正在操作";
                }
                else if (running)
                {
                    startBtn.Text = "停止驱动";
                    notifyIcon.Icon = icon;
                    startMenu.Image = right;
                }
                else
                {
                    startBtn.Text = "启动驱动";
                    notifyIcon.Icon = iconGray;
                    startMenu.Image = unright;
                }
            });
        }

        private void Switch(object sender, EventArgs e)
        {
            if (running)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }
        private void Start()
        {
            /*
            if (config.UseHijack == false && config.UseSysProxy == false)
            {
                MessageBox.Show("至少要使用一种方式，不然没意义");
                return;
            }

            if (config.UseHijack && config.Process == null)
            {
                MessageBox.Show("未选进程劫持规则");
                return;
            }
            if (config.UseSysProxy && config.SysProxy == null)
            {
                MessageBox.Show("未选系统代理规则");
                return;
            }
            */

            if (starting) return;
            starting = true;
            SetButtonTest();

            Task.Run(() =>
            {
                try
                {
                    hijackController.Start();
                    SysProxyController.Start();

                    starting = false;
                    running = true;
                    config.Running = true;
                    config.Save();
                }
                catch (Exception ex)
                {
                    Stop();
                    starting = false;
                    Debug.WriteLine(ex + "");
                    MessageBox.Show(ex.Message);
                }
                SetButtonTest();
                CommandHelper.Windows(string.Empty, new string[] { "ipconfig/flushdns" });
            });
        }
        private void Stop()
        {
            if (starting) return;
            starting = true;
            SetButtonTest();
            Task.Run(() =>
            {
                try
                {
                    hijackController.Stop();
                    SysProxyController.Stop();
                    starting = false;
                    running = false;
                    config.Running = false;
                    config.Save();
                    GCHelper.FlushMemory();
                }
                catch (Exception ex)
                {
                    starting = false;
                    MessageBox.Show(ex.Message);
                }
                SetButtonTest();
                CommandHelper.Windows(string.Empty, new string[] { "ipconfig/flushdns" });
            });
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            Switch(null, null);
        }
        private void OnWinLoad(object sender, EventArgs e)
        {
            GCHelper.FlushMemory();
            if (config.Running)
            {
                Start();
            }
            if (hideForm)
            {
                HideForm();
            }
        }


    }
}