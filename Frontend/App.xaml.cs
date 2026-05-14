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
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IDispatcherService, WpfDispatcherService>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<SearchUserViewModel>();
            services.AddTransient<CreateServerViewModel>();
            services.AddSingleton<ChannelListViewModel>();
            services.AddSingleton<ServerListViewModel>(sp => new ServerListViewModel(
                sp.GetRequiredService<IApiService>(),
                async serverId => await sp.GetRequiredService<ChannelListViewModel>().LoadChannelsAsync(serverId)));
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<HomeViewModel>();
        }
    }
}
