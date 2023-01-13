using NHibernate;
using System;
using System.Data;

namespace TGP.Seguridad.DataAccess.Infrastructure.Transactions
{
    public class Transaction : IDisposable
    {
        private ITransaction _trx;

        protected ITransaction Trx
        {
            set { this._trx = value; }

            get { return this._trx; }
        }

        public bool IsActive
        {
            get
            {
                if (this.Trx != null)
                {
                    return this.Trx.IsActive;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Commit()
        {
            if (this.Trx != null && this.IsActive && !this.Trx.WasCommitted && !this.Trx.WasRolledBack)
            {
                this.Trx.Commit();
            }
        }

        public void Rollback()
        {
            if (this.Trx != null && this.IsActive && !this.Trx.WasCommitted && !this.Trx.WasRolledBack)
            {
                this.Trx.Rollback();
            }
        }

        public void BeginTransaction()
        {
            this.Trx.Begin();
        }

        public void BeginTransaction(IsolationLevel isoLevel)
        {
            this.Trx.Begin(isoLevel);
        }

        public void BeginTransaction(ISession session)
        {
            this.Trx = session.BeginTransaction();
        }

        public void BeginTransaction(ISession session, IsolationLevel isoLevel)
        {
            this.Trx = session.BeginTransaction(isoLevel);
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
