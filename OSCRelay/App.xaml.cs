using System;
using System.Windows;
using Global.Data.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSCRelay.Services;
using OSCRelay.Services.Implementations;
using IServiceProvider = OSCRelay.Services.IServiceProvider;

namespace OSCRelay
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<ISettingsManagerService, SettingsManagerService>();
                services.AddSingleton<IAvatarInfoService, AvatarInfoService>();
                services.AddSingleton<IOSCService, OSCService>();
                services.AddSingleton<IRelayService, RelayService>();
                services.AddSingleton<ICustomLoggerService, CustomLoggerService>();
                services.AddSingleton<IServiceProvider, SocketIOService>();
            }).Build();
            // ServiceCollection services = new ServiceCollection();
            // ConfigureServices(services);
            // serviceProvider = services.BuildServiceProvider();

            // OSCService = new OSCService(9001);
            // AvatarInfoService = new AvatarInfoService();
            // UserSettings = new UserSettings("usr_7806ce34-692c-48f4-af11-f6bd2daa05fe", "", "");
        }
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            
            MainWindow startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();
            
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            MainWindow startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            await startupForm.SaveSettings();
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}