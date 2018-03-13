using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericAirways.Contracts
{
    public interface IRepository <T> where T : class
    {
        void InitDbContext(IDbContext<T> context);
        IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);
        IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        Task<T> GetSingleAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        void Add(params T[] items);
        void Update(params T[] items);
        void Remove(params T[] items);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        void Add(T item, Action<T> preAdd=null, Action<T> postAdd=null);

    }


}
