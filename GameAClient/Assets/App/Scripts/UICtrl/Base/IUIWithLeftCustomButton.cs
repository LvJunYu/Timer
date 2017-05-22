 /********************************************************************
 ** Filename : IUIWithLeftCustomButton.cs
 ** Author : quansiwei
 ** Date : 2015/5/6 23:28
 ** Summary : 拥有自定义左按钮
 ***********************************************************************/



using SoyEngine;
using UnityEngine.UI;
using UnityEngine;


namespace GameA
{/*用户左边按钮*/
    public interface IUIWithLeftCustomButton
    {
        Button GetLeftButton();
//        void OnLeftButtonClick(UICtrlTitlebar titleBar); 
    }
}