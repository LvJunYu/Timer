/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
	public class UICtrlLevelPoint : MonoBehaviour
    {
        #region 常量与字段
		public GameObject Active;
		public GameObject Disactive;
		public Button LevelBtn;
		public GameObject StarLight1;
		public GameObject StarLight2;
		public GameObject StarLight3;
		public Text LevelTitle;

		/// <summary>
		/// 章节id
		/// </summary>
		private int _chapterId;
		/// <summary>
		/// 关卡在章节中的序号
		/// </summary>
		private int _levelIdx;
		private bool _isBonus;
        #endregion

        #region 属性

        #endregion

        #region 方法
		void Start () {
			LevelBtn.onClick.AddListener (OnClick);
		}

		public void RefreshInfo (int chapterId, int levelIdx, Game.Table_StandaloneLevel tableLevel) {
			_chapterId = chapterId;
			_levelIdx = levelIdx;
			_isBonus = tableLevel.Type != 0;
			// normal level
			if (!_isBonus) {
				if (AppData.Instance.AdventureData.UserData.SectionList.Count > (chapterId - 1) &&
					AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].NormalLevelUserDataList.Count > levelIdx) {

					var levelData = AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].NormalLevelUserDataList [levelIdx];
					StarLight1.SetActive (levelData.SimpleData.Star1Flag);
					StarLight2.SetActive (levelData.SimpleData.Star2Flag);
					StarLight3.SetActive (levelData.SimpleData.Star3Flag);
					Active.SetActive (true);
					Disactive.SetActive (false);
				} else {
					StarLight1.SetActive (false);
					StarLight2.SetActive (false);
					StarLight3.SetActive (false);
					if (levelIdx == AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteLevel) {
						Active.SetActive (true);
						Disactive.SetActive (false);
					} else {
						Active.SetActive (false);
						Disactive.SetActive (true);
					}
						
				}
			} else {
				if (AppData.Instance.AdventureData.UserData.SectionList.Count > (chapterId - 1) &&
					AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].BonusLevelUserDataList.Count > levelIdx) {

					var levelData = AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].BonusLevelUserDataList [levelIdx];
					if (levelData.SimpleData.SuccessCount > 0) {
						StarLight1.SetActive (true);	
					} else {
						StarLight1.SetActive (false);
					}
				} else {
					Active.SetActive (false);
					Disactive.SetActive (true);
					StarLight1.SetActive (false);
				}
//				} else {
				if (AppData.Instance.AdventureData.UserData.SectionList.Count > (chapterId - 1) &&
					AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].GotStarCnt >= (levelIdx * 9 + 9)) {
						Active.SetActive (true);
						Disactive.SetActive (false);
					} else {
						Active.SetActive (false);
						Disactive.SetActive (true);
					}
//				}
			}
			LevelTitle.text = tableLevel.Name;
		}
        
		private void OnClick () {
			SendMessageUpwards ("OnLevelClicked", new IntVec3(_chapterId, _levelIdx, _isBonus ? 1 : 0), SendMessageOptions.RequireReceiver);
		}
        #region 接口



        #endregion 接口
        #endregion

    }
}
