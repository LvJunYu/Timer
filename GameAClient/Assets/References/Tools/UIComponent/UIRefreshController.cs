  /********************************************************************
  ** Filename : UIRefreshController.cs
  ** Author : quan
  ** Date : 9/22/2016 7:56 PM
  ** Summary : UIRefreshController.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class UIRefreshController : MonoBehaviour
    {
        [SerializeField]
        private ScrollRectEx _scroller;
        [SerializeField]
        private LayoutElement _refreshUI;
        [SerializeField]
        private RectTransform _arrowUI;
        [SerializeField]
        private Image _loadingUI;
        [SerializeField]
        private Sprite[] _loadingSpriteAry;
        [SerializeField]
        private Text _tip;
        [SerializeField]
        private int _refreshUISize = 120;
        private EState _state;
        private bool _commandStop;
        private const float MinRefreshTime = 0.2f;
        private float _leftTime;
        private Action _refreshCallback;
        private int _animationInx;
        private const float AnimationFrameTime = 0.3f;
        private float _animationFrameLeftTime;

        public void SetRefreshCallback(Action refreshCallback)
        {
            _refreshCallback = refreshCallback;
        }

        public void StartRefresh()
        {
            SetState(EState.Refreshing);
        }

        public void ExitRefresh()
        {
            _commandStop = true;
        }

        private void Awake()
        {
            if(_scroller == null)
            {
                _scroller = gameObject.GetComponentInParent<ScrollRectEx>();
            }
            _scroller.onValueChanged.AddListener(OnValueChange);
            _scroller.OnEndDragEvent.AddListener(OnDragEnd);
            SetState(EState.Detecting);
        }

        private void OnValueChange(Vector2 normalizedPos)
        {
            if(_state != EState.Detecting && _state != EState.DetectingReach)
            {
                return;
            }
            RefreshDetectingUI();
            if(_scroller.content.anchoredPosition.y < -_refreshUISize)
            {
                SetState(EState.DetectingReach);
            }
            else
            {
                SetState(EState.Detecting);
            }
        }

        private void RefreshDetectingUI()
        {
            float startSize =  _refreshUISize * 0.7f;
            float rotateSize = _refreshUISize - startSize;
            float factor = Mathf.Clamp01((-_scroller.content.anchoredPosition.y - startSize) / rotateSize);
            _arrowUI.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-180, 0, factor));
        }

        private void OnDragEnd(PointerEventData data )
        {
            if(_state == EState.DetectingReach)
            {
                SetState(EState.Refreshing);
            }
        }

        private void SetState(EState state)
        {
            if(_state == state)
            {
                return;
            }
            switch(_state)
            {
                case EState.Detecting:
                    {
                        break;
                    }
                case EState.DetectingReach:
                    {
                        break;
                    }
                case EState.Refreshing:
                    {
                        Vector2 oriPos = _scroller.content.anchoredPosition;
                        oriPos.y -= _refreshUISize;
                        _refreshUI.minHeight = _refreshUI.preferredHeight = 0;
                        Canvas.ForceUpdateCanvases();
                        _scroller.content.anchoredPosition = oriPos;
                        _commandStop = false;
                        break;
                    }
            }
            _state = state;
            switch(_state)
            {
                case EState.Detecting:
                    {
                        DictionaryTools.SetContentText(_tip, "下拉刷新");
                        _arrowUI.gameObject.SetActive(true);
                        _loadingUI.gameObject.SetActive(false);
                        RefreshDetectingUI();
                        break;
                    }
                case EState.DetectingReach:
                    {
                        DictionaryTools.SetContentText(_tip, "松开刷新");
                        _arrowUI.localEulerAngles = Vector3.zero;
                        break;
                    }
                case EState.Refreshing:
                    {
                        _commandStop = false;
                        _leftTime = MinRefreshTime;
                        DictionaryTools.SetContentText(_tip, "加载中...");
                        Vector2 oriPos = _scroller.content.anchoredPosition;
                        oriPos.y += _refreshUISize;
                        _refreshUI.minHeight = _refreshUI.preferredHeight = _refreshUISize;
                        Canvas.ForceUpdateCanvases();
                        _scroller.content.anchoredPosition = oriPos;
                        if(_scroller.verticalNormalizedPosition < 1)
                        {
                            _scroller.verticalNormalizedPosition = 1;
                        }
                        if(_refreshCallback != null)
                        {
                            _refreshCallback.Invoke();
                        }
                        _arrowUI.gameObject.SetActive(false);
                        _loadingUI.gameObject.SetActive(true);
                        _animationFrameLeftTime = AnimationFrameTime;
                        _animationInx = 0;
                        _loadingUI.sprite = _loadingSpriteAry[_animationInx];
                        break;
                    }
            }
        }

        private void Update()
        {
            switch(_state)
            {
                case EState.Detecting:
                    {
                        break;
                    }
                case EState.DetectingReach:
                    {
                        break;
                    }
                case EState.Refreshing:
                    {
                        _animationFrameLeftTime -= Time.deltaTime;
                        if(_animationFrameLeftTime <= 0)
                        {
                            _animationFrameLeftTime = AnimationFrameTime;
                            _animationInx = (_animationInx + 1) % _loadingSpriteAry.Length;
                            _loadingUI.sprite = _loadingSpriteAry[_animationInx];
                        }
                        _leftTime -= Time.deltaTime;
                        if(_commandStop && _leftTime <=0)
                        {
                            SetState(EState.Detecting);
                        }
                        break;
                    }
            }
        }


        private enum EState
        {
            None,
            /// <summary>
            /// 检测是否刷新
            /// </summary>
            Detecting,
            DetectingReach,
            /// <summary>
            /// 正在刷新
            /// </summary>
            Refreshing,
        }
    }
}

