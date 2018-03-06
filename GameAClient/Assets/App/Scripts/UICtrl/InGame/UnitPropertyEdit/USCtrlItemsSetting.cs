using System.Collections.Generic;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class USCtrlItemsSetting : USCtrlBase<USViewItemsSetting>
    {
        public static int[] Items = {6001, 6002, 6003, 6004};
        private EResScenary _resScenary;
        private UnitExtraDynamic _unitExtra;
        private Stack<UMCtrlSurpriseBoxItem> _umPool = new Stack<UMCtrlSurpriseBoxItem>(70);
        private List<UMCtrlSurpriseBoxItem> _umList = new List<UMCtrlSurpriseBoxItem>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            for (int i = 0; i < Items.Length; i++)
            {
                var um = GetUmItem();
                um.SetParent(_cachedView.AllItemRtf);
                um.Set(Items[i], true, this);
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.SetActiveEx(true);
            RefreshView();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.SetActiveEx(false);
        }

        private void RefreshView()
        {
            for (int i = 0; i < _umList.Count; i++)
            {
                FreeUmItem(_umList[i]);
            }

            _umList.Clear();
            var list = _unitExtra.SurpriseBoxItems.ToList<ushort>();
            for (int i = 0; i < list.Count; i++)
            {
                AddItem(list[i], true);
            }
        }

        public void AddItem(int id, bool init = false)
        {
            if (!init)
            {
                _unitExtra.SurpriseBoxItems.Add((ushort) id);
            }

            var um = GetUmItem();
            um.SetParent(_cachedView.AddedItemRtf);
            um.Set(id, false, this, _umList.Count);
            _umList.Add(um);
        }

        public void DeleteItem(int index)
        {
            if (index < _umList.Count)
            {
                _unitExtra.SurpriseBoxItems.RemoveAt(index);
                FreeUmItem(_umList[index]);
            }
        }

        private void FreeUmItem(UMCtrlSurpriseBoxItem umCtrlChat)
        {
            umCtrlChat.SetParent(_cachedView.PoolDock);
            _umPool.Push(umCtrlChat);
        }

        private UMCtrlSurpriseBoxItem GetUmItem()
        {
            if (_umPool.Count > 0)
            {
                return _umPool.Pop();
            }

            var um = new UMCtrlSurpriseBoxItem();
            um.Init(_cachedView.PoolDock, _resScenary);
            return um;
        }

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetUnitExtra(UnitExtraDynamic unitExtra)
        {
            _unitExtra = unitExtra;
        }
    }
}