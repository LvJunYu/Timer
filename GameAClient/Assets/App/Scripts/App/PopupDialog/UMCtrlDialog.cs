/********************************************************************
  ** Filename : UMCtrlDialog.cs
  ** Author : quan
  ** Date : 15/7/5 下午3:07
  ** Summary : 通用弹窗
  ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SoyEngine;

namespace GameA
{
    public class UMCtrlDialog : UMCtrlBase<UMViewDialog>
    {
        #region 常量与字段

        private readonly WaitForSeconds _autoCloseSeconds = new WaitForSeconds(2f);
        private Action[] _callbackAry = new Action[3];

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ButtonAry[0].onClick.AddListener(OnButton1Click);
            _cachedView.ButtonAry[1].onClick.AddListener(OnButton2Click);
            _cachedView.ButtonAry[2].onClick.AddListener(OnButton3Click);
        }

        private void OnCloseBtn()
        {
            if (this._cachedView)
            {
                this.Destroy();
            }
        }

        public void Set(string msg, string title, params KeyValuePair<string, Action>[] btnParam)
        {
            DictionaryTools.SetContentText(_cachedView.Content, msg);
            if (string.IsNullOrEmpty(title))
            {
                DictionaryTools.SetContentText(_cachedView.Title, "提示");
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.Title, title);
            }
            for (int i = 0; i < 3; i++)
            {
                if (i >= btnParam.Length)
                {
                    _cachedView.ButtonAry[i].gameObject.SetActive(false);
                }
                else
                {
                    _cachedView.ButtonAry[i].gameObject.SetActive(true);
                    DictionaryTools.SetContentText(_cachedView.ButtonTextAry[i], btnParam[i].Key);
                    _callbackAry[i] = btnParam[i].Value;
                    if (btnParam.Length == 1)
                    {
                        _cachedView.ButtonBgAry[i].sprite = _cachedView.BgSprite[1];
                    }
                    else
                    {
                        _cachedView.ButtonBgAry[i].sprite = _cachedView.BgSprite[i];
                    }
                }
            }
            _cachedView.Trans.sizeDelta = new Vector2(40f, 40f);
            if (btnParam.Length == 0)
            {
                _cachedView.ButtonListDock.SetActive(false);
                _cachedView.SeperatorDock.SetActive(false);
                CoroutineProxy.Instance.StartCoroutine(AutoClose());
                _cachedView.FullScreenMask.raycastTarget = true;
                _cachedView.FullScreenMask.enabled = true;
            }
            else
            {
                _cachedView.ButtonListDock.SetActive(true);
                _cachedView.SeperatorDock.SetActive(true);
                _cachedView.FullScreenMask.raycastTarget = true;
                _cachedView.FullScreenMask.enabled = true;
            }
        }

        IEnumerator AutoClose()
        {
            yield return _autoCloseSeconds;
            if (this._cachedView)
            {
                this.Destroy();
            }
        }

        private void OnButton1Click()
        {
            try
            {
                if (_callbackAry[0] != null)
                {
                    _callbackAry[0].Invoke();
                }
            }
            catch
            {
            }
            finally
            {
                this.Destroy();
            }
        }

        private void OnButton2Click()
        {
            try
            {
                if (_callbackAry[1] != null)
                {
                    _callbackAry[1].Invoke();
                }
            }
            catch
            {
            }
            finally
            {
                this.Destroy();
            }
        }

        private void OnButton3Click()
        {
            try
            {
                if (_callbackAry[2] != null)
                {
                    _callbackAry[2].Invoke();
                }
            }
            catch
            {
            }
            finally
            {
                this.Destroy();
            }
        }

        #endregion
    }
}