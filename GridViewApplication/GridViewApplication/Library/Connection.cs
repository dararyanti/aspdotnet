using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GridViewApplication.Library
{
    public static class Connection
    {
        private static string strServerType = "MsSQL";
        public static string ConnectionStringMsSQL = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString" + strServerType].ConnectionString;

    }
}