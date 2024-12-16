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
			var helper = App.AppHost.Services.GetRequiredService<AnalysisHelper>();
			string startTimeString = null;
			string endTimeString = null;

			//Zeit-Filter
			DateTime? startTime = null, endTime = null;
			

			if (zeitraumVon.SelectedDate.HasValue)
			{
				// var vonDate=zeitraumVon.SelectedDate.Value;
				// int vonStunde = int.Parse(stundenAbCombo.SelectedValue.ToString());
				// int vonMinute = int.Parse(minutenAbCombo.SelectedValue.ToString());
				// int vonSekunde = int.Parse(sekundenAbCombo.SelectedValue.ToString());
				// startTimeString = helper.BuildDateTime(vonDate, vonStunde, vonMinute);
				
			}
			if (zeitraumBis.SelectedDate.HasValue)
			{
				// var bisDate = zeitraumBis.SelectedDate.Value;
				// int bisStunde = int.Parse(stundenBisCombo.SelectedValue.ToString());
				// int bisMinute = int.Parse(minutenBisCombo.SelectedValue.ToString());
				// endTimeString = helper.BuildDateTime(bisDate, bisStunde, bisMinute);
			}


			// IP-Filter
			string ipFilter = string.IsNullOrWhiteSpace(searchIpTxt.Text) || !DbHelper.IpIsValid(searchIpTxt.Text) ? null : searchIpTxt.Text;
			

			//MessageBox.Show($"StartTime: {startTimeString}, EndTime: {endTimeString}, IP Filter: {ipFilter}");

			//Datenbankabfrage
			var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
			List<LogEintrag> logEintrag = dbHelper.GetFilteredLogEntriesI(startTimeString, endTimeString, ipFilter);

			MessageBox.Show($"Anzahl der Einträge: {logEintrag.Count}");

			//DataGrid reseten
			LogDataGrid.ItemsSource = null;
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
