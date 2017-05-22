 /********************************************************************
 ** Filename : IUISocialCtrl.cs
 ** Author : quansiwei
 ** Date : 2015/5/6 23:04
 ** Summary : SocialUI接口
 ***********************************************************************/


 /*社交ui接口*/
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;


namespace GameA
{
    public interface IUISocialCtrl
    {
        RectTransform UITrans
        {
            get;
        }
        void OpenBegin(object param = null);
        void OpenComplete(object param = null);

        void SetUIStack(UIStack uiStack);

        void ClearUIStack();

        void BringToFront();

        void Close();
    }
}