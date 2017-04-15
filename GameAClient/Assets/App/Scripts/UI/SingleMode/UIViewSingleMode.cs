using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewSingleMode : UIViewBase
    {
		public ScrollRectEx2 ChapterScrollRect;
		public Button ReturnBtn;
		public Button MatchBtn;
		public Button TreasureBtn;
		public Button EncBtn;

		public Image EnergyBar;
		public Text EnergyNumber;
		public Text ChapterTitle;
		public Text StarNumber;

		public UICtrlChapter[] Chapters;
		public Image InputBlock;

		public Callback<System.Object> LevelClickedCB;

		private void OnLevelClicked (object param) {
			if (LevelClickedCB != null)
				LevelClickedCB.Invoke (param);
		}
    }
}
