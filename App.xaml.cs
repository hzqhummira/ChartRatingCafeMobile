using Microsoft.Maui.Controls;
using ChartRatingCafe.Pages;

namespace ChartRatingCafe
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
