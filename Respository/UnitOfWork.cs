using asp_net_web_api.API.Models;

namespace  asp_net_web_api.API.Respository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IItemRepository ItemsRepository {get; private set;}
        public ICategoryRepository CategoryRepository {get; private set;}

        public UnitOfWork(AppDbContext context){
            _context = context;
            ItemsRepository = new ItemRepository(_context);
            CategoryRepository = new CategoryRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}