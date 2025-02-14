using System.Threading.Tasks;

namespace FitRate.Core.Abstractions
{
    public interface ITokenStorage
    {
        Task SetTokenAsync(string key, string token);
        Task<string?> GetTokenAsync(string key);
    }
}
