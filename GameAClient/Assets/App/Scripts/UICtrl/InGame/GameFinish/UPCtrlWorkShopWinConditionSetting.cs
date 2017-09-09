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
            InitData();
            InitUI();
            _cachedView.SureBtn.onClick.AddListener(OnButtonEnsureClick);
            _cachedView.CloseBtn.onClick.AddListener(OnButtonCancleClick);
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
            UpdateData();
            UpdateUIItem();
            UpdateLifeItem();
        }
        
        private FinishCondition _curCondition;

        private void OnButtonEnsureClick()
        {
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                EditMode.Instance.MapStatistics.SetWinCondition(i, _curCondition.SettingValue[(int) i]);
            }
            EditMode.Instance.MapStatistics.TimeLimit = _curCondition.TimeLimit;
            EditMode.Instance.MapStatistics.LifeCount = _curCondition.LifeCount;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.WindowClosed);
        }

        private void OnButtonCancleClick()
        {
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.WindowClosed);
        }

        private void OnLifeItemButtonClick(int lifeId)
        {
            if (_curCondition.LifeCount == lifeId)
            {
                return;
            }
            _curCondition.LifeCount = lifeId;
            UpdateLifeItem();
        }

        private void InitData()
        {
            _curCondition = new FinishCondition();
            _curCondition.SettingValue = new bool[(int) EWinCondition.Max];
            _curCondition.TimeLimit = 0;
        }

        private void InitUI()
        {
            for (int i = 0; i < _cachedView.ItemArray.Length; i++)
            {
                var item = _cachedView.ItemArray[i];
                if (item != null)
                    item.InitItem((EWinCondition) i, _curCondition);
            }
            for (int i = 0; i < _cachedView.LifeItemArray.Length; i++)
            {
                var item = _cachedView.LifeItemArray[i];
                item.Init(i + 1, OnLifeItemButtonClick);
            }
            _cachedView.LifeShowText.text = "初始生命";
        }

        private void UpdateData()
        {
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                _curCondition.SettingValue[(int) i] = EditMode.Instance.MapStatistics.HasWinCondition(i);
            }
            _curCondition.TimeLimit = EditMode.Instance.MapStatistics.TimeLimit;
            _curCondition.LifeCount = EditMode.Instance.MapStatistics.LifeCount;
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
                _cachedView.LifeItemArray[i].UpdateShow(_curCondition.LifeCount);
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