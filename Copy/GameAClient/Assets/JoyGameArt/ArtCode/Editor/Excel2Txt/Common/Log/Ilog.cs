/********************************************************************

** Filename : ILog
** Author : ake
** Date : 2016/4/1 11:29:29
** Summary : ILog
***********************************************************************/

using System;

namespace Common
{
	public enum ELogLevel
	{
		Log = 0,
		Error,
		Max,
	}
	public interface ILog
	{
		void Log(string content, params object[] parameterArray);
		void Error(string content, params object[] parameterArray);
	}
}