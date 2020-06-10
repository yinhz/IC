using System;
using System.Data;
using System.Data.Common;

namespace IC.Core.Data
{
    /// <summary>
    /// 工作单元
    /// 注意： 默认使用工作单元的肯定会开启事务
    /// </summary>
    public interface IAdoUnitOfWork : IAdo, IDisposable
    {
        DbTransaction GetOrBeginTransaction();

        IsolationLevel? IsolationLevel { get; }

        void Commit();

        void Rollback();

        void Close();

        void Open();
    }

    public abstract class AdoUnitOfWork : Ado, IAdoUnitOfWork
    {
        private DbTransaction transaction { get; set; }
        public IsolationLevel? IsolationLevel { get; private set; }

        public AdoUnitOfWork(string connStr, AdoDatabaseType databaseType = AdoDatabaseType.Oracle)
            : base(connStr, databaseType)
        {
            base.UnitOfWork = this;
            // 用工作单元，自动打开
            this.Open();
        }

        public void Close()
        {
            this.Dispose();
        }

        public override void Dispose()
        {
            this.transaction = null;
            base.Dispose();
        }

        public void Commit()
        {
            this.transaction?.Commit();
        }

        public DbTransaction GetOrBeginTransaction()
        {
            if (this.DbConnection == null)
                throw new Exception("You need open connection first!");

            if (this.DbConnection.State != ConnectionState.Open)
            {
                throw new Exception("The connection not open!");
            }

            if (this.transaction != null)
                return this.transaction;

            if (this.transaction == null)
            {
                if (this.DbConnection == null)
                {
                    this.Open();
                }
            }

            this.transaction = this.DbConnection.BeginTransaction();

            return this.transaction;
        }

        public void Open()
        {
            if (this.DbConnection == null)
            {
                throw new Exception("Open Connection failed");
            }

            if (this.DbConnection.State != ConnectionState.Open)
                this.DbConnection.Open();
        }

        public void Rollback()
        {
            transaction?.Rollback();
        }
    }
}
