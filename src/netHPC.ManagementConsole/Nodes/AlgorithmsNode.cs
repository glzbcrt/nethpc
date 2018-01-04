
using Microsoft.ManagementConsole;
using MMCAction = Microsoft.ManagementConsole.Action;

using netHPC.ManagementConsole.Lists;
using netHPC.ManagementConsole.Nodes;
using netHPC.ManagementConsole.Wizards;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Nodes
{
    class AlgorithmsNode : ScopeNode
    {
        #region AlgorithmsNode()
        public AlgorithmsNode()
        {
            DisplayName = "Algorithms";
            SelectedImageIndex = ImageIndex = 1;

            MMCAction actionAdd = new MMCAction("&Add Algorithm", "Adds a new algorithm to netHPC");
            actionAdd.Tag = "actionAdd";
            ActionsPaneItems.Add(actionAdd);

            MmcListViewDescription mmcListViewDescription = new MmcListViewDescription();
            mmcListViewDescription.DisplayName = "Algorithms";
            mmcListViewDescription.Options = MmcListViewOptions.SingleSelect;
            mmcListViewDescription.ViewType = typeof(AlgorithmList);
            ViewDescriptions.Add(mmcListViewDescription);
            ViewDescriptions.DefaultIndex = 0;

            EnabledStandardVerbs = StandardVerbs.Refresh;
        } 
        #endregion

        #region OnExpand(AsyncStatus status)
        protected override void OnExpand(AsyncStatus status)
        {
            base.OnExpand(status);
            LoadAlgorithms();
        } 
        #endregion

        #region OnRefresh(AsyncStatus status)
        protected override void OnRefresh(AsyncStatus status)
        {
            base.OnRefresh(status);
            LoadAlgorithms();
        } 
        #endregion

        #region LoadAlgorithms()
        private void LoadAlgorithms()
        {
            Children.Clear();
            foreach (Algorithm algorithm in SnapInTools.Entities.Algorithm.OrderBy("it.name"))
                Children.Add(GenerateNodeFromAlgorithm(algorithm));
        } 
        #endregion

        #region OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        protected override void OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        {
            base.OnAction(action, status);

            if ((action.Tag != null) && (action.Tag.ToString() == "actionAdd"))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select the assembly containing the algoritm to be distributed";
                openFileDialog.CheckPathExists = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.DefaultExt = "*.dll";
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "netHPC Algorithms|*.dll";

                if (SnapIn.Console.ShowDialog(openFileDialog) == DialogResult.OK)
                {
                    AddAlgorithmWizard addAlgorithmWizard = new AddAlgorithmWizard(openFileDialog.FileName);
                    if (SnapIn.Console.ShowDialog(addAlgorithmWizard) == DialogResult.OK)
                        Children.Add(GenerateNodeFromAlgorithm(addAlgorithmWizard.Algorithm));
                }
            }
        } 
        #endregion

        #region GenerateNodeFromAlgorithm(Algorithm algorithm)
        private AlgorithmNode GenerateNodeFromAlgorithm(Algorithm algorithm)
        {
            AlgorithmNode algorithmNode = new AlgorithmNode();
            algorithmNode.DisplayName = algorithm.Name;
            algorithmNode.SubItemDisplayNames.Add(algorithm.Description);
            algorithmNode.SubItemDisplayNames.Add(algorithm.DateCreated.ToString());
            algorithmNode.SubItemDisplayNames.Add(algorithm.DateModified.ToString());
            algorithmNode.Tag = algorithm;

            return algorithmNode;
        } 
        #endregion
    }
}
