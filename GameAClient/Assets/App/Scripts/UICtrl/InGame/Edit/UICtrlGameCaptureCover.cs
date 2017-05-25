  /********************************************************************
  ** Filename : UICtrlGameCaptureCover.cs
  ** Author : quan
  ** Date : 11/12/2016 11:04 PM
  ** Summary : UICtrlGameCaptureCover.cs
  ***********************************************************************/

using System;
using System.Collections.Generic;

using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlGameCaptureCover: UICtrlInGameBase<UIViewGameCaptureCover>
    {
        private bool _isProcessing = false;
        private RectTransform _rootTran;
        private Texture2D _coverTexture;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _rootTran = _cachedView.CanvasGroup.rectTransform();
            _cachedView.RawImage.color = Color.white;
            _coverTexture = new Texture2D(1, 1, TextureFormat.RGB24, false, true);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Capture();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.CaptureGameCover, CaptureGameCover);
        }

        protected override void OnDestroy()
        {
            if(_coverTexture != null)
            {
                UnityEngine.Object.Destroy(_coverTexture);
                _coverTexture = null;
            }
            base.OnDestroy();
        }
        #region event

        private void CaptureGameCover()
        {
            if(_isProcessing)
            {
                return;
            }
            _isProcessing = true;
            SocialGUIManager.Instance.OpenUI<UICtrlGameCaptureCover>();
        }

        #endregion

        #region  private
        private void Capture()
        {
            _cachedView.CanvasGroup.alpha = 0;
            _cachedView.WhiteCover.color = Color.white;
            _rootTran.anchoredPosition = Vector2.zero;
            _rootTran.localScale = Vector3.one;
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.1f);
            seq.AppendCallback(()=>{
                var iconBytes = GM2DGame.Instance.CaptureLevel();
                GM2DGame.Instance.NeedSave = true;
//                Debug.LogError("FileSize: " + iconBytes.Length/1024);
                GM2DGame.Instance.IconBytes = iconBytes;
                _coverTexture.LoadImage(iconBytes);
                _cachedView.RawImage.texture = _coverTexture;
                _cachedView.RawImage.SetAllDirty();
                _cachedView.CanvasGroup.alpha = 1;
            });
            seq.Append(DOTween.ToAlpha(
                ()=>_cachedView.WhiteCover.color, 
                c=>{_cachedView.WhiteCover.color = c;},
                0, 0.4f
            ));
            seq.AppendInterval(0.3f);
            seq.Append(_rootTran.DOScale(Vector3.zero, 0.3f));
            seq.AppendCallback(()=>{
                SocialGUIManager.Instance.CloseUI<UICtrlGameCaptureCover>();
                _isProcessing = false;
            });
            seq.SetAutoKill(true);
            seq.Play();
        }

        #endregion
    }
}