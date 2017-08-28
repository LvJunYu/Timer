using SoyEngine;

namespace GameA.Game
{
    public class AdventureGuideBase
    {
        public virtual void Init()
        {
            LogHelper.Info("Guide {0} Init", GetType().Name);
        }

        public virtual void UpdateLogic()
        {
            
        }

        public virtual void Dispose()
        {
            
        }
    }
}