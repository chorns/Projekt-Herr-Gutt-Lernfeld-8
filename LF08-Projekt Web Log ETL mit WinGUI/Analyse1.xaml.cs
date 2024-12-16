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
			var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
			string startTimeString = null;
			string endTimeString = null;

			//Zeit-Filter
			DateTime? startTime = null, endTime = null;

			// Überprüfung der Zeiträume
			if (zeitraumVon.SelectedDate.HasValue && !zeitraumBis.SelectedDate.HasValue)
			{
				MessageBox.Show("Bitte geben Sie auch ein Enddatum an, wenn ein Startdatum ausgewählt wurde.");
				return;
			}
			else if (!zeitraumVon.SelectedDate.HasValue && zeitraumBis.SelectedDate.HasValue)
			{
				MessageBox.Show("Bitte geben Sie auch ein Startdatum an, wenn ein Enddatum ausgewählt wurde.");
				return;
			}


			if (zeitraumVon.SelectedDate.HasValue)
			{
				var vonDate=zeitraumVon.SelectedDate.Value;
				int vonStunde = stundenAbCombo.SelectedValue != null ? int.Parse(stundenAbCombo.SelectedValue.ToString()) : 0;
				int vonMinute = minutenAbCombo.SelectedValue != null ? int.Parse(minutenAbCombo.SelectedValue.ToString()) : 0;
				int vonSekunde = sekundenAbCombo.SelectedValue != null ? int.Parse(sekundenAbCombo.SelectedValue.ToString()) : 0;
				startTimeString = helper.BuildDateTime(vonDate, vonStunde, vonMinute, vonSekunde);
			}

			if (zeitraumBis.SelectedDate.HasValue)
			{
				var bisDate = zeitraumBis.SelectedDate.Value;
				int bisStunde = stundenBisCombo.SelectedValue != null ? int.Parse(stundenBisCombo.SelectedValue.ToString()) : 0;
				int bisMinute = minutenBisCombo.SelectedValue != null ? int.Parse(minutenBisCombo.SelectedValue.ToString()) : 0;
				int bisSekunde = sekundenBisCombo.SelectedValue != null ? int.Parse(sekundenBisCombo.SelectedValue.ToString()) : 0;
				endTimeString = helper.BuildDateTime(bisDate, bisStunde, bisMinute, bisSekunde);
			}


			// IP-Filter
			string ipFilter = string.IsNullOrWhiteSpace(searchIpTxt.Text)  ? null : searchIpTxt.Text;
			if (!string.IsNullOrEmpty(searchIpTxt.Text)&& !dbHelper.IpIsValid(searchIpTxt.Text))
			{
				MessageBox.Show("IP-Adresse ist ungültig");
			}
			else
			{
				ipFilter = searchIpTxt.Text;
				//MessageBox.Show($"StartTime: {startTimeString}, EndTime: {endTimeString}, IP Filter: {ipFilter}");
				//Datenbankabfrage
				List<LogEintrag> logEintrag = dbHelper.GetFilteredLogEntriesI(startTimeString, endTimeString, ipFilter);

				MessageBox.Show($"Anzahl der Einträge: {logEintrag.Count}");

				//DataGrid reseten
				LogDataGrid.ItemsSource = null;
				//Ergebnis anzeigen
				LogDataGrid.ItemsSource = logEintrag;
			}
			
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
			for (int i = 0; i < 60; i++)
			{
				sekundenAbCombo.Items.Add(i);
				sekundenBisCombo.Items.Add(i);
			}
		}

		
	}
}
