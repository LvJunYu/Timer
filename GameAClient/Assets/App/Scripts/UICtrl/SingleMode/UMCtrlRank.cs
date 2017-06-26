
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlRank : UMCtrlBase<UMViewRank>
    {
        private Record _record;
        private int _index;
        private string First= "icon_first";
        private string Second= "icon_second";
        private string Third= "icon_third";


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
            _cachedView.Playerlvl.text = _record.UserInfo.LevelData.PlayerLevel.ToString();
            RefreshView();
            JudgeRankImage(rank);
        }

        public void JudgeRankImage(int rank)
        {
            if (rank == 1)
            {
                Sprite fashion = null;
                // todo update api
//                if (GameResourceManager.Instance.TryGetSpriteByName(First, out fashion))
//                {
//                    _cachedView.RankBg.sprite = fashion;
//                }
            }
            else if (rank == 2)
            {
                Sprite fashion = null;
                // todo update api
//                if (GameResourceManager.Instance.TryGetSpriteByName(Second, out fashion))
//                {
//                    _cachedView.RankBg.sprite = fashion;
//                }
            }
            else if (rank == 3)
            {
                Sprite fashion = null;
                // todo update api
//                if (GameResourceManager.Instance.TryGetSpriteByName(Third, out fashion))
//                {
//                    _cachedView.RankBg.sprite = fashion;
//                }
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
    }
}