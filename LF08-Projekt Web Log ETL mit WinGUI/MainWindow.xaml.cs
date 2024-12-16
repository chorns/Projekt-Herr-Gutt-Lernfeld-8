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
		private DefaultUserControl defaultUserControl;
		private ImportLogUserControl importLogUserControl;
		private Analyse1 analyse1;
		private Analyse2 analyse2;
		private Analyse3 analyse3;

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
						if (defaultUserControl == null)
						{
							defaultUserControl = new DefaultUserControl();
						}

						page = defaultUserControl;
						break;
					case "importLogFilesButton":
						if (importLogUserControl == null)
						{
							importLogUserControl = new ImportLogUserControl();
						}
						page = importLogUserControl;
						break;
					case "analyzeLogButtonI":
						if (analyse1 == null)
						{
							analyse1= new Analyse1();
						}
						page = analyse1;
						break;
					case "analyzeLogButtonII":
						if(analyse2 == null)
						{
							analyse2 = new Analyse2();
						}
						page = analyse2;
						break;
					case "analyzeLogButtonIII":
						if(analyse3==null)
						{
							analyse3 = new Analyse3();
						}
						page = analyse3;
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