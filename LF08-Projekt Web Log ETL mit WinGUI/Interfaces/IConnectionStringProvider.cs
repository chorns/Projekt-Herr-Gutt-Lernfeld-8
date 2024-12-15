using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Interfaces
{
	public interface IConnectionStringProvider
	{
		public string GetConnectionString();
		public void SetConnectionString(string connectionString);
	}
}
