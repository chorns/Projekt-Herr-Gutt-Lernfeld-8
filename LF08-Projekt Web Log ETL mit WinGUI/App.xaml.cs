using System.Windows;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI
{
	
	public partial class App : Application
	{
		public static IHost? AppHost { get; private set; }
		public App()
		{
			AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
				{
					ConfigureServices(services);
				})
				.Build();
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
		private void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<MainWindow>();
			services.AddSingleton<DefaultUserControl> ();
			services.AddSingleton<ImportLogUserControl>();
			services.AddSingleton<Analyse1>();
			services.AddSingleton<IConnectionStringProvider,ConnectionStringProvider>();
			services.AddTransient<DbHelper>();
		}
	}

}
