using System;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;

namespace GameA.Game
{
    public class GameModeWorkshopEdit : GameModeEdit
    {
	    #if WORKSHOPGUIDE
	    private AdventureGuideBase _guideBase;
	    private int _section = 1;
	    private EAdventureProjectType _projectType = EAdventureProjectType.APT_Normal;
	    private int _level = 1;
		#endif
	    
	    
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
				_project.Name = "我的大作";
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
			if (EditMode.Instance.IsInState(EditModeState.Add.Instance))
			{
				EditMode.Instance.StartAdd();
			}
			EditMode.Instance.ChangeEditorLayer(EEditorLayer.Capture);
			byte[] mapDataBytes = MapManager.Instance.SaveMapData();
			mapDataBytes = MatrixProjectTools.CompressLZMA(mapDataBytes);
			IconBytes = CaptureLevel();
			EditMode.Instance.RevertEditorLayer();
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

	    public override void ChangeMode(EMode mode)
	    {
		    base.ChangeMode(mode);
		    
#if WORKSHOPGUIDE
		    if (_guideBase != null)
		    {
			    _guideBase.Dispose();
			    _guideBase = null;
		    }
		    if (mode == EMode.EditTest)
		    {
			    SocialGUIManager.Instance.OpenUI<UICtrlMobileInputControl>();
			    AdventureGuideManager.Instance.TryGetGuide(_section, _projectType, _level, out _guideBase);
			    if (_guideBase != null)
			    {
				    _guideBase.Init();
			    }
		    }
#endif
	    }

	    public override void UpdateLogic()
	    {
		    base.UpdateLogic();
		    
#if WORKSHOPGUIDE
		    if (_mode == EMode.EditTest)
		    {
			    if (_guideBase != null)
			    {
				    _guideBase.UpdateLogic();
			    }
		    }
#endif
	    }
	    
	    public override void Update()
	    {
		    base.Update();
		    
#if WORKSHOPGUIDE
		    if (_mode == EMode.EditTest)
		    {
			    if (_guideBase != null)
			    {
				    _guideBase.Update();
			    }
		    }
#endif
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
