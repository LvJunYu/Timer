/********************************************************************
** Filename : UnityLogEntity  
** Author : ake
** Date : 4/26/2016 8:32:21 PM
** Summary : UnityLogEntity  
***********************************************************************/


using System;
using UnityEngine;

namespace Common
{
    public class UnityLogEntity: ILog
    {
        private string[] _logLevelContentFormatArray;

        public UnityLogEntity()
        {
            _logLevelContentFormatArray = new string[(int)ELogLevel.Max];
            _logLevelContentFormatArray[(int)ELogLevel.Log] = "Log:({0}){1}";
            _logLevelContentFormatArray[(int)ELogLevel.Error] = "Error:({0}){1}";
        }

        public void Log(string content, params object[] parameterArray)
        {
            Printf(string.Format(content, parameterArray), ELogLevel.Log);
        }

        public void Error(string content, params object[] parameterArray)
        {
            Printf(string.Format(content, parameterArray), ELogLevel.Error);
        }

        private void Printf(string content, ELogLevel logLevel)
        {
            string value = string.Format(_logLevelContentFormatArray[(int) logLevel], DateTime.Now.ToString("HH:mm:ss"),
                content);
            if (logLevel == ELogLevel.Error)
            {
                Debug.LogError(value);
            }
            else
            {
                Debug.Log(value);
            }
        }
    }
}
