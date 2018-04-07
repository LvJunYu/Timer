using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityEditor.XCodeEditor
{
    public partial class XCClass : System.IDisposable
    {
        private string filePath;
        private string fullText;

        public string FullText
        {
            get {
                return fullText;
            }
        }

        public XCClass(string fPath)
        {
            filePath = fPath;
            if( !System.IO.File.Exists( filePath ) ) {
                Debug.LogError( filePath +"路径下文件不存在" );
                return;
            }

            StreamReader streamReader = new StreamReader(filePath);
            fullText = streamReader.ReadToEnd();
            streamReader.Close();
        }

        public void WriteBelow(string below, string text, int startInx = 0)
        {
            int beginIndex = fullText.IndexOf(below, startInx);
            if(beginIndex == -1){
                Debug.LogError(filePath +"中没有找到标致"+below);
                return; 
            }

            int endIndex = fullText.LastIndexOf("\n", beginIndex + below.Length);

            fullText = fullText.Substring(0, endIndex) + "\n"+text+"\n" + fullText.Substring(endIndex);
        }

        public void Replace(string below, string newText, int startInx = 0)
        {
            int beginIndex = fullText.IndexOf(below, startInx);
            if(beginIndex == -1){
                Debug.LogError(filePath +"中没有找到标致"+below);
                return; 
            }

            int endIndex = startInx + below.Length;

            fullText = fullText.Substring(0, endIndex) + "\n"+newText+"\n" + fullText.Substring(endIndex);
        }

        public void ReplaceRegex(string pattern, string text, int startInx=0)
        {
            string head = fullText.Substring(0, startInx);
            string tail = fullText.Substring(startInx);
            tail = Regex.Replace(tail, pattern, text);
            fullText = head + tail;
        }

        public void Remove(int startInx, int len)
        {
            fullText = fullText.Remove(startInx, len);
        }

        public void Insert(int startInx, string text)
        {
            fullText = fullText.Insert(startInx, text);
        }

        public void Save()
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(fullText);
            streamWriter.Close();
        }

        public void Dispose()
        {
            
        }
    }
}