/********************************************************************
** Filename : UICtrlGameFinish  
** Author : ake
** Date : 4/29/2016 8:24:49 PM
** Summary : UICtrlGameFinish  
***********************************************************************/


using System;
using DG.Tweening;
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
		   // JudgeExpAndLvl();
		}

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            _cachedView.ShineRotateRoot.localRotation = Quaternion.Euler (0, 0, -Time.realtimeSinceStartup * 20f);
        }

		#region   private

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
		}
        #region UIEvent

        private void OnReturnBtn ()
        {
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

        #endregion
        private float CountExpRatio(float exp,int level)
        {
            return exp/TableManager.Instance.Table_PlayerLvToExpDic[level].AdvExp;
        }

        public void GainExperienceAnimation(float endValue,Callback callback=null)
        {
            if (_cachedView != null)
                //   _cachedView.ExpBar.fillAmount.DOLocalMoveY(12, 0.4f, false);
                _cachedView.ExpBar.DOFillAmount(endValue, 1.0f).OnComplete(() => { if(callback!=null) callback(); });

        }
        private void JudgeExpAndLvl()
	    {
	        int PlayerLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            long currentPlayerExp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp;
	        long RewardExp=0;
            if(AppData.Instance.AdventureData.LastAdvReward!=null&& AppData.Instance.AdventureData.LastAdvReward.ItemList.Count!=0)
            for (int i = 0; i < AppData.Instance.AdventureData.LastAdvReward.ItemList.Count; i++)
	        {
	            if (AppData.Instance.AdventureData.LastAdvReward.ItemList[i].Type == 3)
	            {
	                RewardExp = AppData.Instance.AdventureData.LastAdvReward.ItemList[i].Count;
	            }
            }
            _cachedView.PlusExp.text = String.Format("+{0}",RewardExp);
            if (currentPlayerExp - RewardExp > 0)
            {
               long initialExp = currentPlayerExp - RewardExp;
               _cachedView.ExpBar.fillAmount = CountExpRatio(initialExp, PlayerLevel);
                GainExperienceAnimation(CountExpRatio(currentPlayerExp, PlayerLevel));
            }
	        else
            {
                long initialExp = TableManager.Instance.Table_PlayerLvToExpDic[PlayerLevel - 1].AdvExp - (currentPlayerExp - RewardExp);
                _cachedView.ExpBar.fillAmount = CountExpRatio(initialExp, PlayerLevel-1);
                 GainExperienceAnimation(1.0f, UpGrade);
            }
	    }

	    private void UpGrade()
        {
            int PlayerLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            long currentPlayerExp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp;
            GainExperienceAnimation(CountExpRatio(currentPlayerExp, PlayerLevel));
	        _cachedView.PlusExp.SetActiveEx(false);
            _cachedView.UpGrade.SetActiveEx(true);
            _cachedView.level.text = PlayerLevel.ToString();
        }

	    private void UpdateView()
		{
            switch (_showState) {
                case EShowState.Win:
                    _cachedView.Win.SetActive (true);
                    _cachedView.Lose.SetActive (false);
                    _cachedView.RetryBtn.gameObject.SetActive (true);
                    _cachedView.ReturnBtn.gameObject.SetActive (true);
                    _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                    _cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.Score.gameObject.SetActive (true);
                    _cachedView.ScoreOutLine.gameObject.SetActive (true);
                    _cachedView.Score.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                    _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                    // 奖励
                    _cachedView.RewardObj.SetActive (false);
                    _cachedView.ExpBarObj.SetActive(false);
                    //                    UpdateReward (AppData.Instance.AdventureData.LastAdvReward);

                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
                    break;
                case EShowState.Lose:
                    _cachedView.Win.SetActive (false);
                    _cachedView.Lose.SetActive (true);
                    _cachedView.ReturnBtn.gameObject.SetActive (true);
                    _cachedView.RetryBtn.gameObject.SetActive (true);
                    _cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                    _cachedView.Score.gameObject.SetActive (false);
                    _cachedView.ScoreOutLine.gameObject.SetActive (false);
                    _cachedView.RewardObj.SetActive (false);
                    _cachedView.ExpBarObj.SetActive(false);
                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishLose");
                    break;
            case EShowState.AdvBonusWin:
                _cachedView.Win.SetActive (true);
                _cachedView.Lose.SetActive (false);
                _cachedView.RetryBtn.gameObject.SetActive (false);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.Score.gameObject.SetActive (true);
                _cachedView.ScoreOutLine.gameObject.SetActive (true);
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                // 奖励
                _cachedView.RewardObj.SetActive (true);
                _cachedView.ExpBarObj.SetActive (true);
                JudgeExpAndLvl();
                UpdateReward (AppData.Instance.AdventureData.LastAdvReward);

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
                break;
            case EShowState.AdvNormalWin:
                _cachedView.Win.SetActive (true);
                _cachedView.Lose.SetActive (false);
                _cachedView.RetryBtn.gameObject.SetActive (false);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                _cachedView.NextBtn.gameObject.SetActive (false);

                // 如果是一章中的最后一关，则不显示下一关按钮
//                if (GameManager.Instance.CurrentGame.Project.LevelId == Game.ConstDefineGM2D.AdvNormallevelPerChapter) {
//                    _cachedView.NextBtn.gameObject.SetActive (false);
//                } else {
//                    _cachedView.NextBtn.gameObject.SetActive (true);
//                }
                // 得分
                _cachedView.Score.gameObject.SetActive (true);
                _cachedView.ScoreOutLine.gameObject.SetActive (true);
                _cachedView.Score.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                _cachedView.ScoreOutLine.text = Game.PlayMode.Instance.SceneState.CurScore.ToString ();
                // 奖励
                _cachedView.RewardObj.SetActive (true);
                _cachedView.ExpBarObj.SetActive(true);
                JudgeExpAndLvl();

                    UpdateReward(AppData.Instance.AdventureData.LastAdvReward);

                int starCnt = AppData.Instance.AdventureData.LastAdvStarCnt;
                if (starCnt == 3) {
                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
                } else if (starCnt == 2) {
                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin2Star");
                } else if (starCnt == 1) {
                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin1Star");
                } else
                {
                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin1Star");
                }
                break;
            case EShowState.AdvBonusLose:
                _cachedView.Win.SetActive (false);
                _cachedView.Lose.SetActive (true);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                _cachedView.RetryBtn.gameObject.SetActive (false);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                _cachedView.Score.gameObject.SetActive (false);
                _cachedView.ScoreOutLine.gameObject.SetActive (false);
                _cachedView.RewardObj.SetActive (true);
                _cachedView.ExpBarObj.SetActive(true);
                JudgeExpAndLvl();

                UpdateReward(AppData.Instance.AdventureData.LastAdvReward);

                _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishLose");
                break;
            case EShowState.AdvNormalLose:
                _cachedView.Win.SetActive (false);
                _cachedView.Lose.SetActive (true);
                _cachedView.ReturnBtn.gameObject.SetActive (true);
                    _cachedView.RetryBtn.gameObject.SetActive (false);
                _cachedView.NextBtn.gameObject.SetActive (false);
                _cachedView.ContinueEditBtn.gameObject.SetActive (false);
                _cachedView.Score.gameObject.SetActive (false);
                _cachedView.ScoreOutLine.gameObject.SetActive (false);
                _cachedView.RewardObj.SetActive (true);
                _cachedView.ExpBarObj.SetActive (true);
                JudgeExpAndLvl();
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
                _cachedView.ExpBarObj.SetActive (true);
                JudgeExpAndLvl();
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
                _cachedView.ExpBarObj.SetActive(true);
                JudgeExpAndLvl();

                    UpdateReward(LocalUser.Instance.MatchUserData.LastChallengeReward);

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
                _cachedView.ExpBarObj.SetActive(false);


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
                _cachedView.ExpBarObj.SetActive(false);


                    _cachedView.GetComponent<Animation> ().Play ("UICtrlGameFinishWin3Star");
                break;
            }
            return;
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

		private void PauseGame()
		{
            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeEdit != null)
            {
                gameModeEdit.ChangeMode(GameModeEdit.EMode.Edit);
            }
		}

		#endregion

		#region event

		#endregion
	}
}