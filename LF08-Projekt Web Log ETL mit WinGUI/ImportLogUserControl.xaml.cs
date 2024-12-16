using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.IO;
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
			//Dateinamen aus Pfad holen
			string fileName = Path.GetFileName(filePath);

			//Check ob TextBox leer ist
			if (string.IsNullOrEmpty(filePath))
			{
				MessageBox.Show("Bitte wählen Sie eine Log-Datei aus", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				if (logFileListBox.Items.Contains(fileName))
				{
					MessageBox.Show("Datei wurde bereits importiert", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					//Dateinamen in ListBox speichern
					logFileListBox.Items.Add(fileName);

					logFileListBox.Visibility= Visibility.Visible;
					listBoxLbl.Visibility = Visibility.Visible;
					
					//Daten importieren
					dbHelper.ImportData(filePath);
					MessageBox.Show("Daten wurden erfolgreich importiert", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}
	}
}
