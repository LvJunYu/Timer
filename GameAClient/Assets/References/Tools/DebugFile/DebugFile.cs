using System.Collections.Generic;
using System.IO;

namespace SoyEngine
{
    public abstract class DebugFile
    {
        private static Dictionary<string, DebugFile> _dict = new Dictionary<string, DebugFile>();

        public virtual bool Enable
        {
            get { return true; }
        }
        
        public static DebugFile Get(string name)
        {
            return _dict[name];
        }
        
        public static DebugFile Create(string name, string path)
        {
            DebugFile debugFile;
            if (_dict.TryGetValue(name, out debugFile))
            {
                LogHelper.Error("Debug File Has Exist, Name: {0}", name);
                return debugFile;
            }
            return InternalCreate(name, path);
        }

        public static DebugFile GetOrCreate(string name, string path)
        {
            DebugFile debugFile;
            if (_dict.TryGetValue(name, out debugFile))
            {
                return debugFile;
            }
            return InternalCreate(name, path);
        }

        private static DebugFile InternalCreate(string name, string path)
        {
            DebugFile debugFile;
            if (LogHelper.LogLevel > LogHelper.ELogLevel.Debug)
            {
                debugFile = new DefaultDebugFile(name, path);
            }
            else
            {
                debugFile = new EmptyDebugFile(name, path);
            }
            _dict.Add(name, debugFile);
            return debugFile;
        }


        protected string _name;

        private DebugFile(string name, string path)
        {
            _name = name;
        }

        public abstract void Write(string str);
        public abstract void WriteLine(string str);

        public virtual void Close()
        {
            if (_dict.ContainsKey(_name))
            {
                _dict.Remove(_name);
            }
        }
    
        private class EmptyDebugFile : DebugFile
        {
            public override bool Enable
            {
                get { return false; }
            }

            public EmptyDebugFile(string name, string path) : base(name, path)
            {
            }
            
            public override void Write(string str)
            {
            }

            public override void WriteLine(string str)
            {
            }
        }
        
        private class DefaultDebugFile : DebugFile
        {
            private StreamWriter _streamWriter;
            public DefaultDebugFile(string name, string path) : base(name, path)
            {
                _streamWriter = new StreamWriter(new FileStream(path, FileMode.Create));
            }

            public override void Write(string str)
            {
                if (_streamWriter == null)
                {
                    return;
                }
                _streamWriter.Write(str);
            }

            public override void WriteLine(string str)
            {
                if (_streamWriter == null)
                {
                    return;
                }
                _streamWriter.WriteLine(str);
            }

            public override void Close()
            {
                DebugFile debugFile;
                if (!_dict.TryGetValue(_name, out debugFile))
                {
                    return;
                }
                if (debugFile != this)
                {
                    return;
                }
                _streamWriter.Close();
                _streamWriter = null;
                base.Close();
            }
        }
        
    }
}