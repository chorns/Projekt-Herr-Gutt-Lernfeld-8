using System.Text;

namespace LF08_Projekt_Web_Log_ETL_mit_WinGUI.Helper;

public class AnalysisHelper
{
	public string BuildDateTime(DateTime date,int hour, int minute)
	{
		StringBuilder dateAsString = new StringBuilder();
		dateAsString.Append(date.Year);
		dateAsString.Append("-");
		dateAsString.Append(date.Month.ToString("00"));
		dateAsString.Append("-");
		dateAsString.Append(date.Day.ToString("00"));
		dateAsString.Append(" ");
		dateAsString.Append(hour.ToString("00"));
		dateAsString.Append(":");
		dateAsString.Append(minute.ToString("00"));
		dateAsString.Append(":00");
		dateAsString.Append(".000000");
		return dateAsString.ToString();

	}
}