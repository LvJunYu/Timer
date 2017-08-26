using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using SoyEngine;
using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public class EditModeStateMachineHelper
    {
        private readonly StateMachine<EditMode, EditModeState.Base> _stateMachine;
        private HashSet<EditModeState.Base> _initedStateSet;
        private EDragMode _dragMode;
        private Gesture _lastDragGesture;
        private bool _pinchActive;
        private Gesture _lastPinchGesture;

        public EditModeStateMachineHelper(StateMachine<EditMode, EditModeState.Base> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Init()
        {
            _initedStateSet = new HashSet<EditModeState.Base>();
            _dragMode = EDragMode.None;
            InputManager.Instance.OnTouchStart += OnTouchStart;
            InputManager.Instance.OnTouchUp += OnTouchUp;
            InputManager.Instance.OnPinch += OnPinch;
            InputManager.Instance.OnPinchEnd += OnPinchEnd;
            InputManager.Instance.OnDragStart += OnDragStart;
            InputManager.Instance.OnDrag += OnDrag;
            InputManager.Instance.OnDragEnd += OnDragEnd;
            InputManager.Instance.OnDragStartTwoFingers += OnDragStartTwoFingers;
            InputManager.Instance.OnDragTwoFingers += OnDragTwoFingers;
            InputManager.Instance.OnDragEndTwoFingers += OnDragEndTwoFingers;
            InputManager.Instance.OnTap += OnTap;
            InputManager.Instance.OnMouseWheelChange += OnMouseWheelChange;
            InputManager.Instance.OnMouseRightButtonDragStart += OnMouseRightButtonDragStart;
            InputManager.Instance.OnMouseRightButtonDrag += OnMouseRightButtonDrag;
            InputManager.Instance.OnMouseRightButtonDragEnd += OnMouseRightButtonDragEnd;
            _stateMachine.AfterChangeStateCallback += OnAfterStateChange;
            _stateMachine.BeforeChangeStateCallback += OnBeforeStateChange;
        }

        public void Dispose()
        {
            InputManager.Instance.OnTouchStart -= OnTouchStart;
            InputManager.Instance.OnTouchUp -= OnTouchUp;
            InputManager.Instance.OnPinch -= OnPinch;
            InputManager.Instance.OnPinchEnd -= OnPinchEnd;
            InputManager.Instance.OnDragStart -= OnDragStart;
            InputManager.Instance.OnDrag -= OnDrag;
            InputManager.Instance.OnDragEnd -= OnDragEnd;
            InputManager.Instance.OnDragStartTwoFingers -= OnDragStartTwoFingers;
            InputManager.Instance.OnDragTwoFingers -= OnDragTwoFingers;
            InputManager.Instance.OnDragEndTwoFingers -= OnDragEndTwoFingers;
            InputManager.Instance.OnTap -= OnTap;
            InputManager.Instance.OnMouseWheelChange -= OnMouseWheelChange;
            InputManager.Instance.OnMouseRightButtonDragStart -= OnMouseRightButtonDragStart;
            InputManager.Instance.OnMouseRightButtonDrag -= OnMouseRightButtonDrag;
            InputManager.Instance.OnMouseRightButtonDragEnd -= OnMouseRightButtonDragEnd;
            
            _stateMachine.AfterChangeStateCallback -= OnAfterStateChange;
            _stateMachine.BeforeChangeStateCallback -= OnBeforeStateChange;
            
            foreach (var state in _initedStateSet)
            {
                state.Dispose();
            }
            _initedStateSet.Clear();
            _initedStateSet = null;
            _dragMode = EDragMode.None;
        }
        
        #region InputEvent

        private void OnTouchStart(Gesture gesture)
        {
            switch (_dragMode)
            {
                case EDragMode.None:
                    break;
                case EDragMode.DragOneFinger:
                    if (gesture.touchCount != 1)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        OnDragEnd(_lastDragGesture);
                    }
                    break;
                case EDragMode.DragTwoFinger:
                    if (gesture.touchCount != 2)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        _lastDragGesture.deltaPinch = 0;
                        OnDragEndTwoFingers(_lastDragGesture);
                    }
                    break;
            }
            if (_pinchActive)
            {
                if (gesture.touchCount != 2)
                {
                    _lastPinchGesture.deltaPinch = 0;
                    OnPinchEnd(_lastPinchGesture);
                }
            }
        }
        
        private void OnTouchUp(Gesture gesture)
        {
            switch (_dragMode)
            {
                case EDragMode.None:
                    break;
                case EDragMode.DragOneFinger:
                    if (gesture.touchCount != 2)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        OnDragEnd(_lastDragGesture);
                    }
                    break;
                case EDragMode.DragTwoFinger:
                    if (gesture.touchCount != 3)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        _lastDragGesture.deltaPinch = 0;
                        OnDragEndTwoFingers(_lastDragGesture);
                    }
                    break;
            }
            if (_pinchActive)
            {
                if (gesture.touchCount != 3)
                {
                    _lastPinchGesture.deltaPinch = 0;
                    OnPinchEnd(_lastPinchGesture);
                }
            }
        }
        
        private void OnPinch(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            _pinchActive = true;
            _lastPinchGesture = gesture;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnPinch(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnPinch(gesture);
            }
        }

        private void OnPinchEnd(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (!_pinchActive)
            {
                return;
            }
            _pinchActive = false;
            _lastPinchGesture = null;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnPinchEnd(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnPinchEnd(gesture);
            }
        }

        private void OnDragStart(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (gesture.touchCount > 1)
            {
                return;
            }
            if (_dragMode != EDragMode.None)
            {
                return;
            }
            _dragMode = EDragMode.DragOneFinger;
            _lastDragGesture = gesture;
            if (gesture.isOverGui)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragStart(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragStart(gesture);
            }
        }

        private void OnDrag(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragOneFinger)
            {
                return;
            }
            if (gesture.fingerIndex != _lastDragGesture.fingerIndex)
            {
                return;
            }
            _lastDragGesture = gesture;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDrag(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDrag(gesture);
            }
        }

        private void OnDragEnd(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragOneFinger)
            {
                return;
            }
            if (gesture.fingerIndex != _lastDragGesture.fingerIndex)
            {
                return;
            }
            _dragMode = EDragMode.None;
            _lastDragGesture = null;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragEnd(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragEnd(gesture);
            }
        }

        private void OnDragStartTwoFingers(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (_dragMode != EDragMode.None)
            {
                return;
            }
            _dragMode = EDragMode.DragTwoFinger;
            _lastDragGesture = gesture;

            if (gesture.isOverGui)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragStartTwoFingers(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragStartTwoFingers(gesture);
            }
        }

        private void OnDragTwoFingers(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragTwoFinger)
            {
                return;
            }
            _lastDragGesture = gesture;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragTwoFingers(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragTwoFingers(gesture);
            }
        }

        private void OnDragEndTwoFingers(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragTwoFinger)
            {
                return;
            }
            _dragMode = EDragMode.None;
            _lastDragGesture = null;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragEndTwoFingers(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragEndTwoFingers(gesture);
            }
        }

        private void OnTap(Gesture gesture)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (gesture.touchCount > 1)
            {
                return;
            }
            if (gesture.isOverGui)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnTap(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnTap(gesture);
            }
        }

        private void OnMouseWheelChange(Vector3 arg1, Vector2 arg2)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseWheelChange(arg1, arg2);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseWheelChange(arg1, arg2);
            }
        }

        private void OnMouseRightButtonDragStart(Vector3 obj)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseRightButtonDragStart(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseRightButtonDragStart(obj);
            }
        }

        private void OnMouseRightButtonDrag(Vector3 arg1, Vector2 arg2)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseRightButtonDrag(arg1, arg2);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseRightButtonDrag(arg1, arg2);
            }
        }

        private void OnMouseRightButtonDragEnd(Vector3 arg1, Vector2 arg2)
        {
            if (!EditMode.Instance.Enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseRightButtonDragEnd(arg1, arg2);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseRightButtonDragEnd(arg1, arg2);
            }
        }

        #endregion

        private void OnBeforeStateChange(EditModeState.Base oldState, EditModeState.Base newState)
        {
            if (!_initedStateSet.Contains(newState))
            {
                newState.Init();
                _initedStateSet.Add(newState);
            }
        }
        
        private void OnAfterStateChange(EditModeState.Base oldState, EditModeState.Base newState)
        {
            Messenger.Broadcast(EMessengerType.AfterEditModeStateChange);
        }
        
        private enum EDragMode
        {
            None,
            DragOneFinger,
            DragTwoFinger,
        }
    }
}