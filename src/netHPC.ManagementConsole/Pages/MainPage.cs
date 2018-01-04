using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Pages
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;
            webBrowserControl.HtmlResource = "index.html";         
        }
    }
}
