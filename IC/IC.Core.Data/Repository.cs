using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace IC.Core.Data
{
    [Serializable]
    public class Entity : ICloneable
    {
        public static string GetPropertyName<T>(Expression<Func<T>> express)
        {
            var memExpress = (MemberExpression)express.Body;

            if (memExpress == null)
                throw new Exception("The expression is valid");

            return memExpress.Member.Name;
        }

        public virtual object Clone()
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);

                return formatter.Deserialize(ms);
            }
        }
    }

    public interface IAdoRepository<TEntity, TKey> where TEntity : Entity, new()
    {
        IAdo Ado { get; }
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
        string TableName { get; set; }
        TEntity Entity { get; }
        /// <summary>
        /// 更新
        /// 注意： 不支持自己更新自己
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        [Obsolete]
        int Update(TKey key, Dictionary<string, object> dbParameters);
        /// <summary>
        /// 传入KeyName，作更新
        /// 注意： 不支持自己更新自己
        /// </summary>
        /// <param name="keyName">主键名称</param>
        /// <param name="key">主键值</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        int Update(string keyName, TKey key, Dictionary<string, object> dbParameters);
        /// <summary>
        /// 多where条件更新
        /// 注意： 不支持自己更新自己
        int Update(Dictionary<string, object> whereParameters, Dictionary<string, object> dbParameters);
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete]
        int Delete(TKey key);
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        int Delete(string keyName, TKey key);
        /// <summary>
        /// 多where条件删除
        /// </summary>
        /// <param name="whereParameters"></param>
        /// <returns></returns>
        int Delete(Dictionary<string, object> whereParameters);
    }

    public class AdoRepository<TEntity, TKey> : IAdoRepository<TEntity, TKey> where TEntity : Entity, new()
    {
        // 未来考虑反射支持
        public virtual string TableName { get; set; }

        public AdoRepository(IAdo ado, string tableName = null)
        {
            this.Ado = ado;
            this.Entity = new TEntity();

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(TEntity).Name;
            }

            this.TableName = tableName;
        }

        public IAdo Ado { get; private set; }

        public TEntity Entity { get; private set; }

        public IDbConnection DbConnection => this.Ado?.DbConnection;

        public IDbTransaction DbTransaction => this.Ado?.UnitOfWork?.GetOrBeginTransaction();

        public virtual int Update(TKey key, Dictionary<string, object> dbParameters)
            => Update(null, key, dbParameters);

        public virtual int Update(string keyName, TKey key, Dictionary<string, object> dbParameters)
        {
            // 未来考虑反射支持
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            if (dbParameters == null || dbParameters.Count == 0)
                throw new ArgumentException("dbParameters");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"UPDATE {TableName} SET ");

            int fisrt = 0;
            foreach (var dbParameter in dbParameters)
            {
                if (fisrt > 0)
                    sb.Append(",");
                fisrt++;
                string upperColumn = dbParameter.Key.ToUpper();
                sb.AppendLine($" {upperColumn} = :{upperColumn} ");
            }

            string upperKeyName = keyName.ToUpper();
            sb.AppendLine($" WHERE {upperKeyName} = :{upperKeyName}");

            IEnumerable<KeyValuePair<string, object>> _dbParameters = dbParameters.Concat(
                new Dictionary<string, object> {
                    { upperKeyName, key }
                }
            );

            return this.Ado.ExecuteNonQuery(System.Data.CommandType.Text, sb.ToString(), this.Ado.CreateDbParameters(_dbParameters));
        }

        public virtual int Update(Dictionary<string, object> whereParameters, Dictionary<string, object> dbParameters)
        {
            if (whereParameters == null || whereParameters.Count == 0)
                throw new ArgumentException("whereParameters");

            if (dbParameters == null || dbParameters.Count == 0)
                throw new ArgumentException("dbParameters");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"UPDATE {TableName} SET ");

            foreach (var dbParameter in dbParameters)
            {
                string upperColumn = dbParameter.Key.ToUpper();
                sb.AppendLine($" {upperColumn} = :{upperColumn} ");
            }

            sb.Append(" WHERE ");

            bool firstLine = true;
            foreach (var whereParameter in whereParameters)
            {
                string upperKeyName = whereParameter.Key.ToUpper();
                if (firstLine)
                {
                    sb.Append($" {upperKeyName} = :{upperKeyName}");
                    firstLine = false;
                }
                sb.AppendLine($" AND {upperKeyName} = :{upperKeyName} ");
            }

            IEnumerable<KeyValuePair<string, object>> _dbParameters = dbParameters.Concat(whereParameters);

            return this.Ado.ExecuteNonQuery(System.Data.CommandType.Text, sb.ToString(), this.Ado.CreateDbParameters(_dbParameters));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int Delete(TKey key)
            => Delete(string.Empty, key);

        public virtual int Delete(string keyName, TKey key)
        {
            if (string.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            string upperKeyName = keyName.ToUpper();

            return this.Ado.ExecuteNonQuery(
                System.Data.CommandType.Text,
                $"DELETE FROM {TableName} WHERE {upperKeyName} = :{upperKeyName}",
                this.Ado.CreateDbParameters(new Dictionary<string, object>() { { upperKeyName, key } })
                );
        }

        public int Delete(Dictionary<string, object> whereParameters)
        {
            if (whereParameters == null || whereParameters.Count == 0)
                throw new ArgumentException("whereParameters");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"DELETE FROM {TableName} WHERE ");

            bool firstLine = true;
            foreach (var whereParameter in whereParameters)
            {
                string upperKeyName = whereParameter.Key.ToUpper();
                if (firstLine)
                {
                    sb.Append($" {upperKeyName} = :{upperKeyName}");
                    firstLine = false;
                }
                sb.AppendLine($" AND {upperKeyName} = :{upperKeyName} ");
            }

            return this.Ado.ExecuteNonQuery(
                System.Data.CommandType.Text,
                sb.ToString(),
                this.Ado.CreateDbParameters(whereParameters)
                );
        }
    }
}
