using System;
using System.Collections.Generic;
using System.IO;
using ComponentAce.Compression.Libs.zlib;

namespace JoyGameBuildVersionTool
{
    internal class Program
    {
        private static List<DatFileInfo> _fileInfoList;
        public static void Main(string[] args)
        {
            Process("/Users/quan/Downloads/JoyGameRes/InputResRoot/version/0.1.0/",
                "/Users/quan/Downloads/JoyGameRes/OutputResRoot/", "0.1.0");
        }

        private static void Process(string inputPath, string outputPath, string version)
        {
            DirectoryInfo di = new DirectoryInfo(inputPath);
            var list = di.GetFiles("*", SearchOption.AllDirectories);
            _fileInfoList = new List<DatFileInfo>(list.Length);
            byte[] writeBuffer = new byte[1024];
            foreach (var fileInfo in list)
            {
                var datFileInfo = new DatFileInfo();
                _fileInfoList.Add(datFileInfo);
                datFileInfo.MD5 = MD5Utils.Encrypt(File.ReadAllBytes(fileInfo.FullName));
                datFileInfo.Name = fileInfo.FullName.Substring(inputPath.Length);
                string targetFileName = outputPath + "/version/" + version + "/" + datFileInfo.Name+".dat";
                var targetDir = Path.GetDirectoryName(targetFileName);
                if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                using (ZOutputStream zOutputStream = new ZOutputStream(File.Create(targetFileName), zlibConst.Z_DEFAULT_COMPRESSION))
                {
                    using (var fi = fileInfo.OpenRead())
                    {
                        int len;
                        while ((len = fi.Read(writeBuffer, 0, writeBuffer.Length))>0)
                        {
                            zOutputStream.Write(writeBuffer, 0, len);
                        }
                    }
                }
            }
            using (var os = File.CreateText(outputPath+"version/" +version+"/manifest.xml.raw"))
            {
                os.WriteLine("<root>");
                foreach (var datFileInfo in _fileInfoList)
                {
                    os.WriteLine("<file>");
                    os.Write("<name>");
                    os.Write(datFileInfo.Name.Replace("/","\\"));
                    os.Write("</name>");
                    os.WriteLine();
                    os.Write("<md5>");
                    os.Write(datFileInfo.MD5);
                    os.Write("</md5>");
                    os.WriteLine();
                    os.Write("</file>");
                    os.WriteLine();

                }
                os.WriteLine("</root>");
            }
            
            using (ZOutputStream zOutputStream = new ZOutputStream(File.Create(outputPath+"version/" +version+"/manifest.xml.dat"), zlibConst.Z_DEFAULT_COMPRESSION))
            {
                using (var fi = File.OpenRead(outputPath+"version/" +version+"/manifest.xml.raw"))
                {
                    int len;
                    while ((len = fi.Read(writeBuffer, 0, writeBuffer.Length))>0)
                    {
                        zOutputStream.Write(writeBuffer, 0, len);
                    }
                }
            }
            File.Delete(outputPath+"version/" +version+"/manifest.xml.raw");
        }
        private class DatFileInfo
        {
            public string Name;
            public string MD5;
        }
    }
}