/********************************************************************
** Filename : GM2DGame
** Author : Dong
** Date : 2015/5/6 15:27:36
** Summary : GM2DGame
 *从App任何点进去游戏、关卡的时候，都是直接显示详情界面。
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections;
using SoyEngine;
using System;
using UnityEngine.UI;

namespace GameA.Game
{       

    public enum EMode
    {
        None,
		// 正常编辑
        Edit,
		// 编辑时测试
        EditTest,
		// 正常游戏
        Play,
		// 播放录像
        PlayRecord,
		// 改造编辑
		ModifyEdit,
        // 冒险模式普通关卡
        AdvNormal,
        // 冒险模式奖励关卡
        AdvBonus,
    }

    public class GM2DGame : GameBase
    {
        public static GM2DGame Instance;
        /// <summary>
        /// 创作工具程序版本号 新做地图无法用老程序打开时变化 比如新加物品类型
        /// </summary>
        public const int Version = 4;
        /// <summary>
        /// 地图数据版本号 地图数据含义改变时变化 比如原来记录碰撞体区域，现在改为数据区域
        /// </summary>
        public const int MapVersion = 1;
	    public const string GameName = "GameMaker2D";
        private EMode _mode = EMode.None;
        private GameObject _inputControl;
        private bool _resDone;

        private byte[] _iconBytes;
        private byte[] _recordBytes;
        private float _recordUsedTime;
        private bool _needSave;
        private Table_Matrix _tableMatrix;

        /// <summary>
        /// 当前正在播放的录像数据，只有当游戏模式是录像时才有意义
        /// </summary>
        private GM2DRecordData _gm2drecordData;


	    private GameSettingData _settings;

	    private bool _runInApp = false;

	    public bool RunInApp
	    {
		    set { _runInApp = value; }
	    }

	    public GameSettingData Settings
	    {
		    get { return _settings; }
	    }

        public Table_Matrix TableMatrix
        {
            get
            {
                return _tableMatrix;
            }
        }

        public EMode CurrentMode
        {
            get { return _mode; }
        }

        public override ScreenOrientation ScreenOrientation
        {
            get { return ScreenOrientation.LandscapeLeft; }
        }

        public byte[] IconBytes
        {
            get
            {
                if(_iconBytes == null)
                {
                    _iconBytes = LocalCacheManager.Instance.Load(LocalCacheManager.EType.Image, _project.IconPath);
                }
                return this._iconBytes;
            }
            set { _iconBytes = value; }
        }

        public byte[] RecordBytes
        {
            get
            {
                if(MapDirty)
                {
                    _recordBytes = null;
                    _recordUsedTime = 0;
                    _needSave = true;
                }
                return _recordBytes;
            }
            set
            {
                _recordBytes = value;
                if(_recordBytes != null && MapDirty)
                {
                    NeedSave = true;
                    MapDirty = false;
                }
            }
        }

        public float RecordUsedTime
        {
            get { return _recordUsedTime; }
            set { _recordUsedTime = value; }
        }

        public bool NeedSave
        {
            get
            {
                if(MapDirty)
                {
                    _needSave = true;
                }
                return _needSave;
            }
            set
            {
                _needSave = value;
            }
        }

        public bool MapDirty
        {
            get
            {
                if(EditMode.Instance == null
                    || EditMode.Instance.MapStatistics == null)
                {
                    return false;
                }
                return EditMode.Instance.MapStatistics.NeedSave;
            }
            set
            {
                if(EditMode.Instance != null
                    && EditMode.Instance.MapStatistics != null)
                {
                    EditMode.Instance.MapStatistics.NeedSave = value;
                }
            }
        }

	    public int GameScreenWidth
	    {
		    get
		    {
				if (SocialGUIManager.Instance.RunRecordInApp && !SocialGUIManager.Instance.RecordFullScreen)
			    {
				    return Screen.height;
			    }
			    return Screen.width;
		    }
	    }

	    public int GameScreenHeight
	    {
		    get
		    {
				if (SocialGUIManager.Instance.RunRecordInApp && !SocialGUIManager.Instance.RecordFullScreen)
				{
					return Screen.width;
				}
				return Screen.height;
		    }
	    }

        #region 方法

	    private void Awake()
	    {
			_settings = new GameSettingData();
		}

		
        public GM2DGame():base()
        {
            Messenger.AddListener(EMessengerType.OnGameLoadError, OnGameLoadError);
			Messenger.AddListener (EMessengerType.GameFinishSuccess, OnGameFinishSuccess);
			Messenger.AddListener (EMessengerType.GameFinishFailed, OnGameFinishFailed);
        }

        public override bool Play(Project project)
        {
            _project = project;
            return Init(GameManager.EStartType.Play);
        }
        public override bool PlayAdvNormal (Project project)
        {
            _project = project;
            return Init (GameManager.EStartType.AdventureNormal);
        }
        public override bool PlayAdvBonus (Project project)
        {
            _project = project;
            return Init (GameManager.EStartType.AdventureBonus);
        }
        public override bool Create(Project project)
        {
            _project = project;
            return Init(GameManager.EStartType.Create);
        }

        public override bool Edit(Project project)
        {
            _project = project;
            return Init(GameManager.EStartType.Edit);
        }

		public override bool ModifyEdit (Project project) {
			_project = project;
			return Init (GameManager.EStartType.ModifyEdit);
		}

        public override bool PlayRecord(Project project, Record record)
        {
            _project = project;
            _record = record;
            return Init(GameManager.EStartType.PlayRecord);
        }

		public override bool Pause()
		{
			PlayMode.Instance.Pause();
			return true;
		}

		public override bool Continue()
		{
			PlayMode.Instance.Continue();
			return true;
		}

		public override bool Stop()
        {
            StopAllCoroutines();
            if (MapManager.Instance != null)
            {
                MapManager.Instance.Stop();
            }
            Destroy(_inputControl);
			LocaleManager.Instance.ExitGame();
			return true;
        }

	    public override float GetLogicTimeFromGameStart()
	    {
		    if (PlayMode.Instance == null)
		    {
			    return 0;
		    }
		    return PlayMode.Instance.SceneState.PassedTime;
	    }

	    public override int GetLogicFrameCountFromGameStart()
	    {
			if (PlayMode.Instance == null)
			{
				return 0;
			}
			return PlayMode.Instance.LogicFrameCnt;
		}

	    public override bool Restart()
	    {
			PlayMode.Instance.RePlay();
			return true;
	    }

        public override void QuitGame (Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
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
            if (EProjectStatus.PS_Challenge == _project.ProjectStatus)
            {
                if (!LocalUser.Instance.MatchUserData.ChallengeResultCommitted) {
                    CommitChallengeGameResult (
                        () => {
                            if (null != successCB) {
                                successCB.Invoke ();
                            }
                            SocialApp.Instance.ReturnToApp ();
                        },
                        code => {
                            if (null != failureCB) {
                                failureCB.Invoke ((int)code);
                            }
                            SocialApp.Instance.ReturnToApp ();
                        }
                    );
                } else {
                    if (null != successCB) {
                        successCB.Invoke ();
                    }
                    SocialApp.Instance.ReturnToApp ();
                }
            } else if (EProjectStatus.PS_AdvNormal == _project.ProjectStatus)
            {
                CommitAdventureGameResult (
                    () => {
                        if (null != successCB) {
                            successCB.Invoke ();
                        }
                        SocialApp.Instance.ReturnToApp ();
                    },
                    code => {
                        if (null != failureCB) {
                            failureCB.Invoke ((int)code);
                        }
                        SocialApp.Instance.ReturnToApp ();
                    }
                );
            } else if (EProjectStatus.PS_AdvBonus == _project.ProjectStatus)
            {
                CommitAdventureGameResult (
                    () => {
                        if (null != successCB) {
                            successCB.Invoke ();
                        }
                        SocialApp.Instance.ReturnToApp ();
                    },
                    code => {
                        if (null != failureCB) {
                            failureCB.Invoke ((int)code);
                        }
                        SocialApp.Instance.ReturnToApp ();
                    }
                );
            } else if (EProjectStatus.PS_Reform == _project.ProjectStatus)
            {
                if (NeedSave) {
                    // 保存改造关卡
                    //SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在保存改造关卡");
                    SaveModifyProject (() => {
                            //SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
	                        if (null != successCB)
	                        {
	                            successCB.Invoke ();
	                        }
	                        SocialApp.Instance.ReturnToApp ();
	                    },
                        code => {
                            //SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                            if (null != failureCB) {
                                failureCB.Invoke ((int)code);
                            }
                            SocialApp.Instance.ReturnToApp ();
                            // todo error handle
                            LogHelper.Error ("SaveModifyProject failed");
                        }
                    );
                    return;
                }
            } else if (EProjectStatus.PS_Public == _project.ProjectStatus)
            {
                SocialApp.Instance.ReturnToApp ();
            } else if (EProjectStatus.PS_Private == _project.ProjectStatus)
            {
                if (ELocalDataState.LDS_UnCreated == _project.LocalDataState) {
                    _project.Name = "我的酱油大作";
                    _project.Summary = "这个家伙没写简介";
                    _project.DownloadPrice = 1;
                }
                if (NeedSave || ELocalDataState.LDS_UnCreated == _project.LocalDataState) {
                    Save (
                        _project.Name, 
                        _project.Summary,
                        _project.DownloadPrice,
                        () => {
                            if (null != successCB) {
                                successCB.Invoke ();
                            }
                            SocialApp.Instance.ReturnToApp ();
                        }, result => {
                        //保存失败

                        LogHelper.Error ("Save private projcet failed {0}", result);
                        if (null != failureCB) {
                            failureCB.Invoke ((int)result);
                        }
                        SocialApp.Instance.ReturnToApp ();
                    });
                } else {
                    if (null != successCB) {
                        successCB.Invoke ();
                    }
                    SocialApp.Instance.ReturnToApp ();    
                }
            } else
            {
                SocialApp.Instance.ReturnToApp ();
            }
        }

	    public bool Init(GameManager.EStartType eGameInitType)
        {
			LogHelper.Debug("GM2DGame Init " + eGameInitType);
            Instance = this;
            _eGameInitType = eGameInitType;
//	        if (!SocialGUIManager.Instance.RunRecordInApp)
//	        {
//				SocialGUIManager.Instance.GetUI<UICtrlScreenRotate>().ChangeScreenOrientation(ScreenOrientation.Landscape);
//			}
            StartCoroutine(InitMario());
            return true;
        }

        private IEnumerator InitMario()
        {
			if (_eGameInitType == GameManager.EStartType.Play) {
				_mode = EMode.Play;
			} else if (_eGameInitType == GameManager.EStartType.PlayRecord) {
				_mode = EMode.PlayRecord;
			} else if (_eGameInitType == GameManager.EStartType.ModifyEdit) {
				_mode = EMode.ModifyEdit;
			}
            else if (_eGameInitType == GameManager.EStartType.AdventureNormal) {
                _mode = EMode.AdvNormal;
            } else if (_eGameInitType == GameManager.EStartType.AdventureBonus) {
                _mode = EMode.AdvBonus;
            } else 
            {
                _mode = EMode.Edit;
            }
            yield return new WaitForSeconds(0.5f);
            _resDone = false;
            LocalResourceManager.Instance.DoUpdateGame("GameMaker2D", OnUpdateSuccess, OnProcess, OnUpdateFailed);
            while (!_resDone)
            {
                yield return new WaitForSeconds(0.1f);
            }
            EnvManager.Instance.Init();
            LocaleManager.Instance.EnterGame();
            //GameResourceManager rm = gameObject.AddComponent<GameResourceManager>();
            //if (!rm.Init(GameName))
            //{
            //    LogHelper.Error("GameResourceManager initFailed");
            //}
            //gameObject.AddComponent<GameLocaleManager>();
            gameObject.AddComponent<GameParticleManager>();
            gameObject.AddComponent<GameAudioManager>();


            if (_project != null)
            {
                _tableMatrix = TableManager.Instance.GetMatrix(1);
            }
            gameObject.AddComponent<UnitManager>();
            UnitManager.Instance.Init();
            gameObject.AddComponent<CameraManager>();
            var playObject = new GameObject("PlayMode");
            playObject.AddComponent<PlayMode>();
            DeadMarkManager.Instance.Init();
            if(_eGameInitType == GameManager.EStartType.PlayRecord)
            {
                byte[] recordBytes = MatrixProjectTools.DecompressLZMA(_record.RecordData);
                if(recordBytes == null)
                {
                    OnGameLoadError("录像解析失败");
                    yield break;
                }
                _gm2drecordData = GameMapDataSerializer.Instance.Deserialize<GM2DRecordData>(recordBytes);
                if(_gm2drecordData == null)
                {
                    OnGameLoadError("录像解析失败");
                    yield break;
                }
                PlayMode.Instance.ERunMode = ERunMode.Record;
                PlayMode.Instance.InputDatas = _gm2drecordData.Data;
            }

            yield return null;
            gameObject.AddComponent<InputManager>();
            //gameObject.AddComponent<GM2DGUIManager>();
            gameObject.AddComponent<MapManager>();
            MapManager.Instance.Init(_eGameInitType, _project);
            yield return null;
            while (!MapManager.Instance.GenerateMapComplete)
            {
				Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, 0.8f + MapManager.Instance.MapProcess * 0.2f);
                yield return new WaitForSeconds(0.2f);
            }
            //LoadGameInputController();
			Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess,1f);
			if (_eGameInitType == GameManager.EStartType.Play) {
				ChangeToMode (EMode.Play);
			} else if (_eGameInitType == GameManager.EStartType.PlayRecord) {
				ChangeToMode (EMode.PlayRecord);
			} else if (_eGameInitType == GameManager.EStartType.Edit) {
				ChangeToMode (EMode.Edit);
			} else if (_eGameInitType == GameManager.EStartType.ModifyEdit) {
				ChangeToMode (EMode.ModifyEdit);
			} else if (_eGameInitType == GameManager.EStartType.Create) {
				ChangeToMode (EMode.Edit);
			}
	        if (SocialGUIManager.Instance.RunRecordInApp)
	        {
				var tex = SocialGUIManager.Instance.RenderRecordTexture;
		        //GM2DGUIManager.Instance.RenderUICamera.targetTexture = tex;
				CameraManager.Instance.RendererCamera.targetTexture = tex;
	        }
	        
			SetMainPlayer ();
            Messenger.Broadcast(GameA.EMessengerType.OnGameStartComplete);
        }

		/// <summary>
		/// 设置主角的数据和模型
		/// </summary>
		private void SetMainPlayer() {
			MainUnit mainPlayer = PlayMode.Instance.MainUnit;
			if (mainPlayer == null)
				return;
			// todo set data

			ChangePartsSpineView view = mainPlayer.View as ChangePartsSpineView;
			if (view == null)
				return;
            int headId = 1;
            int upperId = 1;
            int lowerId = 1;
            int appendageId = 1;

            if (EMode.PlayRecord == _mode)
            {
                if (_gm2drecordData != null && _gm2drecordData.Avatar != null) {
                    headId = _gm2drecordData.Avatar.Head;
                    upperId = _gm2drecordData.Avatar.Upper;
                    lowerId = _gm2drecordData.Avatar.Lower;
                    appendageId = _gm2drecordData.Avatar.Appendage;
                }
            } else
            {
                if (LocalUser.Instance.UsingAvatarData.IsInited) {
                    if (LocalUser.Instance.UsingAvatarData.Head != null &&
                        LocalUser.Instance.UsingAvatarData.Head.IsInited) {
                        headId = (int)LocalUser.Instance.UsingAvatarData.Head.Id;
                    }
                    if (LocalUser.Instance.UsingAvatarData.Upper != null &&
                        LocalUser.Instance.UsingAvatarData.Upper.IsInited) {
                        upperId = (int)LocalUser.Instance.UsingAvatarData.Upper.Id;
                    }
                    if (LocalUser.Instance.UsingAvatarData.Lower != null &&
                        LocalUser.Instance.UsingAvatarData.Lower.IsInited) {
                        lowerId = (int)LocalUser.Instance.UsingAvatarData.Lower.Id;
                    }
                    if (LocalUser.Instance.UsingAvatarData.Appendage != null &&
                        LocalUser.Instance.UsingAvatarData.Appendage.IsInited) {
                        appendageId = (int)LocalUser.Instance.UsingAvatarData.Appendage.Id;
                    }
                }
            }
            //			view.SetParts (2, SpinePartsDefine.ESpineParts.Head);
            if (LocalUser.Instance.UsingAvatarData.Head != null) {
                view.SetParts (headId, SpinePartsHelper.ESpineParts.Head);
            }
            if (LocalUser.Instance.UsingAvatarData.Upper != null) {
                view.SetParts (upperId, SpinePartsHelper.ESpineParts.Upper);
            }
            if (LocalUser.Instance.UsingAvatarData.Lower != null) {
                view.SetParts (lowerId, SpinePartsHelper.ESpineParts.Lower);
            }
            if (LocalUser.Instance.UsingAvatarData.Appendage != null) {
                view.SetParts (appendageId, SpinePartsHelper.ESpineParts.Appendage);
            }
		}

        public void ChangeToMode(EMode mode)
        {
            _mode = mode;
			if (mode == EMode.EditTest) {
                SocialGUIManager.Instance.CloseUI<UICtrlItem> ();
				SocialGUIManager.Instance.CloseUI<UICtrlCreate> ();
				SocialGUIManager.Instance.OpenUI<UICtrlEdit> ();
				SocialGUIManager.Instance.GetUI<UICtrlEdit> ().ChangeToEditTestMode ();
				SocialGUIManager.Instance.CloseUI<UICtrlScreenOperator> ();
				SocialGUIManager.Instance.OpenUI<UICtrlSceneState> ();
                SocialGUIManager.Instance.CloseUI<UICtrlModifyEdit> ();
				InputManager.Instance.ShowGameInput ();
			} else if (mode == EMode.Edit) {
				SocialGUIManager.Instance.OpenUI<UICtrlCreate> ();
				SocialGUIManager.Instance.OpenUI<UICtrlScreenOperator> ();
				SocialGUIManager.Instance.OpenUI<UICtrlEdit> ();
				SocialGUIManager.Instance.GetUI<UICtrlEdit> ().ChangeToEditMode ();
				SocialGUIManager.Instance.CloseUI<UICtrlSceneState> ();
				InputManager.Instance.HideGameInput ();
			} else if (mode == EMode.Play) {
				SocialGUIManager.Instance.OpenUI<UICtrlEdit> ().ChangeToPlayMode ();
				SocialGUIManager.Instance.CloseUI<UICtrlCreate> ();
				SocialGUIManager.Instance.CloseUI<UICtrlScreenOperator> ();
				SocialGUIManager.Instance.OpenUI<UICtrlSceneState> ();
				InputManager.Instance.ShowGameInput ();
			} else if (mode == EMode.PlayRecord) {
				SocialGUIManager.Instance.OpenUI<UICtrlEdit> ().ChangeToPlayRecordMode ();
				SocialGUIManager.Instance.CloseUI<UICtrlCreate> ();
				SocialGUIManager.Instance.CloseUI<UICtrlScreenOperator> ();
				SocialGUIManager.Instance.OpenUI<UICtrlSceneState> ();
				InputManager.Instance.HideGameInput ();
			} else if (mode == EMode.ModifyEdit) {
				SocialGUIManager.Instance.OpenUI<UICtrlEdit> ().ChangeToModifyMode ();
				SocialGUIManager.Instance.CloseUI<UICtrlSceneState> ();
                SocialGUIManager.Instance.OpenUI<UICtrlModifyEdit> ();
				InputManager.Instance.HideGameInput ();
			}
        }

        public void Save(string name, string summary, int downloadPrice,
            Action successCallback = null, Action<EProjectOperateResult> failedCallback = null)
        {
            if(name == null)
            {
                name = _project.Name;
            }
            if(summary == null)
            {
                summary = _project.Summary;
            }
            byte[] mapDataBytes = MapManager.Instance.SaveMapData();
            mapDataBytes = MatrixProjectTools.CompressLZMA(mapDataBytes);
            if(IconBytes == null)
            {
                IconBytes = CaptureLevel();
            }
            if (name == null
                || mapDataBytes == null
                || mapDataBytes.Length == 0)
            {
                if(failedCallback != null)
                {
                    failedCallback.Invoke(EProjectOperateResult.POR_Error);
                }
                return;
            }
            bool passFlag = CheckCanPublish();

            _project.Save(
                name, 
                summary,
                mapDataBytes,
                IconBytes,
                downloadPrice,
                passFlag, 
                RecordUsedTime, 
                RecordBytes,
                EditMode.Instance.MapStatistics.TimeLimit,
                EditMode.Instance.MapStatistics.MsgWinCondition,
                ()=>{
                    NeedSave = false;
                    MapDirty = false;
                    if(successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }, failedCallback);
        }

        /// <summary>
        /// 保存改造关卡
        /// </summary>
        public void SaveModifyProject (Action successCallback = null, Action<EProjectOperateResult> failedCallback = null) {
            byte[] mapDataBytes = MapManager.Instance.SaveMapData();
            byte[] compressedBytes = MatrixProjectTools.CompressLZMA (mapDataBytes);
            //            _project.SetBytesData (mapDataBytes);
            //Debug.Log ("_________________________3 " + _project.ProjectId + " " + _project.PassFlag);
            _project.SaveModifyProject (compressedBytes, successCallback, failedCallback);
        }

        public bool CheckCanPublish(bool showPrompt = false)
        {
            if (RecordBytes == null && (NeedSave || !_project.PassFlag))
            {
                if (showPrompt)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameErrorLog,
                        LocaleManager.GameLocale("ui_publish_failed_finish_first"));
                }
                return false;
            }
            return true;
        }

        public void UseBoostItem (List<int> itemTypes)
        {
            if (null == _project) return;
            long token = 0;
            if (EProjectStatus.PS_Challenge == _project.ProjectStatus)
            {
                token = LocalUser.Instance.MatchUserData.PlayChallengeToken;
            } else if (EProjectStatus.PS_AdvNormal == _project.ProjectStatus)
            {
                token = AppData.Instance.AdventureData.LastRequiredLevelToken;
            }

            if (0 != token)
            {
                RemoteCommands.UseProps (
                    token,
                    itemTypes,
                    msg =>
                    {
                        if ((int)EUsePropsCode.UPC_Success == msg.ResultCode)
                        {
                            LogHelper.Info ("Use boost item success");
                        } else
                        {
                            // todo error handle
                            // 失败了应该终止关卡退回主菜单重新请求一遍数据
                            LogHelper.Error ("Use boost item failed");
                        }
                    },
                    code =>
                    {
                        // todo error handle
                        LogHelper.Error ("Use boost item failed");
                    }
                );
            }
        }

        public byte[] CaptureLevel()
        {
            const int ImageWidth = 960;
            const int ImageHeight = 640;
            Vector2 captureScreenSize = Vector2.zero;
            Rect captureRect = new Rect();
            captureRect.height = ImageHeight;
            captureRect.width = ImageWidth;
            captureScreenSize.Set(Mathf.CeilToInt(1f*GameScreenWidth/GameScreenHeight*ImageHeight), ImageHeight);
            captureRect.y = 0;
            captureRect.x = (captureScreenSize.x - ImageWidth) * 0.5f;
            Texture2D t2 = ClientTools.CaptureCamera(CameraManager.Instance.RendererCamera, captureScreenSize, captureRect);
            return t2.EncodeToJPG(90);
        }

        private void OnDestroy()
        {
			Messenger.RemoveListener (EMessengerType.OnGameLoadError, OnGameLoadError);
			Messenger.RemoveListener (EMessengerType.GameFinishSuccess, OnGameFinishSuccess);
			Messenger.RemoveListener (EMessengerType.GameFinishFailed, OnGameFinishFailed);
            Instance = null;
        }

        private void LoadGameInputController()
        {
            //#if UNITY_EDITOR
            //UnityEngine.Object obj = Resources.Load("EasyTouchControlsCanvas");
            //            if (obj == null) {
            //                obj = GameResourceManager.Instance.LoadMainAssetObject ("EasyTouchControlsCanvas");
            //            }
            //            _inputControl = CommonTools.InstantiateObject (obj);
            //#else
            //            var obj = GameResourceManager.Instance.LoadMainAssetObject("EasyTouchControlsCanvas");
            //            _inputControl = CommonTools.InstantiateObject(obj);
            //#endif
            //         var parent = SocialGUIManager.Instance.GetFirstGroupParent()as RectTransform;
            //      var tmp1 = _inputControl.GetComponent<CanvasScaler>();
            //DestroyImmediate(tmp1);
            //var tmp3 = _inputControl.GetComponent<GraphicRaycaster>();
            //DestroyImmediate(tmp3);
            //var tmp2 = _inputControl.GetComponent<Canvas>();
            //DestroyImmediate(tmp2);

            //      var rectTrans = _inputControl.transform as RectTransform;
            //CommonTools.SetParent(rectTrans, parent);
            //      rectTrans.anchorMax = parent.anchorMax;
            //rectTrans.anchorMin = parent.anchorMin;
            //rectTrans.anchoredPosition = parent.anchoredPosition;
            //rectTrans.anchoredPosition3D = parent.anchoredPosition3D;
            //rectTrans.offsetMax = parent.offsetMax;
            //rectTrans.offsetMin = parent.offsetMin;
            //rectTrans.pivot = parent.pivot;
            //rectTrans.sizeDelta = parent.sizeDelta;

            //CommonTools.SetAllLayerIncludeHideObj(rectTrans, (int)ELayer.UI);

            //InputManager.Instance.GameInputControl = _inputControl.GetComponent<GameInputControl>();
            InputManager.Instance.GameInputControl = SocialGUIManager.Instance.GetUI<UICtrlGameInputControl> ();
        }

        private void CommitAdventureGameResult (Action successCB, Action<ENetResultCode> failureCB) 
        {
			float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;

            //SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
            AppData.Instance.AdventureData.CommitLevelResult (
                PlayMode.Instance.SceneState.GameSucceed,
                usedTime,
                PlayMode.Instance.SceneState.CurScore,
                PlayMode.Instance.SceneState.GemGain,
                PlayMode.Instance.SceneState.MonsterKilled,
                PlayMode.Instance.SceneState.SecondLeft,
                PlayMode.Instance.MainUnit.Life,
                GetRecord (),
				() => {
					LogHelper.Info("游戏成绩提交成功");
                    if (null != successCB) {
                        successCB.Invoke ();
                    }
					//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    //if (PlayMode.Instance.SceneState.GameSucceed) {
                    //    Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
                    //} else {
                    //    Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
                    //}
				}, errCode => {
					//SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					//CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
						//new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
						//	CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFinishSuccess));
						//}), 
						//new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
						//	Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
						//}));
                    if (null != failureCB) {
                        failureCB.Invoke (errCode);
                    }
				}
			);
		}

        private void CommitChallengeGameResult (Action successCB, Action<ENetResultCode> failureCB) {
            LocalUser.Instance.MatchUserData.CommitChallengeResult (
                PlayMode.Instance.SceneState.GameSucceed,
                PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime,
                () => {
                    LogHelper.Info ("——————————————————————提交挑战成功");
                    if (null != successCB) {
                        successCB.Invoke (); 
                    }
                },
                code => {
                    LogHelper.Info ("——————————————————————提交挑战失败");
                    if (null != failureCB) {
                        failureCB.Invoke (code);
                    }
                }
            );
        }

        private byte[] GetRecord ()
        {
            GM2DRecordData recordData = new GM2DRecordData ();
            recordData.Version = Version;
            recordData.FrameCount = ConstDefineGM2D.FixedFrameCount;
            recordData.Data.AddRange(PlayMode.Instance.InputDatas);
            recordData.BoostItem = new BoostItemData ();
            recordData.BoostItem.ExtraLife = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_AddLifeCount1) ? 1 : 0;
            recordData.BoostItem.InvinsibleOnDead = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_DeadInvincibleCount1) ? 1 : 0;
            recordData.BoostItem.LongerInvinsible = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_InvincibleTime2) ? 1 : 0;
            recordData.BoostItem.ScoreBonus = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_ScoreAddPercent20) ? 1 : 0;
            recordData.BoostItem.TimeBonus = PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_TimeAddPercent20) ? 1 : 0;
            recordData.Avatar = new AvatarData ();
            if (null == LocalUser.Instance.UsingAvatarData || !LocalUser.Instance.UsingAvatarData.IsInited)
            {
                recordData.Avatar.Appendage = 1;
                recordData.Avatar.Head = 1;
                recordData.Avatar.Lower = 1;
                recordData.Avatar.Upper = 1;    
            } else
            {
                if (null != LocalUser.Instance.UsingAvatarData.Appendage && LocalUser.Instance.UsingAvatarData.Appendage.IsInited)
                {
                    recordData.Avatar.Appendage = (int)LocalUser.Instance.UsingAvatarData.Appendage.Id;
                } else 
                {
                    recordData.Avatar.Appendage = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Head && LocalUser.Instance.UsingAvatarData.Head.IsInited) {
                    recordData.Avatar.Head = (int)LocalUser.Instance.UsingAvatarData.Head.Id;
                } else {
                    recordData.Avatar.Head = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Lower && LocalUser.Instance.UsingAvatarData.Lower.IsInited) {
                    recordData.Avatar.Lower = (int)LocalUser.Instance.UsingAvatarData.Lower.Id;
                } else {
                    recordData.Avatar.Lower = 1;
                }
                if (null != LocalUser.Instance.UsingAvatarData.Upper && LocalUser.Instance.UsingAvatarData.Upper.IsInited) {
                    recordData.Avatar.Upper = (int)LocalUser.Instance.UsingAvatarData.Upper.Id;
                } else {
                    recordData.Avatar.Upper = 1;
                }
            }
            byte[] recordByte = GameMapDataSerializer.Instance.Serialize(recordData);
            if(recordByte == null)
            {
                LogHelper.Error("录像数据出错");
            }
            else
            {
                recordByte = MatrixProjectTools.CompressLZMA(recordByte);
            }
            return recordByte;
        }

        private void OnUpdateSuccess()
        {
            _resDone = true;
        }

        private void OnProcess(float value)
        {
			Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess,value * 0.8f);
        }

        private void OnUpdateFailed()
        {
            OnGameLoadError();
        }
			
		/// <summary>
		/// 游戏以胜利结束
		/// </summary>
		private void OnGameFinishSuccess () {
			if (!PlayMode.Instance.SceneState.GameSucceed) return;
			PlayMode.Instance.GameFinishSuccess ();
			if(GameManager.Instance.GameMode == EGameMode.PlayRecord)
			{
//				CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f,()=>{
//					if(GameManager.Instance.CurrentGame != null)
//					{
//						bool value = SocialGUIManager.Instance.RunRecordInApp;
//						SocialApp.Instance.ReturnToApp(!value);
//					}
//				}));
//				return;
			}

			Project p = GameManager.Instance.CurrentGame.Project;
            if (EProjectStatus.PS_Private == p.ProjectStatus) {
                p.PassFlag = true;
                Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
                //				byte[] record = GetRecord();
                //				float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;
                //				GM2DGame.Instance.RecordBytes = record;
                //				GM2DGame.Instance.RecordUsedTime = usedTime;
                //
                //				GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
            } else if (p.ProjectStatus == EProjectStatus.PS_AdvNormal ||
                       p.ProjectStatus == EProjectStatus.PS_AdvBonus) 
            {//				&& GameManager.Instance.GameMode == EGameMode.Normal)
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
                CommitAdventureGameResult (
                    () =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
                    },
                    code =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
                        //CommonTools.ShowPopupDialog (
                        //    "游戏成绩提交失败",
                        //    null,
	                       // new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
	                       //   CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFinishSuccess));
	                       // }), 
	                       // new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
	                       //   Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
	                       // })
                        //);
                    }
                );
                return;
            } else if (EProjectStatus.PS_Reform == p.ProjectStatus) {
                //Debug.Log ("_______________________________1 " + p.ProjectId +" " + _project.PassFlag);
                if (false == p.PassFlag) {
                    
                    p.PassFlag = true;
                    NeedSave = true;
                    //Debug.Log ("_______________________________2 " + p.ProjectId +" " + _project.PassFlag);
                }
                Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
            } else if (EProjectStatus.PS_Challenge == p.ProjectStatus) {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "提交成绩中...");
                CommitChallengeGameResult (
                    () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
                    },
                    code => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        Messenger.Broadcast (EMessengerType.GameFinishSuccessShowUI);
                    }
                );
            }
		}

		/// <summary>
		/// 游戏以失败结束
		/// </summary>
		private void OnGameFinishFailed () {
			if (!PlayMode.Instance.SceneState.GameFailed) return;
			PlayMode.Instance.GameFinishFailed ();
			if(GameManager.Instance.GameMode == EGameMode.PlayRecord)
			{
				//				CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f,()=>{
				//					if(GameManager.Instance.CurrentGame != null)
				//					{
				//						bool value = SocialGUIManager.Instance.RunRecordInApp;
				//						SocialApp.Instance.ReturnToApp(!value);
				//					}
				//				}));
				//				return;
			}

			Project p = GameManager.Instance.CurrentGame.Project;
			if(p.ProjectStatus == EProjectStatus.PS_Private)
			{
                Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
				//				byte[] record = GetRecord();
				//				float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;
				//				GM2DGame.Instance.RecordBytes = record;
				//				GM2DGame.Instance.RecordUsedTime = usedTime;
				//
				//				GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
			}
            else if(p.ProjectStatus == EProjectStatus.PS_AdvNormal ||
                   p.ProjectStatus == EProjectStatus.PS_AdvBonus)
				//				&& GameManager.Instance.GameMode == EGameMode.Normal)
			{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "提交成绩中...");
                CommitAdventureGameResult (
                    () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
                    },
                    code => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
                    }
                );
				return;
			}
            else if (p.ProjectStatus == EProjectStatus.PS_Reform) {
                Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
            } else if (p.ProjectStatus == EProjectStatus.PS_Challenge) {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading (this, "提交成绩中...");
                CommitChallengeGameResult (
                    () =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
                    },
                    code =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading (this);
                        Messenger.Broadcast (EMessengerType.GameFinishFailedShowUI);
                    }
                );       
            }
		}
        
        private void OnGameLoadError()
        {
            OnGameLoadError("游戏资源加载出错，正在返回");
        }

        private void OnGameLoadError(string msg)
        {
            CommonTools.ShowPopupDialog(msg);
			Messenger.Broadcast(EMessengerType.OnLoadingErrorCloseUI);
            SocialApp.Instance.ReturnToApp();
        }

        #endregion
    }
}