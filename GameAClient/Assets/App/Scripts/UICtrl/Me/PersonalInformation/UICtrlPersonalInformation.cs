 /********************************************************************
 ** Filename : UICtrlSetting.cs
 ** Author : quan
 ** Date : 16/4/30 下午6:42
 ** Summary : UICtrlSetting.cs
 ***********************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlPersonalInformation : UICtrlGenericBase<UIViewPersonalInformation>
    {
        #region 常量与字段
        private string _name;
        private string _signature;
        private string _maleIcon = "icon_male";
        private string _femaleIcon= "icon_famale";
        private bool _isMale=true;
        private int _Male;
        private Project _representativeProjectle = null;
        private List<UMCtrlAchievement> _cardList = new List<UMCtrlAchievement>();

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
        //    public Button Exit;
        //public Button AddFriend;
        //public Button Modification;
        //public Button SelectPhoto;

        _cachedView.Exit.onClick.AddListener(OnSubmit);
        _cachedView.SelectPhoto.onClick.AddListener(OnPhoto);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnDestroy()
        {
           
           

        }

        private void OnSubmit()
        {

            //Msg_SC_DAT_UserInfoDetail UpdateUserInfo = new Msg_SC_DAT_UserInfoDetail();
            //Debug.Log("name+++"+ _name);
            //UpdateUserInfo.UserInfoSimple.NickName = "111";
            ////UpdateUserInfo.UserInfoSimple.Sex = (ESex)_Male;
            ////UpdateUserInfo.UserInfoSimple.HeadImgUrl = _name;
            //RemoteCommands.UpdateUserInfo(UpdateUserInfo,
            //  (ret) =>
            //  {
            //  }, null
            //  );
            SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
        }

        private void UpdateView()
        {

        }

        public void SetHead(string HeadName)
        {
            Sprite fashion = null;
            if (ResourcesManager.Instance.TryGetSprite(HeadName, out fashion))
            {
                _cachedView.PhotoPortrait.sprite = fashion;
            }

        }

        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
            InitPanel();
            Exp();
            SetAchievement();

            LocalUser.Instance.UserPublishedWorldProjectList.Request(
                LocalUser.Instance.UserGuid,
                0, int.MaxValue,
                EPublishedProjectOrderBy.PPOB_PublishTime,
                EOrderType.OT_Asc,
                ()=> { SetRepresentativeWorks(); },
                null
                );


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
            _cachedView.Lvl.text = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel.ToString();
            _cachedView.CraftLvl.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp.ToString();
            _cachedView.Editing.gameObject.SetActiveEx(false);
            _cachedView.Editable.gameObject.SetActiveEx(true);

            _cachedView.EditDescBtn.onClick.AddListener(OnEditBtn);
            _cachedView.ConfirmDescBtn.onClick.AddListener(OnConfirmDescBtn);
            _cachedView.SelectMaleBtn.onClick.AddListener(SelectMale);
            _cachedView.SelectFemaleBtn.onClick.AddListener(SelectFemale);
            if (_isMale)
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

        private void OnEditBtn()
        {

            _cachedView.Editing.gameObject.SetActiveEx(true);
            _cachedView.Editable.gameObject.SetActiveEx(false);
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
                //Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curSelectedPrivateProject.Content);
            }

            string newSignature = _cachedView.SignatureDescInput.text;
            newSignature = CheckPSignatureValid(newSignature);
            if (!string.IsNullOrEmpty(newSignature) &&
                newSignature != _cachedView.SignatureDesc.text)
            {
                _cachedView.SignatureDesc.text = newSignature;
                //Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curSelectedPrivateProject.Content);
            }

            _cachedView.Editing.gameObject.SetActiveEx(false);
            _cachedView.Editable.gameObject.SetActiveEx(true);

        }


        private void SelectMale()
        {
            _cachedView.SelectMale.SetActiveEx(true);
            _cachedView.SelectFemale.SetActiveEx(false);
            _isMale = true;
            Sprite fashion = null;
            _cachedView.MSex.SetActiveEx(true);
            _cachedView.FSex.SetActiveEx(false);
        }

        private void SelectFemale()
        {
            _cachedView.SelectMale.SetActiveEx(false);
            _cachedView.SelectFemale.SetActiveEx(true);
            _isMale = false;
            Sprite fashion = null;
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

            long initialExp = currentPlayerExp -TableManager.Instance.Table_PlayerLvToExpDic[
            playerLevel].AdvExp;
            //_cachedView.CurExp.text = initialExp.ToString();
            _cachedView.CurExp.text = String.Format("{0}/{1}", initialExp, (TableManager.Instance.Table_PlayerLvToExpDic[playerLevel + 1].AdvExp - TableManager.Instance.Table_PlayerLvToExpDic[playerLevel].AdvExp));
            _cachedView.ExpBar.fillAmount = CountExpRatio(initialExp, playerLevel);

            int playerCraftLevel = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel;
            long currentPlayerCraftExp = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp;
            long initialCraftExp = currentPlayerCraftExp -TableManager.Instance.Table_PlayerLvToExpDic[
            playerCraftLevel].MakerExp;
            //_cachedView.CurCraftExp.text = initialCraftExp.ToString();
            _cachedView.CurCraftExp.text = String.Format("{0}/{1}", initialCraftExp, (TableManager.Instance.Table_PlayerLvToExpDic[playerCraftLevel + 1].MakerExp - TableManager.Instance.Table_PlayerLvToExpDic[playerCraftLevel].MakerExp));
            _cachedView.CraftExpBar.fillAmount = CountCraftExpRatio(initialCraftExp, playerCraftLevel);

        }

        private float CountExpRatio(float exp, int level)
        {
            return (exp
                    //- TableManager.Instance.Table_PlayerLvToExpDic[LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp
                    )
                    / (TableManager.Instance.Table_PlayerLvToExpDic[level + 1].AdvExp - TableManager.Instance.Table_PlayerLvToExpDic[level].AdvExp);
        }

        private float CountCraftExpRatio(float exp, int level)
        {
            return (exp
                    //- TableManager.Instance.Table_PlayerLvToExpDic[LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel].AdvExp
                    )
                    / (TableManager.Instance.Table_PlayerLvToExpDic[level + 1].MakerExp - TableManager.Instance.Table_PlayerLvToExpDic[level].MakerExp);
        }

        public void SetAchievement()
        {
            for (int i = 0; i < 10; i++)
            {
                var UM = new UMCtrlAchievement();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set();
                _cardList.Add(UM);
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
