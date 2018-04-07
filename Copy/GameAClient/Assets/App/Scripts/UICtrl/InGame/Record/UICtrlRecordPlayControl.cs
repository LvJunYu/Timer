using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlRecordPlayControl : UICtrlInGameBase<UIViewRecordPlayControl>
    {
        private GameModePlayRecord _gameModePlayRecord;
        private const int MinSpeedLevel = -1;
        private const int MaxSpeedLevel = 3;
        private int _speedLevel;
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GlobalButton.onClick.AddListener(OnGlobalBtnClick);
            _cachedView.BackButton.onClick.AddListener(OnBackBtnClick);
            _cachedView.PlayButton.onClick.AddListener(OnPlayBtnClick);
            _cachedView.PauseButton.onClick.AddListener(OnPauseBtnClick);
            _cachedView.DecreaseSpeedButton.onClick.AddListener(OnDecreaseSpeedBtnClick);
            _cachedView.IncreaseSpeedButton.onClick.AddListener(OnIncreaseSpeedBtnClick);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _speedLevel = 0;
            _cachedView.ContentDock.SetActiveEx(true);
            _gameModePlayRecord = GM2DGame.Instance.GameMode as GameModePlayRecord;
            _cachedView.PlayButton.SetActiveEx(false);
            _cachedView.PauseButton.SetActiveEx(true);
            RefreshView();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            RefreshCurrentTime();
        }

        private void RefreshView()
        {
            DictionaryTools.SetContentText(_cachedView.TitleText,
                _gameModePlayRecord.Record.UserInfoDetail.UserInfoSimple.NickName + "的过关录像");
            DictionaryTools.SetContentText(_cachedView.TotalTimeText,
                GameATools.GetTimeStringByFrameCount(_gameModePlayRecord.Record.UsedTime));
            RefreshSpeedView();
            RefreshCurrentTime();
        }

        private void RefreshSpeedView()
        {
            _speedLevel = Mathf.Clamp(_speedLevel, MinSpeedLevel, MaxSpeedLevel);
            _gameModePlayRecord.SpeedMultiple = Mathf.Pow(2, _speedLevel);
            DictionaryTools.SetContentText(_cachedView.SpeedText,
                string.Format("{0:0.#}×", _gameModePlayRecord.SpeedMultiple));
            _cachedView.IncreaseSpeedButton.interactable = _speedLevel != MaxSpeedLevel;
            _cachedView.DecreaseSpeedButton.interactable = _speedLevel != MinSpeedLevel;
        }

        private void RefreshCurrentTime()
        {
            DictionaryTools.SetContentText(_cachedView.CurrentTimeText,
                GameATools.GetTimeStringByFrameCount(_gameModePlayRecord.CurrentFrameCount));
            float progress = 1f * _gameModePlayRecord.CurrentFrameCount / _gameModePlayRecord.Record.UsedTime;
            _cachedView.ProgressFgMaskImage.fillAmount = progress;
        }
        

        private void OnGlobalBtnClick()
        {
            _cachedView.ContentDock.SetActiveEx(!_cachedView.ContentDock.activeSelf);
        }

        private void OnBackBtnClick()
        {
            SocialApp.Instance.ReturnToApp();
        }

        private void OnPlayBtnClick()
        {
            _gameModePlayRecord.Continue();
            _cachedView.PlayButton.SetActiveEx(false);
            _cachedView.PauseButton.SetActiveEx(true);
        }

        private void OnPauseBtnClick()
        {
            _gameModePlayRecord.Pause();
            _cachedView.PlayButton.SetActiveEx(true);
            _cachedView.PauseButton.SetActiveEx(false);
        }

        private void OnDecreaseSpeedBtnClick()
        {
            _speedLevel--;
            RefreshSpeedView();
        }

        private void OnIncreaseSpeedBtnClick()
        {
            _speedLevel++;
            RefreshSpeedView();
        }
    }
}