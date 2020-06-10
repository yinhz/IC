using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IC.Core.Data.AdoOracle.Test
{
    /// <summary>
    /// 服务运行的环境
    /// </summary>
    public enum ServiceEnvironment
    {
        Unspecified = 0,
        /// <summary>
        /// WebAPI环境
        /// 可能每次不同的请求需要到不同的MI库请求数据
        /// </summary>
        WebAPI = 1,
        /// <summary>
        /// MIPlugin环境
        /// 每个MIPlugin服务面对的数据库环境应该是一致的
        /// </summary>
        MIPlugin = 2
    }

    /// <summary>
    /// 数据库提供类
    /// 
    /// 用来设置数据库连接信息，初始化数据库所在的环境信息（WebAPI/MIPlugin)
    /// 
    /// 1、WebAPI 环境使用时需要先设置 ServiceEnvironment = WebAPI
    ///    a、需要自行设置 MESDatabaseConnectionString/MESSlaveDatabaseConnectionString 才可以使用 MESDatabase
    ///    b、WebAPI 环境需要自行初始化 MIDatabaseDict。提供所有的 MI 库信息，因为从前端过来的请求，我们无法确认是用的哪个MI库
    ///    才可以使用 MESDatabaes/MIDatabase
    ///    
    /// 2、MIPlugin 环境使用时需要先设置 ServiceEnviroment = MIPlugin
    ///    需要自行设置 MESDatabaseConnectionString/MESSlaveDatabaseConnectionString/DefaultMIDatabaseConnectionString/DefaultMISlaveDatabaseConnectionString
    ///    才可以使用 MESDatabase / MIDatabase
    /// </summary>
    public static class DatabaseProvider
    {
        public static bool Initialized { get; private set; } = false;

        public static string MESDatabaseConnectionString;
        public static string MESSlaveDatabaseConnectionString;

        public static ServiceEnvironment ServiceEnvironment;

        private static AdoDatabase _adoMESDatabase;
        private static AdoDatabase _adoMESSlaveDatabase;
        public static AdoDatabase AdoMESDatabase
        {
            get
            {
                if (_adoMESDatabase != null)
                    return _adoMESDatabase;

                if (string.IsNullOrEmpty(MESDatabaseConnectionString))
                    throw new Exception("Must set MES connection string first!");

                _adoMESDatabase = new AdoDatabase(MESDatabaseConnectionString, AdoDatabaseType.Oracle);

                return _adoMESDatabase;
            }
        }
        public static AdoDatabase AdoMESSlaveDatabase
        {
            get
            {
                if (_adoMESSlaveDatabase != null)
                    return _adoMESSlaveDatabase;

                if (string.IsNullOrEmpty(MESSlaveDatabaseConnectionString))
                    throw new Exception("Must set MES Slave connection string first!");

                _adoMESSlaveDatabase = new AdoDatabase(MESSlaveDatabaseConnectionString, AdoDatabaseType.Oracle);

                return _adoMESSlaveDatabase;
            }
        }
        
        static DatabaseProvider()
        {
        }
    }

    [TestClass]
    public class OracleTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DatabaseProvider.ServiceEnvironment = ServiceEnvironment.MIPlugin;
            DatabaseProvider.MESDatabaseConnectionString 
                = "User Id=flxuser;Password=sanymom2020;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.0.14.151)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=SANYMOMDEV)));";

            // 原生ADO操作
            using (Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DatabaseProvider.MESDatabaseConnectionString))
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                try
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "insert into ATL_WIP_SERIAL_NO1 (QUERYSERIALNO) values ('s_" + System.Guid.NewGuid().ToString() + "')";
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }

            // 不使用事务
            using (var ado = DatabaseProvider.AdoMESDatabase.CreateAdo())
            {
                try
                {
                    ado.ExecuteNonQuery(System.Data.CommandType.Text,
                    "insert into ATL_WIP_SERIAL_NO1 (QUERYSERIALNO) values ('s_" + System.Guid.NewGuid().ToString() + "')");
                }
                catch (Exception ex)
                {
                }
            }

            // 使用事务
            using (var uow = DatabaseProvider.AdoMESDatabase.CreateUnitOfWork())
            {
                try
                {
                    uow.ExecuteNonQuery(System.Data.CommandType.Text,
                    "insert into ATL_WIP_SERIAL_NO1 (QUERYSERIALNO) values ('s_" + System.Guid.NewGuid().ToString() + "')");

                    // commit 必须写在最后
                    uow.Commit();
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                }
            }
        }
    }
}
