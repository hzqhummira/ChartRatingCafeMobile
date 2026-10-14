using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microcharts.Maui;

namespace ChartRatingCafe
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMicrocharts(); // Add this line

            return builder.Build();
        }
    }
}
