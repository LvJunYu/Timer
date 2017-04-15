  /********************************************************************
  ** Filename : UIPersonalProjectScrollHelper.cs
  ** Author : quan
  ** Date : 11/28/2016 4:44 PM
  ** Summary : UIPersonalProjectScrollHelper.cs
  ***********************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SoyEngine;

namespace GameA
{
    public class UIPersonalProjectScrollHelper : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private ScrollRectEx _mainScroll;
        [SerializeField]
        private ScrollRectEx _innerScroll;
        private ScrollRectEx _currentScroll;

        void Start()
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.delta.y >= 0)
            {
                if(!_innerScroll.vScrollingNeeded)
                {
                    ChangeToScroll(_mainScroll, eventData);
                }
                else if(_mainScroll.verticalNormalizedPosition < 0.0001f)
                {
                    ChangeToScroll(_innerScroll, eventData);
                }
                else
                {
                    ChangeToScroll(_mainScroll, eventData);
                }
            }
            else
            {
                if(!_innerScroll.vScrollingNeeded)
                {
                    ChangeToScroll(_mainScroll, eventData);
                }
                else if(_innerScroll.verticalNormalizedPosition > 0.9999f)
                {
                    ChangeToScroll(_mainScroll, eventData);
                }
                else
                {
                    ChangeToScroll(_innerScroll, eventData);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            bool isDirty = false;
            if (eventData.delta.y >= 0)
            {
                if(_mainScroll.verticalNormalizedPosition < 0.0001f)
                {
                    isDirty = ChangeToScroll(_innerScroll, eventData);
                }
                else
                {
                    isDirty = ChangeToScroll(_mainScroll, eventData);
                }
            }
            else
            {
                if(_innerScroll.verticalNormalizedPosition > 0.9999f)
                {
                    isDirty = ChangeToScroll(_mainScroll, eventData);
                }
                else
                {
                    isDirty = ChangeToScroll(_innerScroll, eventData);
                }
            }
            if(!isDirty && _currentScroll != null)
            {
                _currentScroll.OnDrag(eventData);
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (_currentScroll != null)
            {
                _currentScroll.OnEndDrag(eventData);
            }
            _currentScroll = null;
        }

        private bool ChangeToScroll(ScrollRectEx scrollRect, PointerEventData eventData)
        {
            if(_currentScroll == scrollRect)
            {
                return false;
            }
            if(_currentScroll != null)
            {
                _currentScroll.OnEndDrag(eventData);
            }
            _currentScroll = scrollRect;
            if(_currentScroll != null)
            {
                _currentScroll.OnBeginDrag(eventData);
            }
            return true;
        }
    }
}