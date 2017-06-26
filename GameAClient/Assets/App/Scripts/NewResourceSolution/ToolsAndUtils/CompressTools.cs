using System;
using System.IO;
using SevenZip.Compression.LZMA;
using SoyEngine;

namespace NewResourceSolution
{
	public static class CompressTools
	{
		public static void CompressFileLZMA(string inFile, string outFile)
		{
			using (FileStream input = new FileStream(inFile, FileMode.Open))
			using (FileStream output = new FileStream(outFile, FileMode.Create))
			{
				Encoder coder = new Encoder();
				// Write the encoder properties
				coder.WriteCoderProperties(output);
				// Write the decompressed file size.
				output.Write(BitConverter.GetBytes(input.Length), 0, 8);
				// Encode the file.
				coder.Code(input, output, input.Length, -1, null);
			}
		}

        public static void DecompressFileLZMA(string inFile, string outFile, bool overwrite = true)
		{
//            LogHelper.Info ("DecompressFile, {0} to {1}", inFile, outFile);
            FileInfo infi = new FileInfo(inFile);
            if (!infi.Exists)
            {
                LogHelper.Error("File {0} doesn't exist.", inFile);
                return;
            }
            FileInfo outfi = new FileInfo(outFile);
            if (outfi.Exists)
            {
                if (overwrite)
                {
                    File.Delete(outFile);
                }
                else
                {
                    LogHelper.Error("File {0} already exist, decompress file failed.", outFile);
                    return;
                }
            }
            using (FileStream input = new FileStream (inFile, FileMode.Open))
            using(FileStream output = new FileStream (outFile, FileMode.Create))
            {
                Decoder coder = new Decoder();
                // Read the decoder properties
                byte[] properties = new byte[5];
                input.Read (properties, 0, 5);

                // Read in the decompress file size.
                byte[] fileLengthBytes = new byte[8];
                input.Read (fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64 (fileLengthBytes, 0);

                // Decompress the file.
                coder.SetDecoderProperties (properties);
                coder.Code (input, output, input.Length, fileLength, null);

                input.Dispose ();
                output.Dispose ();
            }
		}

        public static void DecompressBytesLZMA (byte[] bytes, string outFile, bool overwrite = true)
        {
            FileInfo outfi = new FileInfo(outFile);
            if (outfi.Exists)
            {
                if (overwrite)
                {
                    File.Delete(outFile);
                }
                else
                {
                    LogHelper.Error("File {0} already exist, decompress file failed.", outFile);
                    return;
                }
            }
            using(MemoryStream input = new MemoryStream(bytes))
            using(FileStream output = new FileStream (outFile, FileMode.Create))
            {
                Decoder coder = new Decoder();
                // Read the decoder properties
                byte[] properties = new byte[5];
                input.Read (properties, 0, 5);

                // Read in the decompress file size.
                byte[] fileLengthBytes = new byte[8];
                input.Read (fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64 (fileLengthBytes, 0);

                // Decompress the file.
                coder.SetDecoderProperties (properties);
                coder.Code (input, output, input.Length, fileLength, null);

                input.Dispose ();
                output.Dispose ();
            }
        }
	}
}