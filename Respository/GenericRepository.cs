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
            // _context.ChangeTracker.Clear();
            try  
            {  
                if(entity == null)  
                {  
                    throw new ArgumentNullException(nameof(entity));  
                }  
                // _context.Set<T>().Update(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }  
            catch (DbUpdateException  dbEx)  
            {  
                var fail = new Exception(dbEx.Message, dbEx);                  
                throw fail;  
            }  
        }

        public void UpdateColumns(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateFactory)
        {
            var entity = _context.Set<T>().FirstOrDefault(predicate);
            
            if (entity != null)
            {
                var updatedEntity = updateFactory.Compile().Invoke(entity);

                var entry = _context.Entry(updatedEntity);
                
                entry.State = EntityState.Modified;

                foreach (var property in entry.OriginalValues.Properties)
                {
                    var propertyName = property.Name;
                    if (!entry.OriginalValues[propertyName].Equals(entry.CurrentValues[propertyName]))
                    {
                        entry.Property(propertyName).IsModified = true;
                    }
                }
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