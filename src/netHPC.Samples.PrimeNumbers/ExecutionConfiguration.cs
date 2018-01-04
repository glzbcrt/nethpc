
using netHPC.SDK;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.Samples.PrimeNumbers
{
    public partial class ExecutionConfiguration : UserControl, IConfigurationDialog
    {
        #region Fields

        private UInt64 m_startValue;
        private UInt64 m_finishValue; 

        #endregion

        #region ExecutionConfiguration()
        public ExecutionConfiguration()
        {
            InitializeComponent();
            comboBoxWorkItemSize.SelectedIndex = 1;
        } 
        #endregion

        #region GetParameters()
        public String GetParameters()
        {
            return String.Format("{0};{1};{2}", m_startValue, m_finishValue, comboBoxWorkItemSize.SelectedIndex);
        } 
        #endregion

        #region GetSummaryText()
        public String GetSummaryText()
        {
            return String.Format("calculate the prime numbers between {0} and {1} using work items with {2} size.", m_startValue, m_finishValue, comboBoxWorkItemSize.SelectedItem.ToString().ToLower());
        } 
        #endregion

        #region ValidateFieldsOnScreen()
        public bool ValidateFieldsOnScreen()
        {
            if (!UInt64.TryParse(textBoxStart.Text, out m_startValue))
            {
                MessageBox.Show("start!");
                return false;
            }

            if (!UInt64.TryParse(textBoxFinish.Text, out m_finishValue))
            {
                MessageBox.Show("finish!");
                return false;
            }

            if (m_finishValue <= m_startValue)
            {
                MessageBox.Show("wrong!!");
                return false;
            }

            return true;
        } 
        #endregion
    }
}
