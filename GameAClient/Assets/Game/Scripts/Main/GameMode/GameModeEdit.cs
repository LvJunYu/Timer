using UnityEngine;
using System.Collections;
using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA.Game
{
    public abstract class GameModeEdit : GameModeBase
    {
		protected EMode _mode = EMode.None;
        protected byte[] _recordBytes;
		protected float _recordUsedTime;
		protected bool _needSave;
        protected byte[] _iconBytes;

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
				if (_recordBytes != null && MapDirty)
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
				if (EditMode.Instance == null
					|| EditMode.Instance.MapStatistics == null)
				{
					return false;
				}
				return EditMode.Instance.MapStatistics.NeedSave;
			}
			set
			{
				if (EditMode.Instance != null
					&& EditMode.Instance.MapStatistics != null)
				{
					EditMode.Instance.MapStatistics.NeedSave = value;
				}
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
				return this._iconBytes;
			}
			set { _iconBytes = value; }
		}

        public override bool Init(Project project, object param, GameManager.EStartType startType)
        {
            if (!base.Init(project, param, startType))
            {
                return false;
            }
            _gameRunMode = EGameRunMode.Edit;
            return true;
		}

        public override void InitByStep()
		{
            InitUI();
            InitGame();
        }

        public abstract void Save(Action successCallback = null,
                                  Action<EProjectOperateResult> failedCallback = null);

        protected virtual void InitUI()
        {
            ChangeMode(EMode.Edit);
        }

        protected virtual void InitGame()
        {
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

        public virtual void ChangeMode(EMode mode)
        {
            if (mode == _mode)
            {
                return;
            }
            _mode = mode;

            if (mode == EMode.EditTest)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlItem>();
//                SocialGUIManager.Instance.CloseUI<UICtrlCreate>();
                SocialGUIManager.Instance.OpenUI<UICtrlEdit>();
                SocialGUIManager.Instance.GetUI<UICtrlEdit>().ChangeToEditTestMode();
                SocialGUIManager.Instance.CloseUI<UICtrlScreenOperator>();
                SocialGUIManager.Instance.OpenUI<UICtrlSceneState>();
                SocialGUIManager.Instance.CloseUI<UICtrlModifyEdit>();
                InputManager.Instance.ShowGameInput();
            }
            else if (mode == EMode.Edit)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlItem>();
                SocialGUIManager.Instance.OpenUI<UICtrlScreenOperator>();
                SocialGUIManager.Instance.OpenUI<UICtrlEdit>();
                SocialGUIManager.Instance.GetUI<UICtrlEdit>().ChangeToEditMode();
                SocialGUIManager.Instance.CloseUI<UICtrlSceneState>();
                InputManager.Instance.HideGameInput();
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
            captureScreenSize.Set(Mathf.CeilToInt(1f * Screen.width / Screen.height * ImageHeight), ImageHeight);
			captureRect.y = 0;
			captureRect.x = (captureScreenSize.x - ImageWidth) * 0.5f;
			Texture2D t2 = ClientTools.CaptureCamera(CameraManager.Instance.RendererCamera, captureScreenSize, captureRect);
			return t2.EncodeToJPG(90);
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
