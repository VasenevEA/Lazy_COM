using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Management;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace LazyCOM
{
    public partial class LazyCOM : Form
    {
        static string[] oldPorts = SerialPort.GetPortNames();
        bool isAdmin;

        public LazyCOM()
        {
            InitializeComponent();
        }

        private void CheckPorts()
        {
            while (true)
            {
                Thread.Sleep(2000);
                string[] newPorts = SerialPort.GetPortNames();

                bool isAdded;
                foreach (string portName in GetDiffPorts(oldPorts, newPorts, out isAdded))
                {
                    var symbol = (isAdded) ? "+" : "-";
                    var Title = (isAdded) ? "Connected:" : "Disconnected:";
                    this.notifyIcon1.ShowBalloonTip(500, Title, symbol + portName, ToolTipIcon.Info);

                    Thread.Sleep(1000);
                }
                oldPorts = newPorts;
            }
        }

        private static List<string> GetDiffPorts(string[] oldPorts, string[] newPorts, out bool isAdded)
        {
            List<string> equalsPort = new List<string>();

            isAdded = newPorts.Length > oldPorts.Length;

            if (isAdded)
            {
                for (int i = 0; i < newPorts.Length; i++)
                {
                    int s = 0;
                    for (int j = 0; j < oldPorts.Length; j++)
                        if (newPorts[i] == oldPorts[j])
                            s++;

                    if (s == 0)
                        equalsPort.Add(newPorts[i]);
                }
            }
            else
            {
                for (int i = 0; i < oldPorts.Length; i++)
                {
                    int s = 0;
                    for (int j = 0; j < newPorts.Length; j++)
                        if (oldPorts[i] == newPorts[j])
                            s++;

                    if (s == 0)
                        equalsPort.Add(oldPorts[i]);
                }
            }
            return equalsPort;
        }

        private string GetPortDescription(string portName)
        {
            var ports = new string[] { };
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%" + portName + "%'");
            var portDesctiprion = String.Empty;
            foreach (ManagementObject queryObj in searcher.Get())
                portDesctiprion = queryObj["Caption"] as string;

            return portDesctiprion;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {
                //Hide();

                this.ShowInTaskbar = false;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Dispose();
            Application.Exit();
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            string portList = "";

            foreach (string port in oldPorts)
                portList += GetPortDescription(port) + "\r\n";

            if (oldPorts.Length == 0)
                portList = "None";

            this.notifyIcon1.ShowBalloonTip(1000, "Ports:", portList, ToolTipIcon.Info);
        }

        private void AutoloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdmin)
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                if (key.GetValue("LazyCOM") != null)
                {
                    key.DeleteValue("LazyCOM");
                }
                else
                {
                    key.SetValue("LazyCOM", Application.ExecutablePath);
                }
            }
            else
            {
                MessageBox.Show("Run program as administator");
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (isAdmin)
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                if (key.GetValue("LazyCOM") != null)
                {
                    АвтозагрузкаToolStripMenuItem.CheckState = CheckState.Checked;
                }
                else
                {
                    АвтозагрузкаToolStripMenuItem.CheckState = CheckState.Unchecked;
                }
            }

        }

        private void LazyCOM_Load(object sender, EventArgs e)
        {
            Process[] pr = Process.GetProcessesByName("LazyCOM");
            if (pr.Length > 1)
            {
                MessageBox.Show("Приложение уже запущено!");
                Application.Exit();
            }

            Thread backgroundThread = new Thread(CheckPorts);
            backgroundThread.IsBackground = true;

            //настройка меню трея
            notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.АвтозагрузкаToolStripMenuItem, this.выходToolStripMenuItem });

            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.ShowInTaskbar = false;

            backgroundThread.Start();
            notifyIcon1.Visible = true;
            АвтозагрузкаToolStripMenuItem.CheckOnClick = true;
        }
    }
}
