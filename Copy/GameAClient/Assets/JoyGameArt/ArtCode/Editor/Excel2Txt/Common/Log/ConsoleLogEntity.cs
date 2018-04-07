/********************************************************************

** Filename : ConsoleLogEntity
** Author : ake
** Date : 2016/4/1 11:32:48
** Summary : ConsoleLogEntity
***********************************************************************/

using System;
using System.Collections.Generic;

namespace Common
{
	public class ConsoleLogEntity:ILog
	{
		private string[] _logLevelContentFormatArray;

		public ConsoleLogEntity()
		{
			_logLevelContentFormatArray = new string[(int)ELogLevel.Max];
			_logLevelContentFormatArray[(int) ELogLevel.Log] = "Log:({0}){1}";
			_logLevelContentFormatArray[(int)ELogLevel.Error] = "Error:({0}){1}";

		}

		public void Log(string content, params object[] parameterArray)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Printf(string.Format(content, parameterArray), ELogLevel.Log);
			Console.ResetColor();
		}

		public void Error(string content, params object[] parameterArray)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Printf(string.Format(content, parameterArray),ELogLevel.Error);
			Console.ResetColor();
		}


		private void Printf(string content, ELogLevel logLevel)
		{
			Console.WriteLine(string.Format(_logLevelContentFormatArray[(int)logLevel],DateTime.Now.ToString("HH:mm:ss"),content));
		}

	}
}