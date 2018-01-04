
using Microsoft.ManagementConsole;
using Node = netHPC.Shared.Node;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.ManagementConsole.Lists
{
    class NodeList : MmcListView
    {
        #region OnInitialize(AsyncStatus status)
        protected override void OnInitialize(AsyncStatus status)
        {
            base.OnInitialize(status);

            Columns[0].SetWidth(50);
            Columns.Add(new MmcListViewColumn("Description", 100));
            Columns.Add(new MmcListViewColumn("Created At", 100));
            Columns.Add(new MmcListViewColumn("Last Report", 100));
            Columns.Add(new MmcListViewColumn("# Cores", 50));
            Columns.Add(new MmcListViewColumn("Clock MHz", 100));
            Columns.Add(new MmcListViewColumn("Status", 100));

            LoadNodes();
        } 
        #endregion

        #region OnRefresh(AsyncStatus status)
        protected override void OnRefresh(AsyncStatus status)
        {
            base.OnRefresh(status);
            LoadNodes();
        } 
        #endregion

        #region LoadNodes()
        private void LoadNodes()
        {
            ResultNodes.Clear();
            foreach (Node node in SnapInTools.Entities.Node.OrderBy("it.name"))
            {
                ResultNode resultNode = new ResultNode();
                resultNode.DisplayName = node.Name;
                resultNode.SubItemDisplayNames.Add(node.Description == null ? String.Empty : node.Description);
                resultNode.SubItemDisplayNames.Add(node.DateCreated.ToString());
                resultNode.SubItemDisplayNames.Add(node.LastReport.ToString());
                resultNode.SubItemDisplayNames.Add(node.NumOfExecUnits.ToString());
                resultNode.SubItemDisplayNames.Add(node.SpeedMHz.ToString());
                resultNode.SubItemDisplayNames.Add(node.Status == 0 ? "Idle" : "Executing");
                ResultNodes.Add(resultNode);
            }
        } 
        #endregion
    }
}
