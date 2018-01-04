
using Microsoft.Win32;

using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace netHPC.Service
{
    static class ServiceTools
    {
        #region Fields

        private static String sm_serverName = "localhost";
        private static String sm_databaseName = "netHPC";
        private static String sm_userName = "netHPC";
        private static String sm_userPassword = "netHPC";
        private static String sm_headNode = "localhost";
        private static Int32 sm_headNodePort = 15000;

        private static Entities sm_netHPCEntities;
        private static Node sm_node;

        #endregion

        #region ServiceTools()
        static ServiceTools()
        {
            RegistryKey registryConfig = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\netHPC");

            sm_serverName = registryConfig.GetValue("databaseServer").ToString();
            sm_databaseName = registryConfig.GetValue("databaseName").ToString();
            sm_userName = registryConfig.GetValue("databaseUsername").ToString();
            sm_userPassword = registryConfig.GetValue("databasePassword").ToString();
            sm_headNode = registryConfig.GetValue("headNodeServer").ToString();
            sm_headNodePort = Convert.ToInt32(registryConfig.GetValue("headNodeTcpPort"));

            sm_netHPCEntities = Utils.GetEntities(sm_serverName, sm_databaseName, sm_userName, sm_userPassword);
        } 
        #endregion

        #region Entities
        public static Entities Entities
        {
            get { return sm_netHPCEntities; }
        } 
        #endregion

        #region ThisNode
        public static Node ThisNode
        {
            get { return sm_node; }
        } 
        #endregion

        #region HeadNodeServerName
        public static String HeadNodeServerName
        {
            get { return sm_headNode; }
            set { sm_headNode = value; }
        } 
        #endregion

        #region HeadNodeTcpPort
        public static Int32 HeadNodeTcpPort
        {
            get { return sm_headNodePort; }
            set { sm_headNodePort = value; }
        } 
        #endregion

        #region UpdateNodeInformation()
        public static void UpdateNodeInformation()
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT MaxClockSpeed, NumberOfCores FROM Win32_Processor");
            Int32 numberOfExecUnits = 0;
            Int32 clockSpeed = 0;

            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            {
                clockSpeed = Int32.Parse(managementObject["MaxClockSpeed"].ToString());
                numberOfExecUnits = numberOfExecUnits + Int32.Parse(managementObject["NumberOfCores"].ToString());
            }

            if (sm_netHPCEntities.Node.Where("it.name = '" + Environment.MachineName + "'").Count() == 1)
                sm_node = sm_netHPCEntities.Node.Where("it.name = '" + Environment.MachineName + "'").First();
            else
            {
                sm_node = new Node();
                sm_node.Name = Environment.MachineName;
                sm_node.DateCreated = DateTime.Now;
                sm_netHPCEntities.AddToNode(sm_node);
            }

            sm_node.LastReport = DateTime.Now;
            sm_node.NumOfExecUnits = numberOfExecUnits;
            sm_node.SpeedMHz = clockSpeed;
            sm_node.Status = 0;

            sm_netHPCEntities.SaveChanges();
        } 
        #endregion
    }
}
