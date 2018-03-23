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
        [DllImport("CryptWindowsX64")]
        public static extern void SetApplicationPath(StringBuilder path);
        [DllImport("CryptWindowsX64")]
        public static extern void AddAssembly(StringBuilder filename);
        [DllImport("CryptWindowsX64")]
        public static extern void ClearAssembly();
        [DllImport("CryptWindowsX64")]
        public static extern int CryptAssembly(int version, int arch, StringBuilder key);
        [DllImport("CryptWindowsX64")]
        public static extern uint Get_Version();
    }

    public class CheckVersion
    {
        public const string Version = "3.0.2";
        public const int VersionValue = 300;
    }
}