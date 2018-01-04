using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace netHPC.Service.ComputingNode
{
    internal class ComputingNodeServiceWrapper : ServiceBase
    {
        private ComputingNodeService m_computingNodeService;

        protected override void OnStart(String[] args)
        {
            base.OnStart(args);
            Thread mainThread = new Thread(new ThreadStart(HandleStart));
            mainThread.Start();
        }

        protected void HandleStart()
        {
            m_computingNodeService = new ComputingNodeService(null);
            m_computingNodeService.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (m_computingNodeService != null)
                m_computingNodeService.Stop();
        }
    }
}
