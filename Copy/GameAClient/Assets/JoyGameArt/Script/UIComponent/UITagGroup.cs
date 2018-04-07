  /********************************************************************
  ** Filename : UITagGroup.cs
  ** Author : quan
  ** Date : 2016/6/10 10:51
  ** Summary : UITagGroup.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SoyEngine
{
    public class UITagGroup : MonoBehaviour
    {
//        public RectTransform FlagTran;
        public bool FreezeButtonPressState;

        private List<Sprite> _defaultSpriteList = new List<Sprite>();
        private List<Button> _buttonList = new List<Button>();
        private List<Action<bool>> _callbackList = new List<Action<bool>>();
        private int _curInx;
        private Selectable.Transition _transition = Selectable.Transition.None;

        public int CurInx
        {
            get
            {
                return this._curInx;
            }
        }

        public void AddButton(Button btn, Action<bool> callback)
        {
            int inx = _buttonList.Count;
            _buttonList.Add(btn);
            _callbackList.Add(callback);
            btn.onClick.AddListener(()=>{
                OnClickTag(inx);
            });
            if(FreezeButtonPressState)
            {
                InitButtonPressState(btn);
            }
        }

        public void SelectIndex(int inx, bool force = false, bool invokeCallback = true)
        {
            if(!force && inx == _curInx)
            {
                return;
            }
            if(inx < 0 || inx >= _buttonList.Count)
            {
                return;
            }
            if(_curInx >=0 && _curInx < _buttonList.Count)
            {
                UpdateView(_curInx, false);
                if(invokeCallback)
                {
                    _callbackList[_curInx].Invoke(false);
                }
            }
            _curInx = inx;
            if(_curInx >=0 && _curInx < _buttonList.Count)
            {
                UpdateView(_curInx, true);
                if(invokeCallback)
                {
                    _callbackList[_curInx].Invoke(true);
                }
            }
        }

        private void UpdateView(int inx, bool selected)
        {
            Button btn = _buttonList[inx];
            if (_transition == Selectable.Transition.ColorTint)
            {
                if(selected)
                {
                    btn.enabled = false;
                    btn.targetGraphic.canvasRenderer.SetColor(btn.colors.pressedColor);
                }
                else
                {
                    btn.enabled = true;
                    btn.targetGraphic.canvasRenderer.SetColor(btn.colors.normalColor);
                }
            }
            else if (_transition == Selectable.Transition.SpriteSwap)
            {
                if(selected)
                {
                    btn.enabled = false;
                    btn.image.sprite = btn.spriteState.pressedSprite;
                }
                else
                {
                    btn.enabled = true;
                    btn.image.sprite = _defaultSpriteList[inx];
                }
            }
//            if(FlagTran != null && selected)
//            {
//                RectTransform btnTran = btn.rectTransform();
//                FlagTran.pivot = btnTran.pivot;
//                FlagTran.anchorMax = btnTran.anchorMax;
//                FlagTran.anchorMin = btnTran.anchorMin;
//                FlagTran.localPosition = btnTran.localPosition;
//                FlagTran.anchoredPosition = btnTran.anchoredPosition;
//                FlagTran.sizeDelta = btnTran.sizeDelta;
//            }
        }

        private void OnClickTag(int inx)
        {
            SelectIndex(inx, false);
        }

        private void InitButtonPressState(Button btn)
        {
            if(_transition == Selectable.Transition.None)
            {
                if(btn.transition != Selectable.Transition.ColorTint
                    && btn.transition != Selectable.Transition.SpriteSwap)
                {
                    LogHelper.Error("UITagGroup AddImage error, Selectable.Transition not support");
                }
                _transition = btn.transition;
            }
            if(_transition != btn.transition)
            {
                LogHelper.Error("UITagGroup AddImage error, Selectable.Transition different");
            }
            if(_transition == Selectable.Transition.SpriteSwap)
            {
                _defaultSpriteList.Add(btn.image.sprite);
            }
        }

        private void OnEnable()
        {
            SelectIndex(_curInx, true, false);
        }
    }
}

