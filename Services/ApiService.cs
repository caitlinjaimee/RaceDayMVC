using System.Net.Http.Json;
using RaceDayMVC.Models;


namespace RaceDayMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("RaceDayAPI");
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var events = await _httpClient.GetFromJsonAsync<List<EventViewModel>>(
                "api/Events");

            return events ?? new List<EventViewModel>();
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>(
                "api/Categories");

            return categories ?? new List<CategoryViewModel>();
        }

        public async Task<LoginResponseViewModel?> LoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();
        }

        public async Task<List<EnrollmentViewModel>> GetEnrollmentsAsync()
        {
            var enrollments = await _httpClient.GetFromJsonAsync<List<EnrollmentViewModel>>(
                "api/Enrollments");

            return enrollments ?? new List<EnrollmentViewModel>();
        }

        public async Task<List<ResultViewModel>> GetResultsAsync()
        {
            var results = await _httpClient.GetFromJsonAsync<List<ResultViewModel>>(
                "api/Results");

            return results ?? new List<ResultViewModel>();
        }
    }
}

