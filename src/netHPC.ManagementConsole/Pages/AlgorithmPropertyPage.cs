
using Microsoft.ManagementConsole;

using netHPC.ManagementConsole;
using netHPC.ManagementConsole.Nodes;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Pages
{
    class AlgorithmPropertyPage : PropertyPage
    {
        #region OnOK()
        protected override bool OnOK()
        {
            base.OnOK();
            return SaveChanges();
        } 
        #endregion

        #region OnApply()
        protected override bool OnApply()
        {
            base.OnApply();
            return SaveChanges();
        } 
        #endregion

        #region SaveChanges()
        private Boolean SaveChanges()
        {
            AlgorithmProperties algorithmProperties = Control as AlgorithmProperties;
            AlgorithmNode algorithmNode = algorithmProperties.AlgorithmNode;
            Algorithm algorithm = algorithmProperties.Algorithm;

            if (algorithmProperties.AlgorithmName.Trim() == String.Empty)
            {
                MessageBox.Show("name!");
                return false;
            }

            algorithmNode.DisplayName = algorithm.Name = algorithmProperties.AlgorithmName;
            algorithmNode.SubItemDisplayNames[0] = algorithm.Description = algorithmProperties.AlgorithmDescription;
            algorithm.DateModified = DateTime.Now;
            algorithmNode.SubItemDisplayNames[2] = algorithmProperties.Algorithm.DateModified.ToString();

            if (algorithmProperties.Assembly != null)
                algorithm.Assembly = algorithmProperties.Assembly;

            SnapInTools.Entities.SaveChanges();

            return true;
        } 
        #endregion
    }
}
