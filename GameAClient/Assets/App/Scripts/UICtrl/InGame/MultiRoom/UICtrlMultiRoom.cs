using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlMultiRoom : UICtrlInGameBase<UIViewMultiRoom>
    {
        private const string EmptyStr = "无";
        private Project _project;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefrshView();
        }

        private void RefrshView()
        {
            RefreshWinConditionView();
        }

        private void RefreshWinConditionView()
        {
            if (_project == null) return;
            var netData = _project.NetData;
            if (netData == null) return;
            _cachedView.TitleTxt.text = _project.Name;
            _cachedView.TimeLimit.text = netData.GetTimeLimit();
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.text = netData.ScoreWinCondition ? netData.WinScore.ToString() : EmptyStr;
            _cachedView.ArriveScore.text = netData.ArriveScore.ToString();
            _cachedView.CollectGemScore.text = netData.CollectGemScore.ToString();
            _cachedView.KillMonsterScore.text = netData.KillMonsterScore.ToString();
            _cachedView.KillPlayerScore.text = netData.KillPlayerScore.ToString();
        }
    }
}