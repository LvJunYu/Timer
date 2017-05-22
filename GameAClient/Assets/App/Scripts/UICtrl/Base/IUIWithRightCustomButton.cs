 /********************************************************************
 ** Filename : IUIWithRightCustomButton.cs
 ** Author : quansiwei
 ** Date : 2015/5/6 23:28
 ** Summary : 拥有自定义右按钮
 ***********************************************************************/



using SoyEngine;
using UnityEngine.UI;
using UnityEngine;


namespace GameA
{/*客户右侧的按钮接口*/
    public interface IUIWithRightCustomButton
    {
        Button GetRightButton();
//        void OnRightButtonClick(UICtrlTitlebar titleBar);
    }
}