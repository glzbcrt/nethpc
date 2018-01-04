using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace netHPC.Service.HeadNode
{
    class HeadNodeServiceWrapper : ServiceBase
    {
        private HeadNodeService m_headNodeService;

        protected override void OnStart(String[] args)
        {
            base.OnStart(args);
            m_headNodeService = new HeadNodeService(args);
            m_headNodeService.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (m_headNodeService != null)
                m_headNodeService.Stop();
        }
    }
}
