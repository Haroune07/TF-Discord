using System.Windows;
using Frontend.Services;
using Frontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend
{
    public partial class App : Application
    {
        public static ServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Services.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ApiService>();
            services.AddSingleton<ChatService>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<SearchUserViewModel>();
            services.AddTransient<CreateServerViewModel>();
            services.AddSingleton<ChannelListViewModel>();
            services.AddSingleton<ServerListViewModel>(sp => new ServerListViewModel(
                sp.GetRequiredService<ApiService>(),
                async serverId => await sp.GetRequiredService<ChannelListViewModel>().LoadChannelsAsync(serverId)));
            services.AddSingleton<MainViewModel>();
        }
    }
}
