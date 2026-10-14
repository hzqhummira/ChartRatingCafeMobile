using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChartRatingCafe.Models;
using System.Diagnostics;
using System.Text.Json;

namespace ChartRatingCafe.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(Constants.ApiBaseUrl) };
        }

        public async Task<ApiResponse<List<Cafe>>> GetCafesAsync()
        {
            try
            {
                var cafes = await _httpClient.GetFromJsonAsync<List<Cafe>>("api/kafe");
                Debug.WriteLine("Cafes fetched successfully:");
                Debug.WriteLine(JsonSerializer.Serialize(cafes)); // Log the fetched cafes
                return ApiResponse<List<Cafe>>.SuccessResponse(cafes);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return ApiResponse<List<Cafe>>.ErrorResponse(e.Message);
            }
        }

        public async Task<ApiResponse<List<Rating>>> GetRatingsAsync(string cafeName)
        {
            try
            {
                var ratings = await _httpClient.GetFromJsonAsync<List<Rating>>($"api/penilaian?cafeName={Uri.EscapeDataString(cafeName)}");

                // Add this debug log to inspect the data after deserialization
                Debug.WriteLine("Deserialized Ratings:");
                foreach (var rating in ratings)
                {
                    Debug.WriteLine($"Id: {rating.Id}, CafeName: {rating.CafeName}, Cleanliness: {rating.Cleanliness}, Price: {rating.Price}, Service: {rating.Service}, OverallRating: {rating.OverallRating}, Comment: {rating.Comment}");
                }

                return ApiResponse<List<Rating>>.SuccessResponse(ratings);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return ApiResponse<List<Rating>>.ErrorResponse(e.Message);
            }
        }

    }
}
