
using System.IO;
using System.Net;
using System.Data.SQLite;
using System.Windows;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Models;
using Microsoft.Extensions.DependencyInjection;


namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper
{
    class DbHelper
    {
		public const string _tableName = "weblogs";
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
            string createTable = $"CREATE TABLE IF NOT EXISTS {_tableName} ("+
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
		public static bool IpIsValid(string ipAdress)
		{
			return IPAddress.TryParse(ipAdress, out _);
		}
		public List<LogEintrag> GetFilteredLogEntries(DateTime? startTime, DateTime? endTime, string ipFilter)
		{
			var logEntries = new List<LogEintrag>();

			string query = $"SELECT * FROM {_tableName} WHERE 1=1";
			if (startTime.HasValue)
				query += " AND timestamp >= @startTime";
			if (endTime.HasValue)
				query += " AND timestamp <= @endTime";
			if (!string.IsNullOrEmpty(ipFilter))
				query += " AND ip_adress = @ipFilter";

			var connectionStringProvider = App.AppHost.Services.GetRequiredService<IConnectionStringProvider>();
			using (var connection = new SQLiteConnection(connectionStringProvider.GetConnectionString()))
			{
				connection.Open();

				using (var command = new SQLiteCommand(query, connection))
				{
					if (startTime.HasValue)
						command.Parameters.AddWithValue("@startTime", startTime.Value);
					if (endTime.HasValue)
						command.Parameters.AddWithValue("@endTime", endTime.Value);
					if (!string.IsNullOrEmpty(ipFilter))
						command.Parameters.AddWithValue("@ipFilter", ipFilter);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							logEntries.Add(new LogEintrag
							{
								Id= reader.GetInt32(0),
								Ip_adress = reader.GetString(1),
								Identity = reader.GetString(2),
								Authentification = reader.GetString(3),
								Timestamp = reader.GetDateTime(4),
								Request = reader.GetString(5),
								Http_Statuscode = reader.GetInt32(6),
								Responsesize = reader.GetInt32(7)
								
							});
						}
					}
				}
			}

			return logEntries;
		}
	}
}

