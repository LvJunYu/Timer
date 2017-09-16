using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class AdventureGuideBase
    {
        private const string HandbookEventPrefix = "Book_";
        protected TriggerUnitEventManager.EventRegister _eventRegister;
        private readonly HashSet<int> _handbookShowSet = new HashSet<int>();
        
        public virtual void Init()
        {
            LogHelper.Info("Guide {0} Init", GetType().Name);
            Messenger<string, bool>.AddListener(EMessengerType.OnTrigger, HandleHandbook);
        }

        private void HandleHandbook(string triggerName, bool active)
        {
            if (!active)
            {
                return;
            }
            if (!triggerName.StartsWith(HandbookEventPrefix))
            {
                return;
            }
            int id;
            if (!int.TryParse(triggerName.Substring(HandbookEventPrefix.Length), out id))
            {
                return;
            }
            if (_handbookShowSet.Contains(id))
            {
                return;
            }
            _handbookShowSet.Add(id);
            SocialGUIManager.Instance.OpenUI<UICtrlInGameUnitHandbook>(id);
        }

        public virtual void Update()
        {
            
        }

        public virtual void UpdateLogic()
        {
            
        }
        
        public virtual void Dispose()
        {
            Messenger<string, bool>.RemoveListener(EMessengerType.OnTrigger, HandleHandbook);
            TriggerUnitEventManager.Instance.ClearAllTriggerState();
            if (_eventRegister != null)
            {
                _eventRegister.UnregisterAllEvent();
            }
        }
    }
}