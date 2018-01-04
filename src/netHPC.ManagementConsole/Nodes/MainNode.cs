
using Microsoft.ManagementConsole;
using MMCAction = Microsoft.ManagementConsole.Action;

using netHPC.ManagementConsole.Dialogs;
using netHPC.ManagementConsole.Pages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Nodes
{
    class MainNode : ScopeNode
    {
        #region MainNode()
        public MainNode()
        {
            DisplayName = "netHPC";
            SnapInTools.MainNode = this;

            SelectedImageIndex = ImageIndex = 0;

            MMCAction actionAdd = new MMCAction("Connect to netHPC", "Connect to a netHPC instance.");
            actionAdd.Tag = "actionConnect";
            ActionsPaneItems.Add(actionAdd);

            FormViewDescription form = new FormViewDescription();
            form.DisplayName = "Initial Page";
            form.ControlType = typeof(MainPage);
            ViewDescriptions.Add(form);
            ViewDescriptions.DefaultIndex = 0;
        } 
        #endregion

        #region LoadChildrenNodes()
        public void LoadChildrenNodes()
        {
            Children.Clear();
            Children.Add(new AlgorithmsNode());
            Children.Add(new NodesNode());
        } 
        #endregion

        #region OnExpand(AsyncStatus status)
        protected override void OnExpand(AsyncStatus status)
        {
            base.OnExpand(status);
            LoadChildrenNodes();
        } 
        #endregion

        #region OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        protected override void OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        {
            base.OnAction(action, status);

            if ((action.Tag != null) && (action.Tag.ToString() == "actionConnect"))
            {
                if (SnapIn.Console.ShowDialog(new ConnectDialog()) == DialogResult.OK)
                    LoadChildrenNodes();
            }
        } 
        #endregion
    }
}
