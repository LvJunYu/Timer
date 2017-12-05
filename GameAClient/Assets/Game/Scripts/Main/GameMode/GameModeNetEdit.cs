using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameA.Game
{
    public class GameModeNetEdit : GameModeEdit
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameSituation = EGameSituation.Net;
            return true;
        }

        public void SetNetBattleType(ENetBattleType eNetBattleType)
        {
            _project.NetData.NetBattleType = eNetBattleType;
        }
        
        public override void OnGameStart()
        {
            base.OnGameStart();
            GameRun.Instance.ChangeState(ESceneState.Edit);
        }

        public override void OnGameFailed()
        {
        }

        public override void OnGameSuccess()
        {
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            if (string.IsNullOrEmpty(_project.Name))
            {
                _project.Name = DateTimeUtil.GetServerTimeNow().ToString("yyyyMMddHHmmss");
            }
            if (NeedSave || ELocalDataState.LDS_UnCreated == _project.LocalDataState)
            {
                Save(() =>
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
            EditMode.Instance.ChangeEditorLayer(EEditorLayer.Capture);
            IconBytes = CaptureLevel();
            EditMode.Instance.RevertEditorLayer();
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
        }
    }
}