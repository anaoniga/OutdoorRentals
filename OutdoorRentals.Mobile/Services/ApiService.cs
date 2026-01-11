using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using OutdoorRentals.Mobile.Models;

namespace OutdoorRentals.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private string? _token;

        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:5209")
            };
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, bool authorized)
        {
            var req = new HttpRequestMessage(method, url);

            if (authorized && !string.IsNullOrWhiteSpace(_token))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }

            return req;
        }

        
        public async Task<bool> LoginAsync(string email, string password)
        {
            var req = CreateRequest(HttpMethod.Post, "/api/auth/login", authorized: false);
            req.Content = JsonContent.Create(new { email, password }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return false;

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("token", out var t)) _token = t.GetString();
            else if (doc.RootElement.TryGetProperty("Token", out var t2)) _token = t2.GetString();

            return !string.IsNullOrWhiteSpace(_token);
        }

        public async Task<List<EquipmentCategoryDto>> GetCategoriesAsync()
        {
            var req = CreateRequest(HttpMethod.Get, "/api/EquipmentCategoriesApi", authorized: true);
            var resp = await _http.SendAsync(req);

            if (!resp.IsSuccessStatusCode) return new List<EquipmentCategoryDto>();

            var items = await resp.Content.ReadFromJsonAsync<List<EquipmentCategoryDto>>(JsonOpts);
            return items ?? new List<EquipmentCategoryDto>();
        }




        public async Task<bool> CreateCategoryAsync(string name)
        {
            var req = CreateRequest(HttpMethod.Post, "/api/EquipmentCategoriesApi", authorized: true);
            req.Content = JsonContent.Create(new { name }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(int id, string name)
        {
            var req = CreateRequest(HttpMethod.Put, $"/api/EquipmentCategoriesApi/{id}", authorized: true);
            req.Content = JsonContent.Create(new { id, name }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var req = CreateRequest(HttpMethod.Delete, $"/api/EquipmentCategoriesApi/{id}", authorized: true);
            var resp = await _http.SendAsync(req);

            return resp.IsSuccessStatusCode;
        }
        public async Task<List<EquipmentDto>> GetEquipmentsAsync()
        {
            var req = CreateRequest(HttpMethod.Get, "/api/EquipmentsApi", authorized: true);
            var resp = await _http.SendAsync(req);

            var body = await resp.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Equipments status: {(int)resp.StatusCode} {resp.StatusCode}");
            System.Diagnostics.Debug.WriteLine(body);

            if (!resp.IsSuccessStatusCode)
                return new List<EquipmentDto>();

            try
            {
                var items = JsonSerializer.Deserialize<List<EquipmentDto>>(body, JsonOpts);
                return items ?? new List<EquipmentDto>();
            }
            catch
            {
                return new List<EquipmentDto>();
            }
        }


        public async Task<bool> CreateEquipmentAsync(EquipmentDto dto)
        {
            var req = CreateRequest(HttpMethod.Post, "/api/EquipmentsApi", authorized: true);
            req.Content = JsonContent.Create(new
            {
                name = dto.Name,
                description = dto.Description,
                dailyRate = dto.DailyRate,
                stockTotal = dto.StockTotal,
                stockAvailable = dto.StockAvailable,
                equipmentCategoryId = dto.EquipmentCategoryId
            }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"CreateEquipment Status: {(int)resp.StatusCode} {resp.StatusCode}");
            System.Diagnostics.Debug.WriteLine(body);

            return resp.IsSuccessStatusCode;
        }



        public async Task<bool> UpdateEquipmentAsync(EquipmentDto dto)
        {
            var req = CreateRequest(HttpMethod.Put, $"/api/EquipmentsApi/{dto.Id}", authorized: true);
            req.Content = JsonContent.Create(new
            {
                id = dto.Id,
                name = dto.Name,
                description = dto.Description,
                dailyRate = dto.DailyRate,
                stockTotal = dto.StockTotal,
                stockAvailable = dto.StockAvailable,
                equipmentCategoryId = dto.EquipmentCategoryId
            }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var req = CreateRequest(HttpMethod.Delete, $"/api/EquipmentsApi/{id}", authorized: true);
            var resp = await _http.SendAsync(req);

            return resp.IsSuccessStatusCode;
        }

        
        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            var req = CreateRequest(HttpMethod.Get, "/api/CustomersApi", authorized: true);
            var resp = await _http.SendAsync(req);

            if (!resp.IsSuccessStatusCode) return new List<CustomerDto>();

            var items = await resp.Content.ReadFromJsonAsync<List<CustomerDto>>(JsonOpts);
            return items ?? new List<CustomerDto>();
        }

        
        public async Task<List<RentalDto>> GetRentalsAsync()
        {
            var req = CreateRequest(HttpMethod.Get, "/api/RentalsApi", authorized: true);
            var resp = await _http.SendAsync(req);

            if (!resp.IsSuccessStatusCode) return new List<RentalDto>();

            var items = await resp.Content.ReadFromJsonAsync<List<RentalDto>>(JsonOpts);
            return items ?? new List<RentalDto>();
        }

        public async Task<bool> CreateRentalAsync(RentalDto dto)
        {
            var req = CreateRequest(HttpMethod.Post, "/api/RentalsApi", authorized: true);
            req.Content = JsonContent.Create(new
            {
                customerId = dto.CustomerId,
                startDate = dto.StartDate,
                endDate = dto.EndDate
            }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRentalAsync(RentalDto dto)
        {
            var req = CreateRequest(HttpMethod.Put, $"/api/RentalsApi/{dto.Id}", authorized: true);
            req.Content = JsonContent.Create(new
            {
                id = dto.Id,
                customerId = dto.CustomerId,
                startDate = dto.StartDate,
                endDate = dto.EndDate
            }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRentalAsync(int id)
        {
            var req = CreateRequest(HttpMethod.Delete, $"/api/RentalsApi/{id}", authorized: true);
            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        
        public async Task<List<RentalItemDto>> GetRentalItemsAsync(int rentalId)
        {
            var req = CreateRequest(HttpMethod.Get, $"/api/RentalItemsApi/byRental/{rentalId}", authorized: true);
            var resp = await _http.SendAsync(req);

            if (!resp.IsSuccessStatusCode) return new List<RentalItemDto>();

            var items = await resp.Content.ReadFromJsonAsync<List<RentalItemDto>>(JsonOpts);
            return items ?? new List<RentalItemDto>();
        }

        public async Task<bool> CreateRentalItemAsync(RentalItemDto dto)
        {
            var req = CreateRequest(HttpMethod.Post, "/api/RentalItemsApi", authorized: true);
            req.Content = JsonContent.Create(new
            {
                rentalId = dto.RentalId,
                equipmentId = dto.EquipmentId,
                quantity = dto.Quantity,
                dailyRate = dto.DailyRate
            }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRentalItemAsync(RentalItemDto dto)
        {
            var req = CreateRequest(HttpMethod.Put, $"/api/RentalItemsApi/{dto.Id}", authorized: true);
            req.Content = JsonContent.Create(new
            {
                id = dto.Id,
                rentalId = dto.RentalId,
                equipmentId = dto.EquipmentId,
                quantity = dto.Quantity,
                dailyRate = dto.DailyRate
            }, options: JsonOpts);

            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRentalItemAsync(int id)
        {
            var req = CreateRequest(HttpMethod.Delete, $"/api/RentalItemsApi/{id}", authorized: true);
            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }


    }
}
