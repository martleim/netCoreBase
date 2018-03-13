using GenericAirways.Model;
using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GenericAirways.Contracts;

namespace GenericAirways.DataAccessMock
{
    public class BaseRepository<T> : IDisposable, IRepository<T> where T : class
    {

        private MockContext<T> context;
        private MockContext<T> Context{
            get{
                if(context==null)
                    throw new Exception("Database context not set");

                return context;

            }
        }

        public void InitDbContext(IDbContext<T> context){
            context=(MockContext<T>)context;
            Context.EnsureCreation();
        }

        public bool LazyLoadingEnabled
        {
            set
            {
                //Context.Configuration.LazyLoadingEnabled = value;
            }
        }

        public bool ProxyCreationEnabled
        {
            set
            {
                //Context.Configuration.ProxyCreationEnabled = value;
            }
        }

        private bool lazyLoading;
        public BaseRepository(DbContext ctx = null, bool _lazyLoading=true)
        {
            lazyLoading = _lazyLoading;
            if (ctx != null)
            {
                //context = ctx;
            }
            else
            {
                context = new MockContext<T>();
                ((MockContext<T>)context).EnsureCreation();
                //initialize();
            }
            //Context.Configuration.LazyLoadingEnabled = lazyLoading;
        }

        private void initialize() {
            //context = new MyDealTaskEntities();
            //Context.Configuration.LazyLoadingEnabled = lazyLoading;
        }

        public virtual void Add(params T[] items)
        {
            foreach (T item in items)
            {
                Context.Entry(item).State = EntityState.Added;
            }
            SaveChanges();
        }

        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;
            IQueryable<T> dbQuery = Context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            list = dbQuery
                .AsNoTracking()
                .ToList<T>();
            return list;
        }

        public virtual IList<T> GetList(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;
            IQueryable<T> dbQuery = Context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            list = dbQuery
                .AsNoTracking()
                .Where(where)
                .ToList<T>();
            return list;
        }

        public virtual T GetSingle(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            IQueryable<T> dbQuery = Context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            item = dbQuery
                .AsNoTracking() //Don't track any changes for the selected item
                .FirstOrDefault(where); //Apply where clause
            return item;
        }

        public virtual Task<T> GetSingleAsync(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = Context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            return dbQuery
                .AsNoTracking() //Don't track any changes for the selected item
                .Where(where).AsQueryable()
                .FirstOrDefaultAsync(); //Apply where clause
        }

        public virtual void Update(params T[] items)
        {
            foreach (T item in items)
            {
                Context.Entry(item).State = EntityState.Modified;
            }
            SaveChanges();
        }


        public virtual void UpdateWithSpecificFields(string[] fields, params T[] items)
        {
            foreach (T item in items)
            {
                Context.Set<T>().Attach(item);
                foreach(var field in fields)
                {
                    Context.Entry(item).Property(field).IsModified = true;
                }
            }
            SaveChanges();
        }

        public virtual void Remove(params T[] items)
        {
            foreach (T item in items)
            {
                Context.Entry(item).State = EntityState.Deleted;
            }
            SaveChanges();
        }



        /*public DbRawSqlQuery<T> SqlQuery(string sql, params object[] parameters)
        {
            return Context.Database.SqlQuery<T>(sql, parameters);
        }*/

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sql, parameters);
        }

        public virtual int Count(Func<T, bool> where)
        {
            IQueryable<T> dbQuery = Context.Set<T>();

            return dbQuery.Count(); //Apply where clause
        }


        public void Dispose()
        {
            context?.Dispose();
        }

        private int SaveChanges()
        {


            try
            {
                var ret = Context.SaveChanges();
                Dispose();
                initialize();
                return ret;
            }
            catch (DbUpdateException updateException)
            {
                CatchValidateException(updateException);
            }
            /*catch (DbEntityValidationException valException)
            {
                CatchValidateException(valException);
            }*/
            return 0;
        }

        private void CatchValidateException(/*Data*/Exception exception)
        {
            /*if (exception is DbEntityValidationException)
            {
                var valException = (DbEntityValidationException)exception;
                foreach (var eve in valException.EntityValidationErrors)
                {
                    var log = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    //APIO.Common.Helper.Logger.log.Info("DbEntityValidationException: " + log);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        var errorLog = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);

                        //APIO.Common.Helper.Logger.log.Info("DbEntityValidationException: " + errorLog);

                    }
                }

                throw new RepositoryValidationException(valException, context);
            }
            if (exception is DbUpdateException)
            {
                throw new RepositoryUpdateException((DbUpdateException)exception, ((exception.InnerException != null && exception.InnerException.InnerException != null)?exception.InnerException.InnerException.Message:"Unspecified Exception"));
            }*/
            throw new Exception("Unspecified Error",exception);
        }

        public void Add(T item, Action<T> preAdd = null, Action<T> postAdd = null)
        {
            if (preAdd != null)
                preAdd(item);

            Context.Entry(item).State = EntityState.Added;

            if (postAdd!=null)
                postAdd(item);

            SaveChanges();
            
        }
    }

    public class RepositoryException : Exception {

        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }

    }

    public class RepositoryValidationException : RepositoryException
    {

        //IEnumerable<DbEntityValidationResult> ValidationErrors;
        public RepositoryValidationException(Exception/*DbEntityValidationException*/ innerException, DbContext context)
            : base("There are some validation errors",innerException)
        {
            //this.ValidationErrors = Context.GetValidationErrors();
        }

        public string[] GetValidationErrors(){
            Dictionary<string, bool> errors = new Dictionary<string, bool>();
            //ValidationErrors.ToList().ForEach(ve => ve.ValidationErrors.ToList().ForEach(ve2 => { if (!errors.ContainsKey(ve2.ErrorMessage)) { errors.Add(ve2.ErrorMessage, true); } }));
            return errors.Keys.ToArray();
            
        }
    }

    public class RepositoryUpdateException : RepositoryException
    {

        string[] ValidationErrors;
        public RepositoryUpdateException(/*DbUpdateException*/Exception innerException, string messages)
            : base(messages, innerException)
        {

        }

    }


    public class MockContext<T> : DbContext, IDbContext<T> where T : class
    {
        public DbSet<T> Entities { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var t = this.GetType().GetGenericArguments()[0];
            optionsBuilder.UseSqlite("Data Source=MockDatabase.db");
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var t = this.GetType().GetGenericArguments()[0];
            modelBuilder.Entity<T>()
                .ToTable(t.Name);
        }

        public async void EnsureCreation(){

            await this.Database.EnsureCreatedAsync();
            await this.Entities.CountAsync();
            

        }

    }

}
