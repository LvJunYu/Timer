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
//using SoyEngine;
using GameA.Game;

namespace GameA
{
	
	[UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlTaskbar : UICtrlGenericBase<UIViewTaskbar>
    {
        #region 常量与字段


		//private ChangePartsSpineView _avatarView;
	    //public RenderTexture AvatarRenderTexture { get;  set; }

	    #endregion

        #region 属性

        #endregion

        #region 方法

		public override void OnUpdate ()
		{
			base.OnUpdate ();
			//if (_cachedView.PlayerAvatarAnimation != null) {
			//	_cachedView.PlayerAvatarAnimation.Update (Time.deltaTime);
			//}
		}

		protected override void OnDestroy ()
		{
			//if (AvatarRenderTexture != null) {
			//	AvatarRenderTexture.Release ();
			//}
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

            #if UNITY_EDITOR
            SocialGUIManager.Instance.OpenPopupUI<UICtrlGMTool>();
            #endif

            _cachedView.WorldButton.onClick.AddListener (OnWorldBtn);
			_cachedView.WorkshopButton.onClick.AddListener (OnCreateBtn);
			_cachedView.SingleModeButton.onClick.AddListener (OnSingleGameBtn);



            _cachedView.LotteryBtn.onClick.AddListener(OnLotteryBtn);





            _cachedView.TestChangeAvatarBtn.onClick.AddListener (OnTestChangeAvatar);
			//_cachedView.DebugClearUserDataBtn.onClick.AddListener (OnDebugClearUserData);

           // Debug.Log("______UICtrlTaskbar_______" + _cachedView.PlayerAvatarAnimation + "_______UICtrlTaskbar______" + _cachedView.PlayerAvatarAnimation.skeleton);

            // todo player avatar at home
   //         AvatarRenderTexture = new RenderTexture (256, 512, 0);
			//_cachedView.AvatarRenderCamera.targetTexture = AvatarRenderTexture;
   //         _cachedView.AvatarImage.texture = _cachedView.AvatarRenderCamera.targetTexture;
                //AvatarRenderTexture;
           // Debug.Log("_______UICtrlTaskbar______" + _cachedView.PlayerAvatarAnimation+ "____UICtrlTaskbar_________" + _cachedView.PlayerAvatarAnimation.skeleton);

   //         _avatarView = new ChangePartsSpineView ();
			//_avatarView.HomePlayerAvatarViewInit (_cachedView.PlayerAvatarAnimation);

//			var levels = TableManager.Instance.Table_StandaloneLevelDic;
//			foreach (var level in levels) {
//				Debug.Log (level.Value.Id + " " + level.Value.Name);
//			}
			//_cachedView.AvatarImage.SetActiveEx(false);
//			LocalUser.Instance.UserLegacy.AvatarData.LoadUsingData(()=>{
//				RefreshAvatar();
//			}, (networkError) => {
//				LogHelper.Error("Network error when get avatarData, {0}", networkError);
//			});
			//LocalUser.Instance.UsingAvatarData.Request(
			//	LocalUser.Instance.UserGuid,
			//	() => {
			//		RefreshAvatar();
			//	}, code => {
			//		LogHelper.Error("Network error when get avatarData, {0}", code);
			//	}
			//);
        }

		protected override void OnOpen (object parameter)
		{
            base.OnOpen (parameter);
            SocialGUIManager.ShowGoldEnergyBar (false);
			RefreshUserInfo ();
            _cachedView.SpineCat.AnimationState.SetAnimation(0, "Run", true);
            //RefreshAvatar ();

        }
			

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


        public void OnCreateBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlWorkShop>();
        }

        public void OnWorldBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlWorld>();
        }

		private void OnSingleGameBtn () {
            SocialGUIManager.Instance.OpenUI<UICtrlSingleMode>();
		}


		/// <summary>
		/// 家园角色被点击
		/// </summary>

		//private void OnAvatarBtn () {
		//	SocialGUIManager.Instance.OpenPopupUI<UICtrlFashionShopMainMenu>();
		//}


		private void OnAvatarBtn () {
            SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();
		}
        private void OnLotteryBtn()
        {
            //Debug.Log("_________________________OnLotteryBtn");
           SocialGUIManager.Instance.OpenUI<UICtrlLottery>();
        }

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
            SetExp();

        }

        private void SetExp()
        {

            long currentPlayerExp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp;
            //Debug.Log("______currentPlayerExp_______"+ currentPlayerExp);
            int MaxPlayerExp =
                TableManager.Instance.Table_PlayerLvToExpDic[
                    LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp;
            //Debug.Log("MaxPlayerExp" + MaxPlayerExp);

            _cachedView.AdventureExperience.fillAmount = (float)currentPlayerExp / MaxPlayerExp;

            long currentCreatorExp = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp;
            int MaxCreatorExp =
                TableManager.Instance.Table_PlayerLvToExpDic[
                    LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel].MakerExp;
            _cachedView.CreatorExperience.fillAmount = (float)currentCreatorExp / MaxCreatorExp;
        }


        #endregion
    }
}