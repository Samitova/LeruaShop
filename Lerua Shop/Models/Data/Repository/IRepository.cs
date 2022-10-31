using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lerua_Shop.Models.Data.Repository
{
    internal interface IRepository<T>
    {
        int Add(T entity);
        int AddRange(IList<T> entities);
        int Edit(T entity);
        int Delete(int id, byte[] timestamp);
        int Delete(T entity);
        T GetOne(int? id);
        T GetOne(Expression<Func<T, bool>> filter);
        bool Any(Expression<Func<T, bool>> filter);

        List<T> GetAll(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties);
        List<T> ExecuteQuery(string sql);
        List<T> ExecuteQuery(string sql, object[] sqlParametersObjects);
    }
}
