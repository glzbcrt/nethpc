
using netHPC;
using netHPC.ManagementConsole;
using netHPC.Shared;
using netHPC.SDK;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Wizards
{
    public partial class StartAlgorithmWizard : Form
    {
        #region Fields

        private IConfigurationDialog m_configuratorDialog;
        private Algorithm m_algorithm;

        #endregion

        #region StartAlgorithmWizard()
        public StartAlgorithmWizard(Algorithm algorithm)
        {
            InitializeComponent();
            m_algorithm = algorithm;
        } 
        #endregion

        #region formWizardBase_Load(object sender, EventArgs e)
        private void formWizardBase_Load(object sender, EventArgs e)
        {
            foreach (Node node in SnapInTools.Entities.Node.OrderBy("it.name"))
            {
                ListViewItem listViewItem = new ListViewItem(node.Name);
                listViewItem.SubItems.Add(node.NumOfExecUnits.ToString());
                listViewItem.SubItems.Add(node.Description);
                listViewItem.Tag = node;
                listViewNodes.Items.Add(listViewItem);
            }
                
            foreach (Type type in Assembly.Load(m_algorithm.Assembly).GetTypes())
            {
                if ((type.IsClass) && (type.Name == "ExecutionConfiguration"))//(type.GetInterface("netHPC.SDK.IConfiguratorDialog") != null))
                {                        
                    m_configuratorDialog = (IConfigurationDialog)Activator.CreateInstance(type);                        
                    tabControlStartExecution.TabPages[1].Controls.Add(m_configuratorDialog as UserControl);             
                }
            }
        } 
        #endregion

        #region buttonCancel_Click(object sender, EventArgs e)
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        } 
        #endregion

        #region buttonSelectAll_Click(object sender, EventArgs e)
        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewNodes.Items)
                listViewItem.Checked = true;
        } 
        #endregion

        #region buttonDeselectAll_Click(object sender, EventArgs e)
        private void buttonDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewNodes.Items)
                listViewItem.Checked = false;
        }
        #endregion

        #region buttonStart_Click(object sender, EventArgs e)
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Trim() == String.Empty)
            {
                MessageBox.Show("name!");
                tabControlStartExecution.SelectedIndex = 0;
                return;
            }

            if (!m_configuratorDialog.ValidateFieldsOnScreen())
            {
                tabControlStartExecution.SelectedIndex = 1;
                return;
            }

            Int32 tmp = 0;
            foreach(ListViewItem listViewItem in listViewNodes.Items)
                tmp = tmp + (listViewItem.Checked ? 1 : -1);

            if (tmp <= 0)
            {
                tabControlStartExecution.SelectedIndex = 2;
                return;
            }

            Node[] arr = new Node[tmp];
            tmp = 0;
            foreach (ListViewItem listViewItem in listViewNodes.Items)
            {
                if (listViewItem.Checked)
                {
                    arr[tmp] = listViewItem.Tag as Node;
                    tmp++;
                }
            }

            try
            {
                tmp = SnapInTools.StartExecution(textBoxName.Text, textBoxDescription.Text, m_algorithm, arr, m_configuratorDialog.GetParameters());
                MessageBox.Show(String.Format("The algorithm's execution number {0} has started.\nPlease verify its execution status through the report.", tmp.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            DialogResult = DialogResult.OK;
            Close();
        } 
        #endregion

        #region listViewNodes_ItemChecked(object sender, ItemCheckedEventArgs e)
        private void listViewNodes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Int32 m_selectedComputers = 0;
            Int32 m_selectedCores = 0;
            Node node;

            foreach (ListViewItem item in listViewNodes.Items)
            {
                node = e.Item.Tag as Node;

                m_selectedComputers += item.Checked ? 1 : 0;
                m_selectedCores += item.Checked ? node.NumOfExecUnits : 0;
            }

            labelSelected.Text = "Selected computer/cores: " + m_selectedComputers.ToString() + "/" + m_selectedCores.ToString();
        } 
        #endregion

        private void tabControlStartExecution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlStartExecution.SelectedIndex == 3)
            {
                textBoxSummary.Text = String.Format("\r\n       Name: {0}\r\nDescription: {1}", textBoxName.Text, textBoxDescription.Text);
            }
        }
    }
}
