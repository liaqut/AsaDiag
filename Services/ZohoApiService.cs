using System.Net.Http.Headers;
using System.Text.Json;

namespace DigiEquipSys.Services
{
    public class ZohoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ZohoApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public string ClientId => _config["Zoho:ClientId"];
        public string ClientSecret => _config["Zoho:ClientSecret"];
        public string RedirectUri => _config["Zoho:RedirectUri"];

        public async Task<string> GetOrganizationIdAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);
            var response = await _httpClient.GetAsync("https://books.zoho.in/api/v3/organizations");
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("organizations")[0].GetProperty("organization_id").GetString();
        }

        public async Task<string> CreatePurchaseOrderAsync(string accessToken, string orgId, object poPayload)
        {
            var json = JsonSerializer.Serialize(poPayload, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine("===== JSON Payload Being Sent to Zoho =====");
            Console.WriteLine(json);
            Console.WriteLine("==========================================");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);
            var response = await _httpClient.PostAsJsonAsync($"https://www.zohoapis.in/books/v3/purchaseorders?organization_id={orgId}", poPayload);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AppendSupplierToZoho(string accessToken, string orgId, object myVendor)
        {
            var json = JsonSerializer.Serialize(myVendor, new JsonSerializerOptions { WriteIndented = true });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);
            var response = await _httpClient.PostAsJsonAsync($"https://www.zohoapis.in/books/v3/contacts?organization_id={orgId}", myVendor);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AppendCustomerToZoho(string accessToken, string orgId, object myCustomer)
        {
            var json = JsonSerializer.Serialize(myCustomer, new JsonSerializerOptions { WriteIndented = true });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);
            var response = await _httpClient.PostAsJsonAsync($"https://www.zohoapis.in/books/v3/contacts?organization_id={orgId}", myCustomer);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AppendItemToZoho(string accessToken, string orgId, object myItemData)
        {
            var json = JsonSerializer.Serialize(myItemData, new JsonSerializerOptions { WriteIndented = true });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);
            var response = await _httpClient.PostAsJsonAsync($"https://www.zohoapis.in/books/v3/items?organization_id={orgId}", myItemData);
            return await response.Content.ReadAsStringAsync();
        }

    }
}



//public async Task<string> GetVendorsAsync(string accessToken, string orgId)
//{
//    try
//    {  https://books.zoho.in/api/v3/purchaseorders
//       
//        _httpClient.DefaultRequestHeaders.Authorization =
//            new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);

//        var response = await _httpClient.GetAsync(
//            $"https://www.zohoapis.in/books/v3/vendors?organization_id={orgId}");

//        return await response.Content.ReadAsStringAsync();
//    }
//    catch (Exception ex)
//    {
//        // Log the exception (you can use any logging framework you prefer)
//        Console.WriteLine($"Error fetching vendors: {ex.Message}");
//        throw; // Re-throw the exception after logging it
//    }
//}


//public class ZohoTokenResponse
//{
//    public string access_token { get; set; }
//    public string refresh_token { get; set; }
//    public string expires_in { get; set; }
//    public string token_type { get; set; }
//}


//public async Task<ZohoTokenResponse> ExchangeCodeForTokenAsync(string code)
//{
//    var content = new FormUrlEncodedContent(new[]
//    {
//    new KeyValuePair<string, string>("grant_type", "authorization_code"),
//    new KeyValuePair<string, string>("client_id", ClientId),
//    new KeyValuePair<string, string>("client_secret", ClientSecret),
//    new KeyValuePair<string, string>("redirect_uri", RedirectUri),
//    new KeyValuePair<string, string>("code", code)
//});

//    var response = await _httpClient.PostAsync("https://accounts.zoho.in/oauth/v2/token", content);
//    response.EnsureSuccessStatusCode();

//    var result = await response.Content.ReadFromJsonAsync<ZohoTokenResponse>();
//    return result;
//}

//public async Task<string> RefreshTokenAsync(string refreshToken)
//{
//    var content = new FormUrlEncodedContent(new[]
//    {
//    new KeyValuePair<string, string>("grant_type", "refresh_token"),
//    new KeyValuePair<string, string>("client_id", ClientId),
//    new KeyValuePair<string, string>("client_secret", ClientSecret),
//    new KeyValuePair<string, string>("refresh_token", refreshToken)
//});

//    var response = await _httpClient.PostAsync("https://accounts.zoho.in/oauth/v2/token", content);
//    var result = await response.Content.ReadFromJsonAsync<ZohoTokenResponse>();
//    return result.access_token;
//}

//public string GetAuthUrl()
//{
//    string scope = "ZohoBooks.fullaccess.all";
//    return $"https://accounts.zoho.in/oauth/v2/auth?scope={scope}&client_id={ClientId}&response_type=code&access_type=offline&redirect_uri={RedirectUri}";
//}