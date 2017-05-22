   /********************************************************************
   ** Filename : UIViewMenuInGame.cs
   ** Author : quan
   ** Date : 2015/7/11 20:39
   ** Summary : 游戏内菜单
   ***********************************************************************/


using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameA
{
    [RequireComponent(typeof(Button))]
    [ExecuteInEditMode]
    public class UIDraggableButton: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public class DragEvent : UnityEvent<PointerEventData>
        {

        }
        public RectTransform RectTransform;
        public Button Button;
        private UnityEvent<PointerEventData> _onBeginDrag = new DragEvent();
        private UnityEvent<PointerEventData> _onDrag = new DragEvent();
        private UnityEvent<PointerEventData> _onEndDrag = new DragEvent();

        public UnityEvent<PointerEventData> OnBeginDragEvent
        {
            get{ return _onBeginDrag; }
        }
        public UnityEvent<PointerEventData> OnDragEvent
        {
            get { return _onDrag; }
        }
        public UnityEvent<PointerEventData> OnEndDragEvent
        {
            get { return _onEndDrag; }
        }

        void Awake()
        {
            Button = GetComponent<Button>();
            RectTransform = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _onDrag.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _onBeginDrag.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _onEndDrag.Invoke(eventData);
        }

        void OnDestory()
        {
            _onBeginDrag.RemoveAllListeners();
            _onBeginDrag = null;
            _onDrag.RemoveAllListeners();
            _onDrag = null;
            _onEndDrag.RemoveAllListeners();
            _onEndDrag = null;
        }
    }
}
