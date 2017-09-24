/********************************************************************
** Filename : UICtrlGameFinish  
** Author : ake
** Date : 4/29/2016 8:24:49 PM
** Summary : UICtrlGameFinish  
***********************************************************************/


using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
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
            ChallengeLose
        }

        private EShowState _showState;
        private bool _finishRes;
        private readonly List<UIParticleItem> _particleList = new List<UIParticleItem>();
        private int _curMarkStarValue;

        private USCtrlGameFinishReward[] _rewardCtrl;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            EShowState showState = (EShowState) parameter;
            _showState = showState;
//			_curMarkStarValue = GM2DGame.Instance.Project.UserRate;
            UpdateView();
            //UpdateLifeItem();
            // JudgeExpAndLvl();
        }

        protected override void OnClose()
        {
            for (int i = 0; i < _particleList.Count; i++)
            {
                _particleList[i].Particle.DestroySelf();
            }
            _particleList.Clear();
            base.OnClose();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _cachedView.ShineRotateRoot.localRotation = Quaternion.Euler(0, 0, -Time.realtimeSinceStartup * 20f);
        }

        #region   private

        private void InitUI()
        {
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.RetryBtn.onClick.AddListener(OnRetryBtn);
            _cachedView.ContinueEditBtn.onClick.AddListener(OnContinueEditBtn);

            _rewardCtrl = new USCtrlGameFinishReward [_cachedView.Rewards.Length];
            for (int i = 0; i < _cachedView.Rewards.Length; i++)
            {
                _rewardCtrl[i] = new USCtrlGameFinishReward();
                _rewardCtrl[i].Init(_cachedView.Rewards[i]);
            }
            _cachedView.UpGrade.SetActiveEx(false);
        }

        #region UIEvent

        private void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            GM2DGame.Instance.QuitGame(
                () => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                true
            );
        }

        private void OnRetryBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在重新开始");
            GM2DGame.Instance.GameMode.Restart(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("启动失败", null,
                        new KeyValuePair<string, Action>("重试",
                            () => { CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnRetryBtn)); }),
                        new KeyValuePair<string, Action>("取消", () => { }));
                    OnReturnBtn();
                }
            );
        }

        private void OnContinueEditBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            PauseGame();
        }

        #endregion

        private float CountExpRatio(float exp, int level)
        {
            return (exp
                       //- TableManager.Instance.Table_PlayerLvToExpDic[LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp
                   )
                   / (TableManager.Instance.Table_PlayerLvToExpDic[level + 1].AdvExp -
                      TableManager.Instance.Table_PlayerLvToExpDic[level].AdvExp);
        }

        public void GainExperienceAnimation(float endValue, Callback callback = null)
        {
            if (_cachedView != null)
                //   _cachedView.ExpBar.fillAmount.DOLocalMoveY(12, 0.4f, false);
                _cachedView.ExpBar.DOFillAmount(endValue, 1.0f).OnComplete(() =>
                {
                    if (callback != null) callback();
                });
        }

        private void JudgeExpAndLvl()
        {
            //Debug.Log("LevelDataInGameFinish:"+ LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel);
            int PlayerLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            long currentPlayerExp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp;
            long RewardExp = 0;
            if (AppData.Instance.AdventureData.LastAdvReward != null &&
                AppData.Instance.AdventureData.LastAdvReward.ItemList.Count != 0)
                for (int i = 0; i < AppData.Instance.AdventureData.LastAdvReward.ItemList.Count; i++)
                {
                    if (AppData.Instance.AdventureData.LastAdvReward.ItemList[i].Type == 3)
                    {
                        RewardExp = AppData.Instance.AdventureData.LastAdvReward.ItemList[i].Count;
                    }
                }
            _cachedView.PlusExp.text = String.Format("+{0}", RewardExp);
            if (currentPlayerExp - RewardExp >= TableManager.Instance.Table_PlayerLvToExpDic[
                    LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp) //没有升级
            {
                long initialExp = currentPlayerExp - RewardExp - TableManager.Instance.Table_PlayerLvToExpDic[
                                      LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp;
                _cachedView.ExpBar.fillAmount = CountExpRatio(initialExp, PlayerLevel);
                GainExperienceAnimation(CountExpRatio(currentPlayerExp - TableManager.Instance.Table_PlayerLvToExpDic[
                                                              LocalUser.Instance.User.UserInfoSimple.LevelData
                                                                  .PlayerLevel]
                                                          .AdvExp, PlayerLevel));
            }
            else //升级
            {
                long initialExp = 0;
                if (LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel == 2)
                {
                    initialExp = (currentPlayerExp - RewardExp);
                }
                else
                {
                    initialExp = (currentPlayerExp - RewardExp) - TableManager.Instance.Table_PlayerLvToExpDic[
                                     LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel - 2].AdvExp;
                }
                _cachedView.ExpBar.fillAmount = CountExpRatio(initialExp, PlayerLevel - 1);

                GainExperienceAnimation(1.0f, UpGrade);
                _cachedView.PlusExp.SetActiveEx(false);
                _cachedView.UpGrade.SetActiveEx(true);
            }
        }

        private void UpGrade()
        {
            _cachedView.ExpBar.fillAmount = 0;
            int PlayerLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            long currentPlayerExp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp - TableManager.Instance
                                        .Table_PlayerLvToExpDic[
                                            LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp;
            GainExperienceAnimation(CountExpRatio(currentPlayerExp - TableManager.Instance.Table_PlayerLvToExpDic[
                                                          LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel]
                                                      .AdvExp, PlayerLevel));
            _cachedView.PlusExp.SetActiveEx(false);
            _cachedView.UpGrade.SetActiveEx(true);
            _cachedView.level.text = PlayerLevel.ToString();
        }

        private void UpdateView()
        {
            switch (_showState)
            {
                case EShowState.Win:
                    _cachedView.Win.SetActive(true);
                    _cachedView.Lose.SetActive(false);
                    _cachedView.RetryBtn.gameObject.SetActive(true);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.Score.gameObject.SetActive(true);
                    _cachedView.ScoreOutLine.gameObject.SetActive(true);
                    _cachedView.Score.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    _cachedView.ScoreOutLine.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    // 奖励
                    _cachedView.RewardObj.SetActive(false);
                    _cachedView.ExpBarObj.SetActive(false);
                    //                    UpdateReward (AppData.Instance.AdventureData.LastAdvReward);

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin3Star");
                    PlayWinEffect();
                    break;
                case EShowState.Lose:
                    _cachedView.Win.SetActive(false);
                    _cachedView.Lose.SetActive(true);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.RetryBtn.gameObject.SetActive(true);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    _cachedView.Score.gameObject.SetActive(false);
                    _cachedView.ScoreOutLine.gameObject.SetActive(false);
                    _cachedView.RewardObj.SetActive(false);
                    _cachedView.ExpBarObj.SetActive(false);
                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishLose");
                    PlayLoseEffect();
                    break;
                case EShowState.AdvBonusWin:
                    _cachedView.Win.SetActive(true);
                    _cachedView.Lose.SetActive(false);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.Score.gameObject.SetActive(true);
                    _cachedView.ScoreOutLine.gameObject.SetActive(true);
                    _cachedView.Score.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    _cachedView.ScoreOutLine.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    // 奖励
                    _cachedView.RewardObj.SetActive(true);
                    _cachedView.ExpBarObj.SetActive(true);
                    JudgeExpAndLvl();
                    UpdateReward(AppData.Instance.AdventureData.LastAdvReward);

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin3Star");
                    PlayWinEffect();
                    break;
                case EShowState.AdvNormalWin:
                    _cachedView.Win.SetActive(true);
                    _cachedView.Lose.SetActive(false);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);

                    // 如果是一章中的最后一关，则不显示下一关按钮
//                if (GameManager.Instance.CurrentGame.Project.LevelId == Game.ConstDefineGM2D.AdvNormallevelPerChapter) {
//                    _cachedView.NextBtn.gameObject.SetActive (false);
//                } else {
//                    _cachedView.NextBtn.gameObject.SetActive (true);
//                }
                    // 得分
                    _cachedView.Score.gameObject.SetActive(true);
                    _cachedView.ScoreOutLine.gameObject.SetActive(true);
                    _cachedView.Score.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    _cachedView.ScoreOutLine.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    // 奖励
                    _cachedView.RewardObj.SetActive(true);
                    _cachedView.ExpBarObj.SetActive(true);
                    JudgeExpAndLvl();

                    UpdateReward(AppData.Instance.AdventureData.LastAdvReward);

                    int starCnt = AppData.Instance.AdventureData.LastAdvStarCnt;
                    if (starCnt == 3)
                    {
                        _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin3Star");
                    }
                    else if (starCnt == 2)
                    {
                        _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin2Star");
                    }
                    else if (starCnt == 1)
                    {
                        _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin1Star");
                    }
                    else
                    {
                        _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin1Star");
                    }
                    PlayWinEffect();
                    break;
                case EShowState.AdvBonusLose:
                    _cachedView.Win.SetActive(false);
                    _cachedView.Lose.SetActive(true);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    _cachedView.Score.gameObject.SetActive(false);
                    _cachedView.ScoreOutLine.gameObject.SetActive(false);
                    _cachedView.RewardObj.SetActive(true);
                    _cachedView.ExpBarObj.SetActive(true);
                    JudgeExpAndLvl();

                    UpdateReward(AppData.Instance.AdventureData.LastAdvReward);

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishLose");
                    PlayLoseEffect();
                    break;
                case EShowState.AdvNormalLose:
                    _cachedView.Win.SetActive(false);
                    _cachedView.Lose.SetActive(true);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    _cachedView.Score.gameObject.SetActive(false);
                    _cachedView.ScoreOutLine.gameObject.SetActive(false);
                    _cachedView.RewardObj.SetActive(true);
                    _cachedView.ExpBarObj.SetActive(true);
                    JudgeExpAndLvl();
                    UpdateReward(AppData.Instance.AdventureData.LastAdvReward);

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishLose");
                    PlayLoseEffect();
                    break;
                case EShowState.ChallengeWin:
                    _cachedView.Win.SetActive(true);
                    _cachedView.Lose.SetActive(false);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);

                    // 得分
                    _cachedView.Score.gameObject.SetActive(true);
                    _cachedView.ScoreOutLine.gameObject.SetActive(true);
                    _cachedView.Score.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    _cachedView.ScoreOutLine.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    // 奖励
                    _cachedView.RewardObj.SetActive(true);
                    _cachedView.ExpBarObj.SetActive(true);
                    JudgeExpAndLvl();
                    UpdateReward(LocalUser.Instance.MatchUserData.LastChallengeReward);

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin3Star");
                    PlayWinEffect();
                    break;
                case EShowState.ChallengeLose:
                    _cachedView.Win.SetActive(false);
                    _cachedView.Lose.SetActive(true);
                    _cachedView.ReturnBtn.gameObject.SetActive(true);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(false);
                    _cachedView.Score.gameObject.SetActive(false);
                    _cachedView.ScoreOutLine.gameObject.SetActive(false);
                    _cachedView.RewardObj.SetActive(true);
                    _cachedView.ExpBarObj.SetActive(true);
                    JudgeExpAndLvl();

                    UpdateReward(LocalUser.Instance.MatchUserData.LastChallengeReward);

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishLose");
                    PlayLoseEffect();
                    break;
                case EShowState.EditorLose:
                    _cachedView.Win.SetActive(false);
                    _cachedView.Lose.SetActive(true);
                    _cachedView.ReturnBtn.gameObject.SetActive(false);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(true);
                    _cachedView.Score.gameObject.SetActive(false);
                    _cachedView.ScoreOutLine.gameObject.SetActive(false);
                    _cachedView.RewardObj.SetActive(false);
                    _cachedView.ExpBarObj.SetActive(false);


                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishLose");
                    break;
                case EShowState.EditorWin:
                    _cachedView.Win.SetActive(true);
                    _cachedView.Lose.SetActive(false);
                    _cachedView.ReturnBtn.gameObject.SetActive(false);
                    _cachedView.RetryBtn.gameObject.SetActive(false);
                    //_cachedView.NextBtn.gameObject.SetActive (false);
                    _cachedView.ContinueEditBtn.gameObject.SetActive(true);
                    _cachedView.Score.gameObject.SetActive(true);
                    _cachedView.ScoreOutLine.gameObject.SetActive(true);
                    _cachedView.Score.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    _cachedView.ScoreOutLine.text = PlayMode.Instance.SceneState.CurScore.ToString();
                    _cachedView.RewardObj.SetActive(false);
                    _cachedView.ExpBarObj.SetActive(false);


                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin3Star");
                    break;
            }
        }

        private void UpdateReward(Reward reward)
        {
            if (null != reward && reward.IsInited)
            {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++)
                {
                    _rewardCtrl[i].Set(reward.ItemList[i].GetSprite(), reward.ItemList[i].Count.ToString()
                    );
                }
                for (; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
            else
            {
                for (int i = 0; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
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

        private void PlayWinEffect()
        {
            var uiparticle =
                GameParticleManager.Instance.GetUIParticleItem(ParticleNameConstDefineGM2D.WinEffect, _cachedView.Trans,
                    _groupId);
            uiparticle.Particle.Play();
            _particleList.Add(uiparticle);
        }

        private void PlayLoseEffect()
        {
            var uiparticle =
                GameParticleManager.Instance.GetUIParticleItem(ParticleNameConstDefineGM2D.LoseEffect,
                    _cachedView.Trans,
                    _groupId);
            uiparticle.Particle.Play();
            _particleList.Add(uiparticle);
        }

        #endregion

        #region event

        #endregion
    }
}