using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;
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
    
    public partial class Analyse3 : UserControl
    {
        public Analyse3()
        {
            InitializeComponent();
			FillStatusComboBox();
		}

		private void analysis3Button_Click(object sender, RoutedEventArgs e)
		{
			var helper = App.AppHost.Services.GetRequiredService<AnalysisHelper>();
			var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
			string ipFilter;
			string startTimeString = null;
			string endTimeString = null;
			string statusCodeFilter = null;


			



			//Zeit-Filter
			DateTime? startTime = null, endTime = null;


			if (zeitraumVon.SelectedDate.HasValue)
			{
				var vonDate = zeitraumVon.SelectedDate.Value;
				int vonStunde = int.Parse(stundenAbCombo.SelectedValue.ToString());
				int vonMinute = int.Parse(minutenAbCombo.SelectedValue.ToString());
				startTimeString = helper.BuildDateTime(vonDate, vonStunde, vonMinute);

			}
			if (zeitraumBis.SelectedDate.HasValue)
			{
				var bisDate = zeitraumBis.SelectedDate.Value;
				int bisStunde = int.Parse(stundenBisCombo.SelectedValue.ToString());
				int bisMinute = int.Parse(minutenBisCombo.SelectedValue.ToString());
				endTimeString = helper.BuildDateTime(bisDate, bisStunde, bisMinute);
			}

			//Status-Code-Filter
			if (statusCodeCombo.SelectedItem != null)
			{
				statusCodeFilter = statusCodeCombo.SelectedItem.ToString();
			}

			// IP-Filter
			ipFilter = searchIpTxt.Text;
			if (string.IsNullOrWhiteSpace(searchIpTxt.Text) || !DbHelper.IpIsValid(searchIpTxt.Text))
			{
				MessageBox.Show("IP-Adresse ist ungültig");
			}
			else
			{
				//Datenbankabfrage
				List<dynamic> logEintrag = dbHelper.GetFilteredLogEntriesIII(startTimeString, endTimeString, ipFilter,statusCodeFilter);

				MessageBox.Show($"Anzahl der Einträge: {logEintrag.Count}");

				//DataGrid reseten
				LogDataGrid.ItemsSource = null;
				//Ergebnis anzeigen
				LogDataGrid.ItemsSource = logEintrag;
			}
		}
		private void FillStatusComboBox()
		{

			var dbHelper = App.AppHost.Services.GetRequiredService<DbHelper>();
			List<string> statusCodeList = dbHelper.GetStatusCodes();
			foreach (var statusCode in statusCodeList)
			{
				statusCodeCombo.Items.Add(statusCode);
			}
		}
	}
}
