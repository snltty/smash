using System.Reflection;
using netch_process.libs;

namespace netch_process
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon = null;
        Image unright = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"netch_process.public.right-gray.png"));
        Image right = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"netch_process.public.right.png"));
        Icon icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"netch_process.public.icon.ico"));
        Icon iconGray = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"netch_process.public.icon-gray.ico"));
        string name = "netch进程劫持代理";

        Config config = new Config();

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();
            InitialTray();
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
            notifyIcon.ContextMenuStrip.Items.Add("自启动", unright, StartUp);
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
            ShowInTaskbar = false;
            Hide();
            if (menuClose == false)
            {
                e.Cancel = true;
            }
            menuClose = false;
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
                    Command.Windows("schtasks.exe", new string[] {
                        "schtasks.exe /create /tn \""+model.Key+"\" /rl highest /sc ONSTART /delay 0000:30 /tr \""+model.Path+"\" /f"
                    });
                    notifyIcon.BalloonTipText = "已设置自启动";
                    notifyIcon.ShowBalloonTip(1000);
                }
                else
                {
                    Command.Windows("schtasks.exe", new string[] {
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
            string res = Command.Windows("", new string[] {
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
                notifyIcon.ContextMenuStrip.Items[0].Image = unright;
            }
            else
            {
                notifyIcon.ContextMenuStrip.Items[0].Image = right;
                isStartUp = true;
            }
        }
        #endregion

        #region 子窗口

        ProxySettingForm proxyWin = null;
        private void OnProxySettingClick(object sender, EventArgs e)
        {
            proxyWin = new ProxySettingForm(config);
            proxyWin.ShowDialog();
            proxyWin.FormClosed += (object sender, FormClosedEventArgs e) =>
            {
                proxyWin = null;
            };
        }

        OptionsForm optionsWin = null;
        private void OnOptionsSettingClick(object sender, EventArgs e)
        {
            optionsWin = new OptionsForm(config);
            optionsWin.ShowDialog();
            optionsWin.FormClosed += (object sender, FormClosedEventArgs e) =>
            {
                optionsWin = null;
            };
        }

        AboutForm aboutWin = null;
        private void OnAboutClick(object sender, EventArgs e)
        {
            aboutWin = new AboutForm();
            aboutWin.ShowDialog();
            aboutWin.FormClosed += (object sender, FormClosedEventArgs e) =>
            {
                aboutWin = null;
            };
        }
        #endregion
    }
}