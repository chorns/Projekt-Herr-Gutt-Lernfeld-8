using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI
{
	
	public partial class ImportLogUserControl : UserControl
	{
		
		public ImportLogUserControl()
		{
			InitializeComponent();
		}

		

		private void filePathTxt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = "Log-Datei auswählen",
				Filter = "Log-Dateien|*.log",
			};
			if (openFileDialog.ShowDialog() == true)
			{
				filePathTxt.Text = openFileDialog.FileName;
			}

		}
		

		private void importLogButton_Click(object sender, RoutedEventArgs e)
		{

			var connectionStringProvider = App.AppHost.Services.GetRequiredService<IConnectionStringProvider>();
			var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
			//DateiPfad aus SaveFileDialog holen
			string filePath = filePathTxt.Text;
			//Check ob TextBox leer ist
			if (string.IsNullOrEmpty(filePath))
			{
				MessageBox.Show("Bitte wählen Sie eine Log-Datei aus", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				dbHelper.ImportData(filePath);
				MessageBox.Show("Daten wurden erfolgreich importiert", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
