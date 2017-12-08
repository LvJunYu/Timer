using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopNetBattleWinCondition : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlSliderSetting _usTimeLimitSetting;
        private USCtrlSliderSetting _usArriveScoreSetting;
        private USCtrlSliderSetting _usCollectGemScoreSetting;
        private USCtrlSliderSetting _usKillMonsterScoreSetting;
        private USCtrlSliderSetting _usKillPlayerScoreSetting;
        private USCtrlSliderSetting _usWinScoreSetting;
        private USCtrlGameSettingItem _usScoreConditionSetting;
        private bool _onlyChangeView;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _usTimeLimitSetting = new USCtrlSliderSetting();
            _usArriveScoreSetting = new USCtrlSliderSetting();
            _usCollectGemScoreSetting = new USCtrlSliderSetting();
            _usKillMonsterScoreSetting = new USCtrlSliderSetting();
            _usKillPlayerScoreSetting = new USCtrlSliderSetting();
            _usWinScoreSetting = new USCtrlSliderSetting();
            _usTimeLimitSetting.Init(_cachedView.TimeLimitSetting);
            _usArriveScoreSetting.Init(_cachedView.ArriveScoreSetting);
            _usCollectGemScoreSetting.Init(_cachedView.CollectGemScoreSetting);
            _usKillMonsterScoreSetting.Init(_cachedView.KillMonsterScoreSetting);
            _usKillPlayerScoreSetting.Init(_cachedView.KillPlayerScoreSetting);
            _usWinScoreSetting.Init(_cachedView.WinScoreSetting);
            _usTimeLimitSetting.Set(1, 3, value => EditMode.Instance.MapStatistics.NetBattleTimeLimit = value, 1,"{0}分钟");
            _usArriveScoreSetting.Set(0, 500, value => EditMode.Instance.MapStatistics.NetBattleArriveScore = value,10);
            _usCollectGemScoreSetting.Set(0, 500, value => EditMode.Instance.MapStatistics.NetBattleCollectGemScore = value, 10);
            _usKillMonsterScoreSetting.Set(0, 500, value => EditMode.Instance.MapStatistics.NetBattleKillMonsterScore = value, 10);
            _usKillPlayerScoreSetting.Set(0, 500, value => EditMode.Instance.MapStatistics.NetBattleKillPlayerScore = value, 10);
            _usWinScoreSetting.Set(0, 500, value => EditMode.Instance.MapStatistics.NetBattleWinScore = value, 10);
            _usScoreConditionSetting = new USCtrlGameSettingItem();
            _usScoreConditionSetting.Init(_cachedView.ScoreConditionSetting);

            for (int i = 0; i < _cachedView.WinConditionTogs.Length; i++)
            {
                var inx = i;
                _cachedView.WinConditionTogs[i].onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        EditMode.Instance.MapStatistics.NetBattleTimeWinCondition = inx;
                    }
                });
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NetBattleWinConditionPannel.SetActiveEx(true);
            _cachedView.BtnDock1.SetActiveEx(true);
            _cachedView.BtnDock2.SetActiveEx(false);
            RefeshView();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.NetBattleWinConditionPannel.SetActiveEx(false);
        }

        public void RefeshView()
        {
            for (int i = 0; i < _cachedView.WinConditionTogs.Length; i++)
            {
                _cachedView.WinConditionTogs[i].isOn = EditMode.Instance.MapStatistics.NetBattleTimeWinCondition == i;
            }
            _usScoreConditionSetting.SetData(EditMode.Instance.MapStatistics.NetBattleScoreWinCondition, value =>
            {
                EditMode.Instance.MapStatistics.NetBattleScoreWinCondition = value;
                _cachedView.WinScoreSetting.SetActiveEx(value);
            });
            _cachedView.WinScoreSetting.SetActiveEx(EditMode.Instance.MapStatistics.NetBattleScoreWinCondition);
            _usTimeLimitSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleTimeLimit);
            _usArriveScoreSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleArriveScore);
            _usCollectGemScoreSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleCollectGemScore);
            _usKillMonsterScoreSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleKillMonsterScore);
            _usKillPlayerScoreSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleKillPlayerScore);
            _usWinScoreSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleWinScore);
        }
    }
}