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
    public class PushButton : ButtonEx
    {
        public Image TargetGraphic;

        public delegate void VoidDelegate ();

        public VoidDelegate OnPress;
        public VoidDelegate OnRelease;

        private Transform _cachedTransform;

        private bool _isPressed = false;

        void Awake ()
        {
            _cachedTransform = transform;
        }

        void Start ()
        {
            UpdateShow ();
        }

        public override void OnPointerEnter (PointerEventData eventData)
        {
            SetPressed ();
        }

        public override void OnPointerExit (PointerEventData eventData)
        {
            ReleaseButton ();
        }

        public override void OnPointerClick (PointerEventData eventData)
        {
        }

        public override void OnPointerDown (PointerEventData eventData)
        {

            SetPressed ();
        }

        public override void OnPointerUp (PointerEventData eventData)
        {
            ReleaseButton ();
        }

        public void SetPressed ()
        {
            if (_isPressed) {
                LogHelper.Warning ("SetPressed called but _isPressed is true ,go name is {0}", name);
                return;
            }
            _isPressed = true;
            UpdateShow ();
            if (OnPress != null) {
                OnPress ();
            }
        }

        public void ReleaseButton ()
        {
            if (!_isPressed) {
                LogHelper.Warning ("ReleaseButton called but _isPressed is false ,go name is {0}", name);
                return;
            }
            _isPressed = false;
            UpdateShow ();
            if (OnRelease != null) {
                OnRelease ();
            }
        }

        public void ResetState ()
        {
            _isPressed = false;
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