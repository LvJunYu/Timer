using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using System.IO;
using GameA.Game;


namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMailDetail : UICtrlGenericBase<UIViewMailDetail>
    {
        private void Set(Mail mail)
        {



        }

         protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

    }
}
