
using Microsoft.ManagementConsole;
using Action = Microsoft.ManagementConsole.Action;

using netHPC.ManagementConsole;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using netHPC.ManagementConsole.Lists;

namespace netHPC.ManagementConsole.Lists
{
    class ExecutionList : MmcListView
    {
        #region Fields

        private Algorithm m_algorithm; 

        #endregion

        #region OnInitialize(AsyncStatus status)
        protected override void OnInitialize(AsyncStatus status)
        {
            base.OnInitialize(status);
            
            m_algorithm = ScopeNode.Tag as Algorithm;

            Columns[0].SetWidth(400);
            Columns.Add(new MmcListViewColumn("Description", 400));

            LoadExecutions();
        } 
        #endregion

        #region OnRefresh(AsyncStatus status)
        protected override void OnRefresh(AsyncStatus status)
        {
            base.OnRefresh(status);
            LoadExecutions();
        } 
        #endregion

        #region LoadExecutions()
        private void LoadExecutions()
        {
            ResultNodes.Clear();

            m_algorithm.Execution.Load();
            foreach (Execution execution in m_algorithm.Execution)
            {
                ResultNode resultNode = new ResultNode();                
                resultNode.ImageIndex = 3;
                resultNode.DisplayName = execution.Name;
                resultNode.SubItemDisplayNames.Add(execution.Description);
                ResultNodes.Add(resultNode);
            }            
        } 
        #endregion
    }
}
