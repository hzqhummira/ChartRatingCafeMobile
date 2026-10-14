using Microsoft.Maui.Controls;
using System.Collections.Generic;
using ChartRatingCafe.Models;

namespace ChartRatingCafe.Pages
{
    public partial class CommentsPage : ContentPage
    {
        public CommentsPage(List<Rating> ratings)
        {
            InitializeComponent();
            CommentsListView.ItemsSource = ratings;
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            // Close the popup
            await Navigation.PopModalAsync();

        }

    }
}
