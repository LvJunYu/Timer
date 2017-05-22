/********************************************************************
** Filename : EventTriggerListener
** Author : Dong
** Date : 2015/5/5 15:09:30
** Summary : EventTriggerListener
***********************************************************************/


using UnityEngine;
using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class EventTriggerListener : EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);

        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onSelect;
        public VoidDelegate onUp;
        public VoidDelegate onUpdateSelect;

        public static EventTriggerListener Get(GameObject go)
        {
            return go.GetComponent<EventTriggerListener>() ?? go.AddComponent<EventTriggerListener>();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick(gameObject);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(gameObject);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(gameObject);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(gameObject);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(gameObject);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null) onUpdateSelect(gameObject);
        }
    }
}