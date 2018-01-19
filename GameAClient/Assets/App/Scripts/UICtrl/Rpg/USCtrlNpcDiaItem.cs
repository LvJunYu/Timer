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

        public void Set(NpcDia dia, List<NpcDia> diaList, int index, Action callback)
        {
            _diaList = diaList;
            _index = index;
            _dia = dia;
            _callback = callback;
            string name = NpcDia.GetNpcFaceSpriteName(dia.NpcId, dia.FaceId);
            JoyResManager.Instance.TryGetSprite(name, out _sprite);
            _cachedView.IconImage.sprite = _sprite;
            _cachedView.DiaText.text = dia.Dia;
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


        public void setDiasble()
        {
            _cachedView.SetActiveEx(false);
        }

        public void setEenable()
        {
            _cachedView.SetActiveEx(true);
        }
    }
}