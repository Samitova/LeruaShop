using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.Data.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Lerua_Shop.Models.Data.Repository
{
    public class BaseRepository<T> : IDisposable, IRepository<T> where T : EntityBase, new()
    {
        private readonly DbStoreContext _db;
        private readonly DbSet<T> _table;
        public BaseRepository(DbStoreContext context)
        {
            _db = context;
            _table = _db.Set<T>();
        }
        protected DbStoreContext Context => _db;
        public void Dispose()
        {
            _db?.Dispose();
        }

        internal int SaveChanges()
        {
            try
            {
                return _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {                
                throw;
            }
            catch (DbUpdateException ex)
            {                
                throw;
            }
            catch (CommitFailedException ex)
            {                
                throw;
            }
            catch (Exception ex)
            {                
                throw;
            }
        }
        public T GetOne(int? id) => _table.Find(id);
        public T GetOne(Expression<Func<T, bool>> filter) => _table.FirstOrDefault(filter);
        public virtual List<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public bool Any(Expression<Func<T, bool>> filter) => _table.Any(filter);
        public List<T> ExecuteQuery(string sql) => _table.SqlQuery(sql).ToList();
        public List<T> ExecuteQuery(string sql, object[] sqlParametersObjects)
            => _table.SqlQuery(sql, sqlParametersObjects).ToList();
        public int Add(T entity)
        {
            _table.Add(entity);
            return SaveChanges();
        }
        public int AddRange(IList<T> entities)
        {
            _table.AddRange(entities);
            return SaveChanges();
        }
        public int Edit(T entity)
        {
            var existingEntity = _table.Find(entity.Id);
            _db.Entry(existingEntity).CurrentValues.SetValues(entity);
            return SaveChanges();
        }
        public int Delete(int id, byte[] timestamp)
        {
            _db.Entry(new T() { Id = id, Timestamp = timestamp }).State = EntityState.Deleted;
            return SaveChanges();
        }
        public int Delete(T entity)
        {
            _db.Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

    }
}