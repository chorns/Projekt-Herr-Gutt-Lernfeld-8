using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.Diagnostics.PerformanceData;
using System.Windows;
using System.Windows.Documents;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Services;
using Microsoft.Extensions.DependencyInjection;


namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper
{
    class DbHelper
    {
        private const string _dbName = "weblogs.db";
        private static string _dbPath= System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,_dbName);
		
		public void InitialiseDatabase()
        {
			var connectionStringProvider = App.AppHost.Services.GetRequiredService<IConnectionStringProvider>();
			connectionStringProvider.SetConnectionString(_dbName);
			if (!File.Exists(_dbName))
            {
                SQLiteConnection.CreateFile(_dbName);
                MessageBox.Show($"Datenbank wurde unter \n\n{_dbPath} \n\nerstellt.");
            }

            using var connection =new SQLiteConnection(connectionStringProvider.GetConnectionString());
            connection.Open();

			CreateTable(connection);
		}
        private static void CreateTable(SQLiteConnection connection)
        {
            string createTable = "CREATE TABLE IF NOT EXISTS weblogs ("+
                                 "ip_adress TEXT NOT NULL, " +
                                 "identity TEXT,"+
                                 "authentification TEXT,"+
                                 "timestamp DATETIME NOT NULL, " +
                                 "request TEXT NOT NULL, " +
                                 "http_statuscode INTEGER NOT NULL, " +
                                 "responsesize INTEGER NOT NULL," +
                                 "PRIMARY KEY (ip_adress,timestamp,request));";

            using var command = new SQLiteCommand(createTable, connection);
            command.ExecuteNonQuery();
		}
  
		public StringBuilder GetColumnNames()
		{
			StringBuilder columnNames = new StringBuilder();
			var connectionStringProvider = App.AppHost.Services.GetRequiredService<ConnectionStringProvider>();
			using var connection = new SQLiteConnection(connectionStringProvider.GetConnectionString());
			try
			{
				connection.Open();
				SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM pragma_table_info('weblogs')", connection);
				SQLiteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					columnNames.Append(reader["name"].ToString());
					columnNames.Append(", ");
				}
			}
			catch(SQLiteException ex)
			{
				MessageBox.Show("Es ist ein Fehler aufgetreten!\n\n" + ex);
			}

			return columnNames;
		}
		public string BuildQuery(string filePath, string tableName)
		{
			StringBuilder queryBuilder =new StringBuilder();
			var columns = GetColumnNames();
			queryBuilder.Append($"INSERT INTO {tableName} (");
			queryBuilder.Append(string.Join(", ", columns));
			queryBuilder.Remove(queryBuilder.Length - 1, 1);
			queryBuilder.Append(") VALUES ");

			columns.Append("(@");
			columns.Append(string.Join(", @", columns));
			columns.Append(")");

			queryBuilder.Append(columns);
			queryBuilder.Append(";");
			string query = queryBuilder.ToString();
			return query;
			
		}
		public void ImportData(string filePath)
		{
			int counter = 0;
			bool ipNotValid = false;
			var logLines= File.ReadLines(filePath);
			var connectionStringProvider = App.AppHost.Services.GetRequiredService<IConnectionStringProvider>();
			using (var connection = new SQLiteConnection(connectionStringProvider.GetConnectionString()))
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction())
				{
					foreach (var line in logLines)
					{
						//Logzeile in einzelne Teile zerlegen
						var logParts = line.Split(" ", 5);
						var ipAdress = logParts[0];
						var identity = logParts[1];
						var identityAuthentification = logParts[2];

						//IP-Adresse validieren
						if (!IpIsValid(ipAdress))
						{
							ipNotValid = true;
							counter++;
							continue;
						}
						//TimeStamp extrahieren
						int timeStampStart = line.IndexOf("[") + 1;
						int timeStampEnd = line.IndexOf("]");
						string timeStamp = line.Substring(timeStampStart, timeStampEnd - timeStampStart);

						//Request in einzelne Teile zerlegen
						int requestStart = line.IndexOf("\"") + 1;
						int requestEnd = line.LastIndexOf("\"");
						string request = line.Substring(requestStart, requestEnd - requestStart);

						//Statuscode und Responsesize extrahieren
						string[] statusParts = line.Substring(requestEnd + 1).Trim().Split(" ");
						int httpStatusCode = int.Parse(statusParts[0]);
						int responseSize = statusParts[1] == "-" ? 0 : int.Parse(statusParts[1]);

						//auf Duplikate überprüfen
						var checkQuery= "SELECT COUNT(*) FROM weblogs WHERE ip_adress=@ip_adress AND timestamp=@timestamp AND request=@request";
						using (var cmdCheck = new SQLiteCommand(checkQuery, connection))
						{
							cmdCheck.Parameters.AddWithValue("@ip_adress", ipAdress);
							cmdCheck.Parameters.AddWithValue("@timestamp", timeStamp);
							cmdCheck.Parameters.AddWithValue("@request", request);

							var count = Convert.ToInt32(cmdCheck.ExecuteScalar());
							if (count > 0)
							{
								MessageBox.Show($"Eintrag bereits vorhanden und wurde übersprungen.\n\nIP-Adresse: {ipAdress}\nTimestamp: {timeStamp}\nRequest: {request}");
								continue;
							}
						}


						var query = $"INSERT INTO weblogs (" +
									$"ip_adress, " +
									$"identity, " +
									$"authentification, " +
									$"timestamp, " +
									$"request, " +
									$"http_statuscode, " +
									$"responsesize) " +

									$"VALUES (" +
									$"@ip_adress, " +
									$"@identity, " +
									$"@authentification, " +
									$"@timestamp, " +
									$"@request, " +
									$"@http_statuscode, " +
									$"@responsesize);";
						using (var cmd = new SQLiteCommand(query, connection))
						{
							cmd.Parameters.AddWithValue("@ip_adress", ipAdress);
							cmd.Parameters.AddWithValue("@identity", identity);
							cmd.Parameters.AddWithValue("@authentification", identityAuthentification);
							cmd.Parameters.AddWithValue("@timestamp", timeStamp);
							cmd.Parameters.AddWithValue("@request", request);
							cmd.Parameters.AddWithValue("@http_statuscode", httpStatusCode);
							cmd.Parameters.AddWithValue("@responsesize", responseSize);
							cmd.ExecuteNonQuery();
						}
					}
					transaction.Commit();
				}
				
			}
			if (ipNotValid)
			{
				MessageBox.Show($"Es wurden {counter} ungültige IP-Adressen übersprungen.");
			}
		}
		private bool IpIsValid(string ipAdress)
		{
			return IPAddress.TryParse(ipAdress, out _);
		}
	}
}
