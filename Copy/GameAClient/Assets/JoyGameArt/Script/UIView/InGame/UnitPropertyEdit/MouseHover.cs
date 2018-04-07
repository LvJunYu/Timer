using GameA.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameA
{
    public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private UnityAction _mouseHoverCallback;
        private UnityAction _mouseHoverLeaveCallback;
        private bool _startComputingTime = false;
        private float _time;
        private float _maxTime;

        private void Update()
        {
            if (_startComputingTime)
            {
                _time++;
            }
            if (_time > _maxTime)
            {
                _startComputingTime = false;
                _time = 0.0f;
                if (_mouseHoverCallback != null)
                {
                    _mouseHoverCallback.Invoke();
                }
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _startComputingTime = true;
            _time = 0.0f;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _startComputingTime = false;
            _time = 0.0f;
            if (_mouseHoverLeaveCallback != null)
            {
                _mouseHoverLeaveCallback.Invoke();
            }
        }

        public void SetCallback(float second, UnityAction hovercallback, UnityAction leavecallback)
        {
            _maxTime = second * 30f;
            _mouseHoverCallback = hovercallback;
            _mouseHoverLeaveCallback = leavecallback;
            _startComputingTime = false;
            _time = 0.0f;
        }

        public void RemoveCallBack()
        {
            _maxTime = 0.0f;
            _mouseHoverCallback = null;
            _mouseHoverLeaveCallback = null;
            _startComputingTime = false;
            _time = 0.0f;
        }
    }
}