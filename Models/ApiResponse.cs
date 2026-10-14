using System.Text.Json.Serialization;

namespace ChartRatingCafe.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        // If your API does not return a "Data" field, you can remove or rename this
        public T Data { get; set; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T> { Success = true, Data = data };
        }

        public static ApiResponse<T> ErrorResponse(string errorMessage)
        {
            return new ApiResponse<T> { Success = false, ErrorMessage = errorMessage };
        }
    }
}
