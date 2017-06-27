using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using System.IO;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlSocialRelationship : UICtrlGenericBase<UIViewSocialRelationship>
    {
        private int _startIndex = 0;
        private int _endIndex = 10;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitGroupId();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            _cachedView.FollowCount.text = LocalUser.Instance.User.RelationStatistic.FollowCount.ToString();
            _cachedView.FollowerCount.text = LocalUser.Instance.User.RelationStatistic.FollowerCount.ToString();
            LoadMyRelationUserList();
            LoadMyRelationStatistic();
        }

        public void OnCloseBtnClick()
        {
            //CardList.Clear();
            SocialGUIManager.Instance.CloseUI<UICtrlSocialRelationship>();
        }

        private void LoadMyRelationUserList()
        {
            LocalUser.Instance.RelationUserList.Request(
                LocalUser.Instance.UserGuid,
                ERelationUserType.RUT_FollowedByMe,
                _startIndex,
                _endIndex,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc,
                null,
                code => { LogHelper.Error("Network error when get ReFreshMyRelationUserList, {0}", code); }
                );
        }

        private void LoadMyRelationStatistic()
        {
            LocalUser.Instance.LoadUserData(
                null,
                code => { LogHelper.Error("Network error when get RefreshMyRelationStatistic, {0}", code); }
                );
        }
    }
}