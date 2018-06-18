using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Entity;
using System.IO;
using System.Security.Cryptography;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Spring.Context.Support;

namespace SpecLab.Business.Services
{
    public class ConnectionService
    {
        public string EncryptUserId { get; set; }

        public string EncryptPassword { get; set; }

        public string EntityConnectionString { get; set; }

        public string SqlConnectionString { get; set; }

        private static DbConnection DbConnection { get; set; }

        public DbConnection getEntityConnection()
        {
            if (DbConnection == null)
            {
                string connectionString = EntityConnectionString;
                connectionString = connectionString.Replace(":userId", CommonUtils.Decrypt(EncryptUserId));
                connectionString = connectionString.Replace(":password", CommonUtils.Decrypt(EncryptPassword));

                DbConnection = new System.Data.Entity.Core.EntityClient.EntityConnection(connectionString);
                DbConnection.Open();
            }

            return DbConnection;
        }

        public SqlConnection getSqlConnection()
        {
            string connectionString = SqlConnectionString;
            connectionString = connectionString.Replace(":userId", CommonUtils.Decrypt(EncryptUserId));
            connectionString = connectionString.Replace(":password", CommonUtils.Decrypt(EncryptPassword));

            return new SqlConnection(connectionString);
        }
        public SqlConnection getLabconnectSqlConnection()
        {
            string connectionString = SqlConnectionString;
            connectionString = connectionString.Replace(":userId", "svc_vm_lishn");
            connectionString = connectionString.Replace(":password", "vmlishn_20180320");

            return new SqlConnection(connectionString);
        }
    }
}
