using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Controls
{
    public partial class WebBrowserControl : UserControl
    {
        #region Fields

        private String m_htmlResource; 

        #endregion

        #region WebBrowserControl()
        public WebBrowserControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        } 
        #endregion

        #region HtmlResource
        public String HtmlResource
        {
            get { return m_htmlResource; }
            set
            {
                if ((value == null) || (value.Trim() == String.Empty))
                    return;

                m_htmlResource = value;
                webBrowser.DocumentStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("netHPC.ManagementConsole.Html." + m_htmlResource);
            }
        } 
        #endregion
    }
}
