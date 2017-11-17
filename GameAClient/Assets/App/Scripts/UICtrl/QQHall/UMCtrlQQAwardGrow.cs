using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlQQAwardGrow : UMCtrlBase<UMViewQQAwardGrow>, IDataItemRenderer
    {
        private int _coinNum;
        private int _diamodNum;
        private EQQGamePrivilegeType _privilegeType;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public int Index { get; set; }
        public object Data { get; private set; }

        public void Set(object data)
        {
            var award = data as Table_QQHallGrowAwardStatus;
            if (award == null)
            {
                return;
            }
            SetAward(award.GrowAward, award.Status, award.Type);
            return;
        }

        public void Unload()
        {
            throw new System.NotImplementedException();
        }

        public void SetAward(Table_QQHallGrowAward award, EQQGameRewardStatus isColltion, EQQGamePrivilegeType type)
        {
            bool _levelUp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel >= award.Id;
            DictionaryTools.SetContentText(_cachedView.AwarLivesNum, award.Id.ToString());
            DictionaryTools.SetContentText(_cachedView.AwardCoinsNum, award.CoinNum.ToString());
            DictionaryTools.SetContentText(_cachedView.AwarDiamondDiNum, award.DiamodNum.ToString());
            _coinNum = award.CoinNum;
            _diamodNum = award.DiamodNum;
            _cachedView.HaveColltionBtn.SetActiveEx(isColltion == EQQGameRewardStatus.QGRS_Received);
            _cachedView.ColltionBtn.SetActiveEx(isColltion == EQQGameRewardStatus.QGRS_CanReceive);
            _privilegeType = type;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ColltionBtn.onClick.AddListener(OnColltionBtn);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnColltionBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlLittleLoading>();
            RemoteCommands.ReceiveQQGameReward(_privilegeType, EQQGamePrivilegeSubType.QGPST_Grow, 0,
                EQQGameBlueVipType.QGBVT_All,
                msg =>
                {
                    if (msg.ResultCode == (int) EExecuteCommandCode.ECC_Success)
                    {
                        EndColltion();
                    }
                    else
                    {
                    }
                    SocialGUIManager.Instance.CloseUI<UICtrlLittleLoading>();
                },
                code => { SocialGUIManager.Instance.CloseUI<UICtrlLittleLoading>(); });
        }

        private void EndColltion()
        {
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += _coinNum;
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += _diamodNum;
            _cachedView.ColltionBtn.SetActiveEx(false);
        }
    }
}