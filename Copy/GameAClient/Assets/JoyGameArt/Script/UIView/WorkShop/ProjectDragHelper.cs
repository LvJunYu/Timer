using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameA
{
    public class ProjectDragHelper : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler,
        IEndDragHandler,
        IPointerUpHandler
    {
        public Action OnBeginDragAction;
        public Action OnEndDragAction;

        public Action OnDragAction;

//        private Vector2 _addPos = Vector2.zero;
        private int _dragtype;
        public ScrollRect ScrollRect;
        private bool _startTime;
        private float _time = 0.0f;

        private void Update()
        {
            if (_startTime)
            {
                _time += Time.deltaTime;
                if (_time > 1.0f)
                {
                    if (OnBeginDragAction != null)
                    {
                        OnBeginDragAction.Invoke();
                        _dragtype = 1;
                        ResetTime();
                    }
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragtype == 1)
            {
                (transform as RectTransform).anchoredPosition += eventData.delta;
                if (OnDragAction != null)
                {
                    OnDragAction.Invoke();
                }
            }

            if (_dragtype == 2)
            {
                if (ScrollRect != null)
                {
                    ScrollRect.OnDrag(eventData);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ResetTime();
            if (_dragtype == 0)
            {
                if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
                {
                    if (OnBeginDragAction != null)
                    {
                        OnBeginDragAction.Invoke();
                    }

                    _dragtype = 1;
                }
                else
                {
                    _dragtype = 2;
                    ScrollRect.OnBeginDrag(eventData);
                }

                if (OnBeginDragAction == null)
                {
                    _dragtype = 2;
                    ScrollRect.OnBeginDrag(eventData);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragtype == 1)
            {
                if (OnEndDragAction != null)
                {
                    OnEndDragAction.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _startTime = true;
            _time = 0.0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ResetTime();
            if (OnEndDragAction != null && _dragtype == 1)
            {
                OnEndDragAction.Invoke();
            }

            if (_dragtype == 2)
            {
                ScrollRect.OnEndDrag(eventData);
            }

            _dragtype = 0;
        }

        private void ResetTime()
        {
            _startTime = false;
            _time = 0.0f;
        }
    }
}