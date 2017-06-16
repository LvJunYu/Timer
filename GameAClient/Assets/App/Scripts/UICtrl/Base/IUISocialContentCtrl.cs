//  /********************************************************************
//  ** Filename : IUISocialContentCtrl.cs
//  ** Author : quan
//  ** Date : 2016/6/20 16:31
//  ** Summary : IUISocialContentCtrl.cs
//  ***********************************************************************/
using System;
using UnityEngine.UI;

namespace GameA
{/*社交ui的控制接口*/
    public interface IUISocialContentCtrl:IUISocialCtrl
    {
        ScrollRect GetBoundsScrollRect();
        void ScrollToTop();
    }
}

