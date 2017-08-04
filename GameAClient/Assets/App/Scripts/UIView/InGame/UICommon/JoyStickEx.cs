using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using GameA.Game;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SoyEngine;
using SoyEngine.Proto;
using Spine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    public class JoyStickEx : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static float s_minDeltaDistance = 0.2f;
        
        public GameObject DirectionImg;
        public GameObject RightArrowPressed;
        public GameObject LeftArrowPressed;
        public GameObject UpArrowPressed;
        public GameObject DownArrowPresseds;

        public GameObject RightArrowNormal;
        public GameObject LeftArrowNormal;
        public GameObject UpArrowNormal;
        public GameObject DownArrowNormal;

        private float _width;
        private float _height;

        private Vector2 _centerPos;

        private bool _hovering = false;
        private PointerEventData _hoveringEventData;

        void Start()
        {
            RectTransform rectTrans = transform as RectTransform;
            _width = rectTrans.GetWidth();
            _height = rectTrans.GetHeight();
            _centerPos = new Vector2(_width * 0.5f, _height * 0.5f);
            _hovering = false;
        }

        void Update()
        {
            if (_hovering)
            {
                DirectionImg.SetActive(true);
                Vector2 inputPos;
#if (((UNITY_ANDROID || UNITY_IPHONE || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR))
                var touch = UnityEngine.Input.GetTouch(_hoveringEventData.pointerId);
                inputPos = touch.position;
                #else
                inputPos = Input.mousePosition;
#endif

                Vector2 localPos;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, inputPos,
                    _hoveringEventData.enterEventCamera, out localPos))
                    return;
                Vector2 delta = localPos - _centerPos;
                HandleDeltaPosition(delta);
            }
            else
            {
                DirectionImg.SetActive(false);
            }
        }
        public void OnDrag(PointerEventData data)
        {
            Vector2 localPos;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, data.position, data.pressEventCamera, out localPos))
                return;

            Vector2 delta = localPos - _centerPos;
            HandleDeltaPosition(delta);
        }

        public void OnPointerEnter (PointerEventData data)
        {
            _hoveringEventData = data;
            _hovering = true;
            Vector2 localPos;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, data.position, data.enterEventCamera, out localPos))
                return;

            Vector2 delta = localPos - _centerPos;
            HandleDeltaPosition(delta);
        }

        public void OnPointerExit (PointerEventData data)
        {
            _hovering = false;
            HandleDeltaPosition(Vector2.zero);
        }

        public void OnPointerUp(PointerEventData data)
        {
            _hovering = false;
            HandleDeltaPosition(Vector2.zero);
        }


        public void OnPointerDown(PointerEventData data)
        {
            _hovering = true;
            Vector2 localPos;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, data.position, data.pressEventCamera, out localPos))
                return;

            Vector2 delta = localPos - _centerPos;
            HandleDeltaPosition(delta);
        }

        private void HandleDeltaPosition(Vector2 deltaPos)
        {
            deltaPos.x /= _width * 0.5f;
            deltaPos.y /= _height * 0.5f;
            deltaPos.x = Mathf.Clamp(deltaPos.x, -1f, 1f);
            deltaPos.y = Mathf.Clamp(deltaPos.y, -1f, 1f);

            if (Mathf.Abs(deltaPos.x) < s_minDeltaDistance) deltaPos.x = 0;
            if (Mathf.Abs(deltaPos.y) < s_minDeltaDistance) deltaPos.y = 0;
            CrossPlatformInputManager.SetAxis(InputManager.TagHorizontal, deltaPos.x);
            CrossPlatformInputManager.SetAxis(InputManager.TagVertical, deltaPos.y);

            RightArrowPressed.SetActive(deltaPos.x > 0);
            RightArrowNormal.SetActive(deltaPos.x <= 0);
            LeftArrowPressed.SetActive(deltaPos.x < 0);
            LeftArrowNormal.SetActive(deltaPos.x >= 0);
            UpArrowPressed.SetActive(deltaPos.y > 0);
            UpArrowNormal.SetActive(deltaPos.y <= 0);
            DownArrowPresseds.SetActive(deltaPos.y < 0);
            DownArrowNormal.SetActive(deltaPos.y >= 0);

            if (deltaPos.magnitude > 0.1f)
            {
                DirectionImg.transform.right = deltaPos;
            }

        }
    }
}