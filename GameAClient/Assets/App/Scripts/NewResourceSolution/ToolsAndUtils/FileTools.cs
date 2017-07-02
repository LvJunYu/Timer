using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using SoyEngine;

namespace NewResourceSolution
{
	public static class FileTools
	{
        public static bool TryReadFileToBytes (string fileFullName, out byte[] bytes)
        {
            bytes = null;
            FileInfo fi = new FileInfo (fileFullName);
            if (!fi.Exists) {
                return false;
            }
            using (FileStream fs = new FileStream (fileFullName, FileMode.Open, FileAccess.Read)) {
                bytes = new byte[fs.Length];
                fs.Read (bytes, 0, (int)fs.Length);
                fs.Close ();
                return true;
            }
        }
		public static byte[] ReadFileToBytes (string fileFullName)
		{
			FileInfo fi = new FileInfo(fileFullName);
			if (!fi.Exists)
			{
				LogHelper.Error("File {0} doesn't exist.", fileFullName);
				return null;
			}
            using (FileStream fs = new FileStream (fileFullName, FileMode.Open, FileAccess.Read)) {
                byte[] buffer = new byte[fs.Length];
                fs.Read (buffer, 0, (int)fs.Length);
                fs.Close ();
                return buffer;
            }
		}

        public static bool TryReadFileToString (string fileFullName, out string content)
        {
            content = string.Empty;
            byte[] bytes;
            if (TryReadFileToBytes (fileFullName, out bytes))
            {
                if (null == bytes || bytes.Length == 0)
                {
                    return false;
                }
                content = Encoding.UTF8.GetString(bytes);
                return true;
            } 
            else
            {
                return false;
            }
        }

		public static string ReadFileToString (string fileFullName)
		{
			byte[] bytes = ReadFileToBytes(fileFullName);
			if (null == bytes || bytes.Length == 0)
			{
				return null;
			}
			return Encoding.UTF8.GetString(bytes);
		}

		public static bool WriteBytesToFile (byte[] bytes, string fileFullName, bool overwrite = true)
		{
			FileInfo fi = new FileInfo(fileFullName);
			if (fi.Exists)
			{
				if (overwrite)
				{
					File.Delete(fileFullName);
				}
				else
				{
					LogHelper.Error("File {0} already exist, write file failed.", fileFullName);
					return false;
				}
			}
            using (FileStream fs = new FileStream (fileFullName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write (bytes, 0, (int)bytes.Length);
                fs.Flush ();
                fs.Close ();
                return true;
            }
		}

		public static bool WriteStringToFile(string content, string fileFullName, bool overwrite = true)
		{
			if (string.IsNullOrEmpty(content))
			{
				LogHelper.Error("WriteStringToFile failed content IsNullOrEmpty! FileFullName: {0} .", fileFullName);
				return false;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(content);
			if (bytes.Length == 0)
			{
				LogHelper.Error("WriteStringToFile failed array size is 0! FileFullName: {0}", fileFullName);
				return false;
			}

			return WriteBytesToFile(bytes, fileFullName, overwrite);
		}

		public static void RenameFile (string fileFullName, string newFileFullName, bool overwrite = true)
		{
//			LogHelper.Info("RenameFile, {0} to {1}", fileFullName, newFileFullName);
			if (!File.Exists(fileFullName))
			{
				LogHelper.Error("Source file {0} does not exist, rename file failed.", fileFullName);
				return;
			}
			if (File.Exists(newFileFullName))
			{
				if (overwrite)
				{
//					LogHelper.Info("{0} exist, delete");
					File.Delete(newFileFullName);
//					LogHelper.Info("After exist, File.Exists(newFileFullName): {0}", File.Exists(newFileFullName));
				}
				else
				{
					LogHelper.Error("File {0} already exist, rename file failed.", newFileFullName);
					return;
				}
			}

			File.Move(fileFullName, newFileFullName);
		}

		public static void CopyFileSync(string sourceFileFullName, string destFileFullName, bool overwrite = true)
		{
			if (!File.Exists(sourceFileFullName))
			{
				LogHelper.Error("Source file {0} does not exist, copy file failed.", sourceFileFullName);
				return;
			}
			if (File.Exists(destFileFullName))
			{
				if (overwrite)
				{
					File.Delete(destFileFullName);
				}
				else
				{
					LogHelper.Error("File {0} already exist, copy file failed.", destFileFullName);
					return;
				}
			}
			File.Copy(sourceFileFullName, destFileFullName);
		}

		public static long GetFileMd5 (string fileFullName, out string md5)
		{
			using (FileStream fs = new FileStream(fileFullName, FileMode.Open))
			{
				MD5 md5Provider = new MD5CryptoServiceProvider();
				byte[] retVal = md5Provider.ComputeHash(fs);
				long size = fs.Length;
				fs.Close();
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				md5 = sb.ToString();
				return size;
			}
		}

        public static string GetBytesMd5 (byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                MD5 md5Provider = new MD5CryptoServiceProvider();
                byte[] retVal = md5Provider.ComputeHash(ms);
                long size = ms.Length;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

		public static bool CheckAndCreateFolder (string path)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			if (!di.Exists)
			{
				di.Create();
				return false;
			}
			return true;
		}

        public static bool DeleteFolder (string path, bool recursive = true)
        {
            DirectoryInfo di = new DirectoryInfo (path);
            if (di.Exists)
            {
                if ((di.GetFiles ().Length != 0 || di.GetDirectories ().Length != 0) && !recursive)
                    return false;
                di.Delete (true);
            }
            return false;
        }

        public static bool DeleteFile (string fullFileName)
        {
            FileInfo fi = new FileInfo (fullFileName);
            {
                if (fi.Exists)
                {
                    fi.Delete ();
                    return true;
                }
            }
            return false;
        }
	}
}