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
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPersonalInformation : UICtrlInGameBase<UIViewPersonalInformation>, IUIWithTitle
    {
        #region 常量与字段
        private const string AccountSettingIconSpriteName = "face_s";
        private const string ClearCacheIconSpriteName = "face_s";
        private ListMenuView _listMenuView;
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

        _cachedView.Exit.onClick.AddListener(OnDestroy);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnDestroy()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
        }

        private void UpdateView()
        {

        }

        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
            InitPanel();
            Exp();
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

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "设置";
        }

        #endregion
    }
}
