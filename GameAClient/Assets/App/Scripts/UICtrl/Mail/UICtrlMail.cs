using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using System.IO;
using GameA.Game;


namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMail : UICtrlGenericBase<UIViewMail>
    {
        public List<UMCtrlMail> _cardList = new List<UMCtrlMail>();
        private int StartIndex = 0;
        private int MaxCount = 10;


        public void OnCloseBtnClick()
        {
            //CardList.Clear();
            SocialGUIManager.Instance.CloseUI<UICtrlMail>();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitGroupId();
            //_cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            //_cachedView.FollowCount.text = LocalUser.Instance.User.RelationStatistic.FollowCount.ToString();
            //_cachedView.FollowerCount.text = LocalUser.Instance.User.RelationStatistic.FollowerCount.ToString();
            //InitTagGroup();
            LoadMyMailList();
            _cachedView.DeleteAll.onClick.AddListener(Delete);
        }

        private void Delete()
        {
         List<long> idList = new List<long>();
         RemoteCommands.DeleteMail(
         EDeleteMailTargetType.EDMTT_All,
         idList,
         null, null
         );

        }

        private void LoadMyMailList()
        {
            LocalUser.Instance.Mail.Request(StartIndex, MaxCount,
            ()=>
            {
                Set(LocalUser.Instance.Mail.DataList);
            }
        ,
         null);

        }

        public void Set(List<Mail> fittingList)
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }
            _cardList.Clear();
            for (int i = 0; i < fittingList.Count; i++)
            {
                SetEachCard(fittingList[i]);
            }
            //RefreshPage();
        }

        private void SetEachCard(Mail fitting)
        {
            if (_cachedView != null)
            {
                var UM = new UMCtrlMail();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(fitting);
                _cardList.Add(UM);
            }
        }

         protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

       

    }
}
