using Emeint.Core.Exceptions;
using Emeint.Core.Managers;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using mConnect.Framework.Infrastructure.GenericRepository;

namespace Emeint.Common.Infrastructure.GenericRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext, new()
    {
        private Hashtable _repositories;

        private DbTransaction _transaction;


        private string _typeName;
        public string TypeName
        {
            get { return _typeName ?? (_typeName = typeof(TContext).Name); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual TContext DataContext
        {
            get
            {

                if (HttpContext.Current.Items[TypeName] == null) // Per Request
                {
                    HttpContext.Current.Items[TypeName] = new TContext();
                    // Disable proxy object creation.
                    AllowSerialization = true;
                    //(HttpContext.Current.Items[TypeName] as TContext).Configuration.ProxyCreationEnabled = true;
                }
                return HttpContext.Current.Items[TypeName] as TContext;
            }
        }

        public bool AllowSerialization
        {
            get { return DataContext.Configuration.ProxyCreationEnabled; }
            set { DataContext.Configuration.ProxyCreationEnabled = value; }
        }


        public int SaveChanges(bool throwException = false)
        {
            try
            {
                DataContext.Database.Log = message => System.Diagnostics.Debug.WriteLine(message);

                var rowsAffected = DataContext.SaveChanges();
                return rowsAffected;
            }
            catch (DbEntityValidationException e)
            {
                LogsManager.Error(e);

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        throw new BaseException(ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbex)
            {
                var upex = dbex.InnerException as UpdateException;
                if (upex != null)
                {
                    LogsManager.Error(upex);
                    RefreshContext();
                    var sqlex = upex.InnerException as SqlException;
                    if (sqlex != null)
                    {
                        if (sqlex.Number == 2627)
                        {
                            throw new BaseException("Duplicate Data");
                        }
                        if (sqlex.Number == 547)//The DELETE statement conflicted with the REFERENCE constraint \"FK_SparePartVehicleLineSparePart\". The conflict occurred in database \"HyundaiDevelopment\", table \"Automotive.VehicleLineSpareParts\", column 'SparePartId'.\r\nThe statement has been terminated."}
                        {
                            throw new BaseException("Database constraint conflict: " + sqlex.Message);
                        }
                        throw sqlex;
                    }
                }
                return 0;
                //throw new Exception();
            }

            catch (Exception ex)
            {
                LogsManager.Error(ex);

                RefreshContext();
                throw new BaseException("Cannot delete item because it is attached to another items");
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                int opStatus = await DataContext.SaveChangesAsync();

                return opStatus;
            }
            catch (Exception)
            {
                RefreshContext();
                throw;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var opStatus = await DataContext.SaveChangesAsync(cancellationToken);

                return opStatus;

            }
            catch (Exception)
            {
                RefreshContext();
                throw;
            }
        }


        public void BeginTransaction()
        {

            if (DataContext.Database.Connection.State != ConnectionState.Open)
            {
                DataContext.Database.Connection.Open();
            }
            _transaction = DataContext.Database.Connection.BeginTransaction(IsolationLevel.RepeatableRead);
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public int Commit()
        {
            var saveChanges = SaveChanges();
            _transaction.Commit();
            return saveChanges;
        }

        public Task<int> CommitAsync()
        {
            var saveChangesAsync = SaveChangesAsync();
            _transaction.Commit();
            return saveChangesAsync;
        }

        public void Dispose()
        {
            //if (_objectContext.Connection.State == ConnectionState.Open)
            //    _objectContext.Connection.Close();

            if (DataContext.Database.Connection != null && DataContext.Database.Connection.State == ConnectionState.Open)
                DataContext.Database.Connection.Close();

            //Dispose(true);
            if (DataContext != null)
                DataContext.Dispose();

            GC.SuppressFinalize(this);

        }


        void RefreshContext()
        {
            //Dispose();
            //_dataContext = null;
            //_repositories = null;
        }


        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            var repositoryType = typeof(Repository<,>);

            var repositoryInstance =
                Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TContext), typeof(T)), DataContext);

            if (_repositories.ContainsKey(type))
                _repositories[type] = repositoryInstance;
            else
                _repositories.Add(type, repositoryInstance);

            return repositoryInstance as IRepository<T>;
        }
    }
}
