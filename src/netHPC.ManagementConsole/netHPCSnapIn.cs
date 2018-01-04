
using Microsoft.ManagementConsole;

using netHPC.ManagementConsole.Dialogs;
using netHPC.ManagementConsole.Nodes;
using netHPC.ManagementConsole.Properties;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole
{
    [SnapInSettings("{AB4E00FE-16FC-402b-A72E-150B1BFD1BB4}", Vendor="www.nethpc.com", DisplayName = "netHPC", Description = "netHPC is a tool that allows algorithms to be distributed among computers to improve processing time.")]
    [SnapInAbout("netHPC.ManagementConsole.dll", ApplicationBaseRelative = true, DisplayNameId = 0x65, DescriptionId = 0x66, VendorId = 0x67, VersionId = 0x68, IconId = 105, LargeFolderBitmapId = 0x70, SmallFolderBitmapId = 0x6f, SmallFolderSelectedBitmapId = 0x6f, FolderBitmapsColorMask = 0xff00)]
    public class netHPCSnapIn : SnapIn
    {
        #region OnInitialize()
        protected override void OnInitialize()
        {
            base.OnInitialize();

            SmallImages.Add(Resources.netHPC);
            SmallImages.Add(Resources.Algorithms);
            SmallImages.Add(Resources.Algorithm);
            SmallImages.Add(Resources.Execution);
            SmallImages.Add(Resources.Nodes);
            SmallImages.Add(Resources.Node);

            RootNode = new MainNode();

            IsModified = true;
        } 
        #endregion

        #region OnLoadCustomData(AsyncStatus status, byte[] persistenceData)
        protected override void OnLoadCustomData(AsyncStatus status, byte[] persistenceData)
        {
            SnapInTools.LoadCustomData(persistenceData);
        } 
        #endregion

        #region OnSaveCustomData(SyncStatus status)
        protected override byte[] OnSaveCustomData(SyncStatus status)
        {
            return SnapInTools.SaveCustomData();
        } 
        #endregion

        #region OnShowInitializationWizard()
        protected override bool OnShowInitializationWizard()
        {
            return new ConnectDialog().ShowDialog() == DialogResult.OK;
        } 
        #endregion
    }
}
