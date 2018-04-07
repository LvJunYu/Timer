/********************************************************************

** Filename : LogMan
** Author : ake
** Date : 2016/4/1 11:51:05
** Summary : LogMan
***********************************************************************/

using System;

namespace Common
{
	public class LogMan
	{
		private static ILog _sLogEntity;

		public static void SetLogEntity(ILog logEntity)
		{
			_sLogEntity = logEntity;
		}
		public static void Log(string content, params object[] parameters)
		{
			if (_sLogEntity != null)
			{
				_sLogEntity.Log(content,parameters);
			}
		}

		public static void Error(string content, params object[] parameters)
		{
			if (_sLogEntity != null)
			{
				_sLogEntity.Error(content, parameters);
			}
		}
	}
}
