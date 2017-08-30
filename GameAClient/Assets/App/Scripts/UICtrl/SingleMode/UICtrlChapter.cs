/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System.Collections;
using System.Runtime.Remoting.Messaging;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
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

	    private UIParticleItem _travelEffect;
	    private bool _isDoingAnimation;
	    private Table_StandaloneChapter _tableStandaloneChapter;

        #endregion

        #region 属性

        #endregion

        #region 方法

		public void RefreshInfo (Table_StandaloneChapter table, bool doPassAnimate = false) {
			if (_isDoingAnimation)
			{
				return;
			}
			_tableStandaloneChapter = table;
			if (NormalLevels.Length == 0) {
				NormalLevels = new UICtrlLevelPoint[9];
				for (int i = 0; i < 9; i++) {
					GameObject levelObj = Instantiate (NormalLevelPrefab, LevelRoot);
					NormalLevels[i] = levelObj.GetComponent<UICtrlLevelPoint> ();
					var rectTransform = levelObj.GetComponent<RectTransform>();
					rectTransform.anchoredPosition = NormalLevelPos[i].anchoredPosition;
					rectTransform.localScale = Vector3.one;
					var tableLevel = TableManager.Instance.GetStandaloneLevel(_tableStandaloneChapter.NormalLevels [i]);
					if (tableLevel == null) {
						LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
					}
					else
					{
						NormalLevels[i].SetData(_tableStandaloneChapter.Id, i+1, tableLevel);
					}
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
					var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels [i]);
					if (tableLevel == null) {
						LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
					}
					else
					{
						BonusLevels[i].SetData(table.Id, i+1, tableLevel);
					}
				}
			}

//			if (table.Id > AppData.Instance.AdventureData.UserData.AdventureUserProgress.SectionUnlockProgress) {
//				LockImage.SetActive (true);
//			} else 
            {
//				LockImage.SetActive(false);
	            if (doPassAnimate)
	            {
		            _isDoingAnimation = true;
		            StartCoroutine(AnimationFlow());
	            }
	            else
	            {
		            for (int i = 0, n = NormalLevels.Length; i < n; i++) {
			            NormalLevels [i].RefreshInfo ();
		            }
		            for (int i = 0, n = BonusLevels.Length; i < n; i++) {
			            var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels [i]);
			            if (tableLevel == null) {
				            LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
			            }
			            BonusLevels [i].RefreshInfo ();
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
		}

	    private IEnumerator AnimationFlow()
	    {
		    var advData = AppData.Instance.AdventureData;
		    if (advData.LastPlayedLevelType == EAdventureProjectType.APT_Bonus)
		    {
			    BonusLevels[advData.LastPlayedLevelIdx - 1].RefreshInfo(true);
		    }
		    else
		    {
			    NormalLevels[advData.LastPlayedLevelIdx - 1].RefreshInfo(true);
			    int nextLevel;
			    if (advData.TryGetNextNormalLevel(advData.LastPlayedChapterIdx, advData.LastPlayedLevelIdx, out nextLevel))
			    {
				    yield return new WaitForSeconds(1f);
				    NormalLevels[nextLevel - 1].RefreshInfo(true);
//				    yield return new WaitForSeconds(0.5f);
//				    var uiMain = SocialGUIManager.Instance.GetUI<UICtrlSingleMode>();
//				    if (uiMain.IsOpen
//				        && uiMain.CurrentChapter == _tableStandaloneChapter.Id
//				        && !SocialGUIManager.Instance.GetUI<UICtrlAdvLvlDetail>().IsOpen)
//				    {
//					    IntVec3 intVec3 = new IntVec3();
//                        intVec3.x = _tableStandaloneChapter.Id;
//                        intVec3.y = nextLevel;
//                        intVec3.z = 0;
//						SocialGUIManager.Instance.OpenUI<UICtrlAdvLvlDetail>(intVec3);
//				    }
			    }
			    _isDoingAnimation = false;
		    }
		    yield return null;
	    }
        
        #region 接口



        #endregion 接口
        #endregion

    }
}
