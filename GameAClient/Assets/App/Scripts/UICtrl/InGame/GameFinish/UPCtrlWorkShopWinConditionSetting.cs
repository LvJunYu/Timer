using GameA.Game;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 游戏胜利条件设置界面
    /// </summary>
    public class UPCtrlWorkShopWinConditionSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
            _cachedView.SureBtn.onClick.AddListener(OnButtonEnsureClick);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.WinConditionPannel.SetActive(true);
            UpdateAll();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.WinConditionPannel.SetActive(false);
        }

        public void UpdateAll()
        {
            UpdateUIItem();
            UpdateLifeItem();
        }

        private void OnButtonEnsureClick()
        {
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
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
            for (int i = 0; i < _cachedView.ItemArray.Length; i++)
            {
                var item = _cachedView.ItemArray[i];
                if (item != null)
                    item.InitItem((EWinCondition) i, _mainCtrl.CurCondition);
            }
            for (int i = 0; i < _cachedView.LifeItemArray.Length; i++)
            {
                var item = _cachedView.LifeItemArray[i];
                item.Init(i + 1, OnLifeItemButtonClick);
            }
            _cachedView.LifeShowText.text = "初始生命";
        }

        private void UpdateUIItem()
        {
            for (int i = 0; i < _cachedView.ItemArray.Length; i++)
            {
                var item = _cachedView.ItemArray[i];
                if (item != null)
                    item.UpdateShow();
            }
        }

        private void UpdateLifeItem()
        {
            for (int i = 0; i < _cachedView.LifeItemArray.Length; i++)
            {
                _cachedView.LifeItemArray[i].UpdateShow(_mainCtrl.CurCondition.LifeCount);
            }
        }
    }

    public class FinishCondition
    {
        public bool[] SettingValue;
        public int TimeLimit;
        public int LifeCount;
    }
}