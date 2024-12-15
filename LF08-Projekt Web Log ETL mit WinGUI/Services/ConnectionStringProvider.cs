using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Services
{
	public class ConnectionStringProvider: IConnectionStringProvider
	{
		private string _connectionString;
		public string GetConnectionString()
		{
			return _connectionString;
		}
		public void SetConnectionString(string connectionString)
		{
			_connectionString = "Data Source="+connectionString+";Version=3;";
		}
	}
}
