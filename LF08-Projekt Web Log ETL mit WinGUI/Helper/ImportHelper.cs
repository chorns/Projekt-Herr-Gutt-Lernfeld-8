using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper
{
	public class ImportHelper
	{
		public DataTable LoadDataToDataTable(string filePath)
		{
			DataTable dataTable = new DataTable();
			using (var reader = new StreamReader(filePath, Encoding.UTF8))
			{
				while (!reader.EndOfStream)
				{
					string[] lines = reader.ReadLine().Split(',');
					if (lines != null)
					{
						dataTable.Rows.Add(lines);
					}
				}
			}

			return dataTable;
		}
	}
}
