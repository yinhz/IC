using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Core.Data.AdoOracle
{
    public class AdoOracle : Ado
    {
        public AdoOracle(string connStr,
            AdoDatabaseType databaseType)
            : base(connStr, databaseType)
        {

        }

        private DbConnection dbConnection;

        public override DbConnection DbConnection
        {
            get
            {
                if (dbConnection == null)
                {
                    this.dbConnection = new Oracle.ManagedDataAccess.Client.OracleConnection(this.ConnectionString);
                }

                return dbConnection;
            }
        }

        public override DbCommand CreateCommand()
        {
            return new Oracle.ManagedDataAccess.Client.OracleCommand() { BindByName = true };
        }

        public override DbParameter[] CreateDbParameters(IEnumerable<KeyValuePair<string, object>> dbParameters)
        {
            if (dbParameters == null)
                return null;

            return dbParameters.Select(
                (keyValuePair) =>
                {
                    if (keyValuePair.Value is string)
                    {
                        return
                            new Oracle.ManagedDataAccess.Client.OracleParameter()
                            {
                                ParameterName = keyValuePair.Key,
                                OracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.NVarchar2,
                                Value = keyValuePair.Value
                            };
                    }

                    return new Oracle.ManagedDataAccess.Client.OracleParameter(keyValuePair.Key, keyValuePair.Value);
                }
            ).ToArray();
        }
    }
}
