
namespace asp_net_web_api.API.Respository{
    public interface IUnitOfWork : IDisposable{

        IItemRepository ItemsRepository { get; }
        ICategoryRepository CategoryRepository { get; }
    
        int Complete();
        public IUnitOfWork Create();

    }
}