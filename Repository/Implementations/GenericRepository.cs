using System.Linq.Expressions;
using asp_net_web_api.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace asp_net_web_api.API.Respository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private DbSet<T>? _entities;  

        public GenericRepository(AppDbContext context){
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAll(){
            return await _context.Set<T>().AsNoTracking<T>().ToListAsync();
        }

       public async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includes){
            IQueryable<T> query = Table; 

            if(includes != null)
            {
                query = includes.Aggregate(query, (currentQuery, include) => currentQuery.Include(include));
            }

            return await query.ToListAsync();
        }

        public T? GetById(int id) {
            return _context.Set<T>().Find(id);
        }

        public T? GetById(int id, params Expression<Func<T, object>>[] includes) {
            IQueryable<T> query = Table;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            
            var idProperty = typeof(T).GetProperty("Id") ?? throw new ArgumentException("ID property not found");

            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, idProperty);
            var equals = Expression.Equal(property, Expression.Constant(id));

            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return query.FirstOrDefault(lambda);
        }


        public async void Add(T entity) {
            try  
            {  
                if(entity == null)  
                {  
                    throw new ArgumentNullException(nameof(entity));  
                }  
                await _context.Set<T>().AddAsync(entity);
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
                int id = (int)entity.GetType().GetProperty("Id").GetValue(entity);
                var oldobj = _context.Set<T>().Find(id);
                var UpdatedObj = CheckUpdateObject<T>(oldobj, entity);
                _context.Entry(oldobj).CurrentValues.SetValues(UpdatedObj);
              
                // _context.Set<T>().Update(entity);
                // _context.Entry(entity).State = EntityState.Modified;
            }  
            catch (DbUpdateException  dbEx)  
            {  
                var fail = new Exception(dbEx.Message, dbEx);                  
                throw fail;  
            }  
        }

        public  T CheckUpdateObject<T>(T originalObj, T updateObj)
        {
            foreach (var property in updateObj.GetType().GetProperties())
            {
                if (property.GetValue(updateObj) == null)
                {
                    //property.SetValue(updateObj,originalObj.GetType().GetProperty(property.Name).GetValue(originalObj, null));
                    _context.Entry(updateObj).Property(property.Name).IsModified = false;
                }
            }
            return updateObj;
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
            return _context.Set<T>().AsNoTracking().Where(expression);
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