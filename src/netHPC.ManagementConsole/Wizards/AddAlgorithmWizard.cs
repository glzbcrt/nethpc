
using netHPC.SDK;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Wizards
{
    public partial class AddAlgorithmWizard : Form
    {
        private Algorithm m_algorithm;

        public AddAlgorithmWizard(String algorithmPath)
        {
            InitializeComponent();
            textBoxAlgorithm.Text = algorithmPath;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxName.Text.Trim()))
            {
                SnapInTools.ShowError("You must fill the algorithm name.");
                return;
            }
            
            //try
            //{
            //    Assembly algorithmAssembly = Assembly.LoadFile(textBoxAlgorithm.Text);
            //    foreach (Type type in algorithmAssembly.GetTypes())
            //    {
            //        if (type.IsClass)
            //        {
            //            MessageBox.Show(type.GetCustomAttributes(typeof(AboutDialogAttribute), true)[0].ToString());
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            try
            {
                m_algorithm = new Algorithm();
                m_algorithm.Name = textBoxName.Text;
                m_algorithm.Description = textBoxDescription.Text;
                m_algorithm.DateCreated = DateTime.Now;
                m_algorithm.DateModified = DateTime.Now;

                BinaryReader binaryReader = new BinaryReader(File.Open(textBoxAlgorithm.Text, FileMode.Open));
                m_algorithm.Assembly = binaryReader.ReadBytes((Int32)binaryReader.BaseStream.Length);
                binaryReader.Close();

                SnapInTools.Entities.AddToAlgorithm(m_algorithm);
                SnapInTools.Entities.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }

        public Algorithm Algorithm
        {
            get { return m_algorithm; }
        }

        #region buttonCancel_Click(object sender, EventArgs e)
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        } 
        #endregion
    }
}
