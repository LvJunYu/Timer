using System;
using System.Collections.Generic;
using System.Collections;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    public abstract class GameModeEdit : GameModeBase
    {
		protected EMode _mode = EMode.None;
        protected byte[] _recordBytes;
		protected bool _needSave;
        protected byte[] _iconBytes;
	    protected bool _recordSuccess;
	    protected int _recordUsedTime;
	    protected int _recordScore;
	    protected int _recordScoreItemCount;
	    protected int _recordKillMonsterCount;
	    protected int _recordLeftTime;
	    protected int _recordLeftLife;

		public byte[] RecordBytes
		{
			get
			{
				if (MapDirty)
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
				if (_recordBytes != null && (MapDirty || !_project.PassFlag))
				{
					NeedSave = true;
					MapDirty = false;
					_recordUsedTime = PlayMode.Instance.GameSuccessFrameCnt;
					_recordScore = PlayMode.Instance.SceneState.CurScore;
					_recordScoreItemCount = PlayMode.Instance.SceneState.GemGain;
					_recordKillMonsterCount = PlayMode.Instance.SceneState.MonsterKilled;
					_recordLeftTime = PlayMode.Instance.SceneState.SecondLeft;
					_recordLeftLife = PlayMode.Instance.MainPlayer.Life;
				}
			}
		}

		public bool NeedSave
		{
			get
			{
				if (MapDirty)
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
				return EditMode.Instance.MapStatistics.NeedSave;
			}
			set
			{
				EditMode.Instance.MapStatistics.NeedSave = value;
			}
		}

		public byte[] IconBytes
		{
			get
			{
				if (_iconBytes == null)
				{
					_iconBytes = LocalCacheManager.Instance.Load(LocalCacheManager.EType.Image, _project.IconPath);
				}
				return _iconBytes;
			}
			set { _iconBytes = value; }
		}

        public override bool Init(Project project, object param, GameManager.EStartType startType, MonoBehaviour corountineProxy)
        {
            if (!base.Init(project, param, startType, corountineProxy))
            {
                return false;
            }
            _gameRunMode = EGameRunMode.Edit;
	        EditMode.Instance.Init();
            return true;
		}

	    public override bool Stop()
	    {
		    EditMode.Instance.Dispose();
		    return base.Stop();
	    }

	    public override IEnumerator InitByStep()
		{
            GameRun.Instance.ChangeState(ESceneState.Edit);
            InitUI();
            InitGame();
			yield return null;
			//Spine预加载
			var tableUnitDict = TableManager.Instance.Table_UnitDic;
			int i = 0;
			foreach (var entry in tableUnitDict)
			{
				i++;
				var table = entry.Value;
				if (table.Id != 1002 && table.Use != 1)
				{
					continue;
				}
				if (table.GeneratedType != (int) EGeneratedType.Spine)
				{
					continue;
				}
                
				string skeletonDataAssetName = string.Format ("{0}_SkeletonData", table.Model);
				SkeletonDataAsset data =
					ResourcesManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, skeletonDataAssetName, 0);
				if (data != null)
				{
					data.GetAnimationStateData();
					BroadcastLoadProgress(0.8f + 0.2f * i / tableUnitDict.Count);
					yield return null;
				}
			}
			//预加载背景音乐
			ResourcesManager.Instance.GetAudio(AudioNameConstDefineGM2D.LevelNormalBgm);
		}

        public abstract void Save(Action successCallback = null,
                                  Action<EProjectOperateResult> failedCallback = null);

        protected virtual void InitUI()
        {
            ChangeMode(EMode.Edit);
        }

        protected virtual void InitGame()
        {
			MainPlayer mainPlayer = PlayMode.Instance.MainPlayer;
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

			if (LocalUser.Instance.UsingAvatarData.IsInited)
			{
				if (LocalUser.Instance.UsingAvatarData.Head != null &&
					LocalUser.Instance.UsingAvatarData.Head.IsInited)
				{
					headId = (int)LocalUser.Instance.UsingAvatarData.Head.Id;
				}
				if (LocalUser.Instance.UsingAvatarData.Upper != null &&
					LocalUser.Instance.UsingAvatarData.Upper.IsInited)
				{
					upperId = (int)LocalUser.Instance.UsingAvatarData.Upper.Id;
				}
				if (LocalUser.Instance.UsingAvatarData.Lower != null &&
					LocalUser.Instance.UsingAvatarData.Lower.IsInited)
				{
					lowerId = (int)LocalUser.Instance.UsingAvatarData.Lower.Id;
				}
				if (LocalUser.Instance.UsingAvatarData.Appendage != null &&
					LocalUser.Instance.UsingAvatarData.Appendage.IsInited)
				{
					appendageId = (int)LocalUser.Instance.UsingAvatarData.Appendage.Id;
				}
			}
			//          view.SetParts (2, SpinePartsDefine.ESpineParts.Head);
			if (LocalUser.Instance.UsingAvatarData.Head != null)
			{
				view.SetParts(headId, SpinePartsHelper.ESpineParts.Head);
			}
			if (LocalUser.Instance.UsingAvatarData.Upper != null)
			{
				view.SetParts(upperId, SpinePartsHelper.ESpineParts.Upper);
			}
			if (LocalUser.Instance.UsingAvatarData.Lower != null)
			{
				view.SetParts(lowerId, SpinePartsHelper.ESpineParts.Lower);
			}
			if (LocalUser.Instance.UsingAvatarData.Appendage != null)
			{
				view.SetParts(appendageId, SpinePartsHelper.ESpineParts.Appendage);
			}
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            if (_project.GetData() == null)
            {
                NeedSave = true;
                MapDirty = true;
            }
            else
            {
                NeedSave = false;
                MapDirty = false;
            }
        }

        public override bool Restart(Action successCb, Action failedCb)
        {
            ChangeMode(EMode.Edit);
            ChangeMode(EMode.EditTest);
            if (successCb != null)
            {
                successCb.Invoke();
            }
            return true;
        }

	    public override void Update()
	    {
		    if (!_run)
		    {
			    return;
		    }
		    GameRun.Instance.Update();
		    if (_mode == EMode.Edit)
		    {
			    EditMode.Instance.Update();
		    }
		    if (GameRun.Instance.LogicTimeSinceGameStarted < GameRun.Instance.GameTimeSinceGameStarted)
		    {
			    if (GameRun.Instance.IsPlaying && _mode == EMode.EditTest && null != PlayerManager.Instance.MainPlayer)
			    {
				    LocalPlayerInput localPlayerInput = PlayerManager.Instance.MainPlayer.Input as LocalPlayerInput;
				    if (localPlayerInput != null)
				    {
					    localPlayerInput.ProcessCheckInput();
					    List<int> inputChangeList = localPlayerInput.CurCheckInputChangeList;
					    for (int i = 0; i < inputChangeList.Count; i++)
					    {
						    _inputDatas.Add(GameRun.Instance.LogicFrameCnt);
						    _inputDatas.Add(inputChangeList[i]);
					    }
					    localPlayerInput.ApplyInputData(inputChangeList);
				    }
			    }
			    GameRun.Instance.UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
		    }
	    }

	    public virtual void ChangeMode(EMode mode)
        {
            if (mode == _mode)
            {
                return;
            }
            _mode = mode;
	        _run = true;
            if (mode == EMode.EditTest)
            {
                EditMode.Instance.StopEdit();
	            PlayMode.Instance.SceneState.Init(EditMode.Instance.MapStatistics);
                if (!GameRun.Instance.ChangeState(ESceneState.Play))
                {
                    ChangeMode(EMode.Edit);
                    return;
                }
	            _inputDatas.Clear();
                SocialGUIManager.Instance.CloseUI<UICtrlItem>();
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
//                SocialGUIManager.Instance.CloseUI<UICtrlCreate>();
                SocialGUIManager.Instance.OpenUI<UICtrlEdit>();
                SocialGUIManager.Instance.GetUI<UICtrlEdit>().ChangeToEditTestMode();
//                SocialGUIManager.Instance.CloseUI<UICtrlScreenOperator>();
                SocialGUIManager.Instance.OpenUI<UICtrlSceneState>();
                SocialGUIManager.Instance.CloseUI<UICtrlModifyEdit>();
                InputManager.Instance.ShowGameInput();
	            GameRun.Instance.Playing();
            }
            else if (mode == EMode.Edit)
            {
                GameRun.Instance.ChangeState(ESceneState.Edit);
	            EditMode.Instance.StartEdit();
	            SocialGUIManager.Instance.CloseUI<UICtrlGameScreenEffect>();
                SocialGUIManager.Instance.OpenUI<UICtrlItem>();
//                SocialGUIManager.Instance.OpenUI<UICtrlScreenOperator>();
                SocialGUIManager.Instance.OpenUI<UICtrlEdit>();
                SocialGUIManager.Instance.GetUI<UICtrlEdit>().ChangeToEditMode();
                SocialGUIManager.Instance.CloseUI<UICtrlSceneState>();
                InputManager.Instance.HideGameInput();
            }
        }

		public byte[] CaptureLevel()
		{
			const int imageWidth = 1280;
			const int imageHeight = 720;
			float imageAspectRatio = 1f * imageWidth / imageHeight;
			Vector2 captureScreenSize = Vector2.zero;
			Rect captureRect = new Rect();
			captureRect.height = imageHeight;
			captureRect.width = imageWidth;
			var mapTiledRect = DataScene2D.Instance.ValidMapRect;
			mapTiledRect.Max += new IntVec2(2, 2) * ConstDefineGM2D.ServerTileScale;
			mapTiledRect.Min -= new IntVec2(2, 2) * ConstDefineGM2D.ServerTileScale;
			Rect mapRect = GM2DTools.TileRectToWorldRect(mapTiledRect);
			float mapAspectRatio = mapRect.width / mapRect.height;
			float oriCameraOrthoSize = CameraManager.Instance.RendererCamera.orthographicSize;
			Vector3 oriCameraPos = CameraManager.Instance.MainCameraPos;
			float cameraOrthoSize;
			Vector3 cameraPos = mapRect.center;
			cameraPos.z = oriCameraPos.z;
			float screenAspectRatio = 1f * Screen.width / Screen.height;
			if (screenAspectRatio  < imageAspectRatio)
			{
				captureScreenSize.Set(imageWidth, Mathf.CeilToInt(imageWidth / screenAspectRatio));
				captureRect.y = (captureScreenSize.y - imageHeight) * 0.5f;
				captureRect.x = 0;
				if (mapAspectRatio > imageAspectRatio)
				{
					cameraOrthoSize = mapRect.width / screenAspectRatio / 2;
					cameraPos.y += mapRect.width / imageAspectRatio / 2 - mapRect.height / 2;
				}
				else
				{
					cameraOrthoSize = mapRect.height * imageAspectRatio / screenAspectRatio / 2;
				}
			}
			else
			{
				captureScreenSize.Set(Mathf.CeilToInt(screenAspectRatio * imageHeight), imageHeight);
				captureRect.y = 0;
				captureRect.x = (captureScreenSize.x - imageWidth) * 0.5f;
				if (mapAspectRatio > imageAspectRatio)
				{
					cameraOrthoSize = mapRect.width / imageAspectRatio / 2;
					cameraPos.y += cameraOrthoSize - mapRect.height / 2;
				}
				else
				{
					cameraOrthoSize = mapRect.height / 2;
				}
			}
			CameraManager.Instance.MainCameraPos = cameraPos;
			CameraManager.Instance.RendererCamera.orthographicSize = cameraOrthoSize;
			EditMode.Instance.CameraMask.Hide();
			BgScene2D.Instance.UpdateLogic(cameraPos);
			Texture2D t2 = ClientTools.CaptureCamera(CameraManager.Instance.RendererCamera, captureScreenSize, captureRect);
			CameraManager.Instance.MainCameraPos = oriCameraPos;
			CameraManager.Instance.RendererCamera.orthographicSize = oriCameraOrthoSize;
			BgScene2D.Instance.ResetByFollowPos(oriCameraPos);
			EditMode.Instance.CameraMask.Show();
			return t2.EncodeToJPG(90);
		}

		public bool CheckCanPublish(bool showPrompt = false)
		{
			if (RecordBytes == null && (NeedSave || !_project.PassFlag))
			{
				if (showPrompt)
				{
					Messenger<string>.Broadcast(EMessengerType.GameErrorLog, "请先自己通关再发布");
				}
				return false;
			}
			return true;
		}

		public enum EMode
		{
			None,
			// 正常编辑
			Edit,
			// 编辑时测试
			EditTest,
		}
    }
}
