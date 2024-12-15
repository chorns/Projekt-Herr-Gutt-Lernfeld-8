using System.Windows;
using System.Windows.Controls;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI
{
	
	public partial class Analyse1 : UserControl
	{
		public Analyse1()
		{
			InitializeComponent();
			FillTimeComboBox();
		}

		private void analysis1Button_Click(object sender, RoutedEventArgs e)
		{
			
			//Zeit-Filter
			DateTime? startTime = null, endTime = null;
			

			if (zeitraumVon.SelectedDate.HasValue)
			{
				var vonDate=zeitraumVon.SelectedDate.Value;
				int vonStunde = int.Parse(stundenAbCombo.SelectedValue.ToString());
				int vonMinute = int.Parse(minutenAbCombo.SelectedValue.ToString());
				startTime = new DateTime(vonDate.Year, vonDate.Month, vonDate.Day, vonStunde, vonMinute, 0);
			}
			if (zeitraumBis.SelectedDate.HasValue)
			{
				var bisDate = zeitraumBis.SelectedDate.Value;
				int bisStunde = int.Parse(stundenBisCombo.SelectedValue.ToString());
				int bisMinute = int.Parse(minutenBisCombo.SelectedValue.ToString());
				endTime = new DateTime(bisDate.Year, bisDate.Month, bisDate.Day, bisStunde, bisMinute, 0);
			}

			//TODO: IP-Filter
			//IP-Filter
			string ipFilter = string.IsNullOrWhiteSpace(searchIpTxt.Text) ? null : searchIpTxt.Text;
			if (!string.IsNullOrWhiteSpace(searchIpTxt.Text)&&!DbHelper.IpIsValid(searchIpTxt.Text))
			{
				MessageBox.Show("Die eingegebene IP-Adresse ist ungültig, es wird ohne IP gefiltert.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			//Datenbankabfrage
			var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
			List<LogEintrag> logEintrag = dbHelper.GetFilteredLogEntries(startTime, endTime, ipFilter);

			//Ergebnis anzeigen
			LogDataGrid.ItemsSource = logEintrag;
		}
		private void FillTimeComboBox()
		{
			for (int i = 0; i < 24; i++)
			{
				stundenAbCombo.Items.Add(i);
				stundenBisCombo.Items.Add(i);
			}
			for (int i = 0; i < 60; i++)
			{
				minutenAbCombo.Items.Add(i);
				minutenBisCombo.Items.Add(i);
			}
		}
    }
}
