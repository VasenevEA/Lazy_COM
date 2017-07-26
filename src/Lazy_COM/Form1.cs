using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Lazy_COM
{
    public partial class Lazy_COM : Form
    {
        static string[] oldPorts = new string[] {};
        static string[] newPorts = new string[] {};

        bool isElevated;


        public Lazy_COM()
        {
            InitializeComponent();
        }

        private static List<string> getEqualsPorts(string[] oldPorts, string[] newPorts)
        {
            List<string> equalsPort = new List<string>();

            if (newPorts.Length > oldPorts.Length)
            {
                for (int i = 0; i < newPorts.Length; i++)
                {
                    int s = 0;
                    for (int j = 0; j < oldPorts.Length; j++)
                    {

                        if (newPorts[i] == oldPorts[j])
                        {
                            s = +1;
                        }
                    }
                    if (s == 0)
                    {
                        equalsPort.Add(newPorts[i]);
                    }
                }
            }

            if (newPorts.Length < oldPorts.Length)
            {
                for (int i = 0; i < oldPorts.Length; i++)
                {
                    int s = 0;
                    for (int j = 0; j < newPorts.Length; j++)
                    {

                        if (oldPorts[i] == newPorts[j])
                        {
                            s = +1;
                        }
                    }
                    if (s == 0)
                    {
                        equalsPort.Add(oldPorts[i]);
                    }
                }
            }
            return equalsPort;
        }
        
        private void checkPorts()
        {
            oldPorts = SerialPort.GetPortNames();

            while (true)
            {
                
                Thread.Sleep(2000);
                newPorts = SerialPort.GetPortNames();

                if (newPorts.Length != oldPorts.Length)
                {
                    foreach (string portName in getEqualsPorts(oldPorts,newPorts))
                    {
                        if (newPorts.Length > oldPorts.Length)
                        {
                            this.notifyIcon1.ShowBalloonTip(500, "COM-Port! ", "+"+portName, ToolTipIcon.Info);    
                        }
                        if(newPorts.Length < oldPorts.Length)
                        {
                            this.notifyIcon1.ShowBalloonTip(500, "COM-Port!", "-"+portName, ToolTipIcon.Info); 
                        }

                        Thread.Sleep(2000);
                    }
                    oldPorts = newPorts;
                }
                
            }
        }
      
        private void Form1_Resize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {
                //Hide();
                 
                this.ShowInTaskbar = false;
            }
        }

     
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Dispose();
            Application.Exit();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            string portList = "";
            
            foreach (string port in oldPorts)
            {
                portList += port + "\r\n";
            }
            if(portList.Length != 0)
            {
            this.notifyIcon1.ShowBalloonTip(1000, "COM-Ports:", portList, ToolTipIcon.Info); 
            }
            else
            {
                this.notifyIcon1.ShowBalloonTip(1000, "COM-Ports:", "None", ToolTipIcon.Info);
            }
        }
 
        private void АвтозагрузкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            

            if (isElevated)
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                if (key.GetValue("Lazy_COM") != null)
                {
                    key.DeleteValue("Lazy_COM");
                }
                else
                {
                    key.SetValue("Lazy_COM", Application.ExecutablePath);
                }
            }
            else
            {
                MessageBox.Show("Run program as administator");
            }
           
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (isElevated)
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                if (key.GetValue("Lazy_COM") != null)
                {
                    // АвтозагрузкаToolStripMenuItem.Text = "Автозагрузка(Да)";
                    АвтозагрузкаToolStripMenuItem.CheckState = CheckState.Checked;
                }
                else
                {
                    // АвтозагрузкаToolStripMenuItem.Text = "Автозагрузка(Нет)";
                    АвтозагрузкаToolStripMenuItem.CheckState = CheckState.Unchecked;
                }
            }
            
        }

        private void Lazy_COM_Load(object sender, EventArgs e)
        {
            Process[] pr = Process.GetProcessesByName("Lazy_COM");
            if (pr.Length > 1)
            {
                MessageBox.Show("Приложение уже запущено!");
                Application.Exit();
            }

            Thread backgroundThread = new Thread(checkPorts);
            backgroundThread.IsBackground = true;

            //настройка меню трея
            notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.АвтозагрузкаToolStripMenuItem, this.выходToolStripMenuItem });

            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.ShowInTaskbar = false;

            backgroundThread.Start();
            notifyIcon1.Visible = true;
            АвтозагрузкаToolStripMenuItem.CheckOnClick = true;
        }
    }
}
