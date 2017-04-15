/********************************************************************
  ** Filename : UIGlobalGestureReturn.cs
  ** Author : quan
  ** Date : 2016/6/21 10:03
  ** Summary : UIGlobalGestureReturn.cs
  ***********************************************************************/
using System;
using UnityEngine;

namespace GameA
{
    public class UIGlobalGestureReturn
    {
        private const float CheckRangeFactor = 0.1f;
        private Action _onGestureBegin;
        private Action<float> _onGestureUpdate;
        private Action<float> _onGestureEnd;

        private int _screenWidth;
        private int _checkRange;
        private EState _state;
        private float _progress;
        private int _touchId;
        private Vector2 _touchStartPos;


        public UIGlobalGestureReturn(Action onBegin, Action<float> onUpdate, Action<float> onEnd)
        {
            _onGestureBegin = onBegin;
            _onGestureUpdate = onUpdate;
            _onGestureEnd = onEnd;
            _state = EState.None;
            _progress = 0f;
            _touchId = -1;
            _screenWidth = Screen.width;
            _checkRange = (int)(_screenWidth * CheckRangeFactor);
        }

        public void Update()
        {
            switch (_state)
            {
                case EState.None:
                    {
                        if (Input.touchCount != 1)
                        {
                            break;
                        }
                        Touch t = Input.GetTouch(0);
                        if (t.phase != TouchPhase.Began)
                        {
                            break;
                        }
                        if (t.position.x < 0 || t.position.x > _checkRange)
                        {
                            break;
                        }
                        SetState(EState.judging);
                        _touchId = t.fingerId;
                        _touchStartPos = t.position;
                        break;
                    }
                case EState.judging:
                    {
                        Touch t = default(Touch);
                        bool flag = false;
                        for(int i=0; i<Input.touchCount; i++)
                        {
                            t = Input.GetTouch(i);
                            if(t.fingerId == _touchId)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if(!flag)
                        {
                            SetState(EState.None);
                            break;
                        }
                        if(t.phase == TouchPhase.Canceled
                            || t.phase == TouchPhase.Ended)
                        {
                            SetState(EState.None);
                            break;
                        }
                        if(t.phase == TouchPhase.Moved)
                        {
                            var delta = t.deltaPosition;
                            if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            {
                                SetState(EState.TouchMove);
                            }
                            else
                            {
                                SetState(EState.None);
                            }
                        }
                        break;
                    }
                case EState.TouchMove:
                    {
                        Touch t = default(Touch);
                        bool flag = false;
                        for(int i=0; i<Input.touchCount; i++)
                        {
                            t = Input.GetTouch(i);
                            if(t.fingerId == _touchId)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if(!flag)
                        {
                            SetState(EState.None);
                            break;
                        }
                        _progress = Mathf.Clamp01((t.position.x-_touchStartPos.x)/_screenWidth);
                        if(_onGestureUpdate != null)
                        {
                            _onGestureUpdate.Invoke(_progress);
                        }
                        if(t.phase == TouchPhase.Canceled
                            || t.phase == TouchPhase.Ended)
                        {
                            SetState(EState.None);
                        }
                        break;
                    }
            }
        }

        private void SetState(EState state)
        {
            if (state == _state)
            {
                return;
            }
            switch (_state)
            {
                case EState.None:
                    break;
                case EState.judging:
                    break;
                case EState.TouchMove:
                    if (_onGestureEnd != null)
                    {
                        _onGestureEnd.Invoke(_progress);
                    }
                    break;
            }
            _state = state;

            switch (_state)
            {
                case EState.None:
                    _touchId = -1;
                    _progress = 0f;
                    break;
                case EState.judging:
                    break;
                case EState.TouchMove:
                    if (_onGestureBegin != null)
                    {
                        _onGestureBegin.Invoke();
                    }
                    break;
            }
        }

        private enum EState
        {
            None,
            judging,
            TouchMove,
        }
    }
}

