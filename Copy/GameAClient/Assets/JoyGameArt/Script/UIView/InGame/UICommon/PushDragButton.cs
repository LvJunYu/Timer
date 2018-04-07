/********************************************************************
** Filename : PushButton  
** Author : ake
** Date : 4/29/2016 10:41:09 AM
** Summary : DragButton  
***********************************************************************/


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoyEngine
{
    public class PushDragButton : ButtonEx
    {
        public Image TargetGraphic;

        public delegate void VoidDelegate ();
        public delegate void PointerEventDelegate (PointerEventData data);

        public VoidDelegate OnPress;
        public VoidDelegate OnRelease;
        public PointerEventDelegate OnDragBegin;
        public PointerEventDelegate OnDragMove;
        public PointerEventDelegate OnDragEnd;

        public RectTransform RectTrans;

        private bool _isPressed = false;
        private bool _isDragged = false;

        public bool IsPressed { 
            get {
                return _isPressed;
            }
        }

        void Awake ()
        {
            RectTrans = GetComponent<RectTransform>();
        }

        void Start ()
        {
            UpdateShow ();
        }

        //public override void OnPointerEnter (PointerEventData eventData)
        //{
        //    Debug.Log ("__________________  OnPointerEnter");
        //}

        //public override void OnPointerExit (PointerEventData eventData)
        //{
        //    Debug.Log ("__________________  OnPointerExit");
        //}

        //public override void OnPointerClick (PointerEventData eventData)
        //{
        //    Debug.Log ("__________________  OnPointerClick");
        //}

        public override void OnPointerDown (PointerEventData eventData)
        {
            //Debug.Log ("__________________  OnPointerDown");
            if (!_isPressed) {
                _isPressed = true;
                UpdateShow ();
                if (OnPress != null) {
                    OnPress ();
                }
                _isDragged = false;
            }
        }

        public override void OnPointerUp (PointerEventData eventData)
        {
            //Debug.Log ("__________________  OnPointerUp");
            if (_isPressed) {
                _isPressed = false;
                UpdateShow ();
                if (OnRelease != null) {
                    OnRelease ();
                }
                if (_isDragged) {
                    _isDragged = false;
                    if (OnDragEnd != null) {
                        OnDragEnd (eventData);
                    }
                }
            }
        }

        public override void OnBeginDrag (PointerEventData eventData)
        {
            //Debug.Log ("__________________  OnBeginDrag");
            if (!_isDragged) {
                _isDragged = true;
                if (OnDragBegin != null) {
                    OnDragBegin (eventData);
                }
            }
        }

        public override void OnEndDrag (PointerEventData eventData)
        {
            //Debug.Log ("__________________  OnEndDrag");
            if (_isDragged) {
                _isDragged = false;
                if (OnDragEnd != null) {
                    OnDragEnd (eventData);
                }
            }
        }

        public override void OnDrag (PointerEventData eventData)
        {
            //Debug.Log ("__________________  OnDrag");
            if (!_isDragged) {
                _isDragged = true;
                if (OnDragBegin != null) {
                    OnDragBegin (eventData);
                }
            }
            if (_isDragged) {
                if (OnDragMove != null) {
                    OnDragMove (eventData);
                }
            }
        }

        //public void SetPressed ()
        //{
        //    if (!_isPressed) {
        //        _isPressed = true;
        //        UpdateShow ();
        //        if (OnPress != null) {
        //            OnPress ();
        //        }
        //    }
        //}

        //public void ReleaseButton ()
        //{
        //    if (_isPressed) {
        //        _isPressed = false;
        //        UpdateShow ();
        //        if (OnRelease != null) {
        //            OnRelease ();
        //        }
        //    }
        //}

        public void ResetState ()
        {
            _isPressed = false;
            _isDragged = false;
            UpdateShow ();
        }

        private void UpdateShow ()
        {
            if (TargetGraphic != null) {
                TargetGraphic.sprite = !_isPressed ? State.highlightedSprite : State.pressedSprite;
            }
        }
    }
}