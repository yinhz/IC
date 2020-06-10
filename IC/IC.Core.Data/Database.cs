using System;
using System.Collections.Generic;
using System.Linq;

namespace IC.Core.Data
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
    /// 数据库对象
    /// </summary>
    public interface IAdoDatabase
    {
        string ConnectionString { get; }
        // 创建Ado对象
        IAdo CreateAdo();
        // 创建工作单元
        IAdoUnitOfWork CreateUnitOfWork();
        // 数据库类型，当前只支持Oracle
        AdoDatabaseType DatabaseType { get; }
    }

    public class AdoDatabase : IAdoDatabase
    {
        public AdoDatabase(string connStr, AdoDatabaseType databaseType = AdoDatabaseType.Oracle)
        {
            this.ConnectionString = connStr;
            this.DatabaseType = databaseType;
        }

        public string ConnectionString { get; private set; }
        public AdoDatabaseType DatabaseType { get; private set; }

        public IAdo CreateAdo()
        {
            switch (DatabaseType)
            {
                case AdoDatabaseType.Oracle:
                    return Activator.CreateInstance(Type.GetType("IC.Core.Data.AdoOracle.AdoOracle, IC.Core.Data.AdoOracle"), this.ConnectionString, this.DatabaseType)
                        as IAdo;
                case AdoDatabaseType.Sql:
                    return Activator.CreateInstance(Type.GetType("IC.Core.Data.AdoSql.AdoSql, IC.Core.Data.AdoSql"), this.ConnectionString, this.DatabaseType)
                        as IAdo;
                default:
                    throw new Exception("Create ado failed!");
            }
        }

        public IAdoUnitOfWork CreateUnitOfWork()
        {
            switch (DatabaseType)
            {
                case AdoDatabaseType.Oracle:
                    return Activator.CreateInstance(Type.GetType("IC.Core.Data.AdoOracle.AdoUnitOfWorkOracle, IC.Core.Data.AdoOracle"), this.ConnectionString, this.DatabaseType) 
                        as IAdoUnitOfWork;
                case AdoDatabaseType.Sql:
                    return Activator.CreateInstance(Type.GetType("IC.Core.Data.AdoSql.AdoUnitOfWorkSql, IC.Core.Data.AdoSql"), this.ConnectionString, this.DatabaseType) 
                        as IAdoUnitOfWork;
                default:
                    throw new Exception("Create ado UnitOfWork failed!");
            }
        }
    }

    public enum AdoDatabaseType
    {
        Oracle,
        Sql
    }
}
