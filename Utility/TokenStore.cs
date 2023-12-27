using System.Collections.Concurrent;

namespace asp_net_web_api.API.Utility
{
    public class TokenStoreCache
    {
        public ConcurrentDictionary<string, string>? Store = new();
    }
}