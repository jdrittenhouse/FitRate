using Microsoft.VisualStudio.TestTools.UnitTesting;
using FitRate.Core.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Text;
using System.Text.Json;
using FitRate.Core.Models;
using FitRate.Core.Services;
using FitRate.Tests.Fakes;

namespace FitRate.Tests
{
    [TestClass]
    public class FirebaseAuthServiceTests
    {
        // A fake HttpMessageHandler to simulate HTTP responses.
        public class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _fakeResponse;
            public FakeHttpMessageHandler(HttpResponseMessage fakeResponse)
            {
                _fakeResponse = fakeResponse;
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_fakeResponse);
            }
        }

        [TestMethod]
        public async Task SignUpWithEmailAsync_Returns_ValidResponse()
        {
            // Arrange: Create a fake successful JSON response.
            var expectedResponse = new FirebaseAuthResponse
            {
                IdToken = "fake_id_token",
                Email = "test@example.com",
                RefreshToken = "fake_refresh_token",
                ExpiresIn = "3600",
                LocalId = "fake_local_id"
            };
            string jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            var httpClient = new HttpClient(fakeHandler);
            var fakeTokenStorage = new FakeTokenStorage();
            // Inject the custom HttpClient into the service.
            var authService = new FirebaseAuthService(httpClient,fakeTokenStorage);

            // Act: Call the SignUpWithEmailAsync method.
            var result = await authService.SignUpWithEmailAsync("test@example.com", "password123");

            // Assert: Verify that the returned response matches our fake response.
            Assert.IsNotNull(result);
            Assert.AreEqual("fake_id_token", result.IdToken);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.AreEqual("fake_refresh_token", result.RefreshToken);
            Assert.AreEqual("fake_local_id", result.LocalId);
        }

        [TestMethod]
        public async Task SignInWithEmailAsync_Returns_ValidResponse()
        {
            // Arrange: Create a fake successful JSON response for sign-in.
            var expectedResponse = new FirebaseAuthResponse
            {
                IdToken = "fake_signin_id_token",
                Email = "user@example.com",
                RefreshToken = "fake_signin_refresh_token",
                ExpiresIn = "3600",
                LocalId = "fake_signin_local_id"
            };
            string jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            var httpClient = new HttpClient(fakeHandler);
            var fakeTokenStorage = new FakeTokenStorage();
            var authService = new FirebaseAuthService(httpClient,fakeTokenStorage);

            // Act: Call the SignInWithEmailAsync method.
            var result = await authService.SignInWithEmailAsync("user@example.com", "password456");

            // Assert: Verify the result.
            Assert.IsNotNull(result);
            Assert.AreEqual("fake_signin_id_token", result.IdToken);
            Assert.AreEqual("user@example.com", result.Email);
            Assert.AreEqual("fake_signin_refresh_token", result.RefreshToken);
            Assert.AreEqual("fake_signin_local_id", result.LocalId);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task SignUpWithEmailAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange: Create a fake error response.
            var errorContent = "{\"error\":{\"message\":\"EMAIL_EXISTS\"}}";
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(errorContent, Encoding.UTF8, "application/json")
            };

            var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            var httpClient = new HttpClient(fakeHandler);
            var fakeTokenStorage = new FakeTokenStorage();
            var authService = new FirebaseAuthService(httpClient,fakeTokenStorage);

            // Act: This call should throw an exception due to the error response.
            await authService.SignUpWithEmailAsync("existing@example.com", "password123");
        }
    }
}
