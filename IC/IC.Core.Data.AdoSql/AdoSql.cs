using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Core.Data.AdoSql
{
    public class AdoSql : Ado
    {
        public AdoSql(string connStr,
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
                    this.dbConnection = new System.Data.SqlClient.SqlConnection(this.ConnectionString);
                }

                return dbConnection;
            }
        }

        public override DbCommand CreateCommand()
        {
            return new System.Data.SqlClient.SqlCommand(); ;
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
                            new System.Data.SqlClient.SqlParameter()
                            {
                                ParameterName = keyValuePair.Key,
                                DbType = System.Data.DbType.String,
                                Value = keyValuePair.Value
                            };
                    }

                    return new System.Data.SqlClient.SqlParameter(keyValuePair.Key, keyValuePair.Value);
                }
            ).ToArray();
        }
    }
}
