
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using NewResourceSolution;

namespace GameA
{
    public class UMCtrlRank : UMCtrlBase<UMViewRank>
    {
        private Record _record;
        private int _index;
        private string First= "icon_crown_1";
        private string Second= "icon_crown_2";
        private string Third= "icon_crown_3";
        private bool _currentfollowstate =false;
        private UserInfoDetail _userInfoDetail;

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _record; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
           
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtn);
            //_cachedView.Follow.onClick.AddListener(Follow);

            //_userInfoDetail = new UserInfoDetail(_record.UserInfo);
            //JudgeRelationshipWithMe(_userInfoDetail);
            //Debug.Log("______UserInfo__________"+_record.UserInfo);
            //UserInfoDetail userInfoDetail = new UserInfoDetail(_record.UserInfo);
            //sJudgeRelationshipWithMe(_userInfoDetail);
        }

        protected override void OnDestroy()
        {
            //_cachedView.PlayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }


        private void OnPlayBtn()
        {
            if (_record == null)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, string.Format ("请求进入录像"));

            _record.RequestPlay (() => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                UICtrlAdvLvlDetail uictrlAdvLvlDetail = SocialGUIManager.Instance.GetUI<UICtrlAdvLvlDetail>();
                SituationAdventureParam param = new SituationAdventureParam();
                param.ProjectType = uictrlAdvLvlDetail.ProjectType;
                param.Section = uictrlAdvLvlDetail.ChapterIdx;
                param.Level = uictrlAdvLvlDetail.LevelIdx;
                param.Record = _record;
                GameManager.Instance.RequestPlayAdvRecord (uictrlAdvLvlDetail.Project, param);
                SocialGUIManager.Instance.ChangeToGameMode ();
            }, (error) => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                SocialGUIManager.ShowPopupDialog("进入录像失败");
            });
        }

        public void Set(object obj,int rank)
        {
            _record = obj as Record;
            _cachedView.NickName.text = _record.UserInfo.NickName;
            _cachedView.Score.text = _record.Score.ToString();
            _cachedView.Rank.text = rank.ToString();
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadPortrait,
                _record.UserInfo.HeadImgUrl,
                _cachedView.DefaultUserHeadTexture);
           
            _cachedView.Playerlvl.text = _record.UserInfo.LevelData.PlayerLevel.ToString();
            RefreshView();
            JudgeRankImage(rank);
            _userInfoDetail = new UserInfoDetail(_record.UserInfo);
            //JudgeRelationshipWithMe(_userInfoDetail);
        }

        public void JudgeRankImage(int rank)
        {
            if (rank == 1)
            {
                _cachedView.Rank.SetActiveEx(false);
                Sprite fashion = null;
                if (ResourcesManager.Instance.TryGetSprite(First, out fashion))
                {
                    _cachedView.RankBg.sprite = fashion;
                }
            }
            else if (rank == 2)
            {
                _cachedView.Rank.SetActiveEx(false);
                Sprite fashion = null;
                if (ResourcesManager.Instance.TryGetSprite(Second, out fashion))
                {
                    _cachedView.RankBg.sprite = fashion;
                }
            }
            else if (rank == 3)
            {
                _cachedView.Rank.SetActiveEx(false);
                Sprite fashion = null;
                if (ResourcesManager.Instance.TryGetSprite(Third, out fashion))
                {
                    _cachedView.RankBg.sprite = fashion;
                }
            }
            else
            {
                _cachedView.RankBg.SetActiveEx(false);
            }

        }

        public void RefreshView()
        {
            if (_record == null)
            {
                return;
            }
            else
            {
                //                _cachedView.Title = 
                //                _cachedView.InfoDock.SetActive(true);
                //                DictionaryTools.SetContentText(_cachedView.Title, _wrapper.Content.Name);
                //                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _wrapper.Content.IconPath, _cachedView.DefaultCoverTexture);
                ////                _cachedView.SeletedMark.SetActiveEx (_wrapper.IsSelected);
                //                _cachedView.PublishTime.text = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_wrapper.Content.CreateTime);
                ////                DictionaryTools.SetContentText(_cachedView.ProjectCategoryText, EnumStringDefine.GetProjectCategoryString(_wrapper.Content.ProjectCategory));
            }
        }

        public void Unload()
        {
        }

        protected void RefreshCardMode(ECardMode mode, bool isSelected)
        {
            //            _cardMode = mode;
            //            if(_cardMode == ECardMode.Selectable)
            //            {
            //                _cachedView.SelectableMask.enabled = true;
            //                _cachedView.SeletedMark.enabled = isSelected;
            //                _cachedView.UnsetectMark.enabled = !isSelected;
            //            }
            //            else
            //            {
            //                _cachedView.SelectableMask.enabled = false;
            //                _cachedView.SeletedMark.enabled = false;
            //                _cachedView.UnsetectMark.enabled = false;
            //            }
        }

        //public void Follow()
        //{
        //    //Debug.Log("______UserInfo__________" + _record.UserInfo);

        //    //UserInfoDetail userInfoDetail = new UserInfoDetail(_record.UserInfo);
        //    //JudgeRelationshipWithMe(userInfoDetail);
        //    _userInfoDetail.UpdateFollowState(!_currentfollowstate, () =>
        //    {

        //        ChangeFollowState();
        //        //JudgeRelationshipWithMe(_userInfoDetail);
        //        //SocialGUIManager.Instance.GetUI<UICtrlAdvLvlDetail>().RefreshAdventureUserLevelDataDetail();
        //    });
        
        //}

        //private void ChangeFollowState()
        //{
        //    _currentfollowstate = !_currentfollowstate;
        //    if (_currentfollowstate)
        //        _cachedView.ShowFollow.text = "取消关注";
        //    else
        //        _cachedView.ShowFollow.text = "关注";
            
        //}

        //private void JudgeRelationshipWithMe(UserInfoDetail userInfoDetail)
        //{
        //    _currentfollowstate = userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe;
        //    Debug.Log("__________关注状态___" + userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe);
        //    if (_currentfollowstate)
        //        _cachedView.ShowFollow.text = "取消关注";
        //    else
        //        _cachedView.ShowFollow.text = "关注";

        //}
    }
}