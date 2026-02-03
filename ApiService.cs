using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlatformWellSync
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://test-demo.aemenersol.com";
        private string? _bearerToken;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<bool> LoginAsync()
{
    try
    {
        var loginData = new
        {
            username = "user@aemenersol.com",
            password = "Test@123"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(loginData),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("/api/Account/Login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            
            try
            {
                var jsonResponse = JObject.Parse(responseString);
                _bearerToken = jsonResponse["token"]?.ToString();
            }
            catch
            {
                _bearerToken = responseString.Trim('"');
            }
            
            if (!string.IsNullOrEmpty(_bearerToken))
            {
                Console.WriteLine("✓ Login successful!");
                Console.WriteLine($"Token: {_bearerToken.Substring(0, Math.Min(20, _bearerToken.Length))}...");
                return true;
            }
            else
            {
                Console.WriteLine("✗ Login failed: No token received");
                return false;
            }
        }
        
        Console.WriteLine($"✗ Login failed with status code: {response.StatusCode}");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Login error: {ex.Message}");
        return false;
    }
}

        public async Task<JArray> GetPlatformWellActualAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _bearerToken);

                var response = await _httpClient.GetAsync("/api/PlatformWell/GetPlatformWellActual");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseString);
                    
                    Console.WriteLine($"✓ Retrieved {data.Count} platforms");
                    return data;
                }
                
                Console.WriteLine("✗ Failed to retrieve data");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error retrieving data: {ex.Message}");
                return null;
            }
        }

        public async Task<JArray> GetPlatformWellDummyAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _bearerToken);

                var response = await _httpClient.GetAsync("/api/PlatformWell/GetPlatformWellDummy");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseString);
                    
                    Console.WriteLine($"✓ Retrieved {data.Count} platforms (Dummy)");
                    return data;
                }
                
                Console.WriteLine("✗ Failed to retrieve dummy data");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error retrieving dummy data: {ex.Message}");
                return null;
            }
        }
    }
}