using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Dialogs
{
    public partial class ConnectDialog : Form
    {
        #region ConnectDialog()
        public ConnectDialog()
        {
            InitializeComponent();

            textBoxServerName.Text = SnapInTools.ServerName;
            textBoxDatabaseName.Text = SnapInTools.DatabaseName;
            textBoxUserName.Text = SnapInTools.UserName;
            textBoxUserPassword.Text = SnapInTools.UserPassword;
            textBoxHeadNode.Text = SnapInTools.HeadNode;
            maskedTextBoxHeadNodePort.Text = SnapInTools.HeadNodePort.ToString();
        } 
        #endregion

        #region buttonCancel_Click(object sender, EventArgs e)
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        } 
        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!SnapInTools.ValidateDatabaseConnection(textBoxServerName.Text, textBoxDatabaseName.Text, textBoxUserName.Text, textBoxUserPassword.Text))
            {
                SnapInTools.ShowError("It was not possible to connect to this server, please check if the information is correct.");
                return;
            }

            if (!SnapInTools.ValidateHeadNodeConnection(textBoxHeadNode.Text, Int32.Parse(maskedTextBoxHeadNodePort.Text)))
            {
                SnapInTools.ShowError("It was not possible to connect to this server, please check if the information is correct.");
                return;
            }

            SnapInTools.ServerName = textBoxServerName.Text;
            SnapInTools.DatabaseName = textBoxDatabaseName.Text;
            SnapInTools.UserName = textBoxUserName.Text;
            SnapInTools.UserPassword = textBoxUserPassword.Text;
            SnapInTools.HeadNode = textBoxHeadNode.Text;
            SnapInTools.HeadNodePort = Int32.Parse(maskedTextBoxHeadNodePort.Text);
            SnapInTools.ConnectDatabase();
            SnapInTools.ConnectHeadNode();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
