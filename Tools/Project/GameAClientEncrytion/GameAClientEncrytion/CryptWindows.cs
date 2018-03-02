/********************************************************************
** Filename : CryptWindows
** Author : quansiwei
** Date : 18/3/2 上午11:54:56
** Summary : CryptWindows
***********************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace J3Tech
{
    public class CryptWindows
    {
        [DllImport("CryptWindows")]
        public static extern void SetApplicationPath(StringBuilder path);
        [DllImport("CryptWindows")]
        public static extern void AddAssembly(StringBuilder filename);
        [DllImport("CryptWindows")]
        public static extern void ClearAssembly();
        [DllImport("CryptWindows")]
        public static extern int CryptAssembly(int version, int arch, StringBuilder key);
        [DllImport("CryptWindows")]
        public static extern uint Get_Version();
    }

    public class CheckVersion
    {
        public const string Version = "3.0.2";
        public const int VersionValue = 300;
    }
}