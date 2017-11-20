using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalInfoAchievement : UMCtrlBase<UMViewPersonalInfoAchievement>, IDataItemRenderer
    {
        private AchievementItem _achievement;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _achievement; }
        }

        public void RefreshView()
        {
            if (_achievement == null) return;
            _cachedView.NameTxt.text = TableManager.Instance.GetAchievement(_achievement.AchievementId).Name;
            _cachedView.DateTxt.text = GameATools.GetYearMonthDayHourMinuteSecondByMilli(_achievement.CreateTime, 1);
        }

        public void Set(object obj)
        {
            _achievement = obj as AchievementItem;
            RefreshView();
        }

        public void Unload()
        {
        }
    }
}