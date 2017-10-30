using GameA.Game;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlBattle : UICtrlAnimationBase<UIViewBattle>
    {
        private bool _pushGoldEnergyStyle;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.MatchModifyBtn.onClick.AddListener(OnMatchModifyBtn);
            _cachedView.MatchShadowBattleBtn.onClick.AddListener(OnMatchShadowBattleBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
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

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }
        
        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp,new Vector3(0,100,0),0.1f);
            SetPart(_cachedView.LeftPannelRtf.transform, EAnimationType.MoveFromLeft);
            SetPart(_cachedView.RightPannelRtf, EAnimationType.MoveFromRight);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlBattle>();
        }

        private void OnMatchShadowBattleBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlShadowBattle>();
        }

        private void OnMatchModifyBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.ModifyMatch))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlModifyMatchMain>();
            }
        }
    }
}