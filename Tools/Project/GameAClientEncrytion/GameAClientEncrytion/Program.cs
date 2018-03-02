using J3Tech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameAClientEncrytion
{
    class Program
    {
        private static string _keyString = "My Key";
        private static string _appPath = "";
        private static string _managedPath = "";

        private static TargetArchitecture _architecture = TargetArchitecture.X86;
        public enum TargetArchitecture
        {
            X86 = 0,
            X64 = 1
        }
        static void Main(string[] args)
        {
            CommandEncrypt();
        }

        /// <summary>
        /// Encrypt by command line. 
        /// UnityPath -batchmode -quit -projectPath YourProjectPath -executeMethod J3Tech.CryptWindowsAssemblyEditor.CommandBuild TargetExePath architecture SecretKey DllName1,DllName2.....
        /// For example: "GameAClientEncrytion.exe E:/Test.exe x86 mySecretKey Assembly-CSharp.dll OtherLib.dll
        /// </summary>
        public static void CommandEncrypt()
        {
            string[] parameters = System.Environment.GetCommandLineArgs();
            _appPath = parameters[1];
            if (parameters[2] == "x86" || parameters[2] == "X86")
            {
                _architecture = TargetArchitecture.X86;
            }
            else
            {
                _architecture = TargetArchitecture.X64;
            }
            string secretKey = parameters[3];
            string filename = Path.GetFileNameWithoutExtension(_appPath);
            string directory = Path.GetDirectoryName(_appPath);
            if (directory != null) if (filename != null) _managedPath = Path.Combine(directory, filename);
            _managedPath += "_Data";
            _managedPath = Path.Combine(_managedPath, "Managed");
            string[] dlls = new string[parameters.Length - 4];
            for (int n = 0, i = 4, imax = parameters.Length; i < imax; ++i, ++n)
            {
                dlls[n] = Path.Combine(_managedPath, parameters[i]);
            }
            Encrypt(_appPath, secretKey, dlls);
        }

        private static bool Encrypt(string targetExe, string secretKey, params string[] dllNames)
        {
            if (CryptWindows.Get_Version() != CheckVersion.VersionValue)
            {
                Console.WriteLine("Before import the new version, please exit unity then delete all files of CodeEncipher. Don't forget to delete the dlls in plugins folder.");
                return false;
            }
            CryptWindows.SetApplicationPath(new StringBuilder(targetExe));
            CryptWindows.ClearAssembly();
            foreach (var dll in dllNames)
            {
                CryptWindows.AddAssembly(new StringBuilder(dll));
            }
            const int version = 50;
            return CryptWindows.CryptAssembly(version, (int)_architecture, new StringBuilder(string.IsNullOrEmpty(secretKey) ? "null" : _keyString)) == 1;
        }
    }
}
