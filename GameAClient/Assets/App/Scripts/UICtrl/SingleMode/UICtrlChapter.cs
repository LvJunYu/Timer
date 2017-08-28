/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
	public class UICtrlChapter : MonoBehaviour
    {
        #region 常量与字段
		public RectTransform[] NormalLevelPos;
		public RectTransform[] BonusLevelPos;
		public UICtrlLevelPoint[] NormalLevels;
		public UICtrlLevelPoint[] BonusLevels;
		public RectTransform LevelRoot;
		public GameObject[] BonusLevelBlockImages;
		public GameObject BackgroundImage;
		public GameObject ForgroundImage;
		public GameObject LockImage;

		public GameObject NormalLevelPrefab;
		public GameObject BonusLevelPrefab;

        #endregion

        #region 属性

        #endregion

        #region 方法

		public void RefreshInfo (Table_StandaloneChapter table) {
			if (NormalLevels.Length == 0) {
				NormalLevels = new UICtrlLevelPoint[9];
				for (int i = 0; i < 9; i++) {
					GameObject levelObj = Instantiate (NormalLevelPrefab, LevelRoot);
					NormalLevels[i] = levelObj.GetComponent<UICtrlLevelPoint> ();
					var rectTransform = levelObj.GetComponent<RectTransform>();
					rectTransform.anchoredPosition = NormalLevelPos[i].anchoredPosition;
					rectTransform.localScale = Vector3.one;
				}
			}
			if (BonusLevels.Length == 0) {
				BonusLevels = new UICtrlLevelPoint[3];
				for (int i = 0; i < 3; i++) {
					GameObject levelObj = Instantiate (BonusLevelPrefab, LevelRoot);
					BonusLevels[i] = levelObj.GetComponent<UICtrlLevelPoint> ();
					var rectTransform = levelObj.GetComponent<RectTransform>();
					rectTransform.anchoredPosition = BonusLevelPos[i].anchoredPosition;
					rectTransform.localScale = Vector3.one;
				}
			}

//			if (table.Id > AppData.Instance.AdventureData.UserData.AdventureUserProgress.SectionUnlockProgress) {
//				LockImage.SetActive (true);
//			} else 
            {
//				LockImage.SetActive(false);
				for (int i = 0, n = NormalLevels.Length; i < n; i++) {
					var tableLevel = TableManager.Instance.GetStandaloneLevel(table.NormalLevels [i]);
					if (tableLevel == null) {
						LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
					}
					NormalLevels [i].RefreshInfo (table.Id, i+1, tableLevel);
				}
				for (int i = 0, n = BonusLevels.Length; i < n; i++) {
					var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels [i]);
					if (tableLevel == null) {
						LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
					}
					BonusLevels [i].RefreshInfo (table.Id, i+1, tableLevel);
					// refresh block imgs state
					if (AppData.Instance.AdventureData.UserData.SectionList.Count > (table.Id - 1) &&
						AppData.Instance.AdventureData.UserData.SectionList [table.Id - 1].BonusLevelUserDataList.Count > i) {
						if (AppData.Instance.AdventureData.UserData.SectionList [table.Id - 1].BonusLevelUserDataList [i].SimpleData.SuccessCount > 0) {
							BonusLevelBlockImages [i].SetActive (false);
						} else {
							BonusLevelBlockImages [i].SetActive (true);
						}
					} else {
						BonusLevelBlockImages [i].SetActive (true);
					}
				}
			}
		}
        
        #region 接口



        #endregion 接口
        #endregion

    }
}
