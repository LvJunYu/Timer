using SoyEngine;

namespace GameA.Game
{
    public class AdventureGuideBase
    {
        protected TriggerUnitEventManager.EventRegister _eventRegister;
        
        public virtual void Init()
        {
            LogHelper.Info("Guide {0} Init", GetType().Name);
        }

        public virtual void Update()
        {
            
        }

        public virtual void UpdateLogic()
        {
            
        }

        public virtual void Dispose()
        {
            TriggerUnitEventManager.Instance.ClearAllTriggerState();
            if (_eventRegister != null)
            {
                _eventRegister.UnregisterAllEvent();
            }
        }
    }
}