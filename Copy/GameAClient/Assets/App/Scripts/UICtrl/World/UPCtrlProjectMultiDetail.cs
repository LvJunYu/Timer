using SoyEngine;

namespace GameA
{
    public class UPCtrlProjectMultiDetail : UPCtrlProjectDetailBase
    {
        private const string NoneStr = "无";

        public override void Open()
        {
            base.Open();
            RefreshView();
        }

        protected override void RefreshView()
        {
            var project = _mainCtrl.Project;
            if (project == null) return;
            var netData = project.NetData;
            if (netData == null) return;
            _cachedView.PlayerCount.text = netData.PlayerCount.ToString();
            _cachedView.LifeCount.text = netData.GetLifeCount();
            _cachedView.ReviveTime.text = netData.GetReviveTime();
            _cachedView.ReviveProtectTime.text = netData.GetReviveProtectTime();
            _cachedView.TimeLimit.text = netData.GetTimeLimit();
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.text = netData.ScoreWinCondition ? netData.WinScore.ToString() : NoneStr;
            _cachedView.ArriveScore.text = netData.ArriveScore.ToString();
            _cachedView.CollectGemScore.text = netData.CollectGemScore.ToString();
            _cachedView.KillMonsterScore.text = netData.KillMonsterScore.ToString();
            _cachedView.KillPlayerScore.text = netData.KillPlayerScore.ToString();
//            _cachedView.ArriveScore.SetActiveEx(Game.PlayMode.Instance.SceneState.FinalCount > 0);
//            _cachedView.CollectGemScore.SetActiveEx(Game.PlayMode.Instance.SceneState.TotalGem > 0);
//            _cachedView.KillMonsterScore.SetActiveEx(Game.PlayMode.Instance.SceneState.MonsterCount > 0);
        }

        protected override void RequestData(bool append = false)
        {
        }

        public override void Clear()
        {
        }
    }
}