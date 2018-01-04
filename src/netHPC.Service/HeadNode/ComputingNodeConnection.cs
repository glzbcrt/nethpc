using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace netHPC.Service.HeadNode
{
    class ComputingNodeConnection
    {
        #region Fields

        private NetworkStream m_networkStream;
        private TcpClient m_tcpClient;
        private Byte[] m_readBuffer; 

        #endregion

        internal ComputingNodeConnection(TcpClient tcpClient)
        {
            m_tcpClient = tcpClient;
            m_networkStream = tcpClient.GetStream();
            m_readBuffer = new Byte[200];
        }

        public TcpClient TcpClient
        {
            get { return m_tcpClient; }
            set { m_tcpClient = value; }
        }
        
        public NetworkStream NetworkStream
        {
            get { return m_networkStream; }
            set { m_networkStream = value; }
        }
        
        public Byte[] ReadBuffer
        {
            get { return m_readBuffer; }
            set { m_readBuffer = value; }
        }
    }
}
