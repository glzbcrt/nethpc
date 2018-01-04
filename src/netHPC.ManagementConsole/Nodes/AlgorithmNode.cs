
using Microsoft.ManagementConsole;
using MMCAction = Microsoft.ManagementConsole.Action;

using netHPC.ManagementConsole.Lists;
using netHPC.ManagementConsole.Pages;
using netHPC.ManagementConsole.Wizards;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Nodes
{
    class AlgorithmNode : ScopeNode
    {
        #region AlgorithmNode()
        public AlgorithmNode()
        {
            SelectedImageIndex = ImageIndex = 2;

            MMCAction actionAdd = new MMCAction("&Start", "Starts a new distributed execution");
            actionAdd.Tag = "actionStart";
            ActionsPaneItems.Add(actionAdd);


            MMCAction actionDelete = new MMCAction("&Delete", "Deletes the algorithm");
            actionDelete.Tag = "actionDelete";
            ActionsPaneItems.Add(actionDelete);

            MMCAction actionReports = new MMCAction("&View Reports", "View the executions report");
            actionReports.Tag = "actionReports";
            ActionsPaneItems.Add(actionReports);

            EnabledStandardVerbs = StandardVerbs.Properties;

            MmcListViewDescription executionList = new MmcListViewDescription();
            executionList.Options = MmcListViewOptions.SingleSelect;
            executionList.DisplayName = "Executions";
            executionList.ViewType = typeof(ExecutionList);
            ViewDescriptions.Add(executionList);
            ViewDescriptions.DefaultIndex = 0;
        } 
        #endregion

        #region OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        protected override void OnAction(Microsoft.ManagementConsole.Action action, AsyncStatus status)
        {
            base.OnAction(action, status);

            if ((action.Tag != null) && (action.Tag.ToString() == "actionStart"))
                SnapIn.Console.ShowDialog(new StartAlgorithmWizard(Tag as Algorithm));

            if ((action.Tag != null) && (action.Tag.ToString() == "actionReports"))
                System.Diagnostics.Process.Start("http://localhost/Reports/Pages/Report.aspx?ItemPath=%2fnetHPC%2fReport&AlgorithmId=" + (Tag as Algorithm).AlgorithmId.ToString());

            if ((action.Tag != null) && (action.Tag.ToString() == "actionDelete"))
            {

            }
        } 
        #endregion

        #region OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        {
            base.OnAddPropertyPages(propertyPageCollection);
            Algorithm algorithm = Tag as Algorithm;

            AlgorithmPropertyPage algorithmPropertyPage = new AlgorithmPropertyPage();
            algorithmPropertyPage.Title = "General";
            algorithmPropertyPage.Control = new AlgorithmProperties();
            ((AlgorithmProperties)algorithmPropertyPage.Control).Algorithm = algorithm;
            ((AlgorithmProperties)algorithmPropertyPage.Control).AlgorithmNode = this;
            propertyPageCollection.Add(algorithmPropertyPage);

            foreach (Type type in Assembly.Load(algorithm.Assembly).GetTypes())
            {
                if ((type.IsClass) && (type.Name.EndsWith("About")))
                {
                    AlgorithmAbout algorithmAbout = new AlgorithmAbout();
                    algorithmAbout.Title = "About";
                    algorithmAbout.Control = Activator.CreateInstance(type) as UserControl;
                    propertyPageCollection.Add(algorithmAbout);
                    break;
                }
            }
        } 
        #endregion
    }
}
