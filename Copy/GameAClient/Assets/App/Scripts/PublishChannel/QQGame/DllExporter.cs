
#if UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;

namespace GameA
{
    public class DllExporter
    {
        #region Win API  
        [DllImport("kernel32.dll")]  
        private static extern IntPtr LoadLibrary(string path);  
  
        [DllImport("kernel32.dll")]  
        private static extern IntPtr GetProcAddress(IntPtr lib, string funcName);  
  
        [DllImport("kernel32.dll")]  
        private static extern bool FreeLibrary(IntPtr lib);  
        #endregion  
  
        private readonly IntPtr _hLib;  
        public DllExporter(String dllPath)  
        {  
            _hLib = LoadLibrary(dllPath);  
        }  
  
        ~DllExporter()  
        {  
            FreeLibrary(_hLib);              
        }
  
        //将要执行的函数转换为委托  
        public Delegate Export (string apiName,Type t)    
        {  
            IntPtr api = GetProcAddress(_hLib, apiName);
            if (api == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return Marshal.GetDelegateForFunctionPointer(api, t);
            }  
        }
    }  
}
#endif