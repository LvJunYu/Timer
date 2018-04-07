/********************************************************************

** Filename : L
** Author : ake
** Date : 2016/3/8 17:59:56
** Summary : L
***********************************************************************/

using System;
using UnityEngine;


public class L
{
    public static void Log(string content,params object[] datas)
    {
        Debug.Log(SerializeData(content, datas));
    }

    public static void Error(string content, params object[] datas)
    {
        Debug.LogError(SerializeData(content, datas));

    }

    private static string SerializeData(string contex, params object[] datas)
    {
		return string.Format("[{0}] {1}", DateTime.Now, string.Format(contex, datas));
    }
}