using SoyEngine;
using SoyEngine.Proto;

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

        public void OnLifeItemButtonClick(int lifeId)
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