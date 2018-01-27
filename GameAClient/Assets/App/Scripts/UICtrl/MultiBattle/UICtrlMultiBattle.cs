using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlMultiBattle : UICtrlAnimationBase<UIViewMultiBattle>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }
    }
}