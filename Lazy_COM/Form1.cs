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
    public partial class Form1 : Form
    {
        static string[] oldPorts = new string[] {"1","2"};
        static string[] newPorts = new string[] {"1","2","123" };
       // static List<String> equalsPorts = new List<String>();
        static string[] equalsPorts = new string[] { };

        public Form1()
        {
            

            InitializeComponent();
            
            notifyIcon1.Visible = false;
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);
            this.Resize += new System.EventHandler(this.Form1_Resize);

            Thread backgroundThread = new Thread(checkPorts);
            backgroundThread.Start();
            //equalsPorts = oldPorts.Except(newPorts).Concat(newPorts.Except(oldPorts)).ToArray();


           // notifyIcon1.ShowBalloonTip(500, "Сообщение", equalsPorts[0]+"123", ToolTipIcon.Info);

            
            
            
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



        private void Form1_Resize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;

                
                
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_BalloonTipShown(object sender, EventArgs e)
        {

        }
    }
}
