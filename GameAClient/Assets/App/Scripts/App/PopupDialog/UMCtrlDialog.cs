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
using UnityEngine.UI;

namespace GameA
{
    public class UMCtrlDialog: UMCtrlBase<UMViewDialog>
    {
        #region 常量与字段
        private const int AutoCloseSeconds = 2;
        private Action[] _callbackAry = new Action[3];

        #endregion
        #region 属性
        #endregion

        #region 方法
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ButtonAry[0].onClick.AddListener(OnButton1Click);
            _cachedView.ButtonAry[1].onClick.AddListener(OnButton2Click);
            _cachedView.ButtonAry[2].onClick.AddListener(OnButton3Click);
        }

        public void Set(string msg, string title, params KeyValuePair<string, Action>[] btnParam)
        {
            DictionaryTools.SetContentText(_cachedView.Content, msg);
            if(string.IsNullOrEmpty(title))
            {
                _cachedView.Title.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.Title.gameObject.SetActive(true);
                DictionaryTools.SetContentText(_cachedView.Title, title);
            }
            for (int i = 0; i < 3; i++)
            {
                if(i >= btnParam.Length)
                {
                    _cachedView.ButtonAry[i].gameObject.SetActive(false);
                }
                else {
                    _cachedView.ButtonAry[i].gameObject.SetActive(true);                    
                    DictionaryTools.SetContentText(_cachedView.ButtonTextAry[i], btnParam[i].Key);
                    _callbackAry[i] = btnParam[i].Value;
                }
            }
            _cachedView.Trans.sizeDelta = new Vector2(40f, 40f);
            if (btnParam.Length == 0)
            {
                _cachedView.StartCoroutine(AutoClose());
                _cachedView.FullScreenMask.raycastTarget = false;
                _cachedView.FullScreenMask.enabled = false;
            }
            else
            {
                _cachedView.FullScreenMask.raycastTarget = true;
                _cachedView.FullScreenMask.enabled = true;
            }

            if(btnParam.Length == 0)
            {
                _cachedView.ContentBg.sprite = _cachedView.BgSprite[3];
            }
            else
            {
                _cachedView.ContentBg.sprite = _cachedView.BgSprite[2];
            }
            Image bgImage = _cachedView.ButtonBgAry[0];
            if(btnParam.Length == 1)
            {
                bgImage.sprite = _cachedView.BgSprite[2];
                bgImage.rectTransform.localScale = new Vector3(1f, -1f, 1f);
            }
            else
            {
                bgImage.sprite = _cachedView.BgSprite[1];
                bgImage.rectTransform.localScale = Vector3.one;
            }

            bgImage = _cachedView.ButtonBgAry[1];
            if(btnParam.Length == 2)
            {
                bgImage.sprite = _cachedView.BgSprite[1];
                bgImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                bgImage.sprite = _cachedView.BgSprite[0];
                bgImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }

            bgImage = _cachedView.ButtonBgAry[2];
            {
                bgImage.sprite = _cachedView.BgSprite[1];
                bgImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }


        IEnumerator AutoClose()
        {
            yield return new WaitForSeconds(AutoCloseSeconds);
            this.Destroy();
        }

        private void OnButton1Click()
        {
            try
            {
                if(_callbackAry[0] != null)
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
                if(_callbackAry[1] != null)
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
                if(_callbackAry[2] != null)
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