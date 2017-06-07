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
using GameA.Game;

namespace GameA
{
	[UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlGameFinish : UICtrlInGameBase<UIViewGameFinish>
	{
		public enum EShowState
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
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			InitUI();
		}

		protected override void OnOpen(object parameter)
		{
			base.OnOpen(parameter);
            EShowState showState = (EShowState)parameter;
            _showState = showState;
//			_curMarkStarValue = GM2DGame.Instance.Project.UserRate;
			UpdateView();
			//UpdateLifeItem();
		}

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            _cachedView.ShineRotateRoot.localRotation = Quaternion.Euler (0, 0, -Time.realtimeSinceStartup * 20f);
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
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在重新开始");
            Game.GM2DGame.Instance.GameMode.Restart(()=>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                },() => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    CommonTools.ShowPopupDialog("启动失败", null, 
//                        new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
//                            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnClickRestartGame));
//                        }),
//                        new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//                        }));
                    OnReturnBtn();
                }
            );
        }
        private void OnNextBtn () {}

        private void OnContinueEditBtn ()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish> ();
            PauseGame ();
        }
//		private void OnClickRestartGame()
//		{
//			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
//            Game.GM2DGame.Instance.GameMode.Restart(()=>
//            {
//				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//				SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
//            },() => {
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                CommonTools.ShowPopupDialog("启动失败", null, 
//                    new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
//                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnClickRestartGame));
//                    }),
//                    new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//                    }));
//			});
//        }


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

            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            GameModePlay gameModePlay = Game.GM2DGame.Instance.GameMode as GameModePlay;
            if (null == gameModePlay)
            {
                return;
            }
            gameModePlay.PlayNext(()=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            }, ()=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                CommonTools.ShowPopupDialog("启动失败", null, 
                    new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnClickNextLevelButton));
                    }),
                    new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
                    }));
			});
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
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                // 奖励
                _cachedView.RewardObj.SetActive (true);
                UpdateReward (AppData.Instance.AdventureData.LastAdvReward);

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
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

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishLose");
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
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                // 奖励
                _cachedView.RewardObj.SetActive (true);
                UpdateReward (LocalUser.Instance.MatchUserData.LastChallengeReward);

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
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

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishLose");
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

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishLose");
                break;
            case EShowState.EditorWin:
                _cachedView.Win.SetActive (true);
                _cachedView.Lose.SetActive (false);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.RetryBtn.gameObject.SetActive (true);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (true);
                _cachedView.Score.gameObject.SetActive (true);
                _cachedView.ScoreOutLine.gameObject.SetActive (true);
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                _cachedView.RewardObj.SetActive (false);

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
                break;
            }
            return;

			//EShowState cur = EShowState.Win;
			//if (Game.PlayMode.Instance.SceneState.GameSucceed)
			//{
   //             if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.Play)
			//	{
   //                 Project p = GameManager.Instance.CurrentGame.Project;
   //                 if(p.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public
   //                     && GameManager.Instance.GameMode == EGameMode.Normal)
   //                 {
   //                     cur = EShowState.WinRankRate;
   //                 }
   //                 else
   //                 {
   //                     if (GameManager.Instance.HasNext())
   //                     {
   //                         cur = EShowState.WinWithNextLevel;
   //                     }
   //                     else
   //                     {
   //                         cur = EShowState.Win;
   //                     }
			//		}
			//	}
   //             if(Game.GM2DGame.Instance.CurrentMode == Game.EMode.PlayRecord)
   //             {
   //                 cur = EShowState.Win;
   //             }
   //             // 冒险关卡普通关卡胜利后
   //             if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvNormal) {
                    
   //             }
   //             // 冒险关卡奖励关卡胜利后
   //             if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.AdvBonus) {
                    
   //             } else {
   //                 cur = EShowState.EditorWin;
                    
   //             }
			//}
			//else if (Game.PlayMode.Instance.SceneState.GameFailed)
			//{
			//	if (Game.GM2DGame.Instance.CurrentMode == Game.EMode.Play
   //                 || Game.GM2DGame.Instance.CurrentMode == Game.EMode.PlayRecord)
			//	{
			//		cur = EShowState.Lose;
			//	}
			//	else
			//	{
			//		cur = EShowState.EditorLose;
			//	}
			//}
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

        //private void SetUIState(EShowState state)
        //{
        //    string key = ((int) state).ToString();
        //    //UIRectTransformStatus.SetStatus(_cachedView.Status.gameObject, key);
        //}

		private void PauseGame()
		{
			Messenger<Game.ECommandType>.Broadcast(EMessengerType.OnCommandChanged, Game.ECommandType.Pause);
            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeEdit != null)
            {
                gameModeEdit.ChangeMode(GameModeEdit.EMode.Edit);
            }
		}

        private void JumpToWinState()
        {
            //if (GameManager.Instance.HasNext())
            //{
            //    SetUIState(EShowState.WinWithNextLevel);
            //}
            //else
            //{
            //    SetUIState(EShowState.Win);
            //}
        }
		#endregion

		#region event

		#endregion
	}
}