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
        public Button Current;
		public Button LevelBtn;
		public GameObject StarLight1;
		public GameObject StarLight2;
		public GameObject StarLight3;
        public GameObject StarDark1;
        public GameObject StarDark2;
        public GameObject StarDark3;


		public Text LevelTitle;

        public Text StartText;
        public Text StartText2;

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
            if (null != Current) {
                Current.onClick.AddListener (OnClick);
            }
		}

		public void RefreshInfo (int chapterId, int levelIdx, Game.Table_StandaloneLevel tableLevel) {
			_chapterId = chapterId;
			_levelIdx = levelIdx;
//            Debug.Log ("level refresh info chapter: " + _chapterId + " level: " + levelIdx);
			_isBonus = tableLevel.Type != 0;
			// normal level
			if (!_isBonus) {
				if (AppData.Instance.AdventureData.UserData.SectionList.Count > (chapterId - 1) &&
					AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].NormalLevelUserDataList.Count > levelIdx-1) {

					var levelData = AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].NormalLevelUserDataList [levelIdx - 1];
                    int starCnt = levelData.SimpleData.GotStarCnt;

                    StarLight1.SetActive (starCnt > 0);
                    StarLight2.SetActive (starCnt > 1);
                    StarLight3.SetActive (starCnt > 2);
                    StarDark1.SetActive (starCnt < 1);
                    StarDark2.SetActive (starCnt < 2);
                    StarDark3.SetActive (starCnt < 3);
                    Current.gameObject.SetActive (false);
					Active.SetActive (true);
					Disactive.SetActive (false);
				} else {
					StarLight1.SetActive (false);
					StarLight2.SetActive (false);
					StarLight3.SetActive (false);
                    StarDark1.SetActive (false);
                    StarDark2.SetActive (false);
                    StarDark3.SetActive (false);
                    if (IsCurrentLevel()) {
                        Current.gameObject.SetActive (true);
						Active.SetActive (false);
						Disactive.SetActive (false);
					} else {
                        Current.gameObject.SetActive (false);
						Active.SetActive (false);
						Disactive.SetActive (true);
					}
						
				}
			} else {
				if (AppData.Instance.AdventureData.UserData.SectionList.Count > (chapterId - 1) &&
					AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].BonusLevelUserDataList.Count > levelIdx - 1) {

					var levelData = AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].BonusLevelUserDataList [levelIdx - 1];
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
                    AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].GotStarCnt >= ((levelIdx - 1) * 9 + 9)) {
						Active.SetActive (true);
						Disactive.SetActive (false);
					} else {
						Active.SetActive (false);
						Disactive.SetActive (true);
					}
//				}0     /9
                if (AppData.Instance.AdventureData.UserData.SectionList.Count > (chapterId - 1)) {
                    StartText.text = string.Format ("{0}",
                        Mathf.Clamp (AppData.Instance.AdventureData.UserData.SectionList [chapterId - 1].GotStarCnt, 0, 9 * tableLevel.Type)
                    );
                    StartText2.text = string.Format("/{0}",
                        9 * tableLevel.Type
                    );
                } else
                {
                    StartText.text = "0";
                    StartText2.text = string.Format("/{0}",
                       9 * tableLevel.Type
                   );
                }
			}
            LevelTitle.text = tableLevel.Name;
        }

        private bool IsCurrentLevel ()
        {
            return (_levelIdx == AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteLevel + 1) ||
                (_chapterId == (AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteSection + 1) &&
                    (_levelIdx == 1 && AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteLevel == 9));
        }
        
		private void OnClick () {
			SendMessageUpwards ("OnLevelClicked", new IntVec3(_chapterId, _levelIdx, _isBonus ? 1 : 0), SendMessageOptions.RequireReceiver);
		}
        #region 接口



        #endregion 接口
        #endregion

    }
}
