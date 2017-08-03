/********************************************************************
** Filename : InputManager
** Author : Dong
** Date : 2015/7/15 星期三 下午 3:16:28
** Summary : InputManager
***********************************************************************/

using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class InputManager : IDisposable
    {
        private static InputManager _instance;

        public static readonly string TagJump = "Jump";
        public static readonly string[] TagSkill = {"Fire1", "Fire2"};
        public static readonly string TagHorizontal = "Horizontal";
        public static readonly string TagVertical = "Vertical";

        public UICtrlGameInputControl GameInputControl;
        private GameObject _easyTouchObject;

        private EPhase _mouseRightButtonDragPhase;

        public event Action<Gesture> OnPinch;
        public event Action<Gesture> OnPinchEnd;
        public event Action<Gesture> OnDragStart;
        public event Action<Gesture> OnDrag;
        public event Action<Gesture> OnDragEnd;
        public event Action<Gesture> OnTap;
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
                EasyTouch.On_TouchDown -= EasyTouchOnOnTouchDown;
                EasyTouch.On_TouchUp -= EasyTouchOnOnTouchUp;
                
                EasyTouch.On_Pinch -= EasyTouchOnOnPinch;
                EasyTouch.On_PinchEnd -= EasyTouchOnOnPinchEnd;
                EasyTouch.On_DragStart -= EasyTouchOnOnDragStart;
                EasyTouch.On_Drag -= EasyTouchOnOnDrag;
                EasyTouch.On_DragEnd -= EasyTouchOnOnDragEnd;
                EasyTouch.On_SimpleTap -= EasyTouchOnSimpleTap;
                _instance = null;
            }
        }

        public void Init()
        {
            _easyTouchObject = new GameObject("EasyTouch");
            _easyTouchObject.AddComponent<EasyTouch>();
            EasyTouch.SetEnable2DCollider(true);
            EasyTouch.AddCamera(CameraManager.Instance.RendererCamera);
            
            
            EasyTouch.On_TouchDown += EasyTouchOnOnTouchDown;
            EasyTouch.On_TouchUp += EasyTouchOnOnTouchUp;
            
            EasyTouch.On_Pinch += EasyTouchOnOnPinch;
            EasyTouch.On_PinchEnd += EasyTouchOnOnPinchEnd;
            EasyTouch.On_DragStart += EasyTouchOnOnDragStart;
            EasyTouch.On_Drag += EasyTouchOnOnDrag;
            EasyTouch.On_DragEnd += EasyTouchOnOnDragEnd;
            EasyTouch.On_SimpleTap += EasyTouchOnSimpleTap;
        }

        private void EasyTouchOnSimpleTap(Gesture gesture)
        {
            if (null != OnTap)
            {
                OnTap.Invoke(gesture);
            }
        }

        private void EasyTouchOnOnDragEnd(Gesture gesture)
        {
            if (null != OnDragEnd)
            {
                OnDragEnd.Invoke(gesture);
            }
        }

        private void EasyTouchOnOnDrag(Gesture gesture)
        {
            if (null != OnDrag)
            {
                OnDrag.Invoke(gesture);
            }
        }

        private void EasyTouchOnOnDragStart(Gesture gesture)
        {
            if (null != OnDragStart)
            {
                OnDragStart.Invoke(gesture);
            }
        }

        private void EasyTouchOnOnPinchEnd(Gesture gesture)
        {
            if (OnPinchEnd != null)
            {
                OnPinchEnd.Invoke(gesture);
            }
        }

        private void EasyTouchOnOnPinch(Gesture gesture)
        {
            if (OnPinch != null)
            {
                OnPinch.Invoke(gesture);
            }
        }

        private void EasyTouchOnOnTouchDown(Gesture gesture)
        {
            IsTouchDown = true;
        }

        private void EasyTouchOnOnTouchUp(Gesture gesture)
        {
            IsTouchDown = false;
        }

        public void ShowGameInput()
        {
            if (Application.isEditor)
            {
                HideGameInput();
                return;
            }
            if (null != GameInputControl)
            {
                GameInputControl.Show();
                GameInputControl.ShowAttack1Btn();
            }
        }

        public void HideGameInput()
        {
            if (null != GameInputControl)
            {
                GameInputControl.Hide();
            }
        }
        
        private enum EPhase
        {
            None,
            Began,
            Moved,
        }
    }
}