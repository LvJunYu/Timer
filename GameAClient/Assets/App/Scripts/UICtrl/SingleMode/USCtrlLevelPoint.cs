﻿/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System.Collections;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
namespace GameA
{
	public class USCtrlLevelPoint : USCtrlBase<USViewLevelPoint>
    {
        #region 常量与字段
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
	    private Vector2 _startPos;
	    private Vector2 _moVector2 = new Vector2(0.0f,15.0f);
	    private Tweener _tweener;
        #endregion

        #region 属性

        #endregion

        #region 方法

	    protected override void OnViewCreated()
	    {
			_cachedView.LevelBtn.onClick.AddListener (OnClick);
            if (null != _cachedView.Current) {
	            _cachedView.Current.onClick.AddListener (OnClick);
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
			_cachedView.LevelTitle.text = _tableLevel.Name;
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
				
					_cachedView.Current.gameObject.SetActive (false);
					_cachedView.Active.SetActive (false);
					_cachedView.Disactive.SetActive (true);
					_lastStarCount = 0;
					for (int i = 0; i < _cachedView.StarLightAry.Length; i++)
					{
						_cachedView.StarLightAry[i].SetActiveEx(false);
					}
					for (int i = 0; i < _cachedView.StarDarkAry.Length; i++)
					{
						_cachedView.StarDarkAry[i].SetActiveEx(false);
					}
				}
				else if (state == EState.Unlock)
				{
					
					if (playAnimation && _state != state)
					{
						var item = GameParticleManager.Instance.GetUIParticleItem(
							ParticleNameConstDefineGM2D.SingleModeNormalLevelUnlock,
							_cachedView.Disactive.transform, groupId);
						item.Particle.Play(4f);
						CoroutineProxy.Instance.StartCoroutine(
							CoroutineProxy.RunWaitForSeconds(1f, () => GameParticleManager.FreeParticleItem(item.Particle)));
						yield return new WaitForSeconds(0.2f);
					}
					_cachedView.Current.gameObject.SetActive (true);
					_cachedView.Active.SetActive (false);
					_cachedView.Disactive.SetActive (false);
					for (int i = 0; i < _cachedView.StarLightAry.Length; i++)
					{
						_cachedView.StarLightAry[i].SetActiveEx(false);
					}
					for (int i = 0; i < _cachedView.StarDarkAry.Length; i++)
					{
						_cachedView.StarDarkAry[i].SetActiveEx(true);
					}
					_lastStarCount = 0;
					SetTween(true,_moVector2);
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
								_cachedView.StarDarkAry[i], groupId);
							item.Particle.Play(4f);
							var inx = i;
							CoroutineProxy.Instance.StartCoroutine(
								CoroutineProxy.RunWaitForSeconds(0.2f, () =>
									{
										_cachedView.StarLightAry[inx].SetActiveEx(true);
										_cachedView.StarDarkAry[inx].SetActiveEx(false);
										GameParticleManager.FreeParticleItem(item.Particle);
									}
								));
							yield return new WaitForSeconds(0.2f);
						}
					}
					else
					{
						
						for (int i = 0; i < _cachedView.StarLightAry.Length; i++)
						{
							_cachedView.StarLightAry[i].SetActiveEx(i<starCnt);
							_cachedView.StarDarkAry[i].SetActiveEx(i>=starCnt);
						}
					}
					if (playAnimation && _state != state)
					{
						var item = GameParticleManager.Instance.GetUIParticleItem(
							ParticleNameConstDefineGM2D.SingleModeNormalLevelComplete,
							_cachedView.Active.transform, groupId);
						item.Particle.Play(4f);
						CoroutineProxy.Instance.StartCoroutine(
							CoroutineProxy.RunWaitForSeconds(1, () => GameParticleManager.FreeParticleItem(item.Particle)));
						yield return new WaitForSeconds(0.2f);
					}
					_cachedView.Current.gameObject.SetActive (false);
					_cachedView.Active.SetActive (true);
					_cachedView.Disactive.SetActive (false);
					_lastStarCount = starCnt;
				}
			}
			else {
				if (state == EState.Lock)
				{
					_cachedView.Active.SetActive (false);
					_cachedView.Disactive.SetActive (true);
					_cachedView.StarLightAry[0].SetActiveEx(false);
					_cachedView.LightImage.gameObject.SetActive(false);
				}
				else if (state == EState.Unlock)
				{
					
					_cachedView.Active.SetActive (true);
					_cachedView.Disactive.SetActive (false);
					_cachedView.StarLightAry[0].SetActiveEx(false);
					_cachedView.LightImage.gameObject.SetActive(true);
					SetTween(true,_moVector2);
					
				}
				else
				{
					_cachedView.Active.SetActive (true);
					_cachedView.Disactive.SetActive (false);
					_cachedView.StarLightAry[0].SetActiveEx(true);
					_cachedView.LightImage.gameObject.SetActive(false);
				}
                if (AppData.Instance.AdventureData.UserData.SectionList.Count > (_chapterId - 1)) {
	                _cachedView.StartText.text = string.Format ("{0}",
                        Mathf.Clamp (AppData.Instance.AdventureData.UserData.SectionList [_chapterId - 1].GotStarCnt, 0, 9 * _tableLevel.Type)
                    );
	                _cachedView.StartText2.text = string.Format("/{0}",
                        9 * _tableLevel.Type
                    );
                } else
                {
	                _cachedView.StartText.text = "0";
	                _cachedView.StartText2.text = string.Format("/{0}",
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
        
		private void OnClick ()
		{
			SocialGUIManager.Instance.GetUI<UICtrlSingleMode>()
				.OnLevelClicked(new IntVec3(_chapterId, _levelIdx, _isBonus ? 1 : 0));
		}

	    public void SetTween(bool isDown ,Vector2 offset)
	    {
		    if (isDown)
		    {
			    _moVector2 = -_moVector2;
		    }
		    _startPos = _cachedView.rectTransform().anchoredPosition;
		    _cachedView.rectTransform().anchoredPosition += offset;
		    _tweener = _cachedView.rectTransform().DOAnchorPosY((_startPos + _moVector2).y, 2.0f);
		    _tweener.SetDelay(offset.y*0.1f);
		    _tweener.OnComplete(SetOnComplete);
	    }

	    public void SetIslandImage(Sprite isLandImageSprite)
	    {
		    _cachedView.IslandImage.sprite = isLandImageSprite;
	    }

	    private void SetOnComplete()
	    {
		    _moVector2 = -_moVector2;
		    _tweener = _cachedView.rectTransform().DOAnchorPosY((_startPos + _moVector2).y, 2.0f);
		    _tweener.OnComplete(SetOnComplete);
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
