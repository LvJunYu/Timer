/********************************************************************
** Filename : EventTriggerListener
** Author : Dong
** Date : 2015/5/5 15:09:30
** Summary : EventTriggerListener
***********************************************************************/


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class EventTriggerListener : EventTrigger
    {
        public static EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener eventTriggerListener = go.GetComponent<EventTriggerListener>()
                                                        ?? go.AddComponent<EventTriggerListener>();
            return eventTriggerListener;
        }
        
        public void AddListener(EventTriggerType type, UnityAction<BaseEventData> call)
        {
            var te = triggers.Find(t => t.eventID == type);
            if (null == te)
            {
                te = new Entry {eventID = type};
                triggers.Add(te);
            }
            te.callback.AddListener(call);
        }
        
        public void RemoveListener(EventTriggerType type, UnityAction<BaseEventData> call)
        {
            Entry te = triggers.Find(t => t.eventID == type);
            if (null == te)
            {
                return;
            }
            te.callback.RemoveListener(call);
        }

        public void RemoveAllListener(EventTriggerType type)
        {
            Entry te = triggers.Find(t => t.eventID == type);
            if (null == te)
            {
                return;
            }
            te.callback.RemoveAllListeners();
        }

        public void RemoveAllListener()
        {
            triggers.Clear();
        }
    }
}