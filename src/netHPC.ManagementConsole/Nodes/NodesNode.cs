
using Microsoft.ManagementConsole;

using netHPC.ManagementConsole.Lists;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole
{
    class NodesNode : ScopeNode
    {
        #region NodesNode()
        public NodesNode()
        {
            DisplayName = "Nodes";
            SelectedImageIndex = ImageIndex = 4;

            MmcListViewDescription mmcListViewDescription = new MmcListViewDescription();
            mmcListViewDescription.DisplayName = "Nodes";
            mmcListViewDescription.Options = MmcListViewOptions.SingleSelect;
            mmcListViewDescription.ViewType = typeof(NodeList);
            ViewDescriptions.Add(mmcListViewDescription);
            ViewDescriptions.DefaultIndex = 0;
        } 
        #endregion
    }
}
