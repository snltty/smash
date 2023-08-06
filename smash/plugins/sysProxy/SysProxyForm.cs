using common.libs.extends;
using smash.plugin;
using smash.plugins.proxy;
using System;
using System.Diagnostics;
using System.Net;

namespace smash.plugins.sysProxy
{
    public partial class SysProxyForm : Form, ITabForm
    {
        public int Order => 2;

        private readonly SysProxyConfig sysProxyConfig;
        public SysProxyForm(SysProxyConfig sysProxyConfig)
        {
            this.sysProxyConfig = sysProxyConfig;
            InitializeComponent();
            BindInfo();
        }

        private void BindInfo()
        {
            BindSysProxy();
        }

        private void BindSysProxy()
        {
            cbUseSysProxy.Checked = sysProxyConfig.UseSysProxy;
            cmbSysProxy.DataSource = null;
           
            if (sysProxyConfig.SysProxy != null)
            {
                int index = 0;
                for (int i = 0; i < sysProxyConfig.SysProxys.Count; i++)
                {
                    if (sysProxyConfig.SysProxys[i].Name == sysProxyConfig.SysProxy.Name)
                    {
                        index = i;
                        break;
                    }
                }
                cmbSysProxy.DataSource = sysProxyConfig.SysProxys.Select(c => c.Name).ToList();
                cmbSysProxy.SelectedIndex = index;
            }
            else if (sysProxyConfig.SysProxys.Count > 0)
            {
                cmbSysProxy.DataSource = sysProxyConfig.SysProxys.Select(c => c.Name).ToList();
                cmbSysProxy.SelectedIndex =  0;
            }
        }
        private void cmbSysProxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSysProxy.SelectedItem != null)
            {
                sysProxyConfig.SysProxy = sysProxyConfig.SysProxys.FirstOrDefault(c => c.Name == cmbSysProxy.SelectedItem.ToString());
                lbPac.Text = $"{sysProxyConfig.SysProxy.Pac}";
                cbIsPac.Checked = sysProxyConfig.SysProxy.IsPac;
                cbIsEnv.Checked = sysProxyConfig.SysProxy.IsEnv;

                sysProxyConfig.Save();
            }
        }
        private void UpdateConfig()
        {
            sysProxyConfig.UseSysProxy = cbUseSysProxy.Checked;
            if (sysProxyConfig.SysProxy != null)
            {
                sysProxyConfig.SysProxy.IsEnv = cbIsEnv.Checked;
                sysProxyConfig.SysProxy.IsPac = cbIsPac.Checked;
            }
            BindInfo();
            sysProxyConfig.Save();
        }
        private void CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }


        SysProxySettingForm sysProxySettingForm;
        private void OnMainMenuOptionsClick(object sender, EventArgs e)
        {
            sysProxySettingForm = new SysProxySettingForm(sysProxyConfig);
            sysProxySettingForm.FormClosed += (a, b) =>
            {
                sysProxySettingForm = null;
                BindInfo();
            };
            sysProxySettingForm.ShowDialog();
        }
    }
}
