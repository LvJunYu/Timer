/********************************************************************
** Filename : UIViewSoy
** Author : Dong
** Date : 2015/4/30 16:34:49
** Summary : UIViewSoy
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewMe : UIViewBase
    {
        public GameObject UserInfoDock;
        public GameObject LoginBtnDock;
        public RawImage UserHeadImg;
        public Image SexImg;
        public Text NickName;
        public Text Profile;
        public Text CurrencyText;
        public UIUserLevel PlayerLevel;
        public UIUserLevel CreatorLevel;
        public Button EditUserInfoBtn;
        public Button UserInfoBtn;
        public Button FollowListBtn;
        public Text FollowListText;
        public Button FollowerListBtn;
        public Text FollowerListText;
        public Button LoginBtn;
        public Button RegisterBtn;
        public Button RightSettingButtonResource;
        public Texture DefaultUserHeadTexture;

        public UIGridMenuItem MyPublishedProjectBtn;
        public UIGridMenuItem MyRecordBtn;
        public UIGridMenuItem MyFavoriteProjectBtn;
        public UIGridMenuItem MyPlayHistoryBtn;

        public UIGridMenuItem ProjectCommentRemindBtn;
        public UIGridMenuItem ProjectReplyRemindBtn;
        public UIGridMenuItem ProjectRateRemindBtn;
        public UIGridMenuItem AnnounceBtn;
        public UIGridMenuItem RecordCommentRemindBtn;
        public UIGridMenuItem NewFollowerRemindBtn;
    }
}
