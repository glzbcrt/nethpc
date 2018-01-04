
using Microsoft.ManagementConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using netHPC.ManagementConsole.Lists;

namespace netHPC.ManagementConsole.Lists
{
    class AlgorithmList : MmcListView
    {
        protected override void OnInitialize(AsyncStatus status)
        {
            base.OnInitialize(status);

            Columns[0].SetWidth(300);            
            Columns.Add(new MmcListViewColumn("Description", 400));
            Columns.Add(new MmcListViewColumn("Created At", 150));
            Columns.Add(new MmcListViewColumn("Modified At", 150));
        }
    }
}
