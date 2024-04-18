using ExcelFaceplateDataExtractorApp.StartupHelpers;
using ExcelFaceplateDataExtractorApp.View;
using ExcelFaceplateDataExtractorApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Windows;

namespace ExcelFaceplateDataExtractorApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        //Log.Logger = new LoggerConfiguration()
        //    .WriteTo.Console()
        //    .CreateLogger();

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                //services.AddSingleton<IWalletManager<Wallet>, WalletManager>();
                //services.AddTransient<ICurrencyConverter, BasicCurrencyConverter>();

                services.AddSingleton<MainWindow>();
                //services.AddWpfComponentFactory<ChildWindow>(); // Child window is same as MainWindow, could be removed

                services.AddTransient<SettingsPageViewModel>();
                services.AddWpfComponentFactory<SettingsPage>();

                services.AddTransient<ColumnNumberSettingViewModel>();
                services.AddWpfComponentFactory<ColumnNumberSettingUserControl>();

                //services.AddTransient<>

                //services.AddTransient<DenominationRowViewModel>();
                //services.AddWpfComponentFactory<DenominationRow>(); // Factory Interf
            })
            .Build();
        Debug.WriteLine("Registered object graph");
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
