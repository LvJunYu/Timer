using System.Collections.Generic;
using UnityEngine;

namespace GameA
{
    public class UICtrlMultiBase : UICtrlAnimationBase<UIViewMultiBase>
    {
        protected bool _hasRequested;
        protected bool _pushGoldEnergyStyle;
        protected OfficialProjectList _data = new OfficialProjectList();
        protected List<Project> _dataList;
        
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.QuickJoinBtn.onClick.AddListener(OnQuickJoinBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }
            if (!_hasRequested)
            {
                RequestData();
            }
        }

        protected virtual void RequestData()
        {
        }

        protected void RefreshView()
        {
            if (_dataList == null || _dataList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < _dataList.Count; i++)
            {
                var um = new UMCtrlOfficialProject();
                um.Init(_cachedView.GridRtf, ResScenary);
                um.Set(_dataList[i]);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp, new Vector3(0, 100, 0), 0.17f);
            SetPart(_cachedView.PannelRtf.transform, EAnimationType.MoveFromDown);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
        }

        protected virtual void OnQuickJoinBtn()
        {
        }

        protected virtual void OnReturnBtn()
        {
        }
    }
}