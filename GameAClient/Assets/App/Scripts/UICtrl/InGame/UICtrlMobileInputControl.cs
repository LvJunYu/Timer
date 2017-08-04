using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMobileInputControl : UICtrlGenericBase<UIViewMobileInputControl>
    {
        #region fields

        #endregion

        #region properties

        #endregion

        #region methods
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InputCtrl;
        }
        #endregion
    }
}