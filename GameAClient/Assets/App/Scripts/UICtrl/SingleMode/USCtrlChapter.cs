/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System.Collections;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
	public class USCtrlChapter : USCtrlBase<USViewChapter>
    {
        #region 常量与字段
	    private USCtrlLevelPoint[] _normalLevels;
	    private USCtrlLevelPoint[] _bonusLevels;

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
			if (_normalLevels == null) {
				_normalLevels = new USCtrlLevelPoint[9];
				for (int i = 0; i < 9; i++) {
					GameObject levelObj = Object.Instantiate (_cachedView.NormalLevelPrefab, _cachedView.LevelRoot);
					_normalLevels[i] = new USCtrlLevelPoint();
					_normalLevels[i].Init(levelObj.GetComponent<USViewLevelPoint>());
					var rectTransform = levelObj.GetComponent<RectTransform>();
					rectTransform.anchoredPosition = _cachedView.NormalLevelPos[i].anchoredPosition;
					rectTransform.localScale = Vector3.one;
					var tableLevel = TableManager.Instance.GetStandaloneLevel(_tableStandaloneChapter.NormalLevels [i]);
					if (tableLevel == null) {
						LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
					}
					else
					{
						_normalLevels[i].SetData(_tableStandaloneChapter.Id, i+1, tableLevel);
					}
				}
			}
			if (_bonusLevels == null) {
				_bonusLevels = new USCtrlLevelPoint[3];
				for (int i = 0; i < 3; i++) {
					GameObject levelObj = Object.Instantiate (_cachedView.BonusLevelPrefab, _cachedView.LevelRoot);
					_bonusLevels[i] = new USCtrlLevelPoint();
					_bonusLevels[i].Init(levelObj.GetComponent<USViewLevelPoint>());
					var rectTransform = levelObj.GetComponent<RectTransform>();
					rectTransform.anchoredPosition = _cachedView.BonusLevelPos[i].anchoredPosition;
					rectTransform.localScale = Vector3.one;
					var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels [i]);
					if (tableLevel == null) {
						LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
					}
					else
					{
						_bonusLevels[i].SetData(table.Id, i+1, tableLevel);
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
		            CoroutineProxy.Instance.StartCoroutine(AnimationFlow());
	            }
	            else
	            {
		            for (int i = 0, n = _normalLevels.Length; i < n; i++) {
			            _normalLevels [i].RefreshInfo ();
		            }
		            for (int i = 0, n = _bonusLevels.Length; i < n; i++) {
			            var tableLevel = TableManager.Instance.GetStandaloneLevel(table.BonusLevels [i]);
			            if (tableLevel == null) {
				            LogHelper.Error ("Can't find tableLevel when refresh ui, chapter: {0}, level: {1}", table.Id, i);
			            }
			            _bonusLevels [i].RefreshInfo ();
			            // refresh block imgs state
			            if (AppData.Instance.AdventureData.UserData.SectionList.Count > (table.Id - 1) &&
			                AppData.Instance.AdventureData.UserData.SectionList [table.Id - 1].BonusLevelUserDataList.Count > i) {
				            if (AppData.Instance.AdventureData.UserData.SectionList [table.Id - 1].BonusLevelUserDataList [i].SimpleData.SuccessCount > 0) {
					            _cachedView.BonusLevelBlockImages [i].SetActive (false);
				            } else {
					            _cachedView.BonusLevelBlockImages [i].SetActive (true);
				            }
			            } else {
				            _cachedView.BonusLevelBlockImages [i].SetActive (true);
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
			    _bonusLevels[advData.LastPlayedLevelIdx - 1].RefreshInfo(true);
		    }
		    else
		    {
			    _normalLevels[advData.LastPlayedLevelIdx - 1].RefreshInfo(true);
			    int nextLevel;
			    if (advData.TryGetNextNormalLevel(advData.LastPlayedChapterIdx, advData.LastPlayedLevelIdx, out nextLevel))
			    {
				    yield return new WaitForSeconds(1f);
				    _normalLevels[nextLevel - 1].RefreshInfo(true);
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
