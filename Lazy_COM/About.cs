using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lazy_COM
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Resize(object sender, EventArgs e)
        {
            this.Height = 300;
            this.Width = 310;
        }
    }
}
