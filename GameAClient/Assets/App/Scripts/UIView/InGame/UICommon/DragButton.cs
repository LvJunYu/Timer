/********************************************************************
** Filename : DragButton  
** Author : ake
** Date : 4/29/2016 10:41:09 AM
** Summary : DragButton  
***********************************************************************/


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoyEngine
{
    public class DragButton : ButtonEx
    {
        public int Index;
        public Image TargetGraphic;

        public delegate void VoidDelegate();

        public VoidDelegate OnPress;
        public VoidDelegate OnRelease;

        private Transform _cachedTransform;
        private DragButtonGroup _group;

        private bool _isPressed = false;

        void Awake()
        {
            _cachedTransform = transform;
            if (_cachedTransform != null && _cachedTransform.parent != null)
            {
                _group = _cachedTransform.parent.GetComponent<DragButtonGroup>();
            }
        }

        void Start()
        {
            UpdateShow();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!CheckCanEnter(this))
            {
                return;
            }
            SetPressed();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {

        }

        public override void OnPointerClick(PointerEventData eventData)
        {
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!CheckCanClick(this))
            {
                return;
            }
            SetPressed();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            OnButtonRelease();
        }

        public void SetPressed()
        {
            if (_isPressed)
            {
                LogHelper.Warning("SetPressed called but _isPressed is true ,go name is {0} index is {1}", name, Index);
                return;
            }
            _isPressed = true;
            UpdateShow();
            SetCurSelectButtonToGroup(this);
            if (OnPress != null)
            {
                OnPress();
            }
        }

        public void ReleaseButton()
        {
            if (!_isPressed)
            {
                LogHelper.Warning("ReleaseButton called but _isPressed is false ,go name is {0} index is {1}",name,Index);
                return;
            }
            _isPressed = false;
            UpdateShow();
            if (OnRelease != null)
            {
                OnRelease();
            }
        }

        public void ResetState ()
        {
            _isPressed = false;
            UpdateShow ();
            if (_group != null) {
                _group.ResetState ();
            }
        }

        private void UpdateShow()
        {
            if (TargetGraphic != null)
            {
                TargetGraphic.sprite = !_isPressed ? State.highlightedSprite : State.pressedSprite;
            }
        }

        private bool CheckCanClick(DragButton button)
        {
            if (_group == null)
            {
                return false;
            }
            return _group.CheckCanClick();
        }

        private bool CheckCanEnter(DragButton button)
        {
            if (_group == null)
            {
                return false;
            }
            return _group.CheckCanEnter(button);
        }

        private void SetCurSelectButtonToGroup(DragButton button)
        {
            if (_group != null)
            {
                _group.SetCurSelectButton(button);
            }
        }

        private void OnButtonRelease()
        {
            if (_group != null)
            {
                _group.ReleaseButton();
            }
        }
    }
}