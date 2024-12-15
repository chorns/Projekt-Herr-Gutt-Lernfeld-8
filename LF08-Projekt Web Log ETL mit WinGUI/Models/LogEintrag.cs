using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Models
{
	public class LogEintrag
	{
		public int Id { get; set; }
		public string Ip_adress { get; set; }
		public string Identity { get; set; }
		public string Authentification { get; set; }
		public DateTime Timestamp { get; set; }
		public string Request { get; set; }
		public int Http_Statuscode { get; set; }
		public int Responsesize { get; set; }
	}
}
