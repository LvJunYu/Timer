/********************************************************************
** Filename : UICtrlTaskbar
** Author : Dong
** Date : 2015/4/30 16:09:07
** Summary : UICtrlTaskbar
***********************************************************************/

using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlTaskbar : UICtrlGenericBase<UIViewTaskbar>
    {
        #region 常量与字段

        private bool _singleModeAvailable = true;
        private bool _worldAvailable = true;
        private bool _workshopAvailable = true;
        private bool _lotteryAvailable = true;
        private bool _weaponAvailable = true;
        private bool _chatAvailable = true;
        private bool _fashionShopAvailable = true;
        private bool _puzzleAvailable;
        private bool _trainAvailable = false;
        private bool _achievementAvailable;
        private bool _mailBoxAvailable;

        private bool _friendsAvailable = true;

//        private bool _isShowingSettingButton = true;
        private UIParticleItem _uiParticleItem;

        private bool _pushGoldEnergyStyle;

        #endregion

        #region 属性

        public bool FashionShopAvailable
        {
            get { return _fashionShopAvailable; }
        }

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Background;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnUserInfoChanged, OnChangeToUserInfo);
        }

        private void OnChangeToUserInfo()
        {
            if (!_isViewCreated)
            {
                return;
            }
            RefreshUserInfo();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            //为了方便UI动画，设置按钮移到UICtrlGoldEnergy页面
//            _cachedView.Account.onClick.AddListener(Account);
//            SocialGUIManager.Instance.OpenUI<UICtrlGMTool>();

            _cachedView.WorldButton.onClick.AddListener(OnWorldBtn);
            _cachedView.WorkshopButton.onClick.AddListener(OnCreateBtn);
            _cachedView.PersonalInformation.onClick.AddListener(UIPersonalInformation);
            _cachedView.SingleModeButton.onClick.AddListener(OnSingleGameBtn);
            _cachedView.LotteryBtn.onClick.AddListener(OnLotteryBtn);
            _cachedView.AvatarBtn.onClick.AddListener(OnAvatarBtn);
            _cachedView.UnlockAll.onClick.AddListener(OnUnlockAll);
            _cachedView.FriendsBtn.onClick.AddListener(OnFriendBtn);
            _cachedView.MailBoxBtn.onClick.AddListener(OnMailBtn);
            _cachedView.PuzzleBtn.onClick.AddListener(OnPuzzleBtn);
            _cachedView.Weapon.onClick.AddListener(OnWeaponBtn);
            _cachedView.TrainBtn.onClick.AddListener(OnTrainBtn);
            _cachedView.AchievementBtn.onClick.AddListener(OnAchievementBtn);
            _cachedView.ChatBtn.onClick.AddListener(OnChatBtn);
            _cachedView.HandBook.onClick.AddListener(OnHandBookBtn);
            SetLock(UIFunction.UI_FashionShop, _fashionShopAvailable);
            SetLock(UIFunction.UI_Friends, _friendsAvailable);
            SetLock(UIFunction.UI_Lottery, _lotteryAvailable);
            SetLock(UIFunction.UI_MailBox, _mailBoxAvailable);
            SetLock(UIFunction.UI_Puzzle, _puzzleAvailable);
            SetLock(UIFunction.UI_Train, _trainAvailable);
            SetLock(UIFunction.UI_Achievement, _achievementAvailable);
            SetLock(UIFunction.UI_SingleMode, _singleModeAvailable);
            SetLock(UIFunction.UI_Workshop, _workshopAvailable);
            SetLock(UIFunction.UI_World, _worldAvailable);
            SetLock(UIFunction.UI_Weapon, _weaponAvailable);
            SetLock(UIFunction.UI_Chat, _chatAvailable);
            _uiParticleItem = GameParticleManager.Instance.GetUIParticleItem(ParticleNameConstDefineGM2D.HomeBgEffect,
                _cachedView.Trans, _groupId);
            _uiParticleItem.Particle.Play();
            OpenTaskBtn();
        }

        protected override void OnDestroy()
        {
            GameParticleManager.FreeParticleItem(_uiParticleItem.Particle);
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>()
                    .PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamondSettingMail);
                _pushGoldEnergyStyle = true;
            }
            RefreshUserInfo();
            GameProcessManager.Instance.RefreshHomeUIUnlock();
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
            base.OnClose();
        }

        public void SetLock(UIFunction uitype, bool ifunlock)
        {
            switch (uitype)
            {
                case UIFunction.UI_SingleMode:
                {
                    _cachedView.SingleMode.SetActiveEx(ifunlock);
//                        _cachedView.SingleModeDisable.SetActiveEx(!ifunlock);
                    _singleModeAvailable = ifunlock;
                }
                    break;

                case UIFunction.UI_Friends:
                {
                    _cachedView.Friends.SetActiveEx(ifunlock);
//                        _cachedView.FriendsDisable.SetActiveEx(!ifunlock);
                    _friendsAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_MailBox:
                {
                    _cachedView.MailBox.SetActiveEx(ifunlock);
//                        _cachedView.MailBoxDisable.SetActiveEx(!ifunlock);
                    _mailBoxAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Puzzle:
                {
                    _cachedView.Puzzle.SetActiveEx(ifunlock);
//                        _cachedView.PuzzleDisable.SetActiveEx(!ifunlock);
                    _puzzleAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Train:
                {
                    _cachedView.Train.SetActiveEx(ifunlock);
//                    _cachedView.TrainDisable.SetActiveEx(!ifunlock);
                    _puzzleAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Achievement:
                {
                    _cachedView.Achievement.SetActiveEx(ifunlock);
//                    _cachedView.AchievementDisable.SetActiveEx(!ifunlock);
                    _achievementAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Workshop:
                {
//                    _cachedView.Workshop.SetActiveEx(ifunlock);
                    _cachedView.WorkshopDisable.SetActiveEx(!ifunlock);
                    _workshopAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_World:
                {
//                    _cachedView.World.SetActiveEx(ifunlock);
                    _cachedView.WorldDisable.SetActiveEx(!ifunlock);
                    _worldAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Lottery:
                {
                    _cachedView.Lottery.SetActiveEx(ifunlock);
//                        _cachedView.LotteryDisable.SetActiveEx(!ifunlock);
                    _lotteryAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Weapon:
                {
                    _cachedView.Weapon.SetActiveEx(ifunlock);
                    _weaponAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_FashionShop:
                {
                    _cachedView.AvatarText.SetActiveEx(ifunlock);
                    _cachedView.AvatarBtn.enabled = ifunlock;
                    _fashionShopAvailable = ifunlock;
                    if (SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().IsOpen)
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().Set(ifunlock);
                    }
                }
                    break;
                case UIFunction.UI_Chat:
                {
                    _cachedView.ChatBtn.SetActiveEx(ifunlock);
                    _chatAvailable = ifunlock;
                }
                    break;
            }
        }

        public enum UIFunction
        {
            UI_SingleMode = 0,
            UI_World = 1,
            UI_Workshop = 2,
            UI_FashionShop = 3,
            UI_Lottery = 4,
            UI_Puzzle = 5,
            UI_MailBox = 6,
            UI_Friends = 7,
            UI_Train = 8,
            UI_Achievement = 9,
            UI_Weapon,
            UI_Chat
        }

        public void OnCreateBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.WorkShop))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWorkShop>();
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("完成冒险模式第一章，解锁工坊功能，制作属于自己关卡~");
            }
        }

        public void UIPersonalInformation()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(LocalUser.Instance.User);
        }

        public void OnSignUpBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSignup>();
        }

//        public void Account()
//        {
//            SocialGUIManager.Instance.OpenUI<UICtrlGameSetting>().ChangeToSettingAtHome();
//        }

        public void OnWorldBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.World))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWorld>();
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("完成冒险模式第一章，解锁世界功能，挑战其他玩家的制作的关卡~");
            }
        }

        public void OnMailBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Mail))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlMail>();
            }
        }

        private void OnSingleGameBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSingleMode>();
        }


        /// <summary>
        /// 家园角色被点击
        /// </summary>
        private void OnAvatarBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Fashion))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();
            }
        }

        private void OnLotteryBtn()
        {
            //Debug.Log("_________________________OnLotteryBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Lottery))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlLottery>();
            }
        }

        private void OnFriendBtn()
        {
            //Debug.Log("_________________________OnLotteryBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.SocialReLationShip))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlSocialRelationship>();
            }
        }

        private void OnPuzzleBtn()
        {
            //Debug.Log("_________________________OnPuzzleBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Puzzle))
            {
//                SocialGUIManager.Instance.OpenUI<UICtrlCharacterUpgrade>();
                SocialGUIManager.Instance.OpenUI<UICtrlPuzzle>();
            }
        }

        private void OnWeaponBtn()
        {
            //Debug.Log("_________________________OnWeaponBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Weapon))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWeapon>();
            }
        }

        private void OnHandBookBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlHandBook>();
        }

        //拼图入口秘密通道
        private int _puzzlePasswordCount;

        private float _lastClickTime;

        private void OnTrainBtn()
        {
            //Debug.Log("_________________________OnTrainBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Train))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlTrain>();
            }
        }

        private void OnAchievementBtn()
        {
            //Debug.Log("_________________________OnAchievementBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.Achievement))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlAchievement>();
            }
        }

        private void OnChatBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlChat>();
        }

        private void RefreshUserInfo()
        {
            _cachedView.NickName.text = LocalUser.Instance.User.UserInfoSimple.NickName;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHeadAvatar,
                LocalUser.Instance.User.UserInfoSimple.HeadImgUrl,
                _cachedView.DefaultUserHeadTexture);
            _cachedView.AdventureLevel.text =
                GameATools.GetLevelString(LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel);
            _cachedView.CreatorLevel.text =
                GameATools.GetLevelString(LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel);
            if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Male)
            {
                _cachedView.MaleIcon.gameObject.SetActive(true);
                _cachedView.FemaleIcon.gameObject.SetActive(false);
            }
            else if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Female)
            {
                _cachedView.MaleIcon.gameObject.SetActive(false);
                _cachedView.FemaleIcon.gameObject.SetActive(true);
            }
            else
            {
                _cachedView.MaleIcon.gameObject.SetActive(true);
                _cachedView.FemaleIcon.gameObject.SetActive(false);
            }
        }

        private void OnUnlockAll()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在执行GM命令");
            RemoteCommands.ExecuteCommand(
                LocalUser.Instance.UserGuid,
                "set advcompletelevel 4 9",
                msg =>
                {
                    //                    if (msg.ResultCode == (int)EExecuteCommandCode.ECC_Success) {
                    //                        ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(()=>{
                    //                            SocialGUIManager.Instance.CloseUI<UICtrlTaskbar> ();
                    //                            SocialGUIManager.Instance.OpenUI<UICtrlTaskbar> ();
                    //                        }, code=>{
                    //                            SocialGUIManager.ShowPopupDialog("网络错误");
                    //                        });
                    //                        helper.AddTask(AppData.Instance.LoadAppData);
                    //                        helper.AddTask(LocalUser.Instance.LoadUserData);
                    //                        helper.AddTask(AppData.Instance.AdventureData.PrepareAllData);
                    //                        helper.AddTask (LocalUser.Instance.LoadPropData);
                    //
                    //                    } else {
                    //                        SocialGUIManager.ShowPopupDialog("网络错误");
                    //                    }
                    SocialGUIManager.ShowPopupDialog("执行成功，请重新启动程序", null,
                        new KeyValuePair<string, Action>("OK", () => { Application.Quit(); }));
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                },
                code =>
                {
                    SocialGUIManager.ShowPopupDialog("网络错误");
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                }
            );
        }

        private void OpenTaskBtn()
        {
//            if (Application.isEditor)
//            {
//                _cachedView.WeaponObject.SetActive(true);
//                _cachedView.HandBookObject.SetActive(true);
//                SetLock(UIFunction.UI_Puzzle, true);
//                SetLock(UIFunction.UI_Train, true);
//                SetLock(UIFunction.UI_Achievement, true);
//            }
//            else
            {
                _cachedView.WeaponObject.SetActive(false);
                _cachedView.HandBookObject.SetActive(false);
                SetLock(UIFunction.UI_Puzzle, false);
                SetLock(UIFunction.UI_Train, false);
                SetLock(UIFunction.UI_Achievement, false);
                SetLock(UIFunction.UI_Lottery, false);
                SetLock(UIFunction.UI_Weapon, false);
            }
        }

        #endregion
    }
}