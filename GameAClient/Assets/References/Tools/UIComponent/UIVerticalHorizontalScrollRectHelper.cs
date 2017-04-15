/********************************************************************
** Filename : UIVerticalHorizontalScrollRectHelper
** Author : quansiwei
** Date : 15/9/29 下午12:23:09
** Summary : UIVerticalHorizontalScrollRectHelper
***********************************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class UIVerticalHorizontalScrollRectHelper : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        public GameObject VerticalScroll;
        [SerializeField]
        public GameObject HorizontalScroll;

        private IBeginDragHandler[] _aryBeginDragHandler;
        private IDragHandler[] _aryDragHandler;
        private IEndDragHandler[] _aryEndDragHandler;
        private int _curInx = 0;//0 = H, 1 = V;

        void Start()
        {
            _aryBeginDragHandler = new IBeginDragHandler[2];
            _aryDragHandler = new IDragHandler[2];
            _aryEndDragHandler = new IEndDragHandler[2];
            InitHandler(HorizontalScroll, 0);
            InitHandler(VerticalScroll, 1);
        }

        private void InitHandler(GameObject go, int inx)
        {
            if (go == null)
            {
                return;
            }
            var bda = go.GetComponents<IBeginDragHandler>();
            _aryBeginDragHandler[inx] = Array.Find(bda, handler => handler != this);
            var da = go.GetComponents<IDragHandler>();
            _aryDragHandler[inx] = Array.Find(da, handler => handler != this);
            var eda = go.GetComponents<IEndDragHandler>();
            _aryEndDragHandler[inx] = Array.Find(eda, handler => handler != this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y))
            {
                _curInx = 1;
            }
            else
            {
                _curInx = 0;
            }
            if (_aryBeginDragHandler[_curInx] != null)
            {
                _aryBeginDragHandler[_curInx].OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_aryDragHandler[_curInx] != null)
            {
                _aryDragHandler[_curInx].OnDrag(eventData);
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (_aryEndDragHandler[_curInx] != null)
            {
                _aryEndDragHandler[_curInx].OnEndDrag(eventData);
            }
        }
    }
}