
namespace GameA
{
    public class UMCtrlAchievementItem : UMCtrlBase<UMViewAchievementItem>
    {
        private AchievementStatisticItem _achievementItem;
        private int _showLv;
        private bool _finished;
        private int _curValue;
        public bool IsShow;

        public void SetDate(AchievementStatisticItem achievementItem, bool finished)
        {
            _achievementItem = achievementItem;
            _finished = finished;
            _curValue = (int)achievementItem.Count;
            Refresh();
        }

        public void Refresh()
        {
            if (_finished)
                _showLv = _achievementItem.FinishLevel;
            else if (_achievementItem.NextLevel != null) 
                _showLv = _achievementItem.NextLevel.Value;
//            _cachedView.Icon.sprite = "XXX";
            _cachedView.NameTxt.text = _achievementItem.LvDic[_showLv].Name;
            _cachedView.DescTxt.text = _achievementItem.LvDic[_showLv].Description;
            _cachedView.RewardTxt.text = _achievementItem.LvDic[_showLv].Reward.ToString();
            _cachedView.CurValueTxt.text = GetCurValueString(_curValue);
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

        private string GetCurValueString(int value)
        {
            return string.Format("{0}/{1}", value, _achievementItem.LvDic[_showLv].Condition);
        }
    }
}