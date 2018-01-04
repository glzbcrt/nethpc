using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Text;

namespace netHPC.Shared
{
    public static class Utils
    {
        #region GetEntities(String serverName, String databaseName, String userName, String userPassword)
        public static Entities GetEntities(String serverName, String databaseName, String userName, String userPassword)
        {
            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder();
            entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
            entityConnectionStringBuilder.Metadata = "res://*/netHPCEntities.csdl|res://*/netHPCEntities.ssdl|res://*/netHPCEntities.msl";
            entityConnectionStringBuilder.ProviderConnectionString = String.Format("Data Source={0};Initial Catalog={1};User Id={2};Pwd={3};MultipleActiveResultSets=True", serverName, databaseName, userName, userPassword);

            return new Entities(entityConnectionStringBuilder.ConnectionString);
        } 
        #endregion
    }
}
