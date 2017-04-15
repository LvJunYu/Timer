using System;
using System.IO;


namespace SevenZip
{
    class SevenZipTool
    {
        //[MenuItem("MyMenu/CompressFile")]
        //static void CompressFile()
        //{
        //    //压缩文件
        //    CompressFileLZMA(Application.dataPath + "/1.jpg", Application.dataPath + "/2.zip");
        //    AssetDatabase.Refresh();

        //}
        //[MenuItem("MyMenu/DecompressFile")]
        //static void DecompressFile()
        //{
        //    //解压文件
        //    DecompressFileLZMA(Application.dataPath + "/2.zip", Application.dataPath + "/3.jpg");
        //    AssetDatabase.Refresh();
        //}


        public static void CompressFileLZMA(string inFile, string outFile)
        {
            LzmaEncoder coder = new LzmaEncoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
        }

        public static void DecompressFileLZMAFromStream(byte[] data, FileStream output,ICodeProgress progress)
        {
            LzmaDecoder coder = new LzmaDecoder();
            MemoryStream input = new MemoryStream(data);

            //// Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            //// Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            //// Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, progress);
        }

        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            LzmaDecoder coder = new LzmaDecoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
        }
    }
}
