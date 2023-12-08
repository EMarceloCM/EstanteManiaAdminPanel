using Blazored.LocalStorage;
using EstanteManiaWebAssembly.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;

namespace EstanteManiaWebAssembly.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _http;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(IHttpClientFactory http, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _http = http;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            try
            {
                var httpClient = _http.CreateClient("EstanteManiaApi");
                var loginAsJson = JsonSerializer.Serialize(loginModel);
                var requestContent = new StringContent(loginAsJson, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/users/Login", requestContent);
                var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

                if(!response.IsSuccessStatusCode)
                {
                    return loginResult;
                }

                await _localStorage.SetItemAsync("authToken", loginResult.Token);
                await _localStorage.SetItemAsync("tokenExpiration", loginResult.Expiration);

                ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(loginModel.Email!);

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", loginResult.Token!);
                return loginResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Logout()
        {
            var httpClient = _http.CreateClient("EstanteManiaApi");
            await _localStorage.RemoveItemAsync("authToken");

            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
