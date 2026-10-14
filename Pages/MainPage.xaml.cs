using Microsoft.Maui.Controls;

namespace ChartRatingCafe.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnViewBarChartClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChartRatingPageOne());
        }

        private async void OnNavigateToPageTwoClicked(object sender, EventArgs e)
        {
            // Navigate to ChartRatingPageTwo
            await Navigation.PushAsync(new ChartRatingPageTwo());
        }
    }
}
