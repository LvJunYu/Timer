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
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

namespace GameA
{
	public class UICtrlLevelPoint : MonoBehaviour
    {
        #region 常量与字段
		public GameObject Active;
		public GameObject Disactive;
        public Button Current;
		public Button LevelBtn;
		public RectTransform[] StarLightAry;
        public RectTransform[] StarDarkAry;
	    


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
	    private Table_StandaloneLevel _tableLevel;
	    private int _lastStarCount;
	    private EState _state = EState.None;
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

	    public void SetData(int chapterId, int levelIdx, Table_StandaloneLevel tableLevel)
	    {
		    _chapterId = chapterId;
		    _tableLevel = tableLevel;
//            Debug.Log ("level refresh info chapter: " + _chapterId + " level: " + levelIdx);
		    _isBonus = tableLevel.Type != 0;
		    _levelIdx = levelIdx;
	    }
		public void RefreshInfo (bool playAnimation = false)
		{
            LevelTitle.text = _tableLevel.Name;
			CoroutineProxy.Instance.StartCoroutine(RefreshView(playAnimation));
		}

	    private IEnumerator RefreshView(bool playAnimation)
	    {
			var advData = AppData.Instance.AdventureData;
			var state = GetState();
		    var groupId = SocialGUIManager.Instance.GetUI<UICtrlSingleMode>().GroupId;
			// normal level
			if (!_isBonus) {
				if (state == EState.Lock)
				{
					Current.gameObject.SetActive (false);
					Active.SetActive (false);
					Disactive.SetActive (true);
					_lastStarCount = 0;
					for (int i = 0; i < StarLightAry.Length; i++)
					{
						StarLightAry[i].SetActiveEx(false);
					}
					for (int i = 0; i < StarDarkAry.Length; i++)
					{
						StarDarkAry[i].SetActiveEx(false);
					}
				}
				else if (state == EState.Unlock)
				{
					if (playAnimation && _state != state)
					{
						var item = GameParticleManager.Instance.GetUIParticleItem(
							ParticleNameConstDefineGM2D.SingleModeNormalLevelUnlock,
							Disactive.transform, groupId);
						item.Particle.Play(4f);
						CoroutineProxy.Instance.StartCoroutine(
							CoroutineProxy.RunWaitForSeconds(1f, () => GameParticleManager.FreeParticleItem(item.Particle)));
						yield return new WaitForSeconds(0.2f);
					}
					Current.gameObject.SetActive (true);
					Active.SetActive (false);
					Disactive.SetActive (false);
					for (int i = 0; i < StarLightAry.Length; i++)
					{
						StarLightAry[i].SetActiveEx(false);
					}
					for (int i = 0; i < StarDarkAry.Length; i++)
					{
						StarDarkAry[i].SetActiveEx(true);
					}
					_lastStarCount = 0;
				}
				else
				{
					
					var levelData = advData.GetAdventureUserLevelDataDetail(_chapterId, EAdventureProjectType.APT_Normal, _levelIdx);
					int starCnt = levelData.SimpleData.GotStarCnt;

					if (playAnimation)
					{
						for (int i = _lastStarCount; i < starCnt; i++)
						{
							var item = GameParticleManager.Instance.GetUIParticleItem(
								ParticleNameConstDefineGM2D.SingleModeGetStar,
								StarDarkAry[i], groupId);
							item.Particle.Play(4f);
							var inx = i;
							CoroutineProxy.Instance.StartCoroutine(
								CoroutineProxy.RunWaitForSeconds(0.2f, () =>
									{
										StarLightAry[inx].SetActiveEx(true);
										StarDarkAry[inx].SetActiveEx(false);
										GameParticleManager.FreeParticleItem(item.Particle);
									}
								));
							yield return new WaitForSeconds(0.2f);
						}
					}
					else
					{
						for (int i = 0; i < StarLightAry.Length; i++)
						{
							StarLightAry[i].SetActiveEx(i<starCnt);
							StarDarkAry[i].SetActiveEx(i>=starCnt);
						}
					}
					if (playAnimation && _state != state)
					{
						var item = GameParticleManager.Instance.GetUIParticleItem(
							ParticleNameConstDefineGM2D.SingleModeNormalLevelComplete,
							Active.transform, groupId);
						item.Particle.Play(4f);
						CoroutineProxy.Instance.StartCoroutine(
							CoroutineProxy.RunWaitForSeconds(1, () => GameParticleManager.FreeParticleItem(item.Particle)));
						yield return new WaitForSeconds(0.2f);
					}
					Current.gameObject.SetActive (false);
					Active.SetActive (true);
					Disactive.SetActive (false);
					_lastStarCount = starCnt;
				}
			} else {
				if (state == EState.Lock)
				{
					Active.SetActive (false);
					Disactive.SetActive (true);
					StarLightAry[0].SetActiveEx(false);
				}
				else if (state == EState.Unlock)
				{
					Active.SetActive (true);
					Disactive.SetActive (false);
					StarLightAry[0].SetActiveEx(false);
				}
				else
				{
					Active.SetActive (true);
					Disactive.SetActive (false);
					StarLightAry[0].SetActiveEx(true);
				}
                if (AppData.Instance.AdventureData.UserData.SectionList.Count > (_chapterId - 1)) {
                    StartText.text = string.Format ("{0}",
                        Mathf.Clamp (AppData.Instance.AdventureData.UserData.SectionList [_chapterId - 1].GotStarCnt, 0, 9 * _tableLevel.Type)
                    );
                    StartText2.text = string.Format("/{0}",
                        9 * _tableLevel.Type
                    );
                } else
                {
                    StartText.text = "0";
                    StartText2.text = string.Format("/{0}",
                       9 * _tableLevel.Type
                   );
                }
			}
		    _state = state;
		    yield return null;
	    }

        private EState GetState ()
        {
	        var advData = AppData.Instance.AdventureData;
	        var progress = advData.UserData.AdventureUserProgress;
	        if (_isBonus)
	        {
		        if (advData.UserData.SectionList.Count > (_chapterId - 1) &&
		            advData.UserData.SectionList[_chapterId - 1].GotStarCnt >= _levelIdx * 9)
		        {
			        var levelData = advData.GetAdventureUserLevelDataDetail(_chapterId,
				        _isBonus ? EAdventureProjectType.APT_Bonus : EAdventureProjectType.APT_Normal, _levelIdx);
			        if (levelData.SimpleData.SuccessCount > 0)
			        {
				        return EState.Pass;
			        }
			        else
			        {
				        return EState.Unlock;
			        }
		        }
		        else
		        {
			        return EState.Lock;
		        }
	        }
	        else
	        {
		        var curLevelProgress = AdventureData.GetNormalProgress(_chapterId, _levelIdx);
		        var completeProgress = AdventureData.GetNormalProgress(progress.CompleteSection, progress.CompleteLevel);
		        if (curLevelProgress > completeProgress + 1)
		        {
			        return EState.Lock;
		        }
		        if (curLevelProgress == completeProgress + 1)
		        {
			        return EState.Unlock;
		        }
		        return EState.Pass;
	        }
        }
        
		private void OnClick () {
			SendMessageUpwards ("OnLevelClicked", new IntVec3(_chapterId, _levelIdx, _isBonus ? 1 : 0), SendMessageOptions.RequireReceiver);
		}
        #region 接口



        #endregion 接口
        #endregion

	    private enum EState
	    {
		    None,
		    Lock,
		    Unlock,
		    Pass
	    }
    }
}
