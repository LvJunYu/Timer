
namespace GameA
{
    public class UMCtrlAchievementItem : UMCtrlBase<UMViewAchievementItem>
    {
        private AchievementStatisticItem _achievementStatisticItem;
        private int _showLv;
        private bool _finished;
        private long _count;
        public bool IsShow;

        public AchievementStatisticItem AchievementStatisticStatisticItem
        {
            get { return _achievementStatisticItem; }
        }

        public void SetDate(AchievementStatisticItem achievementItem, bool finished)
        {
            _achievementStatisticItem = achievementItem;
            _finished = finished;
            _count = achievementItem.Count;
            Refresh();
        }

        public void Refresh()
        {
            if (_finished)
                _showLv = _achievementStatisticItem.FinishLevel;
            else if (_achievementStatisticItem.NextLevel != null) 
                _showLv = _achievementStatisticItem.NextLevel.Value;
//            _cachedView.Icon.sprite = "XXX";
            _cachedView.NameTxt.text = _achievementStatisticItem.LvDic[_showLv].Name;
            _cachedView.DescTxt.text = _achievementStatisticItem.LvDic[_showLv].Description;
            _cachedView.RewardTxt.text = _achievementStatisticItem.LvDic[_showLv].Reward.ToString();
            _cachedView.CurValueTxt.text = GetCurValueString(_count);
            _cachedView.Able.SetActive(!_finished);
            _cachedView.Disable.SetActive(_finished);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
        }

        public void Collect()
        {
            IsShow = false;
            _cachedView.gameObject.SetActive(false);
        }

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
            IsShow = true;
        }

        private string GetCurValueString(long value)
        {
            return string.Format("{0}/{1}", value, _achievementStatisticItem.LvDic[_showLv].Condition);
        }
    }
}