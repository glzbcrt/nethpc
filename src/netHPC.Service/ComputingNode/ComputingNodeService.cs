
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace netHPC.Service.ComputingNode
{
    internal class ComputingNodeService
    {
        public ComputingNodeService(String[] args)
        {
        }

        internal void Start()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(ServiceTools.HeadNodeServerName, ServiceTools.HeadNodeTcpPort);
            NetworkStream networkStream = tcpClient.GetStream();

            Thread[] workerThreads = new Thread[ServiceTools.ThisNode.NumOfExecUnits];
            //Thread[] workerThreads = new Thread[1];

            // Head -> Computing: Banner
            Byte[] arr = new Byte[5000];
            Int32 read = networkStream.Read(arr, 0, arr.Length);
            Console.WriteLine(Encoding.ASCII.GetString(arr, 0, read));

            // Head <- Computing: LogOnAs
            arr = Encoding.ASCII.GetBytes("LogOnAs ComputingNode " + Environment.MachineName);
            networkStream.Write(arr, 0, arr.Length);
            read = networkStream.Read(arr, 0, arr.Length);

            while (true)
            {
                // Head -> Computing: Command
                arr = new Byte[5000];
                read = networkStream.Read(arr, 0, arr.Length);
                String[] arrTmp = Encoding.ASCII.GetString(arr, 0, read).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (arrTmp[0].ToUpper())
                {
                    case "STARTEXECUTION":
                        for (int i = 0; i < workerThreads.Length; i++)
                        {
                            workerThreads[i] = new Thread(new ParameterizedThreadStart(StartWorker));
                            workerThreads[i].Start(new WorkerParameters(Int32.Parse(arrTmp[1]), Int32.Parse(arrTmp[2]), i + 1));
                        }
                        break;

                    case "ABORT":
                        for (int i = 0; i < workerThreads.Length; i++)
                            workerThreads[i].Abort();
                        break;
                }
            }
        }

        internal void Stop()
        {
        }

        internal void StartWorker(Object workerParametersObject)
        {
            WorkerParameters workerParameters = workerParametersObject as WorkerParameters;
            Object algorithmObject = null;
            Type algorithmType = null;

            try
            {
                Entities ent = Utils.GetEntities("george", "netHPC", "netHPC", "netHPC");
                //Algorithm algorithm = ServiceTools.Entities.Algorithm.Where("it.algorithmId = " + workerParameters.AlgorithmId.ToString()).First();
                Algorithm algorithm = ent.Algorithm.Where("it.algorithmId = " + workerParameters.AlgorithmId.ToString()).First();

                netHPC.Samples.PrimeNumbers.Algorithm v = new netHPC.Samples.PrimeNumbers.Algorithm();
                v.Start(workerParameters.AlgorithmId, workerParameters.ExecutionId, workerParameters.ExecUnitId, ServiceTools.ThisNode.NodeId, ServiceTools.HeadNodeServerName, ServiceTools.HeadNodeTcpPort);

                //algorithmType = Assembly.Load(algorithm.Assembly).GetType("netHPC.Samples.PrimeNumbers.Algorithm");
                //algorithmObject = Activator.CreateInstance(algorithmType);

                //algorithmType.InvokeMember("Start", BindingFlags.InvokeMethod, null, algorithmObject, new object[] { workerParameters.AlgorithmId, workerParameters.ExecutionId, workerParameters.ExecUnitId, ServiceTools.ThisNode.NodeId, ServiceTools.HeadNodeServerName, ServiceTools.HeadNodeTcpPort });
            }
            catch (ThreadAbortException)
            {
                if (algorithmObject != null)
                    algorithmType.InvokeMember("Abort", BindingFlags.InvokeMethod, null, algorithmObject, null);
            }
        }
    }
}
