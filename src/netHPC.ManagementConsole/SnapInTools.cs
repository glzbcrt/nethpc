
using netHPC.ManagementConsole.Nodes;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole
{
    class SnapInTools
    {
        #region Fields

        private static String sm_serverName = "localhost";
        private static String sm_databaseName = "netHPC";
        private static String sm_userName = "netHPC";
        private static String sm_userPassword = "netHPC";
        private static String sm_headNode = "localhost";
        private static Int32 sm_headNodePort = 15000;

        private static Boolean sm_configuredSnapIn;

        private static MainNode sm_mainNode;

        private static Entities sm_netHPCEntities;

        #endregion

        #region ServerName
        public static String ServerName
        {
            get { return sm_serverName; }
            set { sm_serverName = value; }
        } 
        #endregion
        
        #region DatabaseName
        public static String DatabaseName
        {
            get { return sm_databaseName; }
            set { sm_databaseName = value; }
        } 
	    #endregion

        #region UserName
        public static String UserName
        {
            get { return sm_userName; }
            set { sm_userName = value; }
        } 
        #endregion

        #region UserPassword
        public static String UserPassword
        {
            get { return sm_userPassword; }
            set { sm_userPassword = value; }
        } 
        #endregion        

        #region HeadNode
        public static String HeadNode
        {
            get { return sm_headNode; }
            set { sm_headNode = value; }
        } 
        #endregion

        #region HeadNodePort
        public static Int32 HeadNodePort
        {
            get { return sm_headNodePort; }
            set { sm_headNodePort = value; }
        } 
        #endregion

        #region ConfiguredSnapIn
        public static Boolean ConfiguredSnapIn
        {
            get { return sm_configuredSnapIn; }
            set { sm_configuredSnapIn = value; }
        } 
        #endregion

        #region MainNode
        public static MainNode MainNode
        {
            get { return sm_mainNode; }
            set { sm_mainNode = value; }
        }
        #endregion

        #region Entities
        public static Entities Entities
        {
            get { return sm_netHPCEntities; }
        } 
        #endregion

        #region LoadCustomData(Byte[] persisted)
        public static void LoadCustomData(Byte[] persisted)
        {
            String[] arrayData = Encoding.Unicode.GetString(persisted).Split(new String[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            sm_serverName = arrayData[0];
            sm_databaseName = arrayData[1];
            sm_userName = arrayData[2];
            sm_userPassword = arrayData[3];
            sm_headNode = arrayData[4];
            sm_headNodePort = Int32.Parse(arrayData[5]);
            sm_configuredSnapIn = true;

            ConnectDatabase();
        } 
        #endregion

        #region ConnectDatabase()
        public static void ConnectDatabase()
        {
            if (sm_netHPCEntities != null)
                sm_netHPCEntities.Dispose();

            sm_netHPCEntities = Utils.GetEntities(sm_serverName, sm_databaseName, sm_userName, sm_userPassword);
        } 
        #endregion

        public static void ConnectHeadNode()
        {
        }

        #region SaveCustomData()
        public static Byte[] SaveCustomData()
        {
            return Encoding.Unicode.GetBytes(String.Format("{0}||{1}||{2}||{3}||{4}||{5}", sm_serverName, sm_databaseName, sm_userName, sm_userPassword, sm_headNode, sm_headNodePort.ToString()));
        } 
        #endregion

        #region ValidateDatabaseConnection(String serverName, String databaseName, String userName, String userPassword)
        public static Boolean ValidateDatabaseConnection(String serverName, String databaseName, String userName, String userPassword)
        {
            SqlConnection sqlConnection = null;
            Boolean returnValue = false;

            try
            {
                sqlConnection = new SqlConnection(String.Format("Data Source={0};Initial Catalog={1};User ID={2};Pwd={3}", serverName, databaseName, userName, userPassword));
                sqlConnection.Open();
                returnValue = true;
            }
            catch { }
            finally
            {
                if ((sqlConnection != null) && (sqlConnection.State != ConnectionState.Closed))
                    sqlConnection.Close();
            }

            return returnValue;
        } 
        #endregion

        public static Boolean ValidateHeadNodeConnection(String headNode, Int32 tcpPort)
        {
            return true;
        }

        #region StartExecution(String name, String description, Algorithm algorithm, Node[] nodes, String algorithmParameters)
        public static Int32 StartExecution(String name, String description, Algorithm algorithm, Node[] nodes, String algorithmParameters)
        {            
            Execution execution = new Execution();
            execution.Algorithm = algorithm;
            execution.AlgorithmId = algorithm.AlgorithmId;
            execution.Name = name;
            execution.Description = description;
            execution.Parameters = algorithmParameters;
            execution.Status = 0;
            execution.DateStart = DateTime.Now;

            foreach (Node node in nodes)
                execution.Node.Add(node);

            sm_netHPCEntities.AddToExecution(execution);
            sm_netHPCEntities.SaveChanges();

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(sm_headNode, sm_headNodePort);
            NetworkStream networkStream = tcpClient.GetStream();

            Byte[] buffer = new Byte[5000];
            Int32 read = networkStream.Read(buffer, 0, buffer.Length);

            buffer = Encoding.ASCII.GetBytes("LogOnAs ManagementConsole " + Environment.MachineName);
            networkStream.Write(buffer, 0, buffer.Length);

            buffer = new Byte[5000];
            read = networkStream.Read(buffer, 0, buffer.Length);

            buffer = Encoding.ASCII.GetBytes(String.Format("StartExecution {0} {1}", algorithm.AlgorithmId, execution.ExecutionId));
            networkStream.Write(buffer, 0, buffer.Length);            

            read = networkStream.Read(buffer, 0, buffer.Length);

            buffer = Encoding.ASCII.GetBytes("Quit ManagementConsole " + Environment.MachineName);
            networkStream.Write(buffer, 0, buffer.Length);

            networkStream.Flush();
            networkStream.Close();
            tcpClient.Close();

            return execution.ExecutionId;
        } 
        #endregion

        #region ShowError(String errorMessage)
        public static void ShowError(String errorMessage)
        {
            MessageBox.Show(errorMessage, "netHPC", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }   
        #endregion      
    }
}
