using FitRate.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRate.Mobile.Services
{
    public class MauiTokenStorage : ITokenStorage
    {
        public Task SetTokenAsync(string key, string token) =>
            SecureStorage.SetAsync(key, token);

        public Task<string?> GetTokenAsync(string key) =>
            SecureStorage.GetAsync(key);
    }

}
