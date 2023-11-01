using System.Linq.Expressions;
using asp_net_web_api.API.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Respository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private DbSet<T>? _entities;  

        public GenericRepository(AppDbContext context){
            _context = context;
        }

        public IEnumerable<T> GetAll(){
            return _context.Set<T>().ToList();
        }

        public T? GetById(int id) {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity) {
            try  
            {  
                if(entity == null)  
                {  
                    throw new ArgumentNullException(nameof(entity));  
                }  
                _context.Set<T>().Add(entity);
            }  
            catch (DbUpdateException  dbEx)  
            {  
                var fail = new Exception(dbEx.Message, dbEx);                  
                throw fail;  
            }  
            
        }

        public void Update(T entity) {
            try  
            {  
                if(entity == null)  
                {  
                    throw new ArgumentNullException(nameof(entity));  
                }  
                _context.Entry(entity).State = EntityState.Modified;
            }  
            catch (DbUpdateException  dbEx)  
            {  
                var fail = new Exception(dbEx.Message, dbEx);                  
                throw fail;  
            }  
        }

        public void Delete(T entity) {
            try  
            {  
                if(entity == null)  
                {  
                    throw new ArgumentNullException(nameof(entity));  
                }  
                _context.Set<T>().Remove(entity);
            }  
            catch (DbUpdateException  dbEx)  
            {  
                var fail = new Exception(dbEx.Message, dbEx);                  
                throw fail;  
            }  
           
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression) {
            return _context.Set<T>().Where(expression);
        }

        public void RemoveRange(IEnumerable<T> entities) {

            _context.Set<T>().RemoveRange(entities);
        }
        
        public void AddRange(IEnumerable<T> entities) {
            _context.Set<T>().AddRange(entities);
        }

        public virtual IQueryable<T> Table  
        {  
            get  
            {  
                return Entities;  
            }  
        }  
  
        private DbSet<T> Entities  
        {  
            get  
            {  
                _entities ??= _context.Set<T>();  
                return _entities;  
            }  
        }  

    }

}