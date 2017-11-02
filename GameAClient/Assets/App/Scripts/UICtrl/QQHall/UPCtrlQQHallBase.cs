using System;
using SoyEngine;

namespace GameA
{
    public abstract class UPCtrlQQHallBase : UPCtrlBase<UICtrlQQHall, UIViewQQHall>
    {
        protected EResScenary _resScenary;
        protected UICtrlQQHall.EMenu _menu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }
        public override void Open()
        {
            base.Open();
            _cachedView.AwardPanel[(int) _menu].SetActiveEx(true);

        }

        public override void Close()
        {
            _cachedView.AwardPanel[(int) _menu].SetActiveEx(false);
            base.Close();
        }


        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlQQHall.EMenu menu)
        {
            _menu = menu;
        }

      
    }
}