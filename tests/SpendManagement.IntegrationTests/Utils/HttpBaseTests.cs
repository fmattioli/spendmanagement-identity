using Flurl;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

using SpendManagement.IntegrationTests.Config;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace SpendManagement.IntegrationTests.Utils
{
    public class HttpBaseTests
    {
        private readonly HttpClient _httpClient;
        public const string APIVersion = "api/v1";

        public HttpBaseTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        protected async Task<(HttpStatusCode StatusCode, string Content)> PostAsync<T>(string resource, T body, bool generateToken = false) where T: class
        {
            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            if(generateToken)
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GenerateJWToken());

            var url = APIVersion + resource;
            using var response = await _httpClient.PostAsync(url, httpContent);
            return (response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpStatusCode StatusCode, string Content)> GetAsync(string resource, string email)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GenerateJWToken());

            var emailQueryParam = "email=" + email;

            var url = APIVersion
                .AppendPathSegment(resource)
                .AppendQueryParam(emailQueryParam);

            using var response = await _httpClient.GetAsync(url);

            return (response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        private static string GenerateJWToken()
        {
            var settings = TestSettings.JwtOptions;
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings!.SecurityKey ?? throw new Exception("Invalid token security key")));

            var credenciais = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                settings.Issuer,
                settings.Audience,
                expires: DateTime.UtcNow.AddMinutes(settings.AccessTokenExpiration),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
