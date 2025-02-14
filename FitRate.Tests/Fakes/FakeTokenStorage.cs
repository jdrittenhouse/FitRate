using System.Collections.Concurrent;
using System.Threading.Tasks;
using FitRate.Core.Abstractions;

namespace FitRate.Tests.Fakes
{
    public class FakeTokenStorage : ITokenStorage
    {
        // A thread-safe dictionary to simulate token storage.
        private readonly ConcurrentDictionary<string, string> _storage = new();

        public Task SetTokenAsync(string key, string token)
        {
            _storage[key] = token;
            return Task.CompletedTask;
        }

        public Task<string?> GetTokenAsync(string key)
        {
            _storage.TryGetValue(key, out string token);
            return Task.FromResult(token);
        }
    }
}
