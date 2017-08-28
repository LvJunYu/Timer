using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class TriggerUnitEventManager
    {
        public static readonly TriggerUnitEventManager Instance = new TriggerUnitEventManager();
        private static EventRegister _currentEventRegister;
        private readonly Dictionary<String, bool> _triggerStateDict = new Dictionary<string, bool>();
        private readonly Dictionary<String, HashSet<Action<bool>>> _triggerListenerDict = new Dictionary<string, HashSet<Action<bool>>>();
        
        private TriggerUnitEventManager()
        {
            Messenger<string, bool>.AddListener(EMessengerType.OnTrigger, Handler);
        }

        public bool GetTriggerState(string triggerEventName)
        {
            bool state;
            if (!_triggerStateDict.TryGetValue(triggerEventName, out state))
            {
                return false;
            }
            return state;
        }

        public EventRegister BeginRegistEvent()
        {
            if (_currentEventRegister != null)
            {
                LogHelper.Error("_currentEventRegister not null");
                return null;
            }
            _currentEventRegister = new EventRegister();
            return _currentEventRegister;
        }
        

        private void Handler(string s, bool b)
        {
            bool state;
            if (!_triggerStateDict.TryGetValue(s, out state))
            {
                state = false;
                _triggerStateDict.Add(s, state);
            }
            if (b != state)
            {
                _triggerStateDict[s] = b;
                BroadcastEvent(s, b);
            }
        }

        private void BroadcastEvent(string s, bool b)
        {
            HashSet<Action<bool>> callbackSet;
            if (!_triggerListenerDict.TryGetValue(s, out callbackSet))
            {
                return;
            }
            using (var iter = callbackSet.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    if (iter.Current != null)
                    {
                        iter.Current.Invoke(b);
                    }
                }
            }
        }

        private void AddEventListener(string eventName, Action<bool> callback)
        {
            HashSet<Action<bool>> cbSet;
            if (!_triggerListenerDict.TryGetValue(eventName, out cbSet))
            {
                cbSet = new HashSet<Action<bool>>();
                _triggerListenerDict.Add(eventName, cbSet);
            }
            cbSet.Add(callback);
        }
        
        private void RemoveEventListener(string eventName, Action<bool> callback)
        {
            HashSet<Action<bool>> cbSet;
            if (!_triggerListenerDict.TryGetValue(eventName, out cbSet))
            {
                LogHelper.Error("RemoveEventListener NotRegist");
                return;
            }
            cbSet.Remove(callback);
            if (cbSet.Count == 0)
            {
                _triggerListenerDict.Remove(eventName);
            }
        }
        
        public class EventRegister
        {
            private static readonly List<Action> TempUnregisterActionList = new List<Action>(16);
            private Action[] _unRegisterActionAry;

            public EventRegister RegistEvent(string eventName, Action<bool> callback)
            {
                if (_currentEventRegister != this)
                {
                    LogHelper.Error("EventRegister RegistEvent Error, Not Begin or Has End");
                    return this;
                }
                if (string.IsNullOrEmpty(eventName) || callback == null)
                {
                    LogHelper.Error("EventRegister RegistEvent Error, eventName is empty or callback is null");
                    return this;
                }
                Instance.AddEventListener(eventName, callback);
                TempUnregisterActionList.Add(()=>{Instance.RemoveEventListener(eventName, callback);});
                return this;
            }

            public EventRegister End()
            {
                if (_currentEventRegister != this)
                {
                    LogHelper.Error("EventRegister End Error, Not Begin or Has End");
                    return this;
                }
                if (TempUnregisterActionList.Count == 0)
                {
                    LogHelper.Error("EventRegister End Error, Not RegistEvent");
                    return this;
                }
                _unRegisterActionAry = TempUnregisterActionList.ToArray();
                TempUnregisterActionList.Clear();
                return this;
            }

            public void UnregisterAllEvent()
            {
                if (_unRegisterActionAry == null)
                {
                    return;
                }
                for (int i = 0; i < _unRegisterActionAry.Length; i++)
                {
                    try
                    {
                        _unRegisterActionAry[i].Invoke();
                    }
                    catch (Exception e)
                    {
                        LogHelper.Warning(e.ToString());
                    }
                }
                _unRegisterActionAry = null;
            }
        }
    }
}