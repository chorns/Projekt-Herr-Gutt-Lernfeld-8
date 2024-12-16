using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI
{
	
	public partial class MainWindow : Window
	{
		private DbHelper _dbHelper;
		public MainWindow()
		{
			InitializeComponent();
			mainContent.Content= new DefaultUserControl();
			var dbHelper=App.AppHost.Services.GetRequiredService<DbHelper>();
			dbHelper.InitialiseDatabase();
		}

		private void LoadUserControl(object sender, RoutedEventArgs e)
		{
			UserControl page=null;
			if (sender is RadioButton radioButton)
			{
				string pageName = radioButton.Name;
				switch(pageName)
				{
					case "homeButton":
						page = new DefaultUserControl();
						HomeButton.IsChecked = false;
						break;
					case "importLogFilesButton":
						page = new ImportLogUserControl();
						break;
					case "analyzeLogButtonI":
						page = new Analyse1();
						break;
					case "analyzeLogButtonII":
						page = new Analyse2();
						break;
					case "analyzeLogButtonIII":
						page = new Analyse3();
						break;
					case "closeButton":
						Application.Current.Shutdown();
						break;
				}
				mainContent.Content = page;
			}
		}
	}
}