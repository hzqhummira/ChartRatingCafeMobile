using System.Text.Json.Serialization; // Ensure you have this using directive if needed

namespace ChartRatingCafe.Models
{
    public class Rating
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("CafeName")]
        public string CafeName { get; set; }

        [JsonPropertyName("Kebersihan")]
        public int Cleanliness { get; set; }

        [JsonPropertyName("Harga")]
        public int Price { get; set; }

        [JsonPropertyName("Perkhidmatan")]
        public int Service { get; set; }

        [JsonPropertyName("OverallRating")]
        public double OverallRating { get; set; }

        [JsonPropertyName("Comment")] // New property for JSON serialization
        public string Comment { get; set; } // New property

        [JsonPropertyName("UserName")] // New property for JSON serialization
        public string UserName { get; set; } // New property
    }
}
