using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Models;
using Microsoft.Extensions.DependencyInjection;
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

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI
{
    public partial class Analyse2 : UserControl
    {
        public Analyse2()
        {
            InitializeComponent();
			FillTimeComboBox();
		}

		private void analysis2Button_Click(object sender, RoutedEventArgs e)
		{
			var helper = App.AppHost.Services.GetRequiredService<AnalysisHelper>();
			string ipFilter;
			string startTimeString = null;
			string endTimeString = null;

			//Zeit-Filter
			DateTime? startTime = null, endTime = null;


			if (zeitraumVon.SelectedDate.HasValue)
			{
				var vonDate = zeitraumVon.SelectedDate.Value;
				int vonStunde = int.Parse(stundenAbCombo.SelectedValue.ToString());
				int vonMinute = int.Parse(minutenAbCombo.SelectedValue.ToString());
				int vonSekunde = int.Parse(sekundenAbCombo.SelectedValue.ToString());
				startTimeString = helper.BuildDateTime(vonDate, vonStunde, vonMinute, vonSekunde);

			}
			if (zeitraumBis.SelectedDate.HasValue)
			{
				var bisDate = zeitraumBis.SelectedDate.Value;
				int bisStunde = int.Parse(stundenBisCombo.SelectedValue.ToString());
				int bisMinute = int.Parse(minutenBisCombo.SelectedValue.ToString());
				int bisSekunde = int.Parse(sekundenBisCombo.SelectedValue.ToString());
				endTimeString = helper.BuildDateTime(bisDate, bisStunde, bisMinute, bisSekunde);
			}

			// IP-Filter
			ipFilter = searchIpTxt.Text;
			if (string.IsNullOrWhiteSpace(searchIpTxt.Text)|| !DbHelper.IpIsValid(searchIpTxt.Text))
			{
				MessageBox.Show("IP-Adresse ist ungültig");
			}
			else
			{
				//Datenbankabfrage
				var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
				List<dynamic> logEintrag = dbHelper.GetFilteredLogEntriesII(startTimeString, endTimeString, ipFilter);

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
