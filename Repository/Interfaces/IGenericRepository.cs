using System.Linq.Expressions;

namespace asp_net_web_api.API.Respository
{
    public interface IGenericRepository<T> where T : class 
    {
        IQueryable<T> Table { get; }  

        IEnumerable<T> GetAll();
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        T? GetById(int id);
        public T? GetById(int id, params Expression<Func<T, object>>[] includes);
        void Add(T entity);
        void Update(T entity);
        void UpdateColumns(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateFactory);
        void Delete(T entity);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void RemoveRange(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);
    }
}
