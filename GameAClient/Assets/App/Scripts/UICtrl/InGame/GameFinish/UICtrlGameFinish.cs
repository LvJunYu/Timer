/********************************************************************
** Filename : UICtrlGameFinish  
** Author : ake
** Date : 4/29/2016 8:24:49 PM
** Summary : UICtrlGameFinish  
***********************************************************************/


using System;
using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
	[UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlGameFinish : UICtrlInGameBase<UIViewGameFinish>
	{
		private enum EShowState
		{
			EditorWin = 1,
			EditorLose,
			WinWithNextLevel,
			Lose,
			Win,
            WinRankRate,
            AdvNormalWin,
            AdvNormalLose,
            AdvBonusWin,
            AdvBonusLose,
            ChallengeWin,
            ChallengeLose,
		}
        private EShowState _showState;
		private bool _finishRes;

		private int _curMarkStarValue;

        private bool _rateRequest = false;

        private USCtrlGameFinishReward [] _rewardCtrl;

		protected override void InitGroupId()
		{
			_groupId = (int)EUIGroupType.InGamePopup;
		}

		protected override void InitEventListener()
		{
			base.InitEventListener();
            RegisterEvent(EMessengerType.GameFinishFailedShowUI, OnFailed);
			RegisterEvent(EMessengerType.GameFinishSuccessShowUI, OnSuccess);
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			InitUI();
		}

		protected override void OnOpen(object parameter)
		{
			base.OnOpen(parameter);
            if (Game.PlayMode.Instance.SceneState.GameFailed) {
                _showState = EShowState.Lose;
                if (EProjectStatus.PS_Challenge == GameManager.Instance.CurrentGame.Project.ProjectStatus)
                {
                    _showState = EShowState.ChallengeLose;
                } else if (EProjectStatus.PS_Reform == GameManager.Instance.CurrentGame.Project.ProjectStatus)
                {
                    _showState = EShowState.EditorLose;
                } else if (EProjectStatus.PS_Private == GameManager.Instance.CurrentGame.Project.ProjectStatus) {
                    _showState = EShowState.EditorLose;
                } else
                {
                    // 冒险关卡普通关卡胜利后
                    if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvNormal) {
                        _showState = EShowState.AdvNormalLose;
                    }
                    // 冒险关卡奖励关卡胜利后
                    if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvBonus) {
                        _showState = EShowState.AdvBonusLose;
                    }
                }
            } else if (Game.PlayMode.Instance.SceneState.GameSucceed) {
                _showState = EShowState.Win;
                if (EProjectStatus.PS_Challenge == GameManager.Instance.CurrentGame.Project.ProjectStatus) {
                    _showState = EShowState.ChallengeWin;
                } else if (EProjectStatus.PS_Reform == GameManager.Instance.CurrentGame.Project.ProjectStatus) {
                    _showState = EShowState.EditorWin;
                } else if (EProjectStatus.PS_Private == GameManager.Instance.CurrentGame.Project.ProjectStatus) {
                    _showState = EShowState.EditorWin;
                } else {
                    // 冒险关卡普通关卡胜利后
                    if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvNormal) {
                        _showState = EShowState.AdvNormalWin;
                    }
                    // 冒险关卡奖励关卡胜利后
                    if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvBonus) {
                        _showState = EShowState.AdvBonusWin;
                    }
                }
            } else {
                SocialGUIManager.Instance.CloseUI <UICtrlGameFinish>();
            }

//			_curMarkStarValue = GM2DGame.Instance.Project.UserRate;
			UpdateView();
			//UpdateLifeItem();
		}

		#region   private

		private void UpdateLifeItem()
		{
			//for (int i = 0; i < _cachedView.MarkStarArray.Length; i++)
			//{
			//	_cachedView.MarkStarArray[i].UpdateShow(_curMarkStarValue);
			//}
		}

		private void InitUI()
		{
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.RetryBtn.onClick.AddListener(OnRetryBtn);
            _cachedView.NextBtn.onClick.AddListener(OnNextBtn);
            _cachedView.ContinueEditBtn.onClick.AddListener (OnContinueEditBtn);

            _rewardCtrl = new USCtrlGameFinishReward [_cachedView.Rewards.Length];
            for (int i = 0; i < _cachedView.Rewards.Length; i++) {
                _rewardCtrl [i] = new USCtrlGameFinishReward ();
                _rewardCtrl [i].Init (_cachedView.Rewards [i]);
            }
			//_cachedView.PlayAgain2.onClick.AddListener(OnClickRestartGame);
			//_cachedView.Public.onClick.AddListener(OnClickPublic);
   //         _cachedView.ReturnToEditor.onClick.AddListener(OnClickReturnEditor);
			//_cachedView.ReturnToEditor2.onClick.AddListener(OnClickReturnEditor);
			//_cachedView.NextLevel.onClick.AddListener(OnClickNextLevelButton);

			//_cachedView.ButtonMark.onClick.AddListener(OnClickMarkButton);

            //_cachedView.SkipBtn .onClick.AddListener(OnClickSkipButton);

   //         _cachedView.ExitGameText.text = GM2DUIConstDefine.GameFinishButtonExitGame;
   //         if(_cachedView.ExitGame2Text != null)
   //         {
   //             _cachedView.ExitGame2Text.text = GM2DUIConstDefine.GameFinishButtonExitGame;
   //         }
   //         _cachedView.PlayAgainText.text = GM2DUIConstDefine.GameFinishButtonReStart;
   //         if(_cachedView.PlayAgain2Text != null)
   //         {
   //             _cachedView.PlayAgain2Text.text = GM2DUIConstDefine.GameFinishButtonReStart;
   //         }
			//_cachedView.PublicText.text = GM2DUIConstDefine.GameFinishButtonPublic;
   //         _cachedView.ReturnToEditorText.text = GM2DUIConstDefine.GameFinishButtonReturnToEditor;
   //         if(_cachedView.ReturnToEditor2Text != null)
   //         {
   //             _cachedView.ReturnToEditor2Text.text = GM2DUIConstDefine.GameFinishButtonReturnToEditor;
   //         }
			//_cachedView.NextLevelText.text = LocaleManager.GameLocale("game_finish_button_next_level");

			//_cachedView.ButtonMarkText.text = GM2DUIConstDefine.GameFinishButtonMark;

			//for (int i = 0; i < _cachedView.MarkStarArray.Length; i++)
			//{
			//	var item = _cachedView.MarkStarArray[i];
			//	item.Init(i + 1, OnMarkStarItemButtonClick);
			//}
		}
        #region UIEvent

        private void OnReturnBtn ()
        {
            //InternalExitGame ();
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish> ();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
            Game.GM2DGame.Instance.QuitGame (
                () => {
                    SocialGUIManager.Instance.GetUI <UICtrlLittleLoading> ().CloseLoading (this);
                },
                code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);   
                },
                true
            );
        }
        private void OnRetryBtn () {
            var tableLevel = AppData.Instance.AdventureData.GetAdvLevelTable (
                AppData.Instance.AdventureData.LastPlayedChapterIdx + 1,
                AppData.Instance.AdventureData.LastPlayedLevelIdx + 1,
                AppData.Instance.AdventureData.LastPlayedLevelType
            );
            if (null == tableLevel) return;
            if (GameATools.CheckEnergy (tableLevel.EnergyCost)) {
                SocialGUIManager.Instance.CloseUI <UICtrlGameFinish>();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (
                    this, "...");
                AppData.Instance.AdventureData.RetryAdvLevel (
                    () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        // set local energy data
                        GameATools.LocalUseEnergy (tableLevel.EnergyCost);
                    },
                    (error) => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    }
                );
            }
        }
        private void OnNextBtn () {}

        private void OnContinueEditBtn ()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish> ();
            PauseGame ();
        }
		private void OnClickRestartGame()
		{
            Project p = GameManager.Instance.CurrentGame.Project;
            if(p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
                && GameManager.Instance.GameMode == EGameMode.Normal)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
                p.BeginPlay(false, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    Game.PlayMode.Instance.RePlay();
                    //SceneManager.Instance.RePlay();
                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
                    SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
                }, code=>{
//                    string tip = null;
//                    if(code == EPlayProjectRetCode.PPRC_ProjectHasBeenDeleted)
//                    {
//                        tip = "作品已被删除，启动失败";
//                    }
//                    else if(code == EPlayProjectRetCode.PPRC_FrequencyTooHigh)
//                    {
//                        tip = "启动过于频繁，启动失败";
//                    }
//                    else
//                    {
//                        if(Application.internetReachability == NetworkReachability.NotReachable)
//                        {
//                            tip = "启动失败，请检查网络环境";
//                        }
//                        else
//                        {
//                            tip = "启动失败，未知错误";
//                        }
//                    }
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    CommonTools.ShowPopupDialog(tip, null, 
//                        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
//                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnClickRestartGame));
//                        }),
//                        new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//                        }));
                });
            }
            else
            {
                Game.PlayMode.Instance.RePlay();
                //SceneManager.Instance.RePlay();
                //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
                SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
            }
        }

		private void OnClickExitGame()
		{
            //if(Game.GM2DGame.Instance.GameInitType == GameManager.EStartType.Create
            //    || Game.GM2DGame.Instance.GameInitType == GameManager.EStartType.Edit)
            //{
            //    if(Game.GM2DGame.Instance.NeedSave)
            //    {
            //        CommonTools.ShowPopupDialog("关卡做出的修改还未保存，是否退出", null,
            //            new System.Collections.Generic.KeyValuePair<string, Action>("取消",()=>{}),
            //            new System.Collections.Generic.KeyValuePair<string, Action>("保存",()=>{
            //                OnClickReturnEditor();
            //                Project project = Game.GM2DGame.Instance.Project;
            //                if(project.LocalDataState == ELocalDataState.LDS_UnCreated)
            //                {
            //                    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>{
            //                        Game.GM2DGUIManager.Instance.OpenUI<Game.UICtrlPublish>();
            //                    }));
            //                }
            //                else
            //                {
            //                    Game.UICtrlPublish.SaveProject(project.Name, project.Summary, 
            //                        project.DownloadPrice, project.PublishRecordFlag, ()=>{
            //                        SocialApp.Instance.ReturnToApp();
            //                    },()=>{
            //                        Game.GM2DGUIManager.Instance.OpenUI<Game.UICtrlPublish>();
            //                    });
            //                }
            //            }),
            //            new System.Collections.Generic.KeyValuePair<string, Action>("退出",()=>{
            //                InternalExitGame();
            //            }));
            //        return;
            //    }
            //}
            //InternalExitGame();
        }

        private void InternalExitGame()
        {
            SocialApp.Instance.ReturnToApp();
            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
        }

        private void OnClickPublic()
		{
            PauseGame();
			//GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
            //CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>{
            //    GM2DGUIManager.Instance.OpenUI<UICtrlPublish>();
            //}));
        }

		private void OnClickReturnEditor()
		{
            PauseGame();
			//GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
        }

		private void OnClickMarkButton()
		{
            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
            //if(!AppLogicUtil.CheckLoginAndTip())
            //{
            //    return;
            //}
            //GM2DGUIManager.Instance.OpenUI<UICtrlGameMark>();
		}

        private void OnClickSkipButton()
        {
            JumpToWinState();
        }

		private void OnClickNextLevelButton()
		{
			if (!GameManager.Instance.HasNext())
			{
                return;
            }
            Project p = GameManager.Instance.GetNextProject();
            if(p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
                && GameManager.Instance.GameMode == EGameMode.Normal)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
                p.BeginPlay(false, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.PlayNext();
                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
                    SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
                }, code=>{
//                    string tip = null;
//                    if(code == EPlayProjectRetCode.PPRC_ProjectHasBeenDeleted)
//                    {
//                        tip = "作品已被删除，启动失败";
//                    }
//                    else if(code == EPlayProjectRetCode.PPRC_FrequencyTooHigh)
//                    {
//                        tip = "启动过于频繁，启动失败";
//                    }
//                    else
//                    {
//                        if(Application.internetReachability == NetworkReachability.NotReachable)
//                        {
//                            tip = "启动失败，请检查网络环境";
//                        }
//                        else
//                        {
//                            tip = "启动失败，未知错误";
//                        }
//                    }
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    CommonTools.ShowPopupDialog(tip, null, 
//                        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
//                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnClickNextLevelButton));
//                        }),
//                        new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//                        }));
                });
            }
            else
            {
                GameManager.Instance.PlayNext();
                //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioButtonClick);
                SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
            }
		}

		private void OnMarkStarItemButtonClick(int index)
        {
   //         if(!AppLogicUtil.CheckLoginAndTip())
   //         {
   //             return;
   //         }
   //         if(_rateRequest)
   //         {
   //             return;
   //         }
			//if (_curMarkStarValue == index)
			//{
   //             JumpToWinState();
   //             return;
			//}
   //         int oldVal = _curMarkStarValue;
			//_curMarkStarValue = index;
   //         _rateRequest = true;
   //         SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
   //         GM2DGame.Instance.Project.UpdateRate(_curMarkStarValue, (flag)=>{
   //             SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
   //             _rateRequest = false;
   //             if(flag)
   //             {
   //                 JumpToWinState();
   //             }
   //             else
   //             {
   //                 CommonTools.ShowPopupDialog("评分失败", null,
   //                     new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
   //                         _curMarkStarValue = oldVal;
   //                         UpdateLifeItem();
   //                         CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(()=>{
   //                             OnMarkStarItemButtonClick(index);
   //                         }));
   //                     }),
   //                     new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
   //                         JumpToWinState();
   //                     })
   //                 );
   //             }
   //         });
			//UpdateLifeItem();
		}

		#endregion

        private void UpdateView()
		{
            switch (_showState) {
            case EShowState.AdvBonusWin:
                break;
            case EShowState.AdvNormalWin:
                _cachedView.Win.SetActive (true);
                _cachedView.Lose.SetActive (false);
                _cachedView.RetryBtn.gameObject.SetActive (true);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                // 如果是一章中的最后一关，则不显示下一关按钮
                if (GameManager.Instance.CurrentGame.Project.LevelId == Game.ConstDefineGM2D.AdvNormallevelPerChapter) {
                    _cachedView.NextBtn.gameObject.SetActive (false);
                } else {
                    _cachedView.NextBtn.gameObject.SetActive (true);
                }
                // 得分
                _cachedView.Score.gameObject.SetActive (true);
                _cachedView.ScoreOutLine.gameObject.SetActive (true);
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.TotalScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.TotalScore.ToString ();
                // 奖励
                _cachedView.RewardObj.SetActive (true);
                UpdateReward (AppData.Instance.AdventureData.LastAdvReward);
                break;
            case EShowState.AdvBonusLose:
                break;
            case EShowState.AdvNormalLose:
                _cachedView.Win.SetActive (false);
                _cachedView.Lose.SetActive (true);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.RetryBtn.gameObject.SetActive (true);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                _cachedView.Score.gameObject.SetActive (false);
                _cachedView.ScoreOutLine.gameObject.SetActive (false);
                _cachedView.RewardObj.SetActive (true);
                UpdateReward (AppData.Instance.AdventureData.LastAdvReward);
                break;
            case EShowState.ChallengeWin:
                _cachedView.Win.SetActive (true);
                _cachedView.Lose.SetActive (false);
                _cachedView.RetryBtn.gameObject.SetActive (false);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);

                // 得分
                _cachedView.Score.gameObject.SetActive (true);
                _cachedView.ScoreOutLine.gameObject.SetActive (true);
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.TotalScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.TotalScore.ToString ();
                // 奖励
                _cachedView.RewardObj.SetActive (true);
                UpdateReward (LocalUser.Instance.MatchUserData.LastChallengeReward);
                break;
            case EShowState.ChallengeLose:
                _cachedView.Win.SetActive (false);
                _cachedView.Lose.SetActive (true);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.RetryBtn.gameObject.SetActive (false);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                _cachedView.Score.gameObject.SetActive (false);
                _cachedView.ScoreOutLine.gameObject.SetActive (false);
                _cachedView.RewardObj.SetActive (true);
                UpdateReward (LocalUser.Instance.MatchUserData.LastChallengeReward);
                break;
            case EShowState.EditorLose:
                _cachedView.Win.SetActive (false);
                _cachedView.Lose.SetActive (true);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.RetryBtn.gameObject.SetActive (true);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (true);
                _cachedView.Score.gameObject.SetActive (false);
                _cachedView.ScoreOutLine.gameObject.SetActive (false);
                _cachedView.RewardObj.SetActive (false);
                break;
            case EShowState.EditorWin:
                _cachedView.Win.SetActive (true);
                _cachedView.Lose.SetActive (false);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.RetryBtn.gameObject.SetActive (true);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (true);
                _cachedView.Score.gameObject.SetActive (false);
                _cachedView.ScoreOutLine.gameObject.SetActive (false);
                _cachedView.RewardObj.SetActive (false);
                break;
            }
            return;

			EShowState cur = EShowState.Win;
			if (Game.PlayMode.Instance.SceneState.GameSucceed)
			{
                if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.Play)
				{
                    Project p = GameManager.Instance.CurrentGame.Project;
                    if(p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
                        && GameManager.Instance.GameMode == EGameMode.Normal)
                    {
                        cur = EShowState.WinRankRate;
                    }
                    else
                    {
                        if (GameManager.Instance.HasNext())
                        {
                            cur = EShowState.WinWithNextLevel;
                        }
                        else
                        {
                            cur = EShowState.Win;
                        }
					}
				}
                if(Game.GM2DGame.Instance.CurrentMode == Game.EMode.PlayRecord)
                {
                    cur = EShowState.Win;
                }
                // 冒险关卡普通关卡胜利后
                if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvNormal) {
                    
                }
                // 冒险关卡奖励关卡胜利后
                if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvBonus) {
                    
                } else {
                    cur = EShowState.EditorWin;
                    
                }
			}
			else if (Game.PlayMode.Instance.SceneState.GameFailed)
			{
				if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.Play
                    || Game.GM2DGame.Instance.CurrentMode == Game.EMode.PlayRecord)
				{
					cur = EShowState.Lose;
				}
				else
				{
					cur = EShowState.EditorLose;
				}
			}
			//SetUIState(cur);
		}

        private void UpdateReward (Reward reward) {
            if (null != reward && reward.IsInited) {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++) {
                    _rewardCtrl [i].Set (reward.ItemList [i].GetSprite (), reward.ItemList[i].Count.ToString ()
                    );
                }
                for (; i < _rewardCtrl.Length; i++) {
                    _rewardCtrl [i].Hide ();
                }
            } else {
                for (int i = 0; i < _rewardCtrl.Length; i++) {
                    _rewardCtrl [i].Hide ();
                }
            }
        }

        private void SetUIState(EShowState state)
        {
            string key = ((int) state).ToString();
            //UIRectTransformStatus.SetStatus(_cachedView.Status.gameObject, key);
        }

		private void PauseGame()
		{
			Messenger<Game.ECommandType>.Broadcast(EMessengerType.OnCommandChanged, Game.ECommandType.Pause);
			Game.GM2DGame.Instance.ChangeToMode(Game.EMode.Edit);
		}

        private void JumpToWinState()
        {
            if (GameManager.Instance.HasNext())
            {
                SetUIState(EShowState.WinWithNextLevel);
            }
            else
            {
                SetUIState(EShowState.Win);
            }
        }
		#endregion

		#region event

		private void OnSuccess()
		{
			//LogHelper.Info("OnSuccess");
			if (_isOpen)
			{
				LogHelper.Error("OnSuccess called but _isOpen is true");
				return;
			}
            if (!Game.PlayMode.Instance.SceneState.GameSucceed) return;
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish> ();
            return;
            //SocialGUIManager.Instance.GetUI<UICtrlGameFinish>().Close();
			_finishRes = true;
            //if(GameManager.Instance.GameMode == EGameMode.PlayRecord)
            //{
            //    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f,()=>{
            //        if(GameManager.Instance.CurrentGame != null)
            //        {
	           //         bool value = SocialGUIManager.Instance.RunRecordInApp;
            //            SocialApp.Instance.ReturnToApp(!value);
            //        }
            //    }));
            //    return;
            //}

            Project p = GameManager.Instance.CurrentGame.Project;
            if(p.ProjectStatus == EProjectStatus.PS_Private)
            {
            //    byte[] record = GetRecord();
            //    float usedTime = Game.PlayMode.Instance.GameSuccessFrameCnt * Game.ConstDefineGM2D.FixedDeltaTime;
            //    Game.GM2DGame.Instance.RecordBytes = record;
            //    Game.GM2DGame.Instance.RecordUsedTime = usedTime;

            //    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>();
            }
            else if (p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
                && GameManager.Instance.GameMode == EGameMode.Normal) {
                    //byte[] record = GetRecord();
                    //float usedTime = Game.PlayMode.Instance.GameSuccessFrameCnt * Game.ConstDefineGM2D.FixedDeltaTime;
                    //_cachedView.GameTimeText.text = string.Format("{0:D}:{1:D2}.{2:D2}",
                    //                //    Mathf.FloorToInt(usedTime/60),
                    //                //    Mathf.FloorToInt(usedTime%60),
                    //                //    Mathf.FloorToInt((usedTime*100)%100)
                    //                //);

                    ////                if(p.ProjectCategory == EProjectCategory.PC_Challenge)
                    ////                {
                    ////                    if(_cachedView.RankTitle != null)
                    ////                    {
                    ////                        _cachedView.RankTitle.gameObject.SetActive(true);
                    ////                    }
                    ////                    _cachedView.RankText.gameObject.SetActive(true);
                    ////                }
                    ////                else
                    ////                {
                    ////                    if(_cachedView.RankTitle != null)
                    ////                    {
                    ////                        _cachedView.RankTitle.gameObject.SetActive(false);
                    ////                    }
                    ////                    _cachedView.RankText.gameObject.SetActive(false);
                    ////                }
                    //                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
                    ////                GameManager.Instance.CurrentGame.Project.CommitPlayResult(true,
                    ////                    usedTime, record, DeadMarkManager.Instance.GetDeadPosition(), (rank, newRecord)=>{
                    ////                    LogHelper.Info("游戏成绩提交成功");
                    ////                    _cachedView.RankText.text = rank == -1? "未入榜":""+(rank+1);
                    ////                    if(_cachedView.NewRecordTip != null)
                    ////                    {
                    ////                        _cachedView.NewRecordTip.enabled = newRecord;
                    ////                    }
                    ////                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    ////                    if (!PlayMode.Instance.SceneState.GameSucceed) return;
                    ////                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                    ////                    GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
                    ////                }, (errCode)=>{
                    ////                    LogHelper.Info("游戏成绩提交失败");
                    ////                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    ////                    if (!PlayMode.Instance.SceneState.GameSucceed) return;
                    ////                    CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
                    ////                        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
                    ////                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnSuccess));
                    ////                        }), 
                    ////                        new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
                    ////                            _cachedView.RankText.text = "无排名";
                    ////                            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                    ////                            GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
                    ////                        }));
                    ////                });
                    //    return;
            } 
            else if (p.ProjectStatus == EProjectStatus.PS_Reform)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>();
            } else {
                SocialGUIManager.Instance.OpenUI <UICtrlGameFinish>();
            }

//            if(GameManager.Instance.GameMode == EGameMode.MatrixGuide)
//            {
//                AppData.Instance.MatrixGuide.SetMatrixGuideStep(p.MatrixGuid, GameManager.Instance.CurProjectIndex+1);
//                if(!GameManager.Instance.HasNext())
//                {
//                    CommonTools.ShowPopupDialog("恭喜你，你已经完成了所有冒险，赶快去首页尝试别的玩家创作的关卡吧~");
//                }
//            }
//            else if(GameManager.Instance.GameMode == EGameMode.OfficialProjectCollection)
//            {
//                SocialGUIManager.Instance.GetUI<UICtrlOfficialProjectCollection>().OfficialProjectCollection.Step = GameManager.Instance.CurProjectIndex+1;
//            }
		}

		private void OnFailed()
		{
			LogHelper.Info("OnFailed");
			if (_isOpen)
			{
				LogHelper.Error("OnFailed called but _isOpen is true");
				return;
            }
            if (!Game.PlayMode.Instance.SceneState.GameFailed) return;

            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish> ();

            return;
            //SocialGUIManager.Instance.GetUI<UICtrlGameFinish>().Close();
            _finishRes = false;
            if(GameManager.Instance.GameMode == EGameMode.PlayRecord)
            {
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f,()=>{
                    if(GameManager.Instance.CurrentGame != null)
                    {
						bool value = SocialGUIManager.Instance.RunRecordInApp;
						SocialApp.Instance.ReturnToApp(!value);
                    }
                }));
                return;
            }

            Project p = GameManager.Instance.CurrentGame.Project;
            if(p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
                && GameManager.Instance.GameMode == EGameMode.Normal)
            {
                byte[] record = GetRecord();
                float usedTime = Game.PlayMode.Instance.GameFailFrameCnt * Game.ConstDefineGM2D.FixedDeltaTime;

                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
//                GameManager.Instance.CurrentGame.Project.CommitPlayResult(false, usedTime, record, DeadMarkManager.Instance.GetDeadPosition(), (rank, newRecord)=>{
//                    LogHelper.Info("游戏成绩提交成功");
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    if (!PlayMode.Instance.SceneState.GameFailed) return;
//                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioFailed);
//                    GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
//                }, (errCode)=>{
//                    LogHelper.Info("游戏成绩提交失败");
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    if (!PlayMode.Instance.SceneState.GameFailed) return;
//                    CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
//                        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
//                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnFailed));
//                        }), 
//                        new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
//                            //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
//                            GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
//                        }));
//                });
            }
            else
            {
                //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioFailed);
                SocialGUIManager.Instance.OpenUI <UICtrlGameFinish>();
            }
		}

        private byte[] GetRecord()
        {
            GM2DRecordData recordData = new GM2DRecordData();
            //recordData.Version = GM2DGame.Version;
            //recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            //recordData.Data.AddRange(PlayMode.Instance.InputDatas);
            //byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            byte[] record = null;
            //if(recordByte == null)
            //{
            //    LogHelper.Error("录像数据出错");
            //}
            //else
            //{
            //    record = MatrixProjectTools.CompressLZMA(recordByte);
            //}
            return record;
        }

		#endregion
	}
}