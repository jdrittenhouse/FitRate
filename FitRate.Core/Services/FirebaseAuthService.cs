using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using FitRate.Core.Abstractions;
using FitRate.Core.Models;
using FitRate.Core.Models.UserModels;

namespace FitRate.Core.Services
{
    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private const string FirebaseApiKey = "AIzaSyCS8FD-1nDTPZmHlYXiomw15kbc8Nm492k";
        
        public FirebaseAuthService(HttpClient httpClient, ITokenStorage tokenStorage)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
        }

        /// <summary>
        /// Signs up a new user using Firebase email/password authentication.
        /// </summary>
        public async Task<FirebaseAuthResponse> SignUpWithEmailAsync(string email, string password)
        {
            var signUpUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseApiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(signUpUrl, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var authResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                // Securely store the tokens for later use.
                await _tokenStorage.SetTokenAsync("firebase_idToken", authResponse.IdToken);
                await _tokenStorage.SetTokenAsync("firebase_refreshToken", authResponse.RefreshToken);
                return authResponse;
            }
            else
            {
                throw new Exception($"Error during sign-up: {responseContent}");
            }
        }

        /// <summary>
        /// Signs in an existing user using Firebase email/password authentication.
        /// </summary>
        public async Task<FirebaseAuthResponse> SignInWithEmailAsync(string email, string password)
        {
            var signInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseApiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(signInUrl, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var authResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                // Securely store the tokens for later use.
                await _tokenStorage.SetTokenAsync("firebase_idToken", authResponse.IdToken);
                await _tokenStorage.SetTokenAsync("firebase_refreshToken", authResponse.RefreshToken);
                return authResponse;
            }
            else
            {
                throw new Exception($"Error during sign-in: {responseContent}");
            }
        }
        
    }
}
