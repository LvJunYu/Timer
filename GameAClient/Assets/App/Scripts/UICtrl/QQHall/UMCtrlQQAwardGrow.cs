using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlQQAwardGrow : UMCtrlBase<UMViewQQAwardGrow> ,IDataItemRenderer
    {

        private int _coinNum;
        private int _diamodNum;
        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public int Index { get; set; }
        public object Data { get; private set; }
        public void Set(object data)
        {
            var award = data as Table_QQHallGrowAward;
            if (award == null )
            {
                return;
            }
            SetAward(award,false);
            return;
        }

        public void Unload()
        {
            throw new System.NotImplementedException();
        }

        public void SetAward(Table_QQHallGrowAward award, bool isColltion)
        {
            bool _levelUp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel >= award.Id;
            DictionaryTools.SetContentText(_cachedView.AwarLivesNum, award.Id.ToString());
            DictionaryTools.SetContentText(_cachedView.AwardCoinsNum, award.CoinNum.ToString());
            DictionaryTools.SetContentText(_cachedView.AwarDiamondDiNum, award.DiamodNum.ToString());
            _coinNum = award.CoinNum;
            _diamodNum = award.DiamodNum;
            _cachedView.HaveColltionBtn.SetActiveEx(isColltion&&_levelUp);
            _cachedView.ColltionBtn.SetActiveEx(!isColltion&&_levelUp);
            
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
            
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += _coinNum;
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += _diamodNum;
            _cachedView.ColltionBtn.SetActiveEx(false);
        }
    }
}