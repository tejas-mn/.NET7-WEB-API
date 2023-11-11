using asp_net_web_api.API.Models;

namespace  asp_net_web_api.API.Respository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private bool _disposed = false;
        public IItemRepository ItemsRepository {get; private set;}
        public ICategoryRepository CategoryRepository {get; private set;}

        public UnitOfWork(AppDbContext context)
        {
            _dbContext = context;
            ItemsRepository = new ItemRepository(_dbContext);
            CategoryRepository = new CategoryRepository(_dbContext);
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(_dbContext);
        }

        public int Complete()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed) 
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }

                _disposed = true;
            }
        }
    }
}