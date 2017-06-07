using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA.Game
{
    public class GameModeWorkshopEdit : GameModeEdit
    {
        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
            }
            _gameSituation = EGameSituation.World;
            return true;
		}

        public override void OnGameFailed()
		{
            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.EditorLose);
        }

        public override void OnGameSuccess()
		{
            byte[] record = GetRecord();
            float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;
            RecordBytes = record;
            RecordUsedTime = usedTime;

            SocialGUIManager.Instance.OpenUI<UICtrlGameFinish>(UICtrlGameFinish.EShowState.EditorWin);
		}

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
		{
			//if (GM2DGame.Instance.GameInitType == GameManager.EStartType.Create
			//|| GM2DGame.Instance.GameInitType == GameManager.EStartType.Edit) {
			//    if (NeedSave) {
			//        SocialGUIManager.ShowPopupDialog ("关卡做出的修改还未保存，是否退出", null,
			//            new KeyValuePair<string, Action> ("取消", () => {
			//            }),
			//            new KeyValuePair<string, Action> ("保存", () => {
			//                if (CurrentMode == EMode.EditTest) {
			//                    Messenger<ECommandType>.Broadcast (EMessengerType.OnCommandChanged, ECommandType.Pause);
			//                    ChangeToMode (EMode.Edit);
			//                }
			//            if (_project.LocalDataState == ELocalDataState.LDS_UnCreated) {
			//                    CoroutineProxy.Instance.StartCoroutine (CoroutineProxy.RunNextFrame (() => {
			//                        SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
			//                        SocialGUIManager.Instance.OpenUI<UICtrlPublish> ();
			//                    }));
			//                } else {
			//                    UICtrlPublish.SaveProject (_project.Name, _project.Summary,
			//                        _project.DownloadPrice, _project.PublishRecordFlag, () => {
			//                            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
			//                            SocialApp.Instance.ReturnToApp ();
			//                        }, () => {
			//                            //保存失败
			//                        });
			//                }
			//            }),
			//            new System.Collections.Generic.KeyValuePair<string, Action> ("退出", () => {
			//                SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
			//                SocialApp.Instance.ReturnToApp ();
			//            }));
			//        return;
			//    }
			//} else if (GM2DGame.Instance.GameInitType == GameManager.EStartType.ModifyEdit) {
			//    if (_needSave) {
			//        // 保存改造关卡
			//        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在保存改造关卡");
			//        GM2DGame.Instance.SaveModifyProject (() => {
			//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
			//            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
			//            SocialApp.Instance.ReturnToApp ();
			//        },
			//            code => {
			//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
			//                SocialGUIManager.Instance.CloseUI<UICtrlGameSetting> ();
			//                SocialApp.Instance.ReturnToApp ();
			//                // todo error handle
			//                LogHelper.Error ("SaveModifyProject failed");
			//            }
			//        );
			//        return;
			//    }
			//}

			if (ELocalDataState.LDS_UnCreated == _project.LocalDataState)
			{
				_project.Name = "我的酱油大作";
				_project.Summary = "这个家伙没写简介";
				_project.DownloadPrice = 1;
			}

			if (NeedSave || ELocalDataState.LDS_UnCreated == _project.LocalDataState)
			{
				Save(
					() =>
					{
						if (null != successCB)
						{
							successCB.Invoke();
						}
						SocialApp.Instance.ReturnToApp();
					}, result =>
					{
							//保存失败

							LogHelper.Error("Save private projcet failed {0}", result);
						if (null != failureCB)
						{
							failureCB.Invoke((int)result);
						}
						SocialApp.Instance.ReturnToApp();
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
				0,
				passFlag,
				RecordUsedTime,
				RecordBytes,
				EditMode.Instance.MapStatistics.TimeLimit,
				EditMode.Instance.MapStatistics.MsgWinCondition,
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
            recordData.Data.AddRange(PlayMode.Instance.InputDatas);
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
