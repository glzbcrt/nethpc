using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace netHPC.SDK
{
    /// <summary>
    /// The algorithm base class. Every algorithm must inherint this class.
    /// </summary>
    /// <typeparam name="WorkItemType"></typeparam>
    public class AlgorithmBase<WorkItemType>
    {
        #region Fields

        private BinaryFormatter m_binaryFormatter = new BinaryFormatter();
        private TcpClient m_tcpClient = new TcpClient();
        private NetworkStream m_networkStream;
        private String m_headNodeServer;
        private Int32 m_headNodeTcpPort;

        private Int32 m_algorithmId;        
        private Int32 m_executionId;
        private Int32 m_execUnitId;
        private Int32 m_nodeId;

        private Int32 m_workItemId = 1;
        private Int32 m_eventId = 1;
        
        private Dictionary<String, DateTime> m_times = new Dictionary<String, DateTime>(2000);

        private SqlCommand m_sqlCommandWorkItem;
        private SqlCommand m_sqlCommandEvent;
        private SqlConnection m_sqlConnection;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        #region Initialize()
        private void Initialize()
        {
            MarkEventStart("Initialize::CreateSqlCommandEvent", true);
            m_sqlCommandEvent = new SqlCommand("INSERT [Event] ([algorithmId], [executionId], [execUnitId], [eventId], [nodeId], [eventName], [dateCreated], [timeElapsed], [internal], [text]) VALUES (@algorithmId, @executionId, @execUnitId, @eventId, @nodeId, @eventName, @dateCreated, @timeElapsed, @internal, @text)");
            m_sqlCommandEvent.Parameters.Add("@algorithmId", SqlDbType.Int).Value = m_algorithmId;
            m_sqlCommandEvent.Parameters.Add("@executionId", SqlDbType.Int).Value = m_executionId;
            m_sqlCommandEvent.Parameters.Add("@nodeId", SqlDbType.Int).Value = m_nodeId;
            m_sqlCommandEvent.Parameters.Add("@execUnitId", SqlDbType.SmallInt).Value = m_execUnitId;
            m_sqlCommandEvent.Parameters.Add("@eventId", SqlDbType.Int);            
            m_sqlCommandEvent.Parameters.Add("@eventName", SqlDbType.NVarChar, 100);
            m_sqlCommandEvent.Parameters.Add("@dateCreated", SqlDbType.DateTime);
            m_sqlCommandEvent.Parameters.Add("@timeElapsed", SqlDbType.BigInt);
            m_sqlCommandEvent.Parameters.Add("@internal", SqlDbType.Bit);
            m_sqlCommandEvent.Parameters.Add("@text", SqlDbType.Text);

            MarkEventStart("Initialize::OpenDatabaseConnection", true);
            m_sqlConnection = new SqlConnection("Data Source=george;Initial Catalog=netHPC;User ID=netHPC;Pwd=netHPC");
            m_sqlConnection.Open();
            m_sqlCommandEvent.Connection = m_sqlConnection;
            MarkEventEnd("Initialize::OpenDatabaseConnection", null, true);

            MarkEventEnd("Initialize::CreateSqlCommandEvent", null, true);

            MarkEventStart("Initialize::CreateSqlCommandWorkItem", true);
            m_sqlCommandWorkItem = new SqlCommand("INSERT [WorkItem] ([algorithmId], [executionId], [execUnitId], [workItemId], [nodeId], [dateCreated], [timeElapsed], [text]) VALUES (@algorithmId, @executionId, @execUnitId, @workItemId, @nodeId, @dateCreated, @timeElapsed, @text)", m_sqlConnection);
            m_sqlCommandWorkItem.Parameters.Add("@algorithmId", SqlDbType.Int).Value = m_algorithmId;
            m_sqlCommandWorkItem.Parameters.Add("@executionId", SqlDbType.Int).Value = m_executionId;
            m_sqlCommandWorkItem.Parameters.Add("@nodeId", SqlDbType.Int).Value = m_nodeId;
            m_sqlCommandWorkItem.Parameters.Add("@execUnitId", SqlDbType.Int).Value = m_execUnitId;
            m_sqlCommandWorkItem.Parameters.Add("@workItemId", SqlDbType.Int);            
            m_sqlCommandWorkItem.Parameters.Add("@dateCreated", SqlDbType.DateTime);
            m_sqlCommandWorkItem.Parameters.Add("@timeElapsed", SqlDbType.BigInt);
            m_sqlCommandWorkItem.Parameters.Add("@text", SqlDbType.Text);
            MarkEventEnd("Initialize::CreateSqlCommandWorkItem", null, true);

            MarkEventStart("Initialize::ConnectHeadNode", true);
            m_tcpClient.Connect(m_headNodeServer, m_headNodeTcpPort);
            m_networkStream = m_tcpClient.GetStream();

            Byte[] arr = new Byte[500];
            m_networkStream.Read(arr, 0, arr.Length);

            arr = Encoding.ASCII.GetBytes("LogOnAs WorkerThread " + Environment.MachineName);
            m_networkStream.Write(arr, 0, arr.Length);

            arr = new Byte[500];
            m_networkStream.Read(arr, 0, arr.Length);

            MarkEventEnd("Initialize::ConnectHeadNode", null, true);
        } 
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        #region Boolean GetWorkItem(out WorkItemType workItem)
        protected Boolean GetWorkItem(out WorkItemType workItem)
        {
            MarkEventStart("GetWorkItem::SendRequest", true);
            Byte[] buffer = Encoding.ASCII.GetBytes("GetWorkItem");            
            m_networkStream.Write(buffer, 0, buffer.Length);
            MarkEventEnd("GetWorkItem::SendRequest", null, true);

            MarkEventStart("GetWorkItem::WaitAndReadResponse", true);
            buffer = new Byte[50000];
            Int32 read = m_networkStream.Read(buffer, 0, buffer.Length);
            MarkEventEnd("GetWorkItem::WaitAndReadResponse", null, true);

            MarkEventStart("GetWorkItem::CheckingResponse", true);
            MemoryStream memoryStream = new MemoryStream(read);
            if (String.Compare(Encoding.ASCII.GetString(buffer, 0, 15), "NoMoreWorkUnits", true) == 0)
            {
                workItem = default(WorkItemType);
                MarkEventEnd("GetWorkItem::CheckingResponse", null, true);
                return false;
            }
            else
            {
                MarkEventStart("GetWorkItem::DeserializingWorkItem", true);
                memoryStream.Write(buffer, 0, read);
                memoryStream.Position = 0;
                workItem = (WorkItemType)m_binaryFormatter.Deserialize(memoryStream);
                m_times.Add("WI_" + m_workItemId, DateTime.Now);
                MarkEventEnd("GetWorkItem::DeserializingWorkItem", null, true);

                MarkEventEnd("GetWorkItem::CheckingResponse", null, true);

                return true;
            }
        } 
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        #region ReportWorkItemResult(String text)
        protected void ReportWorkItemResult(String text)
        {
            MarkEventStart("ReportWorkItemResult::InsertingDataAtDatabase", true);

            // Calculate how much time has passed since this work unit was started. We have a millisecond precision here!
            Double timeElapsed = (DateTime.Now - m_times["WI_" + m_workItemId]).TotalMilliseconds;

            // Insert this event into database.
            m_sqlCommandWorkItem.Parameters["@workItemId"].Value = m_workItemId;
            m_sqlCommandWorkItem.Parameters["@dateCreated"].Value = DateTime.Now;
            m_sqlCommandWorkItem.Parameters["@timeElapsed"].Value = timeElapsed;
            m_sqlCommandWorkItem.Parameters["@text"].Value = text == null ? (Object)DBNull.Value : (Object)text;
            m_sqlCommandWorkItem.ExecuteNonQuery();

            // Now that this work unit has been processed we need to remove it from the dictionary to avoid collisions.
            m_times.Remove("WI_" + m_workItemId);

            // We need to increment this counter here to create the next record with the right primary key.
            m_workItemId++;

            MarkEventEnd("ReportWorkItemResult::InsertingDataAtDatabase", null, true);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="algorithmId"></param>
        /// <param name="executionId"></param>
        /// <param name="execUnitId"></param>
        /// <param name="nodeId"></param>
        /// <param name="headNodeServer"></param>
        /// <param name="headNodeTcpPort"></param>
        #region Start(Int32 algorithmId, Int32 executionId, Int32 execUnitId, Int32 nodeId, String headNodeServer, Int32 headNodeTcpPort)
        public void Start(Int32 algorithmId, Int32 executionId, Int32 execUnitId, Int32 nodeId, String headNodeServer, Int32 headNodeTcpPort)
        {
            m_algorithmId = algorithmId;
            m_executionId = executionId;
            m_execUnitId = execUnitId;
            m_nodeId = nodeId;
            m_headNodeServer = headNodeServer;
            m_headNodeTcpPort = headNodeTcpPort;

            Initialize();

            MarkEventStart("Start::Load", true);
            Load();
            MarkEventEnd("Start::Load", null, true);

            MarkEventStart("Start::Process", true);
            Process();
            MarkEventEnd("Start::Process", null, true);

            MarkEventStart("Start::Unload", true);
            Unload();
            MarkEventEnd("Start::Unload", null, true);

            // Close the database connection.
            m_sqlConnection.Close();

            if (m_times.Count != 0)
                throw new Exception("There are one or more events on the queue. You need to mark their end.");

            Byte[] arr = Encoding.ASCII.GetBytes("Quit WorkerThread " + Environment.MachineName);

            m_networkStream.Flush();
            m_networkStream.Close();
            m_tcpClient.Close();
        } 
        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected void MarkEventStart(String name)
        {
            MarkEventStart(name, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="internalEvent"></param>
        private void MarkEventStart(String name, Boolean internalEvent)
        {
            if ((name == null) || (name.Trim() == String.Empty))
                throw new Exception("The event name is null or empty.");

            String eventName = internalEvent ? "Internal_" + name : name;

            if (m_times.ContainsKey(eventName))
                throw new Exception(String.Format("There is an already open {0} event named {1}.", internalEvent ? "internal" : String.Empty, name));

            m_times.Add(eventName, DateTime.Now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        protected void MarkEventEnd(String name, String text)
        {
            MarkEventEnd(name, text, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="internalEvent"></param>
        private void MarkEventEnd(String name, String text, Boolean internalEvent)
        {
            if ((name == null) || (name.Trim() == String.Empty))
                throw new Exception("The event name is null or empty. Every event needs a name.");

            String eventName = internalEvent ? "Internal_" + name : name;

            if (!m_times.ContainsKey(eventName))
                throw new Exception(String.Format("There is not an open {0} event named {1}. Mark the event begin before calling this method.", internalEvent ? "internal" : String.Empty, name));

            // Calculate how much time has passed since this event was started. We have a millisecond precision here!
            Double timeElapsed = (DateTime.Now - m_times[eventName]).TotalMilliseconds;            

            // Insert this event into database.
            m_sqlCommandEvent.Parameters["@eventId"].Value = m_eventId;
            m_sqlCommandEvent.Parameters["@eventName"].Value = name;
            m_sqlCommandEvent.Parameters["@dateCreated"].Value = DateTime.Now;
            m_sqlCommandEvent.Parameters["@timeElapsed"].Value = timeElapsed;
            m_sqlCommandEvent.Parameters["@internal"].Value = internalEvent;
            m_sqlCommandEvent.Parameters["@text"].Value = text == null ? (Object)DBNull.Value : (Object)text;
            m_sqlCommandEvent.ExecuteNonQuery();

            // Now that this event has been processed we need to remove it from the dictionary to avoid collisions.
            m_times.Remove(eventName);

            // We need to increment this counter here to create the next record with the right primary key.
            m_eventId++;
        } 
        
        #endregion

        #region Virtual Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Load() { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Process() { }
            
        /// <summary>
        /// 
        /// </summary>
        protected virtual void Unload() { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Abort() { }

        #endregion
    }
}

