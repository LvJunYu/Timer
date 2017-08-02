using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameA.Game
{
    public class GameModeWorkshopEdit : GameModeEdit
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
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
            GameRun.Instance.ChangeState(ESceneState.Edit);
        }

        public override void OnGameFailed()
		{
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.EditorLose);
        }

        public override void OnGameSuccess()
		{
            byte[] record = GetRecord();
            RecordBytes = record;
			
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.EditorWin);
		}

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
		{
			if (ELocalDataState.LDS_UnCreated == _project.LocalDataState)
			{
				_project.Name = "我的酱油大作";
				_project.Summary = "这个家伙没写简介";
			}

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
							failureCB.Invoke((int)result);
						}
                        SocialGUIManager.ShowPopupDialog("关卡保存失败，是否放弃修改退出", null,
                            new KeyValuePair<string, Action>("取消", ()=>{}),
                            new KeyValuePair<string, Action>("确定", ()=>SocialApp.Instance.ReturnToApp()));
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
			byte[] mapDataBytes = MapManager.Instance.SaveMapData();
			mapDataBytes = MatrixProjectTools.CompressLZMA(mapDataBytes);
			if (IconBytes == null)
			{
				IconBytes = CaptureLevel();
			}
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
				EditMode2.Instance.MapStatistics.TimeLimit,
				EditMode2.Instance.MapStatistics.MsgWinCondition,
				() =>
				{
					NeedSave = false;
					MapDirty = false;
					if (successCallback != null)
					{
						successCallback.Invoke();
					}
				}, failedCallback);
        }

        private byte[] GetRecord()
        {
            GM2DRecordData recordData = new GM2DRecordData();
            recordData.Version = GM2DGame.Version;
            recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            recordData.Data.AddRange(_inputDatas);
            byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            byte[] record = null;
            if(recordByte == null)
            {
                LogHelper.Error("录像数据出错");
            }
            else
            {
                record = MatrixProjectTools.CompressLZMA(recordByte);
            }
            return record;
        }
    }
}
