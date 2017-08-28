using System;
using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class AdventureGuideManager
    {
        public static readonly AdventureGuideManager Instance = new AdventureGuideManager();
        private readonly Dictionary<string, Type> _adventureGuideHanderDict = new Dictionary<string, Type>();

        public AdventureGuideManager()
        {
            Regist<AdventureGuide_1_N_1>(1, EAdventureProjectType.APT_Normal, 1);
        }

        public bool TryGetGuide(int section, EAdventureProjectType adventureProjectType, int level,
            out AdventureGuideBase hander)
        {
            var key = GetKey(section, adventureProjectType, level);
            Type type;
            if (!_adventureGuideHanderDict.TryGetValue(key, out type))
            {
                hander = null;
                return false;
            }
            hander = Activator.CreateInstance(type) as AdventureGuideBase;
            return true;
        }

        public void Regist<T>(int section, EAdventureProjectType adventureProjectType, int level) where T : AdventureGuideBase
        {
            _adventureGuideHanderDict.Add(GetKey(section, adventureProjectType, level), typeof(T));
        }

        private string GetKey(int section, EAdventureProjectType adventureProjectType, int level)
        {
            return string.Format("{0}|{1}|{2}", section, (int) adventureProjectType, level);
        }
    }
}