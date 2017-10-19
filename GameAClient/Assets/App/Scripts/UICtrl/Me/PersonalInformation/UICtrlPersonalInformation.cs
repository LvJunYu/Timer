using System;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlPersonalInformation : UICtrlAnimationBase<UIViewPersonalInformation>
    {
        #region 常量与字段
        private string _name;
        private string _signature;
        //private int _seletctedHeadImage;todo
        //private string _maleIcon = "icon_male";
        //private string _femaleIcon= "icon_famale";
        private int    _defaultHeadNum = 0;
        private ESex _eMale;
        //private Project _representativeProjectle = null;todo
        private readonly List<UMCtrlAchievement> _cardList = new List<UMCtrlAchievement>();

        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Exit.onClick.AddListener(OnSubmit);
            _cachedView.SelectPhoto.onClick.AddListener(OnPhoto);
            _cachedView.EditDescBtn.onClick.AddListener(OnEditBtn);
            _cachedView.ConfirmDescBtn.onClick.AddListener(OnConfirmDescBtn);
            _cachedView.SelectMaleBtn.onClick.AddListener(SelectMale);
            _cachedView.SelectFemaleBtn.onClick.AddListener(SelectFemale);
        }

        private void OnSubmit()
        {
            //本地刷新最新信息
            LocalUser.Instance.User.RequestInfor(LocalUser.Instance.UserGuid,
                () => { UpdateUserInfo(LocalUser.Instance.User.GetUserInfoDetail); },
                null
                );
        }

        private void UpdateUserInfo(Msg_SC_DAT_UserInfoDetail userInfo)
        {
            //更改信息并上传
            userInfo.UserInfoSimple.Sex = _eMale;
            userInfo.UserInfoSimple.NickName = _name;
            userInfo.Profile = _signature;
            //userInfo.UserInfoSimple.HeadImgUrl = "";
            RemoteCommands.UpdateUserInfo(userInfo,
                (ret) =>
                {
                    if (ret.ResultCode == (int) EUpdateUserInfoCode.UUC_Success)
                    {
                        LocalUser.Instance.User.OnSync(ret.UserInfo);
                        Messenger.Broadcast(EMessengerType.OnUserInfoChanged);
                        //LocalUser.Instance.LoadUserData(
                        //    () => {  }
                        //    , null);
                    }
                    //_returnName=ret.UserInfo.
                    Debug.Log("SubmitNewName" + userInfo.UserInfoSimple.NickName);
                }, null
                );
            SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
        }

        private void UpdateView()
        {
        }

        public void SetHead(int HeadNum)
        {
            //_seletctedHeadImage=HeadNum;todo
            var head = SpriteNameDefine.GetHeadImage(HeadNum);
            Texture fashion;
            if (JoyResManager.Instance.TryGetTexture(head, out fashion))
            {
                _cachedView.PhotoPortrait.texture = fashion;
            }
        }

        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
            Exp();
            SetAchievement();
            //加判断
            //LocalUser.Instance.UserPublishedWorldProjectList.Request(
            //    LocalUser.Instance.UserGuid,
            //    0, int.MaxValue,
            //    EPublishedProjectOrderBy.PPOB_PublishTime,
            //    EOrderType.OT_Asc,
            //    () => { SetRepresentativeWorks(); },
            //    null
            //    );
            Texture fashion;
            if (JoyResManager.Instance.TryGetTexture(LocalUser.Instance.User.UserInfoSimple.HeadImgUrl,out fashion))
            {
                _cachedView.PhotoPortrait.texture = fashion;
            }
            else
            {
                JoyResManager.Instance.TryGetTexture(SpriteNameDefine.GetHeadImage(_defaultHeadNum),out fashion);
                _cachedView.PhotoPortrait.texture = fashion;
            }

            _cachedView.Name.text = LocalUser.Instance.User.UserInfoSimple.NickName;
            _cachedView.SignatureDesc.text = LocalUser.Instance.User.Profile;
            _cachedView.Lvl.text = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel.ToString();
            _cachedView.CraftLvl.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp.ToString();
            _eMale = LocalUser.Instance.User.UserInfoSimple.Sex;

            ChangeEditMode();

            if (_eMale == ESex.S_Male)
            {
                _cachedView.MSex.SetActiveEx(true);
                _cachedView.FSex.SetActiveEx(false);
            }
            else
            {
                _cachedView.MSex.SetActiveEx(false);
                _cachedView.FSex.SetActiveEx(true);
            }
            SetRepresentativeWorks();
        }

        #endregion
        #region 事件处理
        private void InitPanel()
        {
            //_cachedView.NumberOfArts.text=LocalUser.Instance.Account.
            //_cachedView.NumberOfPlayed.text=LocalUser.Instance.Account.
            //_cachedView.NumberOfPraise.text=LocalUser.Instance.Account.
            //_cachedView.NumberOfRecompose.NumberOfArts.text=LocalUser.Instance.Account.
            //if(LocalUser.Instance.UserLegacy.NickName!=null)
            //{ _cachedView.Name.text = LocalUser.Instance.UserLegacy.NickName; }

        }

        private void ChangeEditMode(bool IfEdit=false)
        {
            _cachedView.ShowSex.gameObject.SetActiveEx(!IfEdit);
            _cachedView.Name.gameObject.SetActiveEx(!IfEdit);
            _cachedView.SignatureDesc.gameObject.SetActiveEx(!IfEdit);
            _cachedView.EditDescBtn.gameObject.SetActiveEx(!IfEdit); 
            _cachedView.NameDescInput.gameObject.SetActiveEx(IfEdit);
            _cachedView.SignatureDescInput.gameObject.SetActiveEx(IfEdit);
            _cachedView.EditSex.gameObject.SetActiveEx(IfEdit);
            _cachedView.ConfirmDescBtn.gameObject.SetActiveEx(IfEdit);

        }

        private void OnEditBtn()
        {
            //sex name sig btn
            ChangeEditMode(true);
            _cachedView.NameDescInput.text = _name;
            _cachedView.SignatureDescInput.text = _signature;
            _cachedView.SelectMale.SetActiveEx(false);
            _cachedView.SelectFemale.SetActiveEx(false);
    }

        private void OnPhoto()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlHeadPortraitSelect>();
        }

        private void ModifiUserInformation()
        {
        }

        private void OnConfirmDescBtn()
        {
            string newName = _cachedView.NameDescInput.text;
            newName = CheckNameValid(newName);
            if (!string.IsNullOrEmpty(newName) &&
                newName != _cachedView.Name.text)
            {
                _cachedView.Name.text = newName;
                _name = newName;
                //Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curSelectedPrivateProject.Content);
            }
            string newSignature = _cachedView.SignatureDescInput.text;
            newSignature = CheckPSignatureValid(newSignature);
            if (!string.IsNullOrEmpty(newSignature) &&
                newSignature != _cachedView.SignatureDesc.text)
            {
                _cachedView.SignatureDesc.text = newSignature;
                _signature = newSignature;
                //Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curSelectedPrivateProject.Content);
            }
            ChangeEditMode();
        }


        private void SelectMale()
        {
            _cachedView.SelectMale.SetActiveEx(true);
            _cachedView.SelectFemale.SetActiveEx(false);
            _eMale = ESex.S_Male;
            _cachedView.MSex.SetActiveEx(true);
            _cachedView.FSex.SetActiveEx(false);
        }

        private void SelectFemale()
        {
            _cachedView.SelectMale.SetActiveEx(false);
            _cachedView.SelectFemale.SetActiveEx(true);
            _eMale = ESex.S_Female;
            _cachedView.MSex.SetActiveEx(false);
            _cachedView.FSex.SetActiveEx(true);
        }


        private string CheckNameValid(string title)
        {
            // todo 检测合法性
            return title;
        }

        private string CheckPSignatureValid(string desc)
        {
            // todo 检测合法性
            return desc;
        }

        private void Exp()
        {
            int playerLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            long currentPlayerExp = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp;
            long initialExp = currentPlayerExp - TableManager.Instance.Table_PlayerLvToExpDic[
                playerLevel].AdvExp;
            //_cachedView.CurExp.text = initialExp.ToString();
            _cachedView.CurExp.text = String.Format("{0}/{1}", initialExp,
                (TableManager.Instance.Table_PlayerLvToExpDic[playerLevel + 1].AdvExp -
                 TableManager.Instance.Table_PlayerLvToExpDic[playerLevel].AdvExp));
            _cachedView.ExpBar.fillAmount = CountExpRatio(initialExp, playerLevel);
            int playerCraftLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel;
            long currentPlayerCraftExp = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp;
            long initialCraftExp = currentPlayerCraftExp - TableManager.Instance.Table_PlayerLvToExpDic[
                playerCraftLevel].MakerExp;
            //_cachedView.CurCraftExp.text = initialCraftExp.ToString();
            _cachedView.CurCraftExp.text = String.Format("{0}/{1}", initialCraftExp,
                (TableManager.Instance.Table_PlayerLvToExpDic[playerCraftLevel + 1].MakerExp -
                 TableManager.Instance.Table_PlayerLvToExpDic[playerCraftLevel].MakerExp));
            _cachedView.CraftExpBar.fillAmount = CountCraftExpRatio(initialCraftExp, playerCraftLevel);
        }

        private float CountExpRatio(float exp, int level)
        {
            return (exp
                //- TableManager.Instance.Table_PlayerLvToExpDic[LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp
                )
                   /
                   (TableManager.Instance.Table_PlayerLvToExpDic[level + 1].AdvExp -
                    TableManager.Instance.Table_PlayerLvToExpDic[level].AdvExp);
        }

        private float CountCraftExpRatio(float exp, int level)
        {
            return (exp
                //- TableManager.Instance.Table_PlayerLvToExpDic[LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp
                )
                   /
                   (TableManager.Instance.Table_PlayerLvToExpDic[level + 1].MakerExp -
                    TableManager.Instance.Table_PlayerLvToExpDic[level].MakerExp);
        }

        public void SetAchievement()
        {
            for (int i = 0; i < 10; i++)
            {
                var um = new UMCtrlAchievement();
                um.Init(_cachedView.Dock, ResScenary);
                um.Set();
                _cardList.Add(um);
            }
        }

        public void SetRepresentativeWorks()
        {
            //if (LocalUser.Instance.UserPublishedWorldProjectList.IsInited)
            //{
            //    List<Project> list = LocalUser.Instance.UserPublishedWorldProjectList.ProjectList;
            //    _representativeProjectle = list[0];
            //}
            //if (null != _representativeProjectle)
            //{
            //    ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _representativeProjectle.IconPath, _cachedView.DefaultCoverTexture);
            //    DictionaryTools.SetContentText(_cachedView.Title, _representativeProjectle.Name);
            //    //DictionaryTools.SetContentText(_cachedView.Desc, _representativeProjectle.Summary);
            //}
            //else
            //{
            //    ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
            //    DictionaryTools.SetContentText(_cachedView.Title, "关卡标题");
            //    //DictionaryTools.SetContentText(_cachedView.Desc, "关卡简介");
            //}
        }

        #endregion 事件处理

        #region 接口

        #endregion
    }
}
