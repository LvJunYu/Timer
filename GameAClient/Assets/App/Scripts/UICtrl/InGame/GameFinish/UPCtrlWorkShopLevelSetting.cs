using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlWorkShopLevelSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
            _cachedView.SureBtn_2.onClick.AddListener(OnButtonEnsureClick);
            _cachedView.SureBtn_3.onClick.AddListener(OnButtonEnsureClick);

            _cachedView.TestBtn_2.onClick.AddListener(_mainCtrl.OnTestBtn);
            _cachedView.PublishBtn_2.onClick.AddListener(_mainCtrl.OnPublishBtn);
            _cachedView.TitleInputField_2.onEndEdit.AddListener(_mainCtrl.OnTitleEndEdit);
            _cachedView.DescInputField_2.onEndEdit.AddListener(_mainCtrl.OnDescEndEdit);
        }

        private void UpdateBtns()
        {
            var gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeWorkshopEdit != null)
            {
                bool canPublish = gameModeWorkshopEdit.CheckCanPublish();
                _cachedView.TestBtn_2.gameObject.SetActive(!canPublish);
                _cachedView.TestBtnFinished_2.SetActive(canPublish);
                _cachedView.PublishBtn_2.gameObject.SetActive(canPublish);
                _cachedView.PublishBtnDisable_2.SetActive(!canPublish);
            }
        }

        private void UpdateInfo()
        {
            _cachedView.TitleInputField_2.text = _mainCtrl.CurProject.Name;
            _cachedView.DescInputField_2.text = _mainCtrl.CurProject.Summary;
        }

        public override void Open()
        {
            base.Open();
            _cachedView.LevelSettingPannel.SetActive(true);
            _cachedView.BtnDock1.SetActiveEx(true);
            _cachedView.BtnDock2.SetActiveEx(false);
            UpdateAll();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.LevelSettingPannel.SetActiveEx(false);
        }

        public void UpdateAll()
        {
            UpdateUIItem();
            UpdateLifeItem();
            UpdateBtns();
            UpdateInfo();
        }

        private void OnButtonEnsureClick()
        {
            if (_mainCtrl.IsMulti) return;
            for (EWinCondition i = 0; i < EWinCondition.WC_Max; i++)
            {
                EditMode.Instance.MapStatistics.SetWinCondition(i, _mainCtrl.CurCondition.SettingValue[(int) i]);
            }
            EditMode.Instance.MapStatistics.TimeLimit = _mainCtrl.CurCondition.TimeLimit;
            EditMode.Instance.MapStatistics.LifeCount = _mainCtrl.CurCondition.LifeCount;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.WindowClosed);
        }

        private void OnLifeItemButtonClick(int lifeId)
        {
            if (_mainCtrl.CurCondition.LifeCount == lifeId)
            {
                return;
            }
            _mainCtrl.CurCondition.LifeCount = lifeId;
            UpdateLifeItem();
        }

        private void InitUI()
        {
            for (int i = 0; i < _cachedView.ItemArray_2.Length; i++)
            {
                var item = _cachedView.ItemArray_2[i];
                if (item != null)
                    item.InitItem((EWinCondition) i, _mainCtrl.CurCondition);
            }
            for (int i = 0; i < _cachedView.LifeItemArray_2.Length; i++)
            {
                var item = _cachedView.LifeItemArray_2[i];
                item.Init(i + 1, OnLifeItemButtonClick);
            }
            _cachedView.LifeShowText_2.text = "初始生命";
        }

        private void UpdateUIItem()
        {
            for (int i = 0; i < _cachedView.ItemArray_2.Length; i++)
            {
                var item = _cachedView.ItemArray_2[i];
                if (item != null)
                    item.UpdateShow();
            }
        }

        private void UpdateLifeItem()
        {
            for (int i = 0; i < _cachedView.LifeItemArray_2.Length; i++)
            {
                _cachedView.LifeItemArray_2[i].UpdateShow(_mainCtrl.CurCondition.LifeCount);
            }
        }
    }
}