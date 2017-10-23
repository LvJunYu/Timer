using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlAchievementItem : UMCtrlBase<UMViewAchievementItem>, IDataItemRenderer
    {
        private static string strFormat = "{0}/{1}";
        private AchievementStatisticItem _achievementStatisticItem;
        private int _showLv;
        private bool _finished;

        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _achievementStatisticItem; }
        }

        public void Set(object data)
        {
            var achieveData = data as AchiveItemData;
            if (achieveData == null) return;
            _achievementStatisticItem = achieveData.AchievementStatisticItem;
            _finished = achieveData.Finish;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_finished)
                _showLv = _achievementStatisticItem.FinishLevel;
            else if (_achievementStatisticItem.NextLevel != null)
                _showLv = _achievementStatisticItem.NextLevel.Value;
            _cachedView.NameTxt.text = _achievementStatisticItem.LvDic[_showLv].Name;
            _cachedView.DescTxt.text = _achievementStatisticItem.LvDic[_showLv].Description;
            _cachedView.RewardTxt.text = _achievementStatisticItem.LvDic[_showLv].Reward.ToString();
            _cachedView.CurValueTxt.text = string.Format(strFormat, _achievementStatisticItem.Count,
                _achievementStatisticItem.LvDic[_showLv].Condition);
            _cachedView.Able.SetActive(!_finished);
            _cachedView.Disable.SetActive(_finished);
        }

        public void Unload()
        {
        }
    }
}