using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class GameModeAdventurePlay : GameModePlay, ISituationAdventure
    {
        private SituationAdventureParam _adventureLevelInfo;
        private AdventureGuideBase _guideBase;
        private const string HandbookEventPrefix = "Book_";
        private readonly HashSet<int> _handbookShowSet = new HashSet<int>();


        public SituationAdventureParam GetLevelInfo()
        {
            return _adventureLevelInfo;
        }

        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.Adventure;
            _adventureLevelInfo = param as SituationAdventureParam;
            if (_adventureLevelInfo == null)
            {
                return false;
            }
            AdventureGuideManager.Instance.TryGetGuide(_adventureLevelInfo.Section, _adventureLevelInfo.ProjectType,
                _adventureLevelInfo.Level, out _guideBase);
            if (_guideBase != null)
            {
                _guideBase.Init();
            }
            
            Messenger<string, bool>.AddListener(EMessengerType.OnTrigger, HandleHandbook);
            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_guideBase != null)
            {
                _guideBase.UpdateLogic();
            }
        }
        
        public override void Update()
        {
            base.Update();
            if (_guideBase != null)
            {
                _guideBase.Update();
            }
        }

        public override bool Stop()
        {
            if (_guideBase != null)
            {
                _guideBase.Dispose();
                _guideBase = null;
            }
            Messenger<string, bool>.RemoveListener(EMessengerType.OnTrigger, HandleHandbook);
            return base.Stop();
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            _coroutineProxy.StopAllCoroutines();
            _coroutineProxy.StartCoroutine(GameFlow());
        }

        public override void OnGameFailed()
        {
            UICtrlGameFinish.EShowState showState
                = _adventureLevelInfo.ProjectType == EAdventureProjectType.APT_Normal
                    ? UICtrlGameFinish.EShowState.AdvNormalLose
                    : UICtrlGameFinish.EShowState.AdvBonusLose;

            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
            CommitGameResult(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
                },
                code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
                });
        }

        public override void OnGameSuccess()
        {
            UICtrlGameFinish.EShowState showState
                = _adventureLevelInfo.ProjectType == EAdventureProjectType.APT_Normal
                    ? UICtrlGameFinish.EShowState.AdvNormalWin
                    : UICtrlGameFinish.EShowState.AdvBonusWin;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
            CommitGameResult(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState);
                },
                code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog(
                        "游戏成绩提交失败",
                        null,
                        new KeyValuePair<string, Action>("重试",
                            () =>
                            {
                                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameSuccess));
                            }),
                        new KeyValuePair<string, Action>("跳过",
                            () => { SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(showState); })
                    );
                });
        }

        public override bool Restart(Action successCb, Action failedCb)
        {
            if (_adventureLevelInfo.ProjectType != EAdventureProjectType.APT_Bonus
                && !GameATools.CheckEnergy(_adventureLevelInfo.Table.EnergyCost))
            {
                if (successCb != null)
                {
                    successCb.Invoke();
                }
                return true;
            }
            EAdventureProjectType eAPType = _adventureLevelInfo.ProjectType;

            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求中");

            AppData.Instance.AdventureData.PlayAdventureLevel(
                _adventureLevelInfo.Section,
                _adventureLevelInfo.Level,
                eAPType,
                () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    // set local energy data
                    GameATools.LocalUseEnergy(_adventureLevelInfo.Table.EnergyCost);
                    GameRun.Instance.RePlay();
                    OnGameStart();
                    if (successCb != null)
                    {
                        successCb.Invoke();
                    }
                },
                error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    if (failedCb != null)
                    {
                        failedCb.Invoke();
                    }
                }
            );
            return true;
        }

        public override bool HasNext()
        {
            return _adventureLevelInfo.ProjectType != EAdventureProjectType.APT_Bonus
                   && _adventureLevelInfo.Level != ConstDefineGM2D.AdvNormallevelPerChapter;
        }

        public override bool PlayNext(Action successCb, Action failedCb)
        {
            if (!HasNext())
            {
                if (failedCb != null)
                {
                    failedCb.Invoke();
                }
                return false;
            }
            
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求中");

            var section = _adventureLevelInfo.Section;
            var level = _adventureLevelInfo.Level + 1;
            var projectType = EAdventureProjectType.APT_Normal;
            AppData.Instance.AdventureData.PlayAdventureLevel(
                section,
                level,
                projectType,
                () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    // set local energy data
                    GameATools.LocalUseEnergy(_adventureLevelInfo.Table.EnergyCost);
                    var p = AppData.Instance.AdventureData.GetAdvLevelProject(section, projectType, level);
                    p.PrepareRes(() =>
                    {
                        if (successCb != null)
                        {
                            successCb.Invoke();
                        }
                        GameManager.Instance.RequestStopGame();
                        SocialGUIManager.Instance.ChangeToAppMode();
                        CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => {
                            GC.Collect();
                            SituationAdventureParam param = new SituationAdventureParam
                            {
                                Level = level,
                                ProjectType = projectType,
                                Section = section
                            };
                            GameManager.Instance.RequestPlayAdvNormal(p, param);
                            SocialGUIManager.Instance.ChangeToGameMode();
                        }));
                    }, () =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        if (failedCb != null)
                        {
                            failedCb.Invoke();
                        }
                    });
                },
                error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    if (failedCb != null)
                    {
                        failedCb.Invoke();
                    }
                }
            );
            return true;
        }

        private void CommitGameResult(Action successCB, Action<ENetResultCode> failureCB)
        {
            byte[] record;
            Loom.RunAsync(() =>
            {
                record = GetRecord();
                Loom.QueueOnMainThread(() =>
                {
                    AppData.Instance.AdventureData.CommitLevelResult(
                        PlayMode.Instance.SceneState.GameSucceed,
                        PlayMode.Instance.GameSuccessFrameCnt,
                        PlayMode.Instance.SceneState.CurScore,
                        PlayMode.Instance.SceneState.GemGain,
                        PlayMode.Instance.SceneState.MonsterKilled,
                        PlayMode.Instance.SceneState.SecondLeft,
                        PlayMode.Instance.MainPlayer.Life,
                        record,
                        () =>
                        {
                            LogHelper.Info("游戏成绩提交成功");
                            if (null != successCB)
                            {
                                successCB.Invoke();
                            }
                        }, errCode =>
                        {
                            if (null != failureCB)
                            {
                                failureCB.Invoke(errCode);
                            }
                        }
                    );
                });
            });
        }

        private byte[] GetRecord()
        {
            GM2DRecordData recordData = new GM2DRecordData();
            recordData.Version = GM2DGame.Version;
            recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            recordData.Data.AddRange(_inputDatas);
            recordData.BoostItem = new BoostItemData();
            recordData.BoostItem.ExtraLife =
                PlayMode.Instance.IsUsingBoostItem(EBoostItemType.BIT_AddLifeCount1) ? 1 : 0;
//            recordData.BoostItem.InvinsibleOnDead = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_DeadInvincibleCount1) ? 1 : 0;
//            recordData.BoostItem.LongerInvinsible = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_InvincibleTime2) ? 1 : 0;
//            recordData.BoostItem.ScoreBonus = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_ScoreAddPercent20) ? 1 : 0;
//            recordData.BoostItem.TimeBonus = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_TimeAddPercent20) ? 1 : 0;
            recordData.Avatar = new AvatarData();
            if (null == LocalUser.Instance.UsingAvatarData || !LocalUser.Instance.UsingAvatarData.IsInited)
            {
                recordData.Avatar.Appendage = 1;
                recordData.Avatar.Head = 1;
                recordData.Avatar.Lower = 1;
                recordData.Avatar.Upper = 1;
            }
            else
            {
                if (null != LocalUser.Instance.UsingAvatarData.Appendage &&
                    LocalUser.Instance.UsingAvatarData.Appendage.IsInited)
                {
                    recordData.Avatar.Appendage = (int) LocalUser.Instance.UsingAvatarData.Appendage.Id;
                }
                else
                {
                    recordData.Avatar.Appendage = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Head && LocalUser.Instance.UsingAvatarData.Head.IsInited)
                {
                    recordData.Avatar.Head = (int) LocalUser.Instance.UsingAvatarData.Head.Id;
                }
                else
                {
                    recordData.Avatar.Head = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Lower &&
                    LocalUser.Instance.UsingAvatarData.Lower.IsInited)
                {
                    recordData.Avatar.Lower = (int) LocalUser.Instance.UsingAvatarData.Lower.Id;
                }
                else
                {
                    recordData.Avatar.Lower = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Upper &&
                    LocalUser.Instance.UsingAvatarData.Upper.IsInited)
                {
                    recordData.Avatar.Upper = (int) LocalUser.Instance.UsingAvatarData.Upper.Id;
                }
                else
                {
                    recordData.Avatar.Upper = 1;
                }
            }
            byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            if (recordByte == null)
            {
                LogHelper.Error("录像数据出错");
            }
            else
            {
                recordByte = MatrixProjectTools.CompressLZMA(recordByte);
            }
            return recordByte;
        }

        private IEnumerator GameFlow()
        {
//            UICtrlBoostItem uictrlBoostItem = SocialGUIManager.Instance.OpenUI<UICtrlBoostItem>();
//            yield return new WaitUntil(()=>uictrlBoostItem.SelectComplete);
//
//            List<int> useItems = uictrlBoostItem.SelectedItems;
//            PlayMode.Instance.OnBoostItemSelectFinish(useItems);
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
        
        
        public void HandleHandbook(string triggerName, bool active)
        {
            if (!active)
            {
                return;
            }
            if (!triggerName.StartsWith(HandbookEventPrefix))
            {
                return;
            }
            int id;
            if (!int.TryParse(triggerName.Substring(HandbookEventPrefix.Length), out id))
            {
                return;
            }
            if (_handbookShowSet.Contains(id))
            {
                return;
            }
            _handbookShowSet.Add(id);
            SocialGUIManager.Instance.OpenUI<UICtrlInGameUnitHandbook>(id);
        }
    }
}