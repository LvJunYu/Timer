using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace GameA.Game
{
    public class GameModeNetEdit : GameModeEdit
    {
        public override bool IsMulti
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
            _gameSituation = EGameSituation.NetBattle;
            return true;
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            GameRun.Instance.ChangeState(ESceneState.Edit);
        }

        public override void OnGameFailed()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlEditTestFinish>(UICtrlEditTestFinish.EShowState.MultiLose);
        }

        public override void OnGameSuccess()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlEditTestFinish>(UICtrlEditTestFinish.EShowState.MultiWin);
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

                    if (EditMode.Instance.MapStatistics.NetBattleData != null)
                        _project.Save(
                            _project.Name,
                            _project.Summary,
                            mapDataBytes,
                            IconBytes,
                            passFlag,
                            true,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            null,
                            0,
                            0,
                            true,
                            EditMode.Instance.MapStatistics.NetBattleData,
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

        public override bool CheckCanPublish(bool showPrompt = false)
        {
            return true;
        }
    }
}