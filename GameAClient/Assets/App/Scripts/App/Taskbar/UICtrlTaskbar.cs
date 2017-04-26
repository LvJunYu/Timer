/********************************************************************
** Filename : UICtrlTaskbar
** Author : Dong
** Date : 2015/4/30 16:09:07
** Summary : UICtrlTaskbar
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine.Proto;
using SoyEngine;
using GameA.Game;

namespace GameA
{
	[UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlTaskbar : UICtrlGenericBase<UIViewTaskbar>
    {
        #region 常量与字段

        private UICtrlBase _currentUI;
//        private UnityEngine.UI.Button[] _buttonList;
//        private Color[] DefaultColorList;
//        private Color[] SelectedColorList;

		private ChangePartsSpineView _avatarView;
		private RenderTexture _avatarRenderTexture;

        #endregion

        #region 属性

        #endregion

        #region 方法

		public override void OnUpdate ()
		{
			base.OnUpdate ();
			if (_cachedView.PlayerAvatarAnimation != null) {
				_cachedView.PlayerAvatarAnimation.Update (Time.deltaTime);
			}
		}

		protected override void OnDestroy ()
		{
			if (_avatarRenderTexture != null) {
				_avatarRenderTexture.Release ();
			}
			base.OnDestroy ();
		}

        protected override void InitGroupId()
        {
			_groupId = (int) EUIGroupType.MainFrame;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
//            RegisterEvent(SoyEngine.EMessengerType.OnMeNewMessageStateChanged, RefreshMeNewMessageState);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

//            _buttonList = new UnityEngine.UI.Button[]{
//                _cachedView.Soy.Button,
//                _cachedView.News.Button,
//                _cachedView.Create.Button,
//                _cachedView.Daily.Button,
//                _cachedView.Me.Button,
//            };
//            DefaultColorList = new Color[]{
//                new Color(155f/255, 155f/255, 155f/255),
//                new Color(155f/255, 155f/255, 155f/255),
//                new Color(155f/255, 155f/255, 155f/255),
//                new Color(155f/255, 155f/255, 155f/255),
//                new Color(155f/255, 155f/255, 155f/255),
//            };
//            SelectedColorList = new Color[]{
//                new Color(255f/255, 126f/255, 126f/255),
//                new Color(255f/255, 126f/255, 126f/255),
//                new Color(255f/255, 255f/255, 255f/255),
//                new Color(255f/255, 126f/255, 126f/255),
//                new Color(255f/255, 126f/255, 126f/255),
//            };

//            Vector2 size = _cachedView.ScaleRoot.sizeDelta;
//            float factor = SocialUIConfig.TaskBarHeight / size.y;
//            _cachedView.ScaleRoot.localScale = new Vector3(factor, factor, 1);
//            size.x = UIConstDefine.UINormalScreenWidth / factor;
//            _cachedView.ScaleRoot.sizeDelta = size;

//            RefreshMeNewMessageState();
            #if UNITY_EDITOR
            SocialGUIManager.Instance.OpenPopupUI<UICtrlGMTool> ();
            #endif

			_cachedView.WorldButton.onClick.AddListener (OnNewsBtn);
			_cachedView.WorkshopButton.onClick.AddListener (OnCreateBtn);
			_cachedView.SingleModeButton.onClick.AddListener (OnSingleGameBtn);

			_cachedView.AvatarBtn.onClick.AddListener (OnAvatarBtn);

			_cachedView.TestChangeAvatarBtn.onClick.AddListener (OnTestChangeAvatar);
			_cachedView.DebugClearUserDataBtn.onClick.AddListener (OnDebugClearUserData);

			// todo player avatar at home
			_avatarRenderTexture = new RenderTexture (256, 512, 0);
			_cachedView.AvatarRenderCamera.targetTexture = _avatarRenderTexture;
			_cachedView.AvatarImage.texture = _avatarRenderTexture;

			_avatarView = new ChangePartsSpineView ();
			_avatarView.HomePlayerAvatarViewInit (_cachedView.PlayerAvatarAnimation);

//			var levels = TableManager.Instance.Table_StandaloneLevelDic;
//			foreach (var level in levels) {
//				Debug.Log (level.Value.Id + " " + level.Value.Name);
//			}
			_cachedView.AvatarImage.SetActiveEx(false);
//			LocalUser.Instance.UserLegacy.AvatarData.LoadUsingData(()=>{
//				RefreshAvatar();
//			}, (networkError) => {
//				LogHelper.Error("Network error when get avatarData, {0}", networkError);
//			});
			LocalUser.Instance.UsingAvatarData.Request(
				LocalUser.Instance.UserGuid,
				() => {
					RefreshAvatar();
				}, code => {
					LogHelper.Error("Network error when get avatarData, {0}", code);
				}
			);
        }

		protected override void OnOpen (object parameter)
		{
			RefreshUserInfo ();
			RefreshWalletInfo ();
			RefreshAvatar ();
		}
			
//        public void ShowDefaultPage()
//        {
//            OnSoy();
//        }

//        public void OnSoy()
//        {
////            if(OpenMainUI(typeof (UICtrlSoy)))
////            {
////                SelectButton(_cachedView.Soy.Button);
////            }
//        }

		private void OnTestChangeAvatar () {
//			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求换装...");
//			int type = UnityEngine.Random.Range (0, 3);
//			LocalUser.Instance.UserLegacy.AvatarData.SendChangeAvatarPart (
//				(EAvatarPart)(type + 1),
//				(_avatarView.EquipedPartsIds [type] + 1) % 2 + 1,
//				() => {
//					_avatarView.SetParts ((_avatarView.EquipedPartsIds [type] + 1) % 2 + 1,
//						(SpinePartsHelper.ESpineParts)type, true);
//					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//				}, (netError) => {
//					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//				}
//			);
		}

		private void OnDebugClearUserData () {
//			SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "清空数据");
//			RemoteCommands.ClearUserAll (LocalUser.Instance.UserGuid,
//				ret => {
//					ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(()=>{
//						SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);	
//					}, code=>{
//						LogHelper.Error("Refresh user data failed.");
//						SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//					});
//					helper.AddTask(AppData.Instance.LoadAppData);
//					helper.AddTask(LocalUser.Instance.LoadUserData);
//					helper.AddTask(AppData.Instance.AdventureData.PrepareAllData);
//				}, code => {
//					LogHelper.Error("Clear user data failed.");
//					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//				});
		}

        public void OnNewsBtn()
        {
//            if(OpenMainUI(typeof (UICtrlNewsFeed)))
//            {
//                SelectButton(_cachedView.News.Button);
//            }
        }

        public void OnCreateBtn()
        {
            if(OpenMainUI(typeof (UICtrlMatrixDetail)))
            {
//                SelectButton(_cachedView.Create.Button);
            }
        }

//        public void OnDaily()
//        {
////            if(OpenMainUI(typeof (UICtrlDaily)))
////            {
////                SelectButton(_cachedView.Daily.Button);
////            }
//        }
//
//        public void OnMe()
//        {
//            if(OpenMainUI(typeof (UICtrlMe)))
//            {
//                SelectButton(_cachedView.Me.Button);
//            }
//        }

		private void OnSingleGameBtn () {
			OpenMainUI(typeof (UICtrlSingleMode));
		}

        private bool OpenMainUI(Type type, object param = null)
        {
            UICtrlBase ui = SocialGUIManager.Instance.GetUI(type);
//            if(_currentUI == ui)
//            {
//                return false;
//            }
            SocialGUIManager.Instance.OpenMainUI(type, param);
            _currentUI = ui;
            return true;
        }

		/// <summary>
		/// 家园角色被点击
		/// </summary>
		private void OnAvatarBtn () {
			SocialGUIManager.Instance.OpenPopupUI<UICtrlFashionShop> ();
		}

//        private void SelectButton(UnityEngine.UI.Button button)
//        {
//            for (int i = 0; i < _buttonList.Length; i++)
//            {
//                var btn = _buttonList[i];
//                if(btn == null)
//                {
//                    continue;
//                }
//                Image image = (Image) btn.targetGraphic;
//                if(btn == button)
//                {
//                    image.sprite = btn.spriteState.pressedSprite;
//                    Text text = btn.GetComponentInChildren<Text>();
//                    if(text != null)
//                    {
//                        text.color = SelectedColorList[i];
//                    }
//                    btn.enabled = false;
//                }
//                else
//                {
//                    image.sprite = btn.spriteState.disabledSprite;
//                    Text text = btn.GetComponentInChildren<Text>();
//                    if(text != null)
//                    {
//                        text.color = DefaultColorList[i];
//                    }
//                    btn.enabled = true;
//                }
//            }
//        }

		private void RefreshUserInfo () {
			_cachedView.NickName.text = LocalUser.Instance.User.UserInfoSimple.NickName;
			ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHeadAvatar, 
				LocalUser.Instance.User.UserInfoSimple.HeadImgUrl,
				_cachedView.DefaultUserHeadTexture);
			_cachedView.AdventureLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel.ToString();
			_cachedView.CreatorLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel.ToString();
			if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Male) {
				_cachedView.MaleIcon.gameObject.SetActive (true);
				_cachedView.FemaleIcon.gameObject.SetActive (false);
			} else if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Female) {
				_cachedView.MaleIcon.gameObject.SetActive (false);
				_cachedView.FemaleIcon.gameObject.SetActive (true);
			} else {
				_cachedView.MaleIcon.gameObject.SetActive (true);
				_cachedView.FemaleIcon.gameObject.SetActive (false);
			}
		}
		private void RefreshWalletInfo () {
			_cachedView.MoneyCount.text = LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin.ToString();
			_cachedView.DiamondCount.text = LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond.ToString();
		}
		private void RefreshAvatar () {
//			if (!LocalUser.Instance.UserLegacy.AvatarData.Inited)
//				return;
			_cachedView.AvatarImage.SetActiveEx(true);
			if (LocalUser.Instance.UsingAvatarData.Head != null) {
				_avatarView.SetParts ((int)LocalUser.Instance.UsingAvatarData.Head.Id, SpinePartsHelper.ESpineParts.Head, true);
			}
			if (LocalUser.Instance.UsingAvatarData.Upper != null) {
				_avatarView.SetParts ((int)LocalUser.Instance.UsingAvatarData.Upper.Id, SpinePartsHelper.ESpineParts.Upper, true);
			}
			if (LocalUser.Instance.UsingAvatarData.Lower != null) {
				_avatarView.SetParts ((int)LocalUser.Instance.UsingAvatarData.Lower.Id, SpinePartsHelper.ESpineParts.Lower, true);
			}
			if (LocalUser.Instance.UsingAvatarData.Appendage != null) {
				_avatarView.SetParts ((int)LocalUser.Instance.UsingAvatarData.Appendage.Id, SpinePartsHelper.ESpineParts.Appendage, true);
			}
		}

        #endregion

//        private void RefreshMeNewMessageState()
//        {
////            NotificationData nd = AppData.Instance.NotificationData;
////            bool hasNew = (
////                nd.UnreadAnnounceCount+
////                nd.UnreadProjectCommentCount+
////                nd.UnreadProjectRateCount+
////                nd.UnreadProjectReplyCount+
////                nd.UnreadNewFollowerCount
////            )>0;
////            if(hasNew)
////            {
////                _cachedView.Me.MarkUnread();
////            }
////            else
////            {
////                _cachedView.Me.MarkAllRead();
////            }
//        }
    }
}