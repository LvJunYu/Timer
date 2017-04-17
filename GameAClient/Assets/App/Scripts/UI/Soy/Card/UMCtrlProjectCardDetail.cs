  /********************************************************************
  ** Filename : UMCtrlProjectCardDetail.cs
  ** Author : quan
  ** Date : 2016/7/28 14:44
  ** Summary : UMCtrlProjectCardDetail.cs
  ***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectCardDetail : UMCtrlCardBase<UMViewProjectCardDetail>
    {
        private const string EmptySummary = "作者太懒了，并没有留下什么作品简介~";
        private const string SummaryTemplate = "简介：{0}";
        private Project _content;
        private List<UMCtrlUser40> _recentCompleteUserList = new List<UMCtrlUser40>(5);
//        private List<UMCtrlProjectCommentSimple> _recentCommentList = new List<UMCtrlProjectCommentSimple>(3);

        public override object Data
        {
            get { return _content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Card.onClick.AddListener(OnCardClick);
            _cachedView.CommentBtn.onClick.AddListener(OnCommentBtnClick);
            for(int i = 0; i < 5; i++)
            {
                var item = new UMCtrlUser40();
                item.Init(_cachedView.RecentCompleteUserDock.GetComponent<RectTransform>());
                _recentCompleteUserList.Add(item);
            }

//            for(int i=0; i<3; i++)
//            {
//                var item = new UMCtrlProjectCommentSimple();
//                item.Init(_cachedView.CommentDock.GetComponent<RectTransform>());
//                _recentCommentList.Add(item);
//            }
        }

        protected override void OnDestroy()
        {
            _cachedView.Card.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCardClick()
        {
            ProjectParams param = new ProjectParams(){
                Type = EProjectParamType.Project,
                Project = _content
            };
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param);
        }

        private void OnAuthorClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(_content.UserLegacy);
        }

        private void OnFollowBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
            _content.UserLegacy.UpdateFollowState(!_content.UserLegacy.FollowedByMe, flag=>{
                if(flag)
                {
                    RefreshView();
                }
            });
        }

        private void OnCommentBtnClick()
        {
            ProjectParams param = new ProjectParams(){
                Type = EProjectParamType.Project,
                Project = _content
            };
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param).ChangeToCommentTab();
        }

        public override void Set(object obj)
        {
            _content = obj as Project;
            RefreshView();
        }

        public void RefreshView()
        {
            if(null == _content)
            {
                Unload();
                return;
            }
            User u = _content.UserLegacy;
            DictionaryTools.SetContentText(_cachedView.AuthorName, u.NickName);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.UserLegacy.HeadImgUrl, _cachedView.DefaultTexture);
            _cachedView.CreateTime.text =  DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_content.CreateTime);
            DictionaryTools.SetContentText(_cachedView.Name, _content.Name);
            string summary;
            if(string.IsNullOrEmpty(_content.Summary))
            {
                summary = EmptySummary;
            }
            else
            {
                summary = string.Format(SummaryTemplate, _content.Summary);
            }
            DictionaryTools.SetContentText(_cachedView.Summary, summary);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Icon, _content.IconPath, _cachedView.DefaultTexture);

            _cachedView.RateStar.SetRate(_content.TotalRate);
            DictionaryTools.SetContentText(_cachedView.RateCount, "(" + _content.TotalRateCount +")");
            _cachedView.ProjectCompleteRate.Set(_content.ExtendReady, _content.CompleteCount, _content.FailCount);

//            if(_content.ProjectCommentList == null || _content.ProjectCommentList.Count == 0)
			if (true)
            {
                DictionaryTools.SetContentText(_cachedView.CommentCount, "评论");
                _cachedView.CommentDock.gameObject.SetActive(false);
                _cachedView.CommentTip.SetActive(true);
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.CommentCount, ClientTools.FormatNumberString(_content.TotalCommentCount));
//                for(var i=0; i<_recentCommentList.Count; i++)
//                {
//                    if(i<_content.ProjectCommentList.Count)
//                    {
//                        _recentCommentList[i].Show();
//                        _recentCommentList[i].Set(_content.ProjectCommentList[i]);
//                    }
//                    else
//                    {
//                        _recentCommentList[i].Hide();
//                    }
//                }
                _cachedView.CommentDock.gameObject.SetActive(true);
                _cachedView.CommentTip.SetActive(false);
            }
            if(_content.RecentPlayedUserList == null || _content.RecentPlayedUserList.Count == 0)
            {
                for(var i=0; i<_recentCompleteUserList.Count; i++)
                {
                    _recentCompleteUserList[i].Hide();
                }
                _cachedView.RecentCompleteUserTip.gameObject.SetActive(true);
            }
            else
            {
                _cachedView.RecentCompleteUserTip.gameObject.SetActive(false);
                for(var i=0; i<_recentCompleteUserList.Count; i++)
                {
                    if(i<_content.RecentPlayedUserList.Count)
                    {
                        _recentCompleteUserList[i].Show();
                        _recentCompleteUserList[i].Set(_content.RecentPlayedUserList[i].User);
                    }
                    else
                    {
                        _recentCompleteUserList[i].Hide();
                    }
                }
            }
        }

        public override void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultTexture);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Icon, _cachedView.DefaultTexture);
            for(var i=0; i<_recentCompleteUserList.Count; i++)
            {
                _recentCompleteUserList[i].Hide();
            }
        }
    }
}
