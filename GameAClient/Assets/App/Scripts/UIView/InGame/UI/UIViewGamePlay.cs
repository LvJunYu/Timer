/********************************************************************
** Filename : UIViewGamePlay  
** Author : ake
** Date : 4/27/2016 5:57:44 PM
** Summary : UIViewGamePlay  
***********************************************************************/


using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGamePlay:UIViewBase
    {
        public Text Title;

        public UMViewGamePlayItem[] ItemArray;

        public Button ButtonEnsure;
	    public Button ButtonClose;

        public Text ButtonEnsureText;

	    public Text LifeShowText;

	    public UMViewGameRatingBarItem[] LifeItemArray;

	    public UIRectTransformStatus WinConditionStatus;

    }
}
