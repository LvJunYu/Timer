using System;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;

namespace GameA
{
    public class USCtrSetItem : USCtrlBase<USViewSetItem>
    {
        private const string AddBtnSpriteName = "btn_edit_add";

        public void AddItemClickListener(Action<int> action)
        {
            for (int i = 0; i < _cachedView.ItemBtns.Length; i++)
            {
                var inx = i;
                _cachedView.ItemBtns[i].onClick.AddListener(() => action(inx));
            }
        }

        public void AddDeleteItemBtnListener(Action<int> action)
        {
            for (int i = 0; i < _cachedView.DeleteItemBtns.Length; i++)
            {
                var inx = i;
                _cachedView.DeleteItemBtns[i].onClick.AddListener(() => action(inx));
            }
        }

        public void SetCur(List<ushort> weapons)
        {
            for (int i = 0; i < _cachedView.Imgs.Length; i++)
            {
                if (weapons != null && i < weapons.Count && weapons[i] != 0)
                {
                    var table = TableManager.Instance.GetEquipment(weapons[i]);
                    if (table != null)
                    {
                        _cachedView.Imgs[i].sprite = JoyResManager.Instance.GetSprite(table.Icon);
                        _cachedView.DeleteItemBtns[i].SetActiveEx(true);
                        continue;
                    }
                }

                _cachedView.DeleteItemBtns[i].SetActiveEx(false);
                _cachedView.Imgs[i].sprite = JoyResManager.Instance.GetSprite(AddBtnSpriteName);
            }
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }
    }
}