
namespace asp_net_web_api.API.Respository{
    public interface IUnitOfWork : IDisposable{

        // IItemRepository ItemsRepository { get; }
    
        int Complete();

    }
}