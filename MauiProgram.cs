using KartyaTar2.Services;
using KartyaTar2.ViewModels;
using KartyaTar2.Views;
using Microsoft.Extensions.Logging;

namespace KartyaTar2;

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
            });

        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<PhotoService>();

        builder.Services.AddTransient<CardListViewModel>();
        builder.Services.AddTransient<CardEditViewModel>();
        builder.Services.AddTransient<StatsViewModel>();

        builder.Services.AddTransient<CardListPage>();
        builder.Services.AddTransient<CardEditPage>();
        builder.Services.AddTransient<StatsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
