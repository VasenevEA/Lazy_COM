using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lazy_COM
{
    public partial class Lazy_COM : Form
    {
        static string[] oldPorts = new string[] {};
        static string[] newPorts = new string[] {};
        static string[] equalsPorts = new string[] { };

        public Lazy_COM()
        {
            

            InitializeComponent();

            //по дефолту виден только трей
            notifyIcon1.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);
            //this.Resize += new System.EventHandler(this.Form1_Resize);
            
            //настройка меню трея
            notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.оПрограммеToolStripMenuItem, this.выходToolStripMenuItem });
            
            //запуск потока
            Thread backgroundThread = new Thread(checkPorts); 
            backgroundThread.Start();
        }
 
        private void checkPorts()
        {
            oldPorts = SerialPort.GetPortNames();

            while (true)
            {
                
                Thread.Sleep(2000);
                newPorts = SerialPort.GetPortNames();

                equalsPorts = oldPorts.Except(newPorts).Concat(newPorts.Except(oldPorts)).ToArray(); // linq выражения для нахождения разницы между двумя 
                 
                if(equalsPorts.Length > 0 )
                {
                    foreach (string portName in equalsPorts)
                    {
                        if (newPorts.Length > oldPorts.Length)
                        {
                            this.notifyIcon1.ShowBalloonTip(500, "COM-Port!", "+"+portName, ToolTipIcon.Info);    
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


        /*
        private void Form1_Resize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;  
            }
        }

        */

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.Show();
            //notifyIcon1.Visible = false;
            //WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_BalloonTipShown(object sender, EventArgs e)
        {

        }

      
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
