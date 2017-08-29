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
using UnityEditor;
using UnityEngine;

namespace GameA
{

    [UIAutoSetup]
    public class UICtrlTaskbar : UICtrlGenericBase<UIViewTaskbar>
    {
        #region 常量与字段

        private bool _singleModeAvailable = true;
        private bool _worldAvailable = true;
        private bool _workshopAvailable = true;
        private bool _lotteryAvailable = true;
        private bool _fashionShopAvailable = true;
        private bool _puzzleAvailable = false;
        private bool _trainAvailable = false;
        private bool _mailBoxAvailable = true;
        private bool _friendsAvailable = true;
        private UIParticleItem _uiParticleItem;

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
            _groupId = (int)EUIGroupType.MainFrame;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OpenGameSetting, ShowPuzzleBtn);
            RegisterEvent(EMessengerType.OnUserInfoChanged, OnChangeToUserInfo);
        }

        private void OnChangeToUserInfo()
        {

            RefreshUserInfo();
        }


        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.Account.onClick.AddListener(Account);
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
            _cachedView.Weapon.onClick.AddListener(OnWeapon);
            _cachedView.TrainBtn.onClick.AddListener(OnTrainBtn);
            SetLock(UIFunction.UI_FashionShop, _fashionShopAvailable);
            SetLock(UIFunction.UI_Friends, _friendsAvailable);
            SetLock(UIFunction.UI_Lottery, _lotteryAvailable);
            SetLock(UIFunction.UI_MailBox, _mailBoxAvailable);
            SetLock(UIFunction.UI_Puzzle, _puzzleAvailable);
            SetLock(UIFunction.UI_Train, _trainAvailable);
            SetLock(UIFunction.UI_SingleMode, _singleModeAvailable);
            SetLock(UIFunction.UI_Workshop, _workshopAvailable);
            SetLock(UIFunction.UI_World, _worldAvailable);
            _uiParticleItem = GameParticleManager.Instance.GetUIParticleItem(ParticleNameConstDefineGM2D.HomeBgEffect,
                _cachedView.Trans, _groupId);
            _uiParticleItem.Particle.Play();
            OpenTaskBtn();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
            RefreshUserInfo();
            GameProcessManager.Instance.RefreshHomeUIUnlock();
           
        }

        protected override void OnClose()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
            base.OnClose();
        }
        
        

        public void SetLock(UIFunction UI, bool ifunlock)
        {
            switch (UI)
            {
                case UIFunction.UI_SingleMode:
                    {
                        _cachedView.SingleMode.SetActiveEx(ifunlock);
                        _cachedView.SingleModeDisable.SetActiveEx(!ifunlock);
                        _singleModeAvailable = ifunlock;

                    }
                    break;

                case UIFunction.UI_Friends:
                    {
                        _cachedView.Friends.SetActiveEx(ifunlock);
                        _cachedView.FriendsDisable.SetActiveEx(!ifunlock);
                        _friendsAvailable = ifunlock;

                    }
                    break;
                case UIFunction.UI_MailBox:
                    {
                        _cachedView.MailBox.SetActiveEx(ifunlock);
                        _cachedView.MailBoxDisable.SetActiveEx(!ifunlock);
                        _mailBoxAvailable = ifunlock;
                    }
                    break;
                case UIFunction.UI_Puzzle:
                    {
                        _cachedView.Puzzle.SetActiveEx(ifunlock);
                        _cachedView.PuzzleDisable.SetActiveEx(!ifunlock);
                        _puzzleAvailable = ifunlock;
                    }
                    break;
                case UIFunction.UI_Train:
                {
                    _cachedView.Train.SetActiveEx(ifunlock);
                    _cachedView.TrainDisable.SetActiveEx(!ifunlock);
                    _puzzleAvailable = ifunlock;
                }
                    break;
                case UIFunction.UI_Workshop:
                    {
                        _cachedView.Workshop.SetActiveEx(ifunlock);
                        _cachedView.WorkshopDisable.SetActiveEx(!ifunlock);
                        _workshopAvailable = ifunlock;
                    }
                    break;
                case UIFunction.UI_World:
                    {
                        _cachedView.World.SetActiveEx(ifunlock);
                        _cachedView.WorldDisable.SetActiveEx(!ifunlock);
                        _worldAvailable = ifunlock;

                    }
                    break;
                case UIFunction.UI_Lottery:
                    {
                        _cachedView.Lottery.SetActiveEx(ifunlock);
                        _cachedView.LotteryDisable.SetActiveEx(!ifunlock);
                        _lotteryAvailable = ifunlock;
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
        }


        public void OnCreateBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.WorkShop))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWorkShop>();
            }
        }

        public void UIPersonalInformation()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.WorkShop))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>();
            }
        }

        public void OnSignUpBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSignup>();
        }

        public void Account()
        {
            Messenger.Broadcast(EMessengerType.OpenGameSetting);
            SocialGUIManager.Instance.GetUI<UICtrlGameSetting>().ChangeToSettingAtHome();
        }

        public void OnWorldBtn()
        {
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.World))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWorld>();
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

        private void OnWeapon()
        {
            //Debug.Log("_________________________OnWeaponBtn");
            SocialGUIManager.Instance.OpenUI<UICtrlWeapon>();
            
        }
        //拼图入口秘密通道
        private int _puzzlePasswordCount;
        private float _lastClickTime;
        private void ShowPuzzleBtn()
        {
            if (Time.time - _lastClickTime < 0.5f)
                _puzzlePasswordCount++;
            else
                _puzzlePasswordCount = 0;
            _lastClickTime = Time.time;
            if (_puzzlePasswordCount > 2)
            {
//                _cachedView.PuzzleBtn.transform.parent.gameObject.SetActive(true);
//                _cachedView.PuzzleBtn.gameObject.SetActive(true);
//                _cachedView.PuzzleDisable.SetActive(false);
                SetLock(UIFunction.UI_Puzzle, true);
                _puzzlePasswordCount = 0;
            }
        }
        
        private void OnTrainBtn()
        {
            //Debug.Log("_________________________OnCharacterUpgradeBtn");
            if (GameProcessManager.Instance.IsGameSystemAvailable(EGameSystem.CharacterUpgrade))
            {
                SocialGUIManager.Instance.OpenUI<UICtrlCharacterUpgrade>();
            }
        }

        private void RefreshUserInfo()
        {
            _cachedView.NickName.text = LocalUser.Instance.User.UserInfoSimple.NickName;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHeadAvatar,
                LocalUser.Instance.User.UserInfoSimple.HeadImgUrl,
                _cachedView.DefaultUserHeadTexture);
            //_cachedView.AdventureLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel.ToString();
            //_cachedView.CreatorLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel.ToString();
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
          #if UNITY_EDITOR
          _cachedView.WeaponObject.SetActive(true);
          SetLock(UIFunction.UI_Train, true);
          #endif
        }

        #endregion
    }
}