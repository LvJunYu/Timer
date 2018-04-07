/********************************************************************

** Filename : UIViewScreenOperator
** Author : ake
** Date : 2016/4/11 14:33:32
** Summary : UIViewScreenOperator
***********************************************************************/

using UnityEngine;

namespace GameA
{
    public enum EScreenOperator
    {
        None = -1,
        UpAdd,
        UpDelete,
        LeftAdd,
        LeftDelete,
        RightAdd,
        RightDelete,
        Max,
    }

    public class UIViewScreenOperator : UIViewResManagedBase
    {
        public UMViewItemScreenButton[] ScreenButtonArray;

        public GameObject TopGo;
        public GameObject LeftGo;
        public GameObject RightGo;
    }
}