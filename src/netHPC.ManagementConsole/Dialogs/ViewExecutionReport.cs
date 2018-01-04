
using Microsoft.Reporting.WinForms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Dialogs
{
    public partial class ViewExecutionReport : Form
    {
        #region Fields

        private Int32 m_algorithmId;
        private Int32 m_executionId;

        #endregion

        #region ViewExecutionReport(Int32 algorithmId, Int32 executionId)
        public ViewExecutionReport(Int32 algorithmId, Int32 executionId)
        {
            InitializeComponent();

            m_algorithmId = algorithmId;
            m_executionId = executionId;
        } 
        #endregion

        #region ViewExecutionReport_Load(object sender, EventArgs e)
        private void ViewExecutionReport_Load(object sender, EventArgs e)
        {
            ReportParameter reportParameterAlgorithmId = new ReportParameter("algorithmId", m_algorithmId.ToString());
            ReportParameter reportParameterExecutionId = new ReportParameter("executionId", m_executionId.ToString());

            //reportViewer.LocalReport.SetParameters(new ReportParameter[] { reportParameterAlgorithmId, reportParameterExecutionId });

            //SqlDataAdapter adapter = new SqlDataAdapter("SELECT b.name, SUM(timeElapsed) totalTimeElapsed, COUNT(*) qute FROM WorkItem a INNER JOIN Node b ON a.nodeId = b.nodeId WHERE a.algorithmId = 1 AND a.executionId = 1 GROUP BY b.name ORDER BY totalTimeElapsed DESC", new SqlConnection("Data Source=.;Initial Catalog=netHPC;User Id=netHPC;Pwd=netHPC"));
            //DataSet ds = new DataSet();
            //adapter.Fill(ds);

            //ReportDataSource xx = new ReportDataSource("netHPCDataSet_WorkItems", ds);            
            //reportViewer.LocalReport.DataSources.Add(
            //reportViewer.LocalReport.DataSources.Add(xx);
            //reportViewer.RefreshReport();
        } 
        #endregion
    }
}
