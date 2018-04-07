/********************************************************************
** Filename : InputManager
** Author : Dong
** Date : 2015/7/15 星期三 下午 3:16:28
** Summary : InputManager
***********************************************************************/

using System;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class InputManager : IDisposable
    {
        private static InputManager _instance;

        public const string TagJump = "Jump";
        public const string TagSkill1 = "Fire1";
        public const string TagSkill2 = "Fire2";
        public const string TagSkill3 = "Fire3";
        public const string TagAssist = "Assist";
        public const string TagHorizontal = "Horizontal";
        public const string TagVertical = "Vertical";

        public UICtrlGameInput GameInputCtrlGame;
        private GameObject _easyTouchObject;

        private EPhase _mouseRightButtonDragPhase;

        public event Action<Gesture> OnPinch;
        public event Action<Gesture> OnPinchEnd;
        public event Action<Gesture> OnDragStart;
        public event Action<Gesture> OnDrag;
        public event Action<Gesture> OnDragEnd;
        public event Action<Gesture> OnDragStartTwoFingers;
        public event Action<Gesture> OnDragTwoFingers;
        public event Action<Gesture> OnDragEndTwoFingers;
        public event Action<Gesture> OnTap;
        public event Action<Gesture> OnTouchStart;
        public event Action<Gesture> OnTouchDown;
        public event Action<Gesture> OnTouchUp;

        /// <summary>
        /// Pos, Delta
        /// </summary>
        public event Action<Vector3, Vector2> OnMouseWheelChange;

        public event Action<Vector3> OnMouseRightButtonDragStart;
        public event Action<Vector3, Vector2> OnMouseRightButtonDrag;
        public event Action<Vector3, Vector2> OnMouseRightButtonDragEnd;

        public static InputManager Instance
        {
            get { return _instance ?? (_instance = new InputManager()); }
        }

        public bool IsTouchDown { get; private set; }

        public bool IsMouseRightButton
        {
            get { return EPhase.None != _mouseRightButtonDragPhase; }
        }

        public Vector3 MouseRightButtonDragLastPos { get; private set; }

        public void Update()
        {
            CrossPlatformInputManager.Update();
            if (Application.isMobilePlatform)
            {
                return;
            }
            if (OnMouseWheelChange != null)
            {
                if (Input.mouseScrollDelta.sqrMagnitude > 0.0001f)
                {
                    OnMouseWheelChange.Invoke(Input.mousePosition, Input.mouseScrollDelta);
                }
            }
            if (null != OnMouseRightButtonDragStart
                || null != OnMouseRightButtonDrag
                || null != OnMouseRightButtonDragEnd)
            {
                switch (_mouseRightButtonDragPhase)
                {
                    case EPhase.None:
                        if (Input.GetMouseButton(1))
                        {
                            _mouseRightButtonDragPhase = EPhase.Began;
                            MouseRightButtonDragLastPos = Input.mousePosition;
                        }
                        break;
                    case EPhase.Began:
                        if (Input.GetMouseButton(1))
                        {
                            Vector3 delta = Input.mousePosition - MouseRightButtonDragLastPos;
                            if (delta.sqrMagnitude > 1)
                            {
                                _mouseRightButtonDragPhase = EPhase.Moved;
                                if (null != OnMouseRightButtonDragStart)
                                {
                                    OnMouseRightButtonDragStart.Invoke(MouseRightButtonDragLastPos);
                                }
                                if (null != OnMouseRightButtonDrag)
                                {
                                    OnMouseRightButtonDrag.Invoke(Input.mousePosition, delta);
                                }
                                MouseRightButtonDragLastPos = Input.mousePosition;
                            }
                        }
                        else
                        {
                            _mouseRightButtonDragPhase = EPhase.None;
                        }
                        break;
                    case EPhase.Moved:
                        if (Input.GetMouseButton(1))
                        {
                            Vector3 delta = Input.mousePosition - MouseRightButtonDragLastPos;
                            if (delta.sqrMagnitude > 1)
                            {
                                if (null != OnMouseRightButtonDrag)
                                {
                                    OnMouseRightButtonDrag.Invoke(Input.mousePosition, delta);
                                }
                                MouseRightButtonDragLastPos = Input.mousePosition;
                            }
                        }
                        else
                        {
                            Vector3 delta = Input.mousePosition - MouseRightButtonDragLastPos;
                            if (delta.sqrMagnitude > 1)
                            {
                                if (null != OnMouseRightButtonDrag)
                                {
                                    OnMouseRightButtonDrag.Invoke(Input.mousePosition, delta);
                                }
                            }
                            if (null != OnMouseRightButtonDragEnd)
                            {
                                OnMouseRightButtonDragEnd.Invoke(Input.mousePosition, delta);
                            }
                            _mouseRightButtonDragPhase = EPhase.None;
                        }
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (_easyTouchObject != null)
            {
                Object.Destroy(_easyTouchObject);
                _easyTouchObject = null;
            }
            if (_instance != null)
            {
                EasyTouch.On_TouchDown -= EasyTouchOnTouchDown;
                EasyTouch.On_TouchUp -= EasyTouchOnTouchUp;

                EasyTouch.On_Pinch -= EasyTouchOnPinch;
                EasyTouch.On_PinchEnd -= EasyTouchOnPinchEnd;
                EasyTouch.On_DragStart -= EasyTouchOnDragStart;
                EasyTouch.On_Drag -= EasyTouchOnDrag;
                EasyTouch.On_DragEnd -= EasyTouchOnDragEnd;
                EasyTouch.On_SimpleTap -= EasyTouchOnSimpleTap;
                _instance = null;
            }
        }

        public void Init()
        {
            _easyTouchObject = new GameObject("EasyTouch");
            _easyTouchObject.AddComponent<EasyTouch>();
            EasyTouch.SetEnable2DCollider(true);
            EasyTouch.SetUICompatibily(false);
            EasyTouch.AddCamera(CameraManager.Instance.RendererCamera);

            EasyTouch.On_TouchStart += EasyTouchOnTouchStart;
            EasyTouch.On_TouchDown += EasyTouchOnTouchDown;
            EasyTouch.On_TouchUp += EasyTouchOnTouchUp;

            EasyTouch.On_Pinch += EasyTouchOnPinch;
            EasyTouch.On_PinchEnd += EasyTouchOnPinchEnd;
            EasyTouch.On_DragStart += EasyTouchOnDragStart;
            EasyTouch.On_Drag += EasyTouchOnDrag;
            EasyTouch.On_DragEnd += EasyTouchOnDragEnd;
            EasyTouch.On_SimpleTap += EasyTouchOnSimpleTap;
            EasyTouch.On_DragStart2Fingers += EasyTouchOnDragStart2Fingers;
            EasyTouch.On_Drag2Fingers += EasyTouchOnDrag2Fingers;
            EasyTouch.On_DragEnd2Fingers += EasyTouchOnDragEnd2Fingers;
            EasyTouch.SetSecondFingerSimulation(false);
        }

        private void EasyTouchOnDragStart2Fingers(Gesture gesture)
        {
            if (null != OnDragStartTwoFingers)
            {
                OnDragStartTwoFingers.Invoke(gesture);
            }
//            LogHelper.Info("OnDragStartTwoFingers: " + gesture);
        }

        private void EasyTouchOnDrag2Fingers(Gesture gesture)
        {
            if (null != OnDragTwoFingers)
            {
                OnDragTwoFingers.Invoke(gesture);
            }
//            LogHelper.Info("OnDragTwoFingers: " + gesture);
        }

        private void EasyTouchOnDragEnd2Fingers(Gesture gesture)
        {
            if (null != OnDragEndTwoFingers)
            {
                OnDragEndTwoFingers.Invoke(gesture);
            }
//            LogHelper.Info("OnDragEndTwoFingers: " + gesture);
        }

        private void EasyTouchOnSimpleTap(Gesture gesture)
        {
            if (null != OnTap)
            {
                OnTap.Invoke(gesture);
            }
//            LogHelper.Info("OnTap: " + gesture);
        }

        private void EasyTouchOnDragStart(Gesture gesture)
        {
            if (null != OnDragStart)
            {
                OnDragStart.Invoke(gesture);
            }
//            LogHelper.Info("OnDragStart: " + gesture);
        }

        private void EasyTouchOnDrag(Gesture gesture)
        {
            if (null != OnDrag)
            {
                OnDrag.Invoke(gesture);
            }
//            LogHelper.Info("OnDrag: " + gesture);
        }

        private void EasyTouchOnDragEnd(Gesture gesture)
        {
            if (null != OnDragEnd)
            {
                OnDragEnd.Invoke(gesture);
            }
//            LogHelper.Info("OnDragEnd: " + gesture);
        }

        private void EasyTouchOnPinch(Gesture gesture)
        {
            if (OnPinch != null)
            {
                OnPinch.Invoke(gesture);
            }
//            LogHelper.Info("OnPinch: " + gesture);
        }

        private void EasyTouchOnPinchEnd(Gesture gesture)
        {
            if (OnPinchEnd != null)
            {
                OnPinchEnd.Invoke(gesture);
            }
//            LogHelper.Info("OnPinchEnd: " + gesture);
        }

        private void EasyTouchOnTouchStart(Gesture gesture)
        {
            if (OnTouchStart != null)
            {
                OnTouchStart.Invoke(gesture);
            }
//            LogHelper.Info("OnTouchStart: " + gesture);
        }

        private void EasyTouchOnTouchDown(Gesture gesture)
        {
            IsTouchDown = true;
            if (OnTouchDown != null)
            {
                OnTouchDown.Invoke(gesture);
            }
//            LogHelper.Info("OnTouchDown: " + gesture);
        }

        private void EasyTouchOnTouchUp(Gesture gesture)
        {
            IsTouchDown = false;
            if (OnTouchUp != null)
            {
                OnTouchUp.Invoke(gesture);
            }
//            LogHelper.Info("OnTouchUp: " + gesture);
        }

        public void ShowGameInput()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameInput>();
        }

        public void HideGameInput()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameInput>();
        }

        private enum EPhase
        {
            None,
            Began,
            Moved,
        }
    }
}