using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlWoodCaseItemsSetting : USCtrlBase<USViewWoodCaseItemsSetting>
    {
        private static int[] Items = {6001, 6002, 6003, 6004, 8002, 8005, 8006, 8007, 8008, 8009, 8010, 0};
        private EResScenary _resScenary;
        private UnitExtraDynamic _unitExtra;
        private int _curId;
        private List<UMCtrlWoodCaseItem> _umList = new List<UMCtrlWoodCaseItem>();
        private USCtrlUnitPropertyEditButton _usCtrlWoodCase;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            for (int i = 0; i < Items.Length; i++)
            {
                var um = new UMCtrlWoodCaseItem();
                um.Init(_cachedView.AllItemRtf, _resScenary);
                um.Set(Items[i], this);
                _umList.Add(um);
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.SetActiveEx(true);
            _cachedView.AllItemRtf.anchoredPosition = Vector2.zero;
            RefreshView();
        }

        private void RefreshView()
        {
            for (int i = 0; i < _umList.Count; i++)
            {
                _umList[i].SetSelected(_curId);
            }
        }

        public override void Close()
        {
            base.Close();
            _cachedView.SetActiveEx(false);
        }

        public void Select(int id)
        {
            if (_curId != id)
            {
                _unitExtra.CommonValue = (ushort) id;
                _curId = id;
                RefreshView();
                _usCtrlWoodCase.SetFgImage(UMCtrlWoodCaseItem.GetSprite(id), id == 0, 54, 54);
            }
        }

        public void Set(EResScenary resScenary, USCtrlUnitPropertyEditButton woodCaseCtrl)
        {
            _resScenary = resScenary;
            _usCtrlWoodCase = woodCaseCtrl;
        }

        public void SetUnitExtra(UnitExtraDynamic unitExtra)
        {
            _unitExtra = unitExtra;
            _curId = unitExtra.CommonValue;
        }
    }
}