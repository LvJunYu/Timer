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
            else
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
            GameResourceManager rm = gameObject.AddComponent<GameResourceManager>();
            if (!rm.Init(GameName))
            {
                LogHelper.Error("GameResourceManager initFailed");
            }
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
                GM2DRecordData recordData = GameMapDataSerializer.Instance.Deserialize<GM2DRecordData>(recordBytes);
                if(recordData == null)
                {
                    OnGameLoadError("录像解析失败");
                    yield break;
                }
                PlayMode.Instance.ERunMode = ERunMode.Record;
                PlayMode.Instance.InputDatas = recordData.Data;
            }

            yield return null;
            gameObject.AddComponent<InputManager>();
            gameObject.AddComponent<GM2DGUIManager>();
            gameObject.AddComponent<MapManager>();
            MapManager.Instance.Init(_eGameInitType, _project);
            yield return null;
            while (!MapManager.Instance.GenerateMapComplete)
            {
				Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, 0.8f + MapManager.Instance.MapProcess * 0.2f);
                yield return new WaitForSeconds(0.2f);
            }
            LoadGameInputController();
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
		        GM2DGUIManager.Instance.RenderUICamera.targetTexture = tex;
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
//			view.SetParts (2, SpinePartsDefine.ESpineParts.Head);
			
		}

        public void ChangeToMode(EMode mode)
        {
            _mode = mode;
			if (mode == EMode.EditTest) {
				GM2DGUIManager.Instance.CloseUI<UICtrlItem> ();
				GM2DGUIManager.Instance.CloseUI<UICtrlCreate> ();
				GM2DGUIManager.Instance.OpenUI<UICtrlEdit> ();
				GM2DGUIManager.Instance.GetUI<UICtrlEdit> ().ChangeToEditTestMode ();
				GM2DGUIManager.Instance.CloseUI<UICtrlScreenOperator> ();
				GM2DGUIManager.Instance.OpenUI<UICtrlSceneState> ();
                GM2DGUIManager.Instance.CloseUI<UICtrlModifyEdit> ();
				InputManager.Instance.ShowGameInput ();
			} else if (mode == EMode.Edit) {
				GM2DGUIManager.Instance.OpenUI<UICtrlCreate> ();
				GM2DGUIManager.Instance.OpenUI<UICtrlScreenOperator> ();
				GM2DGUIManager.Instance.OpenUI<UICtrlEdit> ();
				GM2DGUIManager.Instance.GetUI<UICtrlEdit> ().ChangeToEditMode ();
				GM2DGUIManager.Instance.CloseUI<UICtrlSceneState> ();
				InputManager.Instance.HideGameInput ();
			} else if (mode == EMode.Play) {
				GM2DGUIManager.Instance.OpenUI<UICtrlEdit> ().ChangeToPlayMode ();
				GM2DGUIManager.Instance.CloseUI<UICtrlCreate> ();
				GM2DGUIManager.Instance.CloseUI<UICtrlScreenOperator> ();
				GM2DGUIManager.Instance.OpenUI<UICtrlSceneState> ();
				InputManager.Instance.ShowGameInput ();
			} else if (mode == EMode.PlayRecord) {
				GM2DGUIManager.Instance.OpenUI<UICtrlEdit> ().ChangeToPlayRecordMode ();
				GM2DGUIManager.Instance.CloseUI<UICtrlCreate> ();
				GM2DGUIManager.Instance.CloseUI<UICtrlScreenOperator> ();
				GM2DGUIManager.Instance.OpenUI<UICtrlSceneState> ();
				InputManager.Instance.HideGameInput ();
			} else if (mode == EMode.ModifyEdit) {
				GM2DGUIManager.Instance.OpenUI<UICtrlEdit> ().ChangeToModifyMode ();
				GM2DGUIManager.Instance.CloseUI<UICtrlSceneState> ();
                GM2DGUIManager.Instance.OpenUI<UICtrlModifyEdit> ();
				InputManager.Instance.HideGameInput ();
			}
        }

        public void Save(string name, string summary, int downloadPrice,
            bool publishRecordFlag, Action successCallback = null, Action<EProjectOperateResult> failedCallback = null)
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
                publishRecordFlag,
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
            _project.SaveModifyProject (compressedBytes, successCallback, failedCallback);
        }

        public void Publish(string name, string summary, Action successCallback=null, Action<EProjectOperateResult> failedCallback=null)
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
            _project.Publish(name, summary, mapDataBytes, IconBytes,
                RecordUsedTime, RecordBytes, ()=>{
                NeedSave = false;
                MapDirty = false;
                if(successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, failedCallback);
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
#if UNITY_EDITOR
            UnityEngine.Object obj = Resources.Load("EasyTouchControlsCanvas");
            if (obj == null) {
                obj = GameResourceManager.Instance.LoadMainAssetObject ("EasyTouchControlsCanvas");
            }
            _inputControl = CommonTools.InstantiateObject (obj);
#else
            var obj = GameResourceManager.Instance.LoadMainAssetObject("EasyTouchControlsCanvas");
            _inputControl = CommonTools.InstantiateObject(obj);
#endif
	        var parent = GM2DGUIManager.Instance.GetFirstGroupParent()as RectTransform;
	        var tmp1 = _inputControl.GetComponent<CanvasScaler>();
			DestroyImmediate(tmp1);
			var tmp3 = _inputControl.GetComponent<GraphicRaycaster>();
			DestroyImmediate(tmp3);
			var tmp2 = _inputControl.GetComponent<Canvas>();
			DestroyImmediate(tmp2);

	        var rectTrans = _inputControl.transform as RectTransform;
			CommonTools.SetParent(rectTrans, parent);
	        rectTrans.anchorMax = parent.anchorMax;
			rectTrans.anchorMin = parent.anchorMin;
			rectTrans.anchoredPosition = parent.anchoredPosition;
			rectTrans.anchoredPosition3D = parent.anchoredPosition3D;
			rectTrans.offsetMax = parent.offsetMax;
			rectTrans.offsetMin = parent.offsetMin;
			rectTrans.pivot = parent.pivot;
			rectTrans.sizeDelta = parent.sizeDelta;

			CommonTools.SetAllLayerIncludeHideObj(rectTrans, (int)ELayer.UI);

			InputManager.Instance.GameInputControl = _inputControl.GetComponent<GameInputControl>();
        }

		private void CommitAdventureGameResult (bool success) {
			float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;

			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "提交成绩中...");
			AppData.Instance.AdventureData.CommitLevelResult (
				success,
				usedTime,
				UnityEngine.Random.value > 0.5f,
				UnityEngine.Random.value > 0.5f,
				UnityEngine.Random.value > 0.5f,
				PlayMode.Instance.SceneState.TotalScore,
				(int)(10 * UnityEngine.Random.value),
				(int)(10 * UnityEngine.Random.value),
				(int)(10 * UnityEngine.Random.value),
				(int)(10 * UnityEngine.Random.value),
				() => {
					LogHelper.Info("游戏成绩提交成功");
					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
				}, errCode => {
					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					CommonTools.ShowPopupDialog("游戏成绩提交失败", null,
						new System.Collections.Generic.KeyValuePair<string, Action>("重试", ()=>{
							CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnGameFinishSuccess));
						}), 
						new System.Collections.Generic.KeyValuePair<string, Action>("跳过", ()=>{
							Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
						}));
				}
			);
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
			if(p.ProjectStatus == EProjectStatus.PS_Private)
			{
				Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
//				byte[] record = GetRecord();
//				float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;
//				GM2DGame.Instance.RecordBytes = record;
//				GM2DGame.Instance.RecordUsedTime = usedTime;
//
//				GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
			}
			else if(p.ProjectStatus == EProjectStatus.PS_Public)
//				&& GameManager.Instance.GameMode == EGameMode.Normal)
			{
				CommitAdventureGameResult (true);
				return;
			}
            else if (p.ProjectStatus == EProjectStatus.PS_Reform)
			{
                p.PassFlag = true;
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
				//				byte[] record = GetRecord();
				//				float usedTime = PlayMode.Instance.GameSuccessFrameCnt * ConstDefineGM2D.FixedDeltaTime;
				//				GM2DGame.Instance.RecordBytes = record;
				//				GM2DGame.Instance.RecordUsedTime = usedTime;
				//
				//				GM2DGUIManager.Instance.OpenUI<UICtrlGameFinish>();
			}
			else if(p.ProjectStatus == EProjectStatus.PS_Public)
				//				&& GameManager.Instance.GameMode == EGameMode.Normal)
			{
				CommitAdventureGameResult (false);
				return;
			}
			else
			{
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