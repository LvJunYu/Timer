/********************************************************************
** Filename : UIViewGameCompositeEditorTools  
** Author : ake
** Date : 3/14/2017 4:42:09 PM
** Summary : UIViewGameCompositeEditorTools  
***********************************************************************/


using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA.Game
{
	public class UIViewGameCompositeEditorTools: UIViewBase
	{
		public ChangeStateButtonGroup ShowGroupButton;
		public Transform ToolbarRootTrans;

		public ChangeStateButtonGroup SelectMode;
		public ChangeStateButtonGroup MoveMode;

		public Transform MoveButtonRoot;

		public Button MoveUpBt;
		public Button MoveDownBt;
		public Button MoveLeftBt;
		public Button MoveRightBt;

	}
}
