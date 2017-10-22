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
            _gameSituation = EGameSituation.World;
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
                        record,
                        DeadMarkManager.Instance.GetDeadPosition(),
                        () =>
                        {
                            LogHelper.Info("游戏成绩提交成功");
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            if (!PlayMode.Instance.SceneState.GameFailed) return;
                            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Lose);
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
                                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState
                                        .Lose);
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
                        record,
                        DeadMarkManager.Instance.GetDeadPosition(),
                        () =>
                        {
                            LogHelper.Info("游戏成绩提交成功");
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            if (!PlayMode.Instance.SceneState.GameSucceed) return;
                            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
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
                                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.Win);
                                }));
                        });
                });
            });
        }

        public override bool Restart(Action successCb, Action failedCb)
        {
            _project.RequestPlay(() =>
            {
                GameRun.Instance.RePlay();
                OnGameStart();
                if (successCb != null)
                {
                    successCb.Invoke();
                }
            }, code => failedCb());
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
            GameRun.Instance.Playing();
        }
    }
}