
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SoyEngine
{
    public class UITabGroup : MonoBehaviour
    {
        private List<Button> _normalButtonList = new List<Button>();
        private List<Button> _selectedButtonList = new List<Button>();
        private List<Action<bool>> _callbackList = new List<Action<bool>>();
        private int _curInx;

        public int CurInx
        {
            get
            {
                return this._curInx;
            }
        }

        public void AddButton(Button btn, Button selectedBtn, Action<bool> callback)
        {
            int inx = _normalButtonList.Count;
            _normalButtonList.Add(btn);
            _selectedButtonList.Add(selectedBtn);
            selectedBtn.enabled = false;
            selectedBtn.gameObject.SetActive(false);
            _callbackList.Add(callback);
            btn.onClick.AddListener(()=>{
                OnClickTag(inx);
            });
        }

        public void SelectIndex(int inx, bool force = false, bool invokeCallback = true)
        {
            if(!force && inx == _curInx)
            {
                return;
            }
            if(inx < 0 || inx >= _normalButtonList.Count)
            {
                return;
            }
            if(_curInx >=0 && _curInx < _normalButtonList.Count)
            {
                UpdateView(_curInx, false);
                if(invokeCallback)
                {
                    _callbackList[_curInx].Invoke(false);
                }
            }
            _curInx = inx;
            if(_curInx >=0 && _curInx < _normalButtonList.Count)
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
            Button btn = _normalButtonList[inx];
            Button selectedBtn = _selectedButtonList[inx];
            selectedBtn.gameObject.SetActive(selected);
            btn.gameObject.SetActive(!selected);
        }

        private void OnClickTag(int inx)
        {
            SelectIndex(inx, false);
        }

        private void OnEnable()
        {
            SelectIndex(_curInx, true, false);
        }
    }
}

