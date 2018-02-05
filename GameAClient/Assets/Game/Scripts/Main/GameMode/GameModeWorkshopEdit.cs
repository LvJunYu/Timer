using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameA.Game
{
    public class GameModeWorkshopEdit : GameModeEdit
    {
        public override bool SaveShadowData
        {
            get { return true; }
        }

        #region GuideTest

        private AdventureGuideBase _guideBase;
        private const string HandbookEventPrefix = "Book_";
        private readonly HashSet<int> _handbookShowSet = new HashSet<int>();
        private int _section = 1;
        private EAdventureProjectType _projectType = EAdventureProjectType.APT_Normal;
        private int _level = 5;

        #endregion

        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }

            _gameSituation = EGameSituation.World;
            if (Application.isEditor)
            {
                Messenger<string, bool>.AddListener(EMessengerType.OnTrigger, HandleHandbook);
            }

            return true;
        }

        public override bool Stop()
        {
            if (Application.isEditor)
            {
                Messenger<string, bool>.RemoveListener(EMessengerType.OnTrigger, HandleHandbook);
            }

            return base.Stop();
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            GameRun.Instance.ChangeState(ESceneState.Edit);
        }

        public override void OnGameFailed()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlEditTestFinish>(UICtrlEditTestFinish.EShowState.Lose);
        }

        public override void OnGameSuccess()
        {
            byte[] record = GetRecord(true);
            RecordBytes = record;

            SocialGUIManager.Instance.OpenUI<UICtrlEditTestFinish>(UICtrlEditTestFinish.EShowState.Win);
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
//            if (string.IsNullOrEmpty(_project.Name))
//            {
//                _project.Name = DateTimeUtil.GetServerTimeNow().ToString("yyyyMMddHHmmss");
//            }
            if (NeedSave || ELocalDataState.LDS_UnCreated == _project.LocalDataState)
            {
                Save(
                    () =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().Close();
                        if (null != successCB)
                        {
                            successCB.Invoke();
                        }

                        SocialApp.Instance.ReturnToApp();
                    }, result =>
                    {
                        LogHelper.Error("Save private project failed {0}", result);
                        if (null != failureCB)
                        {
                            failureCB.Invoke((int) result);
                        }

                        SocialGUIManager.ShowPopupDialog("关卡保存失败，是否放弃修改退出", null,
                            new KeyValuePair<string, Action>("取消", () => { }),
                            new KeyValuePair<string, Action>("确定", () => SocialApp.Instance.ReturnToApp()));
                    });
            }
            else
            {
                if (null != successCB)
                {
                    successCB.Invoke();
                }

                SocialApp.Instance.ReturnToApp();
            }
        }

        public override void Save(Action successCallback = null, Action<EProjectOperateResult> failedCallback = null)
        {
            if (!NeedSave)
            {
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }

                return;
            }

            if (_mode == EMode.EditTest)
            {
                ChangeMode(EMode.Edit);
            }

            if (!EditMode.Instance.IsInState(EditModeState.Add.Instance))
            {
                EditMode.Instance.StartAdd();
            }

            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
            {
                GetCaptureIconBtyes();
                Loom.RunAsync(() =>
                {
                    byte[] mapDataBytes = MapManager.Instance.SaveMapData();
                    mapDataBytes = MatrixProjectTools.CompressLZMA(mapDataBytes);
                    Loom.QueueOnMainThread(() =>
                    {
                        if (mapDataBytes == null
                            || mapDataBytes.Length == 0)
                        {
                            if (failedCallback != null)
                            {
                                failedCallback.Invoke(EProjectOperateResult.POR_Error);
                            }

                            return;
                        }

                        bool passFlag = CheckCanPublish();

                        _project.Save(
                            _project.Name,
                            _project.Summary,
                            mapDataBytes,
                            IconBytes,
                            passFlag,
                            true,
                            _recordUsedTime,
                            _recordScore,
                            _recordScoreItemCount,
                            _recordKillMonsterCount,
                            _recordLeftTime,
                            _recordLeftLife,
                            RecordBytes,
                            EditMode.Instance.MapStatistics.TimeLimit,
                            EditMode.Instance.MapStatistics.MsgWinCondition,
                            _project.IsMulti,
                            null,
                            () =>
                            {
                                NeedSave = false;
                                MapDirty = false;
                                if (successCallback != null)
                                {
                                    successCallback.Invoke();
                                }
                            }, failedCallback);
                    });
                });
            }));
        }

        public override void ChangeMode(EMode mode)
        {
            if (Application.isEditor)
            {
                if (_guideBase != null)
                {
                    _guideBase.Dispose();
                    _guideBase = null;
                }

                _handbookShowSet.Clear();
            }

            base.ChangeMode(mode);
        }

        protected override void EnterEditTest()
        {
            base.EnterEditTest();
            if (Application.isEditor)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameInput>();
                AdventureGuideManager.Instance.TryGetGuide(_section, _projectType, _level, out _guideBase);
                if (_guideBase != null)
                {
                    _guideBase.Init();
                }
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (Application.isEditor)
            {
                if (_mode == EMode.EditTest)
                {
                    if (_guideBase != null)
                    {
                        _guideBase.UpdateLogic();
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (Application.isEditor)
            {
                if (_mode == EMode.EditTest)
                {
                    if (_guideBase != null)
                    {
                        _guideBase.Update();
                    }
                }
            }
        }

        private byte[] GetRecord(bool win)
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