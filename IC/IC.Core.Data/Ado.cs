using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace IC.Core.Data
{
    /// <summary>
    /// 不需要工作单元情况下使用
    /// 直接执行查询语句等
    /// 
    /// 如果指定了工作单元
    /// 会强制使用工作单元事务/链接信息
    /// </summary>
    public interface IAdo : IDisposable
    {
        DbConnection DbConnection { get; }
        IAdoUnitOfWork UnitOfWork { get; }
        string ConnectionString { get; }

        IAdoRepository<TEntity, TKey> GetAdoRepository<TEntity, TKey>() where TEntity : Entity, new();

        DbCommand CreateCommand();
        DbParameter[] CreateDbParameters(IEnumerable<KeyValuePair<string, object>> dbParameters);
        /// <summary>
        /// 监视数据库命令对象(执行前，调试)
        /// </summary>
        Action<DbCommand> AopCommandExecuting { get; set; }
        /// <summary>
        /// 监视数据库命令对象(执行后，用于监视执行性能)
        /// </summary>
        Action<DbCommand, string> AopCommandExecuted { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        AdoDatabaseType DatabaseType { get; }

        #region SQL执行
        void ExecuteReaderMultiple(int multipleResult, Action<DbDataReader, int> readerHander, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        void ExecuteReaderMultiple(int multipleResult, Action<DbDataReader, int> readerHander, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms);
        void ExecuteReader(Action<DbDataReader> readerHander, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        void ExecuteReader(Action<DbDataReader> readerHander, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms);
        DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        DataSet ExecuteDataSet(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms);
        DataTable ExecuteDataTable(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        DataTable ExecuteDataTable(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms);
        int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        int ExecuteNonQuery(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms);
        object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        object ExecuteScalar(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms);

        #endregion
    }

    public abstract class Ado : IAdo
    {
        public Ado(string connStr,
            AdoDatabaseType databaseType)
        {
            this.ConnectionString = connStr;
            this.DatabaseType = databaseType;
        }

        public string ConnectionString { get; protected set; }

        public abstract DbConnection DbConnection { get; }

        public abstract DbCommand CreateCommand();

        public abstract DbParameter[] CreateDbParameters(IEnumerable<KeyValuePair<string, object>> dbParameters);

        public IAdoUnitOfWork UnitOfWork { get; set; }
        public AdoDatabaseType DatabaseType { get; private set; }

        public virtual Action<DbCommand> AopCommandExecuting { get; set; }
        public virtual Action<DbCommand, string> AopCommandExecuted { get; set; }

        public IAdoRepository<TEntity, TKey> GetAdoRepository<TEntity, TKey>() where TEntity : Entity, new()
        {
            return new AdoRepository<TEntity, TKey>(this);
        }

        /// <summary>
        /// 释放Ado对象
        /// 释放Ado对象时，不能释放 Uow, Uow的连接等信息是 uow 控制的
        /// </summary>
        public virtual void Dispose()
        {
            this.AopCommandExecuting = null;
            this.AopCommandExecuted = null;

            this.DbConnection?.Close();
        }

        #region 内部方法

        (DbCommand cmd, bool isclose) PrepareCommand(CommandType cmdType, string cmdText, DbParameter[] cmdParms)
        {
            DbCommand cmd = CreateCommand();
            bool isclose = false;
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            if (cmdParms != null)
            {
                foreach (var parm in cmdParms)
                {
                    if (parm == null) continue;
                    if (parm.Value == null) parm.Value = DBNull.Value;
                    cmd.Parameters.Add(parm);
                }
            }

            // 使用了工作单元的必须使用事务
            if (UnitOfWork != null)
            {
                var tran = UnitOfWork.GetOrBeginTransaction();
                cmd.Transaction = tran ?? throw new Exception("Can not get transaction from UOW!");
                cmd.Connection = tran.Connection;
            }
            // 未使用工作单元的
            else
            {
                cmd.Connection = this.DbConnection;
            }

            // 所有的方法都会使用 PrepareCommand
            // 在这里确保一下连接已经打开
            // 使用了 uow 的，应该会自己打开
            // 没使用的，则没有事务对象，Ado方法并没有提供打开功能，需要打开
            if (this.DbConnection.State != ConnectionState.Open)
                this.DbConnection.Open();

            AopCommandExecuting?.Invoke(cmd);

            return (cmd, isclose);
        }

        #endregion

        #region 公开方法

        //public (List<OracleParameter> parameters, string strIn) GetInParameters<T>(T[] parameters, string prefix = "W_", OracleDbType oracleDbType = OracleDbType.NVarchar2)
        //{
        //    return null;
        //    List<OracleParameter> dbParameters = new List<OracleParameter>();

        //    string strIn = "";

        //    for (int i = 0; i < parameters.Length; i++)
        //    {
        //        string parameterName = $":{prefix}{i.ToString()}";

        //        if (i == parameters.Length - 1)
        //        {
        //            strIn += parameterName;
        //        }
        //        else
        //            strIn += parameterName + ",";

        //        switch (this.DatabaseType)
        //        {
        //            case AdoDatabaseType.Oracle:
        //                dbParameters.Add(new OracleParameter(parameterName, oracleDbType, parameters[i], System.Data.ParameterDirection.Input));
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return (dbParameters, strIn);
        //}

        public void ExecuteReaderMultiple(int multipleResult, Action<DbDataReader, int> readerHander, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            if (string.IsNullOrEmpty(cmdText))
                throw new ArgumentNullException("cmdText");

            var pc = PrepareCommand(cmdType, cmdText, cmdParms);

            using (var dr = pc.cmd.ExecuteReader())
            {
                int resultIndex = 0;
                while (true)
                {
                    while (true)
                    {
                        bool isread = dr.Read();
                        if (isread == false) break;

                        readerHander?.Invoke(dr, resultIndex);
                    }
                    if (++resultIndex >= multipleResult || dr.NextResult() == false) break;
                }
                dr.Close();
            }

            pc.cmd.Parameters.Clear();
        }

        public void ExecuteReader(Action<DbDataReader> readerHander, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
            => ExecuteReaderMultiple(1, (dr, result) => readerHander(dr), cmdType, cmdText, cmdParms);

        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var ret = new DataSet();
            DataTable dt = null;
            ExecuteReaderMultiple(16, (dr, result) =>
            {
                if (ret.Tables.Count <= result)
                {
                    dt = ret.Tables.Add();
                    for (var a = 0; a < dr.FieldCount; a++) dt.Columns.Add(dr.GetName(a));
                }
                object[] values = new object[dt.Columns.Count];
                dr.GetValues(values);
                dt.Rows.Add(values);
            }, cmdType, cmdText, cmdParms);
            return ret;
        }

        public DataTable ExecuteDataTable(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            var ret = new DataTable();
            ExecuteReader(dr =>
            {
                if (ret.Columns.Count == 0)
                    for (var a = 0; a < dr.FieldCount; a++) ret.Columns.Add(dr.GetName(a));
                object[] values = new object[ret.Columns.Count];
                dr.GetValues(values);
                ret.Rows.Add(values);
            }, cmdType, cmdText, cmdParms);
            return ret;
        }

        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            if (string.IsNullOrEmpty(cmdText)) return 0;

            var pc = PrepareCommand(cmdType, cmdText, cmdParms);

            int val = pc.cmd.ExecuteNonQuery();

            pc.cmd.Parameters.Clear();
            return val;
        }

        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            if (string.IsNullOrEmpty(cmdText)) return null;

            var pc = PrepareCommand(cmdType, cmdText, cmdParms);

            object val = null;

            val = pc.cmd.ExecuteScalar();

            pc.cmd.Parameters.Clear();

            return val;
        }

        public void ExecuteReaderMultiple(int multipleResult, Action<DbDataReader, int> readerHander, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
            => ExecuteReaderMultiple(multipleResult, readerHander, cmdType, cmdText, this.CreateDbParameters(cmdParms));

        public void ExecuteReader(Action<DbDataReader> readerHander, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
            => ExecuteReader(readerHander, cmdType, cmdText, this.CreateDbParameters(cmdParms));

        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
            => ExecuteDataSet(cmdType, cmdText, this.CreateDbParameters(cmdParms));

        public DataTable ExecuteDataTable(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
            => ExecuteDataTable(cmdType, cmdText, this.CreateDbParameters(cmdParms));

        public int ExecuteNonQuery(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
            => ExecuteNonQuery(cmdType, cmdText, this.CreateDbParameters(cmdParms));

        public object ExecuteScalar(CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
            => ExecuteScalar(cmdType, cmdText, this.CreateDbParameters(cmdParms));

        #endregion
    }
}
