using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlNpcDiaItem : USCtrlBase<USViewNpcDiaItem>
    {
        private Sprite _sprite;
        private NpcDia _dia;
        private List<NpcDia> _diaList;
        private int _index;
        private Action _callback;

        private void Clear()
        {
            _cachedView.UpBtn.onClick.RemoveAllListeners();
            _cachedView.DownBtn.onClick.RemoveAllListeners();
            _cachedView.DelteBtn.onClick.RemoveAllListeners();
        }

        public void Set(NpcDia dia, List<NpcDia> diaList, int index, Action callback)
        {
            Clear();
            _cachedView.DelteBtn.onClick.AddListener(OnDelBtn);
            _cachedView.UpBtn.onClick.AddListener(OnUpBtn);
            _cachedView.DownBtn.onClick.AddListener(OnDownBtn);
            _diaList = diaList;
            _index = index;
            SetIndex(_index);
            _dia = dia;
            _callback = callback;
            string name = NpcDia.GetNpcFaceSpriteName(dia.NpcId, dia.FaceId);
            JoyResManager.Instance.TryGetSprite(name, out _sprite);
            _cachedView.IconImage.sprite = _sprite;
            _cachedView.DiaText.text = dia.Dia;
        }

        private void SetIndex(int index)
        {
            switch (index)
            {
                case 0:
                    _cachedView.IndexText.text = "对话一";
                    break;
                case 1:
                    _cachedView.IndexText.text = "对话二";
                    break;
                case 2:
                    _cachedView.IndexText.text = "对话三";
                    break;
                case 3:
                    _cachedView.IndexText.text = "对话四";
                    break;
                case 4:
                    _cachedView.IndexText.text = "对话五";
                    break;
            }
        }

        private void OnDownBtn()
        {
            int newindex = _index + 1;
            if (newindex < _diaList.Count)
            {
                NpcDia temp = _diaList[_index];
                _diaList[_index] = _diaList[newindex];
                _diaList[newindex] = temp;
            }
            _callback.Invoke();
        }

        private void OnUpBtn()
        {
            int newindex = _index - 1;
            if (newindex >= 0)
            {
                NpcDia temp = _diaList[_index];
                _diaList[_index] = _diaList[newindex];
                _diaList[newindex] = temp;
            }
            _callback.Invoke();
        }

        private void OnDelBtn()
        {
            _diaList.RemoveAt(_index);
            _callback.Invoke();
        }


        public void setDiasble(int index)
        {
            _cachedView.DisableObj.SetActive(true);
            _cachedView.EnableObj.SetActive(false);
            SetIndex(index);
        }

        public void setEenable()
        {
            _cachedView.DisableObj.SetActive(false);
            _cachedView.EnableObj.SetActive(true);
        }
    }
}