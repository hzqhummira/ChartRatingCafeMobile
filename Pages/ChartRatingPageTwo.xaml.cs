using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartRatingCafe.Models;
using ChartRatingCafe.Services;
using System.Diagnostics;
using Microcharts;
using SkiaSharp;

namespace ChartRatingCafe.Pages
{
    public partial class ChartRatingPageTwo : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly Dictionary<string, List<Rating>> _ratingsCache;

        public ChartRatingPageTwo()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _ratingsCache = new Dictionary<string, List<Rating>>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCafeRatingsAsync();
        }

        private async Task LoadCafeRatingsAsync()
        {
            try
            {
                var response = await _apiService.GetCafesAsync();
                if (response.Success)
                {
                    var cafes = response.Data;

                    if (cafes.Any())
                    {
                        var allRatings = new List<Rating>();
                        foreach (var cafe in cafes)
                        {
                            if (!_ratingsCache.ContainsKey(cafe.Name))
                            {
                                var ratingResponse = await _apiService.GetRatingsAsync(cafe.Name);
                                if (ratingResponse.Success && ratingResponse.Data != null)
                                {
                                    _ratingsCache[cafe.Name] = ratingResponse.Data;
                                }
                            }

                            var cafeRatings = _ratingsCache[cafe.Name];
                            foreach (var rating in cafeRatings)
                            {
                                if (!allRatings.Any(r => r.Id == rating.Id))
                                {
                                    allRatings.Add(rating);
                                }
                            }
                        }

                        // Debug output to verify ratings
                        foreach (var rating in allRatings)
                        {
                            Debug.WriteLine($"UserName: {rating.UserName}, Comment: {rating.Comment}");
                        }

                        if (allRatings.Any())
                        {
                            var chart = CreateBarChart(allRatings);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                CafeBarChartView.Chart = chart;
                            });
                        }
                        else
                        {
                            await DisplayAlert("No Data", "No ratings available for the selected cafes", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("No Cafes", "No cafes found", "OK");
                    }
                }
                else
                {
                    Debug.WriteLine($"Error loading cafes: {response.ErrorMessage}");
                    await DisplayAlert("Error", "Failed to load cafes", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                await DisplayAlert("Error", "An unexpected error occurred while loading cafe ratings.", "OK");
            }
        }

        private BarChart CreateBarChart(List<Rating> ratings)
        {
            Debug.WriteLine($"Creating bar chart with {ratings.Count} ratings");

            // Define an array of colors for the bars
            var colors = new[] {
                SKColor.Parse("#1E3A8A"),  // Dark Blue
                SKColor.Parse("#60A5FA"),  // Pastel Blue
                SKColor.Parse("#A3A3A3"),  // Grey
                SKColor.Parse("#9CA3AF"),  // Lighter Grey
                SKColor.Parse("#3B82F6"),  // Another shade of blue
            };

            var entries = new List<ChartEntry>();

            // Aggregate data: Create bars with average ratings for each cafe and sort them alphabetically
            var groupedRatings = ratings.GroupBy(r => r.CafeName)
                                        .OrderBy(g => g.Key); // Sort cafes by name (A, B, C, etc.)

            int colorIndex = 0;

            foreach (var group in groupedRatings)
            {
                var cafeName = group.Key;
                var averageRating = group.Average(r => r.OverallRating);

                // Ensure correct bar height by using the average rating
                entries.Add(new ChartEntry((float)averageRating)
                {
                    Label = cafeName, // X-axis label
                    ValueLabel = averageRating.ToString("0.0"), // Y-axis value (rounded to one decimal)
                    Color = colors[colorIndex % colors.Length]  // Cycle through the colors
                });

                colorIndex++; // Move to the next color
            }

            // Create a BarChart with the aggregated data
            return new BarChart
            {
                Entries = entries,
                LabelTextSize = 35,
                ValueLabelTextSize = 30,
                BackgroundColor = SKColors.Transparent,
                MaxValue = 5,
                MinValue = 0,
                BarAreaAlpha = 50,
                AnimationDuration = TimeSpan.FromMilliseconds(3000), // Control animation speed
                IsAnimated = false
            };
        }

        private async void OnViewCommentsButtonClicked(object sender, EventArgs e)
        {
            // Fetch all ratings
            var allRatings = new List<Rating>();
            foreach (var ratings in _ratingsCache.Values)
            {
                allRatings.AddRange(ratings);
            }

            // Ensure unique comments
            var uniqueComments = allRatings
                .GroupBy(r => new { r.Comment, r.CafeName })
                .Select(g => g.First())
                .ToList();

            // Navigate to CommentsPage with uniqueComments
            await Navigation.PushModalAsync(new CommentsPage(uniqueComments));
        }

    }
}
