/********************************************************************
** Filename : LocalUser
** Author : Dong
** Date : 2015/4/7 16:00:53
** Summary : LocalUser
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class LocalUser : DataBase
    {
        #region 常量与字段

        public static readonly LocalUser Instance = new LocalUser();

        private readonly Account _account = Account.Instance;
        private UserInfoDetail _user;
        private readonly UsingAvatarPart _usingAvatarData = new UsingAvatarPart();
        private readonly MailList _mailList = new MailList();
        private readonly ValidAvatarPart _validAvatarData = new ValidAvatarPart();
        private readonly RelationUserList _relationUserList = new RelationUserList();

        private readonly AddUserList _addUserList = new AddUserList();

        // 抽奖相关数据
        private readonly UserRaffleTicket _userRaffleTicket = new UserRaffleTicket();

        // 匹配挑战相关数据
        private readonly MatchUserData _matchUserData = new MatchUserData();

        // 增益道具
        private readonly UserProp _userProp = new UserProp();

        private readonly PersonalProjectList _personalProjectList = new PersonalProjectList();

        private readonly UserPublishedWorldProjectList _userPublishedWorldProjectList =
            new UserPublishedWorldProjectList();

        // 工坊地块数量上限数据Z
        private readonly UserWorkshopUnitData _userWorkshopUnitData = new UserWorkshopUnitData();

        //拼图数据
        private readonly UserPictureFull _userPictureFull = new UserPictureFull();

        private readonly UserUsingPictureFullData _userUsingPictureFullData = new UserUsingPictureFullData();

        private readonly UserPicturePart _userPicturePart = new UserPicturePart();

        //武器数据
        private readonly UserWeaponData _userWeaponData = new UserWeaponData();

        private readonly UserWeaponPartData _userWeaponPartData = new UserWeaponPartData();

        //训练数据
        private readonly UserTrainProperty _userTrainProperty = new UserTrainProperty();

        //成就数据
        private readonly Achievement _achievement = new Achievement();

        //预设数据
        private readonly UnitPreinstallList _unitPreinstallList = new UnitPreinstallList();

        private readonly QQGameReward _qqGameReward = new QQGameReward();

        private readonly NpcDialogPreinstallList _npcDialogPreinstallList = new NpcDialogPreinstallList();

        #endregion

        #region 属性

        public long UserGuid
        {
            get { return _account.UserGuid; }
        }

        public Account Account
        {
            get { return _account; }
        }

        public User UserLegacy
        {
            get { return null; }
        }

        public UserInfoDetail User
        {
            get { return _user; }
        }

        public MailList Mail
        {
            get { return _mailList; }
        }

        public RelationUserList RelationUserList
        {
            get { return _relationUserList; }
        }

        public AddUserList AddUserList
        {
            get { return _addUserList; }
        }

        public UsingAvatarPart UsingAvatarData
        {
            get { return _usingAvatarData; }
        }

        public ValidAvatarPart ValidAvatarData
        {
            get { return _validAvatarData; }
        }

        public QQGameReward QqGameReward
        {
            get { return _qqGameReward; }
        }

        /// <summary>
        /// 抽奖相关数据
        /// </summary>
        /// <value>The raffle ticket.</value>
        public UserRaffleTicket RaffleTicket
        {
            get { return _userRaffleTicket; }
        }

        public MatchUserData MatchUserData
        {
            get { return _matchUserData; }
        }

        public PersonalProjectList PersonalProjectList
        {
            get { return _personalProjectList; }
        }

        public UserPublishedWorldProjectList UserPublishedWorldProjectList
        {
            get { return _userPublishedWorldProjectList; }
        }

        // 增益道具
        public UserProp UserProp
        {
            get { return _userProp; }
        }

        // 工坊地块数量上限数据
        public UserWorkshopUnitData UserWorkshopUnitData
        {
            get { return _userWorkshopUnitData; }
        }

        //拼图数据
        public UserPictureFull UserPictureFull
        {
            get { return _userPictureFull; }
        }

        public UserUsingPictureFullData UserUsingPictureFullData
        {
            get { return _userUsingPictureFullData; }
        }

        public UserPicturePart UserPicturePart
        {
            get { return _userPicturePart; }
        }

        //武器数据
        public UserWeaponData UserWeaponData
        {
            get { return _userWeaponData; }
        }

        public UserWeaponPartData UserWeaponPartData
        {
            get { return _userWeaponPartData; }
        }

        //训练数据
        public UserTrainProperty UserTrainProperty
        {
            get { return _userTrainProperty; }
        }

        //成就数据
        public Achievement Achievement
        {
            get { return _achievement; }
        }

        public UnitPreinstallList UnitPreinstallList
        {
            get { return _unitPreinstallList; }
        }

        public NpcDialogPreinstallList NpcDialogPreinstallList
        {
            get { return _npcDialogPreinstallList; }
        }

        #endregion

        #region 方法

        private LocalUser()
        {
            Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLogout, OnLogout);
        }

        public bool Init()
        {
            _account.ReadCache();
            return true;
        }

        public void LoadUserData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            //            Msg_CS_DAT_UserInfoDetail msg = new Msg_CS_DAT_UserInfoDetail();
            //            msg.UserId = UserGuid;
            //            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserInfoDetail>(SoyHttpApiPath.UserInfoDetail, msg, ret =>
            //            {
            //                _user = UserManager.Instance.OnSyncUserData(ret);
            //                if (successCallback != null)
            //                {
            //                    successCallback.Invoke();
            //                }
            //            }, (errorCode, errorMsg) => {
            //                if (failedCallback != null)
            //                {
            //                    failedCallback.Invoke(errorCode);
            //                }
            //            });
            if (_user == null)
            {
                _user = new UserInfoDetail();
            }
            _user.Request(
                UserGuid,
                successCallback,
                failedCallback
            );
        }

        public void LoadPropData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _userProp.Request(
                UserGuid,
                successCallback,
                failedCallback
            );
        }

        public void LoadWorkshopUnitData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _userWorkshopUnitData.Request(
                UserGuid,
                successCallback,
                failedCallback
            );
        }

        private void OnLogout()
        {
            if (_user != null)
            {
                _user.OnLogout();
                _user = null;
            }
        }

        #endregion
    }
}