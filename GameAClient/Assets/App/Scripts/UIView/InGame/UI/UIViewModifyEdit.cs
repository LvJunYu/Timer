/********************************************************************
** Filename : UIViewEdit
** Author : Dong
** Date : 2015/7/2 16:30:24
** Summary : UIViewEdit
***********************************************************************/

using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewModifyEdit : UIViewBase
    {
		public Button ModifyEraseBtn;
		public Button ModifyModifyBtn;
		public Button ModifyAddBtn;
		public UIViewModifyItem[] ModifyItems;
        public GameObject SelectAddItemPanel;
        public UIViewModifySelectItem[] SelectItems;
    }
}
