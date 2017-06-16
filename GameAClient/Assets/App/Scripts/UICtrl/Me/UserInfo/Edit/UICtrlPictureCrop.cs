  /********************************************************************
  ** Filename : UICtrlPictureCrop.cs
  ** Author : quan
  ** Date : 2016/4/13 14:32
  ** Summary : UICtrlPictureCrop.cs
  ***********************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using DG.Tweening;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPictureCrop : UISocialContentCtrlBase<UIViewPictureCrop>, IUIWithTitle, IUIWithRightCustomButton
    {
        #region 常量与字段
        private const int CropWidth = 500;
        private const int CropHeight = 500;
        private const int HalfCropWidth = CropWidth / 2;
        private const int HalfCropHeight = CropHeight / 2;
        private const int ContentCropWidth = 256;
        private const int ContentCropHeight = 256;
        private Action<byte[]> _callback;

        private Texture2D _rawTexture;
        private UGUIGestureRecognizer _gestureRecognizer;



        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _gestureRecognizer = UGUIGestureRecognizer.GetInstance(_cachedView.gameObject);
            _gestureRecognizer.OnDragBeginHandler += OnDragBegin;
            _gestureRecognizer.OnDragUpdateHandler += OnDragUpdate;
            _gestureRecognizer.OnDragEndHandler += OnDragEnd;
            _gestureRecognizer.OnPinchBeginHandler += OnPinchBegin;
            _gestureRecognizer.OnPinchUpdateHandler += OnPinchUpdate;
            _gestureRecognizer.OnPinchEndHandler += OnPinchEnd;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Tuple<Texture2D, Action<byte[]>> tuple = parameter as Tuple<Texture2D, Action<byte[]>>;
            if(tuple == null)
            {
                LogHelper.Error("UICtrlPictureCrop OnOpen Error, arguments error");
                return;
            }
            _rawTexture = tuple.Item1;
            _callback = tuple.Item2;
            if(!_hasInit)
            {
                InitParam();
                InitMask();
            }
            InitContent();
        }

        protected override void OnClose()
        {
            if(_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
            if(_rawTexture != null)
            {
                GameObject.Destroy(_rawTexture);
                _rawTexture = null;
            }
            _cachedView.ContentImage.texture = null;
            base.OnClose();
        }

        protected override void OnDestroy()
        {
        }

        private byte[] CropPicture()
        {
            Texture2D targetTexture = new Texture2D(ContentCropWidth, ContentCropHeight, TextureFormat.RGB24, false);
            Vector2 finalSize = new Vector2(_oriContentWidth, _oriContentHeight) * _finalScale;

            Vector2 uvStart = new Vector2(-HalfCropWidth, -HalfCropHeight) - (new Vector2(_finalPosition.x, _finalPosition.y) - finalSize* 0.5f);
            uvStart.Set(uvStart.x/finalSize.x, uvStart.y/finalSize.y);
            Vector2 uvSize = new Vector2(CropWidth / finalSize.x, CropHeight / finalSize.y);
            float invWidth = 1f/targetTexture.width;
            float invHeight = 1f/targetTexture.height;
            for(int y = 0; y<targetTexture.height; y++)
            {
                for(int x = 0; x<targetTexture.width; x++)
                {
                    targetTexture.SetPixel(x, y, _rawTexture.GetPixelBilinear(uvStart.x + uvSize.x * x * invWidth, uvStart.y + uvSize.y * y * invHeight));
                }
            }
            var result = targetTexture.EncodeToJPG();
            GameObject.Destroy(targetTexture);
            return result;
        }
        #endregion

        #region 事件处理


        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "头像编辑";
        }
        public UnityEngine.UI.Button GetRightButton()
        {
            return _cachedView.ConfirmButtonRes;
        }

//        public void OnRightButtonClick(UICtrlTitlebar titleBar)
//        {
//            var imgBytes = CropPicture();
//            _uiStack.OpenPrevious();
//            _callback.Invoke(imgBytes);
//        }

        #endregion

        #region 动画实现
        private Vector3 _finalPosition;
        private float _finalScale;
        private Vector3 _beginPosition;
        private float _beginScale;
        private Tweener _tweener;

        private void TryAnimation()
        {
            bool needAnimation = false;
            _beginScale = 1f * _contentRectTransform.sizeDelta.x / _oriContentWidth;
            if(_oriContentWidth * _minScale > _contentRectTransform.sizeDelta.x)
            {
                _finalScale = _minScale;
                needAnimation = true;
            }
            else if(_oriContentWidth * _maxScale < _contentRectTransform.sizeDelta.x)
            {
                _finalScale = _maxScale;
                needAnimation = true;
            }
            else
            {
                _finalScale = 1f * _contentRectTransform.sizeDelta.x / _oriContentWidth;
            }
            Vector2 finalHalfSize =  new Vector2(_oriContentWidth, _oriContentHeight) * _finalScale * 0.5f;
            Vector3 curPos = _contentRectTransform.localPosition;
            _finalPosition = curPos;
            _beginPosition = curPos;

            if(curPos.x - finalHalfSize.x > -HalfCropWidth)
            {
                _finalPosition.x = finalHalfSize.x - HalfCropWidth;
                needAnimation = true;
            }
            else if(curPos.x + finalHalfSize.x < HalfCropWidth)
            {
                _finalPosition.x = -finalHalfSize.x + HalfCropWidth;
                needAnimation = true;
            }
            if(curPos.y - finalHalfSize.y > -HalfCropHeight)
            {
                _finalPosition.y = finalHalfSize.y - HalfCropHeight;
                needAnimation = true;
            }
            else if(curPos.y + finalHalfSize.y < HalfCropHeight)
            {
                _finalPosition.y = -finalHalfSize.y + HalfCropHeight;
                needAnimation = true;
            }
            if(needAnimation)
            {
                if(_tweener != null)
                {
                    _tweener.Restart();
                }
                else
                {
                    _tweener = DOTween.To(factor=>{
                        _contentRectTransform.localPosition = Vector3.Lerp(_beginPosition, _finalPosition, factor);
                        _contentRectTransform.sizeDelta = new Vector2(_oriContentWidth, _oriContentHeight) * Mathf.Lerp(_beginScale, _finalScale, factor);
                    }, 0, 1, 0.6f).SetEase(Ease.OutCirc).SetAutoKill(false);
                }
            }
        }

        private void StopAnimation()
        {
            if(_tweener != null)
            {
                _tweener.Pause();
            }
        }

        #endregion 动画实现

        #region 手势实现
        private bool _hasInit = false;
        private RectTransform _contentRectTransform;
        private int _canvasHeight;
        private int _canvasWidth;
        private int _oriContentWidth;
        private int _oriContentHeight;
        private float _maxScale;
        private float _minScale;

        private void InitParam()
        {
            _canvasHeight = (int)_cachedView.Trans.GetHeight();
            _canvasWidth = (int)_cachedView.Trans.GetWidth();
            _contentRectTransform = _cachedView.ContentImage.rectTransform;
            LogHelper.Info("PictureCrop Size: ({0}, {1})", _canvasWidth, _canvasHeight);
            _hasInit = true;
        }

        private void InitMask()
        {
            int height = (_canvasHeight - CropHeight)/2;
            _cachedView.MaskUp.rectTransform.sizeDelta = new Vector2(0, height);
            _cachedView.MaskDown.rectTransform.sizeDelta = new Vector2(0, height);
        }

        private void InitContent()
        {
            _oriContentWidth = _rawTexture.width;
            _oriContentHeight = _rawTexture.height;
            _cachedView.ContentImage.texture = _rawTexture;
            _cachedView.ContentImage.SetAllDirty();
            _contentRectTransform.localPosition = Vector3.zero;
            _minScale = Mathf.Max(1f*CropHeight/_oriContentHeight, 1f*CropWidth/_oriContentWidth);
            _maxScale = Mathf.Max(1f*CropHeight/ContentCropHeight , _minScale);
            _contentRectTransform.sizeDelta = new Vector2(_oriContentWidth, _oriContentHeight) * _minScale;
            _finalScale = _minScale;
            _finalPosition = Vector3.zero;
        }

        private void UpdateContent(Vector2 delta, float dScale = 1)
        {
            if(_contentRectTransform.localPosition.x - _contentRectTransform.sizeDelta.x * 0.5f > -HalfCropWidth && delta.x>0
                || _contentRectTransform.localPosition.x + _contentRectTransform.sizeDelta.x * 0.5f < HalfCropWidth && delta.x < 0
                || _contentRectTransform.localPosition.y - _contentRectTransform.sizeDelta.y * 0.5f > -HalfCropHeight && delta.y > 0
                || _contentRectTransform.localPosition.y + _contentRectTransform.sizeDelta.y * 0.5f < HalfCropHeight && delta.y <0
                || _oriContentWidth * _minScale > _contentRectTransform.sizeDelta.x && dScale < 1
                || _oriContentWidth * _maxScale < _contentRectTransform.sizeDelta.x && dScale > 1)
            {
                dScale -= (dScale-1) * 0.5f;
                delta *= 0.5f;
            }
            _contentRectTransform.localPosition += new Vector3(delta.x, delta.y, 0);
            _contentRectTransform.sizeDelta *= dScale;
        }

        private void OnDragBegin(UGUIGestureRecognizer.DragEvent dragEvent)
        {
            StopAnimation();
        }

        private void OnDragUpdate(UGUIGestureRecognizer.DragEvent dragEvent)
        {
            UpdateContent(dragEvent.Delta);
        }

        private void OnDragEnd(UGUIGestureRecognizer.DragEvent dragEvent)
        {
            TryAnimation();
        }


        private void OnPinchBegin(UGUIGestureRecognizer.PinchEvent pinchEvent)
        {
            StopAnimation();
        }

        private void OnPinchUpdate(UGUIGestureRecognizer.PinchEvent pinchEvent)
        {
            Vector2 localPinchCenter = (pinchEvent.TouchData0.CurrentPosition + pinchEvent.TouchData1.CurrentPosition) * 0.5f
                - new Vector2(_contentRectTransform.localPosition.x, _contentRectTransform.localPosition.y) - new Vector2(_canvasWidth*0.5f, _canvasHeight*0.5f);
            Vector2 centerDelta = pinchEvent.CenterDelta;
            centerDelta -= localPinchCenter * (pinchEvent.DeltaScale - 1);
            UpdateContent(centerDelta, pinchEvent.DeltaScale);
        }

        private void OnPinchEnd(UGUIGestureRecognizer.PinchEvent pinchEvent)
        {

        }
        #endregion 手势实现
    }
}
