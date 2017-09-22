using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlSocialRelationship : UICtrlAnimationBase<UIViewSocialRelationship>
    {
        private USCtrlSocialRelationship _usctrlPage1;
        private USCtrlSocialRelationship _usctrlPage2;
        //public List<UMCtrlRelationCard> _cardList = new List<UMCtrlRelationCard>();
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
            InitTagGroup();
            LoadMyRelationStatistic();
        }

        public void OnCloseBtnClick()
        {
            //CardList.Clear();
            SocialGUIManager.Instance.CloseUI<UICtrlSocialRelationship>();
        }

        private void LoadMyFollowUserList()
        {
            LocalUser.Instance.FollowRelationUserList.CS_DataType = ERelationUserType.RUT_FollowedByMe;
            Debug.Log("___UserGuid___" + LocalUser.Instance.UserGuid);
            LocalUser.Instance.FollowRelationUserList.RequestFollowList(
                LocalUser.Instance.UserGuid,
                _startIndex,
                _endIndex,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc,
                () =>
                {
                    _usctrlPage1.Set(LocalUser.Instance.FollowRelationUserList.FollowRelationList, ResScenary);
                    
                },
                code => { LogHelper.Error("Network error when get ReFreshMyRelationUserList, {0}", code); }
                );
        }

        private void LoadMyBlockUserList()
        {
            LocalUser.Instance.BlockRelationUserList.CS_DataType = ERelationUserType.RUT_BlockByMe;
            LocalUser.Instance.BlockRelationUserList.RequestBlockList(
                LocalUser.Instance.UserGuid,
                _startIndex,
                _endIndex,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc,
                () =>
                {
                    _usctrlPage2.Set(LocalUser.Instance.BlockRelationUserList.BlockRelationList, ResScenary);
                },
                code => { LogHelper.Error("Network error when get ReFreshMyRelationUserList, {0}", code); }
                );
        }

        private void LoadMyRelationStatistic()
        {
            LocalUser.Instance.LoadUserData(
                 () =>
                 {
                     _cachedView.FollowCount.text = LocalUser.Instance.User.RelationStatistic.FollowCount.ToString();
                     _cachedView.FollowerCount.text = LocalUser.Instance.User.RelationStatistic.FollowerCount.ToString();
                 },
                code => { LogHelper.Error("Network error when get RefreshMyRelationStatistic, {0}", code); }
                );
        }

        protected override void OnOpen(object parameter)
        {
            LoadMyRelationStatistic();
            LoadMyBlockUserList();
            LoadMyFollowUserList();
        }

        //private void SetData()
        //{
        //    _usctrlPage1.Set(LocalUser.Instance.RelationUserList.FollowRelationList);
        //    _usctrlPage2.Set(LocalUser.Instance.RelationUserList.BlockRelationList);


        //}
        //public void Set(List<UserInfoDetail> fittingList)
        //{
        //    if (_cardList.Count > 0)
        //    {
        //        for (int i = 0; i < _cardList.Count; i++)
        //        {
        //            _cardList[i].Destroy();
        //        }
        //    }
        //    _cardList.Clear();
        //    for (int i = 0; i < fittingList.Count; i++)
        //    {
        //        SetEachCard(fittingList[i]);
        //    }
        //    //RefreshPage();
        //}

        //private void SetEachCard(UserInfoDetail fittingFashion)
        //{
        //    if (_cachedView != null)
        //    {
        //        var UM = new UMCtrlRelationCard();
        //        UM.Init(_cachedView.Dock as RectTransform);
        //        UM.Set(fittingFashion);
        //        _cardList.Add(UM);
        //    }
        //}

        private void InitTagGroup()
        {
            _cachedView.TagGroup.AddButton(_cachedView.USView.Page1Btn, OnPage1ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USView.Page2Btn, OnPage2ButtonClick);

            _usctrlPage1 = new USCtrlSocialRelationship();
            _usctrlPage1.Init(_cachedView.Page1);
            _usctrlPage1.RelationPage = USCtrlSocialRelationship.RelationshipPage.Relationshippage1;

            _usctrlPage2 = new USCtrlSocialRelationship();
            _usctrlPage2.Init(_cachedView.Page2);
            _usctrlPage2.RelationPage = USCtrlSocialRelationship.RelationshipPage.Relationshippage2;

        }


        private void OnPage1ButtonClick(bool open)
        {
            if (open)
            {
                _usctrlPage1.Open();
            }
            else
            {
                _usctrlPage1.Close();
            }
        }

        private void OnPage2ButtonClick(bool open)
        {
            if (open)
            {
                _usctrlPage2.Open();
            }
            else
            {
                _usctrlPage2.Close();
            }
        }

    }
}