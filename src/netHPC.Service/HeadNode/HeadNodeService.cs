
using netHPC.SDK;
using netHPC.Shared;
using netHPC.Samples.PrimeNumbers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace netHPC.Service.HeadNode
{
    internal delegate void CommandCallback(ComputingNodeConnection computingNodeConnection, String[] parameters);

    internal class HeadNodeService
    {
        #region Fields

        private TcpListener m_tcpListener;

        private AsyncCallback m_handleAcceptDelegate;
        private AsyncCallback m_handleWriteDelegate;
        private AsyncCallback m_handleReadDelegate;

        private String m_banner;

        private Dictionary<String, CommandCallback> m_commandCallbacks = new Dictionary<String, CommandCallback>();

        private IWorkBreaker<Range> m_workBreaker;

        private BinaryFormatter m_binaryFormatter = new BinaryFormatter();

        private Dictionary<String, ComputingNodeConnection> m_connectedComputingNodes = new Dictionary<String, ComputingNodeConnection>();

        #endregion

        #region HeadNodeService(String[] args)
        public HeadNodeService(String[] args)
        {
        } 
        #endregion

        #region Start()
        internal void Start()
        {
            // Format the banner that we will show to the client.
            String tmp = Assembly.GetExecutingAssembly().FullName;
            tmp = tmp.Substring(tmp.IndexOf("Version=") + 8);
            tmp = tmp.Substring(0, tmp.IndexOf(","));
            m_banner = String.Format("netHPC Head Node v" + tmp);

            m_commandCallbacks.Add("STARTEXECUTION", new CommandCallback(HandleStartExecutionCommand));
            m_commandCallbacks.Add("GETWORKITEM", new CommandCallback(HandleGetWorkItemCommand));
            m_commandCallbacks.Add("ABORT", new CommandCallback(HandleAbortCommand));
            m_commandCallbacks.Add("LOGONAS", new CommandCallback(HandleLogOnAsCommand));
            m_commandCallbacks.Add("QUIT", new CommandCallback(HandleQuitCommand));
            
            // Create the delegates now.
            m_handleAcceptDelegate = new AsyncCallback(HandleAccept);
            m_handleWriteDelegate = new AsyncCallback(HandleWrite);
            m_handleReadDelegate = new AsyncCallback(HandleRead);

            // Initialize the TcpListener.
            m_tcpListener = new TcpListener(IPAddress.Any, 15000);
            m_tcpListener.Start();

            // Accept the connections.
            m_tcpListener.BeginAcceptTcpClient(m_handleAcceptDelegate, null);
        } 
        #endregion

        #region HandleAccept(IAsyncResult ar)
        public void HandleAccept(IAsyncResult ar)
        {
            // We got the connection, dispatch a new accept to process new connections.
            m_tcpListener.BeginAcceptTcpClient(m_handleAcceptDelegate, null);

            // Create an ComputingNodeConnection object to represent this connection.
            ComputingNodeConnection computingNodeConnection = new ComputingNodeConnection(m_tcpListener.EndAcceptTcpClient(ar));

            // Print a banner showing the netHPC version.
            WriteStream(computingNodeConnection, m_banner + " at " + DateTime.Now.ToUniversalTime().ToString() + " UTC");
        } 
        #endregion

        #region HandleWrite(IAsyncResult ar)
        public void HandleWrite(IAsyncResult ar)
        {
            ComputingNodeConnection computingNodeConnection = ar.AsyncState as ComputingNodeConnection;
            computingNodeConnection.NetworkStream.EndWrite(ar);

            computingNodeConnection.NetworkStream.BeginRead(computingNodeConnection.ReadBuffer, 0, computingNodeConnection.ReadBuffer.Length, m_handleReadDelegate, computingNodeConnection);
        } 
        #endregion

        #region HandleRead(IAsyncResult ar)
        public void HandleRead(IAsyncResult ar)
        {
            ComputingNodeConnection computingNodeConnection = ar.AsyncState as ComputingNodeConnection;

            String command;
            String[] commandParameters;            

            if (ReadStream(computingNodeConnection, out command, out commandParameters, ar))
            {
                if (m_commandCallbacks.ContainsKey(command))
                    m_commandCallbacks[command](computingNodeConnection, commandParameters);
                else
                    WriteStream(computingNodeConnection, "Unknow command");
            }
            //else if (computingNodeConnection != null)
                //computingNodeConnection.NetworkStream.BeginRead(computingNodeConnection.ReadBuffer, 0, computingNodeConnection.ReadBuffer.Length, m_handleReadDelegate, computingNodeConnection);
        } 
        #endregion

        #region ReadStream(ComputingNodeConnection computingNodeConnection, IAsyncResult ar)
        private Boolean ReadStream(ComputingNodeConnection computingNodeConnection, out String command, out String[] commandParameters, IAsyncResult ar)
        {
            Int32 read = computingNodeConnection.NetworkStream.EndRead(ar);

            if (read == 0)
            {
                computingNodeConnection.NetworkStream.Flush();
                computingNodeConnection.NetworkStream.Close();
                computingNodeConnection.TcpClient.Close();
                computingNodeConnection = null;

                commandParameters = null;
                command = null;
                return false;
            }
            else
            {
                commandParameters = Encoding.ASCII.GetString(computingNodeConnection.ReadBuffer, 0, read).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                command = commandParameters[0].ToUpper();
                return true;
            }
        } 
        #endregion

        #region WriteStream(ComputingNodeConnection computingNodeConnection, String message)
        private void WriteStream(ComputingNodeConnection computingNodeConnection, String message)
        {
            Byte[] arrTmp = Encoding.ASCII.GetBytes(message + '\n' + '\r');
            computingNodeConnection.NetworkStream.BeginWrite(arrTmp, 0, arrTmp.Length, m_handleWriteDelegate, computingNodeConnection);
        } 
        #endregion

        #region HandleQuitCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        private void HandleQuitCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        {            
            lock (m_connectedComputingNodes)
            {
                if ((String.Compare(parameters[1], "ComputingNode", true) == 0) && (m_connectedComputingNodes.ContainsKey(parameters[2])))
                    m_connectedComputingNodes.Remove(parameters[2]);
            }

            computingNodeConnection.NetworkStream.Close();
            computingNodeConnection.TcpClient.Close();
        } 
        #endregion

        #region private void HandleLogOnAsCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        private void HandleLogOnAsCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        {
            if (String.Compare(parameters[1], "ComputingNode", true) == 0)
            {
                lock (m_connectedComputingNodes)
                    m_connectedComputingNodes.Add(parameters[2], computingNodeConnection);
            }

            WriteStream(computingNodeConnection, "OK");
        }
        #endregion

        #region HandleStartExecutionCommand(ComputingNodeConnection computingNodeConnection, String parameters)
        private void HandleStartExecutionCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        {
            Execution execution = ServiceTools.Entities.Execution.Where("it.algorithmId = " + parameters[1] + " AND it.executionId = " + parameters[2]).First();

            UInt32 nodeCounter = 0;
            UInt32 executionUnitCounter = 0;

            execution.Node.Load();
            foreach (Node node in execution.Node)
            {
                nodeCounter++;
                executionUnitCounter += (UInt32)node.NumOfExecUnits;
            }

            m_workBreaker = new RangeBreaker();
            m_workBreaker.Load(execution.Parameters, nodeCounter, executionUnitCounter);

            lock (m_connectedComputingNodes)
            {
                foreach(String key in m_connectedComputingNodes.Keys)
                    WriteStream(m_connectedComputingNodes[key], String.Format("StartExecution {0} {1}", parameters[1], parameters[2]));
            }

            WriteStream(computingNodeConnection, "ExecutionStarted");            
        }
        #endregion

        #region HandleGetWorkItemCommand(ComputingNodeConnection computingNodeConnection, String parameters)
        private void HandleGetWorkItemCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        {
            lock (m_workBreaker)
            {
                Range rangeTmp;

                if (m_workBreaker.GetWorkItem(out rangeTmp))
                {
                    MemoryStream memoryStream = new MemoryStream(1024);
                    m_binaryFormatter.Serialize(memoryStream, rangeTmp);

                    Int32 length = (Int32)memoryStream.Position;

                    computingNodeConnection.NetworkStream.BeginWrite(memoryStream.GetBuffer(), 0, length, m_handleWriteDelegate, computingNodeConnection);
                }
                else
                    WriteStream(computingNodeConnection, "NoMoreWorkUnits");
            }
        } 
        #endregion

        #region HandleAbortCommand(ComputingNodeConnection computingNodeConnection, String parameters)
        private void HandleAbortCommand(ComputingNodeConnection computingNodeConnection, String[] parameters)
        {

        } 
        #endregion

        #region Stop()
        internal void Stop()
        {
            m_tcpListener.Stop();
        } 
        #endregion
    }
}
