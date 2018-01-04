
using netHPC.Service.ComputingNode;
using netHPC.Service.HeadNode;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace netHPC.Service
{
    internal class Program
    {
        #region Fields

        private static ManualResetEvent sm_manualResetEvent;
        private static HeadNodeService sm_headNodeService;
        private static ComputingNodeService sm_computingNodeService; 

        #endregion

        #region Main(string[] args)
        internal static void Main(string[] args)
        {
            ServiceTools.UpdateNodeInformation();

            if (Environment.UserInteractive)
            {
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                sm_manualResetEvent = new ManualResetEvent(false);

                Trace.Listeners.Add(new ConsoleTraceListener());
                Trace.WriteLine("netHPC - Console mode");

                if ((args.Length > 0) && (String.Compare(args[0], "-head", true) == 0))
                {
                    sm_headNodeService = new HeadNodeService(args);
                    sm_headNodeService.Start();
                }
                else
                {
                    sm_computingNodeService = new ComputingNodeService(args);
                    sm_computingNodeService.Start();
                }

                sm_manualResetEvent.WaitOne();
            }
            else
            {
                if ((args.Length > 0) && (String.Compare(args[0], "-head", true) == 0))
                    ServiceBase.Run(new HeadNodeServiceWrapper());
                else
                    ServiceBase.Run(new ComputingNodeServiceWrapper());
            }
        } 
        #endregion

        #region Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        internal static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Trace.WriteLine("\n=======================================================");
            Trace.WriteLine("CTRL+C pressed, please wait until netHPC is unloaded!!!");
            Trace.WriteLine("=======================================================");

            if (sm_headNodeService != null)
                sm_headNodeService.Stop();
            else
                sm_computingNodeService.Stop();

            sm_manualResetEvent.Set();
        } 
        #endregion
    }
}
