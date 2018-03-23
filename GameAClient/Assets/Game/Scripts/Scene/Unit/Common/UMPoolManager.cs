using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMPoolManager : MonoBehaviour
    {
        private static UMPoolManager _instance;
        private Dictionary<string, List<IUMPoolable>> _cacheDic = new Dictionary<string, List<IUMPoolable>>();

        public static UMPoolManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = UGUITools.CreateUIGroupObject(SocialGUIManager.Instance.UIRoot.transform).gameObject
                        .AddComponent<UMPoolManager>();
                    _instance.gameObject.name = "UMPoolManager";
                }

                return _instance;
            }
        }

        public T Get<T>(RectTransform rectTransform, EResScenary resScenary) where T : class, IUMPoolable, new()
        {
            List<IUMPoolable> umCache;
            if (!_cacheDic.TryGetValue(typeof(T).Name, out umCache))
            {
                _cacheDic.Add(typeof(T).Name, new List<IUMPoolable>(16));
                umCache = _cacheDic[typeof(T).Name];
            }

            var item = umCache.Find(p => !p.IsShow);
            if (item == null)
            {
                item = new T();
                item.Init(rectTransform, resScenary);
               
                umCache.Add(item);
            }
            else
            {
                item.Show();
                item.SetParent(rectTransform);
            }

            return item as T;
        }

        public void Free<T>(T um) where T : IUMPoolable
        {
            um.SetParent(this.rectTransform());
            um.Hide();
        }

        private void OnDestroy()
        {
            foreach (var value in _cacheDic.Values)
            {
                value.ForEach(p => p.Destroy());
            }

            _cacheDic.Clear();
            _instance = null;
        }
    }

    public interface IUMPoolable
    {
        bool IsShow { get; }
        void Show();
        void Hide();
        bool Init(RectTransform rectTransform, EResScenary resScenary, Vector3 localpos = new Vector3());
        void SetParent(RectTransform rectTransform);
        void Destroy();
    }
}