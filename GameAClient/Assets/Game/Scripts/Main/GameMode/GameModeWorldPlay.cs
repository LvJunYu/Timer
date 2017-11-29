using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeWorldPlay : GameModePlay
    {
        protected UICtrlGameFinish.EShowState _successType;
        protected UICtrlGameFinish.EShowState _failType;
        private bool _firstStart;
        private long _battleId;
        private EShadowBattleType _eShadowBattleType;

        public EShadowBattleType ShadowBattleType
        {
            get { return _eShadowBattleType; }
        }

        public override bool SaveShadowData
        {
            get { return true; }
        }

        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            if (param != null)
            {
                _playShadowData = true;
                _firstStart = true;
                _gameSituation = EGameSituation.ShadowBattle;
                _successType = UICtrlGameFinish.EShowState.ShadowBattleWin;
                _failType = UICtrlGameFinish.EShowState.ShadowBattleLose;
                ShadowDataPlayed = null;
                var shadowBattleData = param as Msg_SC_DAT_ShadowBattleData;
                _battleId = shadowBattleData.Id;
                _record = new Record(shadowBattleData.Record);
                if (InitRecord() && _gm2drecordData.ShadowData != null)
                {
                    ShadowDataPlayed = new ShadowData(_gm2drecordData.ShadowData);
                }
                if (shadowBattleData.OriginPlayer != null &&
                    shadowBattleData.OriginPlayer.UserId != LocalUser.Instance.UserGuid)
                {
                    _eShadowBattleType = EShadowBattleType.FriendHelp;
                }
                else
                {
                    _eShadowBattleType = EShadowBattleType.Normal;
                }
            }
            else
            {
                _gameSituation = EGameSituation.World;
                _successType = UICtrlGameFinish.EShowState.Win;
                _failType = UICtrlGameFinish.EShowState.Lose;
            }
            return true;
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            _coroutineProxy.StopAllCoroutines();
            _coroutineProxy.StartCoroutine(GameFlow());
        }

        public override void OnGameFailed()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            byte[] record;
            Loom.RunAsync(() =>
            {
                record = GetRecord(false);
                Loom.QueueOnMainThread(() =>
                {
                    _project.CommitPlayResult(
                        false,
                        PlayMode.Instance.GameFailFrameCnt,
                        PlayMode.Instance.SceneState.CurScore,
                        PlayMode.Instance.SceneState.GemGain,
                        PlayMode.Instance.SceneState.MonsterKilled,
                        PlayMode.Instance.SceneState.SecondLeft,
                        PlayMode.Instance.MainPlayer.Life,
                        PlayMode.Instance.Statistic.KillByTrapCnt,
                        PlayMode.Instance.Statistic.KillByMonsterCnt,
                        PlayMode.Instance.Statistic.BreakBrickCnt,
                        PlayMode.Instance.Statistic.TrampCloudCnt,
                        record,
                        DeadMarkManager.Instance.GetDeadPosition(),
                        () =>
                        {
                            LogHelper.Info("游戏成绩提交成功");
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            if (!PlayMode.Instance.SceneState.GameFailed) return;
                            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(_failType);
                        }, (errCode) =>
                        {
                            LogHelper.Info("游戏成绩提交失败");
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            if (!PlayMode.Instance.SceneState.GameFailed) return;
                            CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
                                new KeyValuePair<string, Action>("重试",
                                    () =>
                                    {
                                        CoroutineProxy.Instance.StartCoroutine(
                                            CoroutineProxy.RunNextFrame(OnGameFailed));
                                    }),
                                new KeyValuePair<string, Action>("跳过", () =>
                                {
                                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(_failType);
                                }));
                        });
                });
            });
        }

        public override void OnGameSuccess()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            byte[] record;
            Loom.RunAsync(() =>
            {
                record = GetRecord(true);
                Loom.QueueOnMainThread(() =>
                {
                    _project.CommitPlayResult(
                        true,
                        PlayMode.Instance.GameSuccessFrameCnt,
                        PlayMode.Instance.SceneState.CurScore,
                        PlayMode.Instance.SceneState.GemGain,
                        PlayMode.Instance.SceneState.MonsterKilled,
                        PlayMode.Instance.SceneState.SecondLeft,
                        PlayMode.Instance.MainPlayer.Life,
                        PlayMode.Instance.Statistic.KillByTrapCnt,
                        PlayMode.Instance.Statistic.KillByMonsterCnt,
                        PlayMode.Instance.Statistic.BreakBrickCnt,
                        PlayMode.Instance.Statistic.TrampCloudCnt,
                        record,
                        DeadMarkManager.Instance.GetDeadPosition(),
                        () =>
                        {
                            LogHelper.Info("游戏成绩提交成功");
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            if (!PlayMode.Instance.SceneState.GameSucceed) return;
                            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(_successType);
                        }, (errCode) =>
                        {
                            LogHelper.Info("游戏成绩提交失败");
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            if (!PlayMode.Instance.SceneState.GameSucceed) return;
                            CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
                                new KeyValuePair<string, Action>("重试",
                                    () =>
                                    {
                                        CoroutineProxy.Instance.StartCoroutine(
                                            CoroutineProxy.RunNextFrame(OnGameFailed));
                                    }),
                                new KeyValuePair<string, Action>("跳过", () =>
                                {
                                    //GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
                                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(_successType);
                                }));
                        });
                });
            });
            //乱入对决好友帮战胜利的处理
            if (_playShadowData && _eShadowBattleType == EShadowBattleType.FriendHelp &&
                PlayMode.Instance.SceneState.CheckShadowWin())
            {
                SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
            }
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            base.QuitGame(successCB, failureCB, forceQuitWhenFailed);
            if (!_playShadowData && _project.ProjectUserData.PlayCount == 0)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWorldProjectComment>(_project);
            }
        }

        public override bool Restart(Action<bool> successCb, Action failedCb)
        {
            if (_playShadowData)
            {
                _project.RequestPlayShadowBattle(_battleId, () =>
                {
                    GameRun.Instance.RePlay();
                    _firstStart = false;
                    OnGameStart();
                    if (successCb != null)
                    {
                        successCb.Invoke(true);
                    }
                }, failedCb);
            }
            else
            {
                _project.RequestPlay(() =>
                {
                    GameRun.Instance.RePlay();
                    _firstStart = false;
                    OnGameStart();
                    if (successCb != null)
                    {
                        successCb.Invoke(true);
                    }
                }, code => failedCb());
            }
            return true;
        }

        protected byte[] GetRecord(bool win)
        {
            GM2DRecordData recordData = new GM2DRecordData();
            recordData.Version = GM2DGame.Version;
            recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            if (SaveShadowData && win)
            {
                recordData.ShadowData = ShadowData.GetRecShadowData();
            }
            recordData.Data.AddRange(_inputDatas);
            byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            byte[] record = null;
            if (recordByte == null)
            {
                LogHelper.Error("录像数据出错");
            }
            else
            {
                record = MatrixProjectTools.CompressLZMA(recordByte);
            }
            return record;
        }

        protected IEnumerator GameFlow()
        {
//            UICtrlCountDown uictrlCountDown = SocialGUIManager.Instance.OpenUI<UICtrlCountDown>();
//            yield return new WaitUntil(() => uictrlCountDown.ShowComplete);
//            if (!Application.isMobilePlatform)
//            {
//                UICtrlSceneState uictrlSceneState = SocialGUIManager.Instance.GetUI<UICtrlSceneState>();
//                uictrlSceneState.ShowHelpPage3Seconds();
//                yield return new WaitUntil(() => uictrlSceneState.ShowHelpPage3SecondsComplete);
//            }
            yield return null;
            _run = true;
            GameRun.Instance.Playing();
            if (_playShadowData && _firstStart)
            {
                if (ShadowBattleType == EShadowBattleType.FriendHelp)
                {
                    SocialGUIManager.Instance.OpenUI<UICtrlShadowBattleHelp>();
                }
                else
                {
                    SocialGUIManager.Instance.OpenUI<UICtrlShadowBattleSurprise>();
                }
            }
        }
    }

    public enum EShadowBattleType
    {
        Normal,
        FriendHelp
    }
}