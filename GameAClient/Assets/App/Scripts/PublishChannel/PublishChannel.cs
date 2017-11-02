using System;

namespace GameA
{
    public class PublishChannel
    {
        public enum EType
        {
            None,
            QQGame,
            Max,
        }

        static PublishChannel()
        {
            Register<PublishChannel>(EType.None);
            Register<ChannelQQGame>(EType.QQGame);
        }

        private static PublishChannel _instance;
        public static PublishChannel Instance
        {
            get { return _instance; }
        }
        private static Type[] _registerArray = new Type[(int) EType.Max];

        private static void Register<T>(EType etype) where T : PublishChannel
        {
            _registerArray[(int) etype] = typeof(T);
        }

        public static void Init(EType type)
        {
            if (_instance != null)
            {
                throw new Exception("PublishChannel Has Inited");
            }
            if (type == EType.Max)
            {
                throw new ArgumentException("not support");
            }
            Type t = _registerArray[(int) type];
            if (t == null)
            {
                throw new ArgumentException(type.ToString()+" don't has Implementation");
            }
            _instance = (PublishChannel) Activator.CreateInstance(t);
            _instance.Init();
        }

        protected virtual void Init()
        {
            
        }

        public virtual void Update()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}