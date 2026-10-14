using Microsoft.Maui.Controls;
using Microcharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartRatingCafe.Models;
using ChartRatingCafe.Services;
using System.Diagnostics;

namespace ChartRatingCafe.Pages
{
    public partial class ChartRatingPageOne : ContentPage
    {
        private readonly ApiService _apiService;
        private List<Cafe> _cafes;
        private List<Rating> _currentCafeRatings;

        public ChartRatingPageOne()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCafesAsync();
        }

        private async Task LoadCafesAsync()
        {
            var response = await _apiService.GetCafesAsync();
            if (response.Success)
            {
                _cafes = response.Data;
                CafePicker.ItemsSource = _cafes.Select(c => c.Name).ToList();
            }
            else
            {
                await DisplayAlert("Error", "Failed to load cafes", "OK");
            }
        }

        private async void OnCafeSelected(object sender, EventArgs e)
        {
            if (CafePicker.SelectedIndex == -1)
                return;

            string selectedCafeName = CafePicker.SelectedItem.ToString();
            CafeNameLabel.Text = selectedCafeName;

            Debug.WriteLine($"Cafe selected: {selectedCafeName}");

            await LoadCafeChartAndRatingsAsync(selectedCafeName); // Load chart data and ratings
        }

        private async Task LoadCafeChartAndRatingsAsync(string cafeName)
        {
            try
            {
                var response = await _apiService.GetRatingsAsync(cafeName);
                if (response.Success)
                {
                    var ratings = response.Data;
                    _currentCafeRatings = ratings.Where(r => r.CafeName == cafeName).ToList();

                    if (_currentCafeRatings.Any())
                    {
                        var averageCleanliness = _currentCafeRatings.Average(r => r.Cleanliness);
                        var averagePrice = _currentCafeRatings.Average(r => r.Price);
                        var averageService = _currentCafeRatings.Average(r => r.Service);

                        float cleanliness = (float)averageCleanliness;
                        float price = (float)averagePrice;
                        float service = (float)averageService;


                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CafeChartView.Chart = CreateCafeChart(cleanliness, price, service);
                            CafeChartView.InvalidateSurface(); // Refresh the chart
                        });
                    }
                    else
                    {
                        CafeChartView.Chart = null;
                        CafeChartView.InvalidateSurface();
                        await DisplayAlert("No Data", "No ratings available for this cafe", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load ratings", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                await DisplayAlert("Error", "An unexpected error occurred.", "OK");
            }
        }

        private DonutChart CreateCafeChart(double cleanliness, double price, double service)
        {
            var entries = new List<ChartEntry>
            {
                new ChartEntry((float)cleanliness)
                {
                    Label = "Kebersihan",
                    ValueLabel = cleanliness.ToString("0.0"),
                    Color = SKColor.Parse("#3E4A89"),
                    ValueLabelColor = SKColors.Black
                },
                new ChartEntry((float)price)
                {
                    Label = "Harga",
                    ValueLabel = price.ToString("0.0"),
                    Color = SKColor.Parse("#6BAED6"),
                    ValueLabelColor = SKColors.Black
                },
                new ChartEntry((float)service)
                {
                    Label = "Perkhidmatan",
                    ValueLabel = service.ToString("0.0"),
                    Color = SKColor.Parse("#AEC7E8"),
                    ValueLabelColor = SKColors.Black
                }
            };

            return new DonutChart
            {
                Entries = entries,
                LabelTextSize = 30,
                BackgroundColor = SKColors.Transparent,
                HoleRadius = 0.3f,
                IsAnimated = true
            };
        }

        // Button click handler for showing comments
        // Button click handler for showing comments
        private async void OnShowCommentsButtonClicked(object sender, EventArgs e)
        {
            if (_currentCafeRatings == null || !_currentCafeRatings.Any())
            {
                await DisplayAlert("No Comments", "No comments available for this cafe", "OK");
                return;
            }

            // Get all the comments for the selected cafe
            var comments = _currentCafeRatings.Select(r => r.Comment).Where(c => !string.IsNullOrEmpty(c)).ToList();

            if (comments.Any())
            {
                // Create a list of numbered comments
                var numberedComments = comments.Select((comment, index) => $"{index + 1}. {comment}").ToList();

                // Show numbered comments in the popup
                CommentsLabel.Text = string.Join("\n\n", numberedComments);
                CommentsPopup.IsVisible = true; // Show the popup
            }
            else
            {
                await DisplayAlert("No Comments", "No comments available for this cafe", "OK");
            }
        }


        // Close the popup
        private void OnClosePopupButtonClicked(object sender, EventArgs e)
        {
            CommentsPopup.IsVisible = false; // Hide the popup
        }
    }
}
