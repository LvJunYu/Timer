using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlInGameUnitHandbook : UICtrlInGameBase<UIViewInGameUnitHandbook>
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }        
        #endregion
    }
}