using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopWinConditionSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            
        }

        public override void Open()
        {
            base.Open();
            _cachedView.WinConditionPannel.SetActive(true);
        }

        public override void Close()
        {
            base.Close();
            _cachedView.WinConditionPannel.SetActive(false);
        }

        
    }
}
