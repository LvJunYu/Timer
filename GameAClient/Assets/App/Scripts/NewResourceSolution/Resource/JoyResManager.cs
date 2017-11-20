using GameA;
using UnityEngine;

namespace NewResourceSolution
{
    public class JoyResManager : ResourcesManager
    {
        public static JoyResManager Instance = new JoyResManager();

        private EResScenary _defaultResScenary;

        public EResScenary DefaultResScenary
        {
            get { return _defaultResScenary; }
        }

        public void SetDefaultResScenary(EResScenary resScenary)
        {
            _defaultResScenary = resScenary;
        }
        
		public Object GetPrefab(EResType resType, string name)
		{
		    return GetPrefab(resType, name, (int) _defaultResScenary);
		}

        public bool TryGetTexture(string name, out Texture texture)
        {
            return TryGetTexture(name, out texture, (int) _defaultResScenary);
        }

        public bool TryGetSprite(string name, out Sprite sprite)
        {
            return TryGetSprite(name, out sprite, (int) _defaultResScenary);
        }

        public Texture GetTexture (string name)
        {
            return GetTexture(name, (int) _defaultResScenary);
        }

        public Sprite GetSprite (string name)
        {
            return GetSprite(name, (int) _defaultResScenary);
        }

        public AudioClip GetAudio (string name)
        {
            return GetAudio(name, (int) _defaultResScenary);
        }

        public string GetJson (string name)
        {
            return GetJson(name, (int) _defaultResScenary);
        }
        
        public T GetAsset<T> (
            EResType resType,
            string name
        ) where T : Object
        {
            return GetAsset<T>(resType, name, (int) _defaultResScenary);
        }
        
        public void UnloadScenary (EResScenary scenary, long resTypeMask = -1L)
        {
            UnloadScenary((int) scenary, resTypeMask);
        }
    }
}