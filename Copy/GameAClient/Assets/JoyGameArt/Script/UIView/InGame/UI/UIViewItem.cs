/********************************************************************
** Filename : UIViewItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 3:30:33
** Summary : UIViewItem
***********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewItem : UIViewResManagedBase
    {
//        public Button Actor;
//        public Button Earth;
//        public Button Mechanism;
//        public Button Collection;
//        public Button Control;
//        public Button Decoration;
        public Button[] CategoryButtns;

        public GameObject[] SelectedCategorys;
        public ScrollRect ScrollRect;
        public HorizontalLayoutGroup LayoutGroup;

        public GameObject EditorTabbarGo;
    }
}