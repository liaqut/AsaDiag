using System.Text.Json;
using System.Net.Http.Headers;
using static Microsoft.AspNetCore.Components.NavigationManager;

namespace DigiEquipSys.Services
{
    public class ZohoTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public ZohoTokenService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public string _clientId => _config["Zoho:ClientId"];
        public string _clientSecret => _config["Zoho:ClientSecret"];
        public string _redirectUri => "http://localhost:7110/zohoauthcallback"; // must match Zoho Console

        private string? _accessToken;
        private string? _refreshToken;
        private DateTime _expiryTime;

        public async Task<bool> ExchangeCodeForTokensAsync(string code)
        {
            using var httpClient = new HttpClient();

            var values = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "redirect_uri", _redirectUri },
            { "code", code }
        };

            var response = await httpClient.PostAsync("https://accounts.zoho.in/oauth/v2/token", new FormUrlEncodedContent(values));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                _accessToken = tokenData.ContainsKey("access_token") ? tokenData["access_token"]?.ToString() : null;
                _refreshToken = tokenData.ContainsKey("refresh_token") ? tokenData["refresh_token"]?.ToString() : null;
                int expiresIn = 3600; // default
                if (tokenData.ContainsKey("expires_in") && tokenData["expires_in"] != null)
                {
                    int.TryParse(tokenData["expires_in"].ToString(), out expiresIn);
                }

                _expiryTime = DateTime.UtcNow.AddSeconds(expiresIn);

                Console.WriteLine($"AccessToken: {_accessToken}");
                Console.WriteLine($"RefreshToken: {_refreshToken ?? "Not returned"}");
                return true;
            }
            return false;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            if (_accessToken != null && DateTime.UtcNow < _expiryTime)
                return _accessToken;

            if (string.IsNullOrEmpty(_refreshToken))
                throw new Exception("No refresh token available. Please authenticate again.");

            using var httpClient = new HttpClient();

            var values = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "refresh_token", _refreshToken }
        };

            var response = await httpClient.PostAsync("https://accounts.zoho.in/oauth/v2/token", new FormUrlEncodedContent(values));
            var json = await response.Content.ReadAsStringAsync();

            var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            _accessToken = tokenData?["access_token"]?.ToString();
            _expiryTime = DateTime.UtcNow.AddSeconds(Convert.ToInt32(tokenData?["expires_in"] ?? "3600"));

            return _accessToken;
        }

        public string GetAuthUrl(string retUrl)
        {
            string returnUrl = retUrl?.ToString() ?? "/";
            string scope = "ZohoBooks.fullaccess.all";
            return $"https://accounts.zoho.in/oauth/v2/auth?scope={scope}&client_id={_clientId}&response_type=code&access_type=offline&redirect_uri={_redirectUri}&prompt=consent&state={Uri.EscapeDataString(returnUrl)}";
        }
    }
}



//_accessToken = tokenData?["access_token"]?.ToString();
//_refreshToken = tokenData?["refresh_token"]?.ToString();
//_expiryTime = DateTime.UtcNow.AddSeconds(Convert.ToInt32(tokenData?["expires_in"] ?? "3600"));
//Console.WriteLine($"AccessToken: {_accessToken}");
//Console.WriteLine($"RefreshToken: {_refreshToken}");