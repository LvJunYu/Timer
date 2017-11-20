
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlRecord : UMCtrlBase<UMViewRecord>
    {
        private Record _record;
        private int _index;
        private Project _project;

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

        private void OnPlayBtn()
        {
            if (_record == null)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, string.Format ("请求进入录像"));

            _project.PrepareRes(() =>
            {
                _record.RequestPlay (() => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    UICtrlAdvLvlDetail uictrlAdvLvlDetail = SocialGUIManager.Instance.GetUI<UICtrlAdvLvlDetail>();
                    SituationAdventureParam param = new SituationAdventureParam();
                    param.ProjectType = uictrlAdvLvlDetail.ProjectType;
                    param.Section = uictrlAdvLvlDetail.ChapterIdx;
                    param.Level = uictrlAdvLvlDetail.LevelIdx;
                    param.Record = _record;
                    GameManager.Instance.RequestPlayAdvRecord (uictrlAdvLvlDetail.Project, param);
                    SocialApp.Instance.ChangeToGame();
                }, (error) => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    SocialGUIManager.ShowPopupDialog("进入录像失败");
                });
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                SocialGUIManager.ShowPopupDialog("进入录像失败");
            });
        }

        public void Set(Record record, Project project, string name)
        {
            _record = record;
            _project = project;
            _cachedView.TapName.text = name;
            _cachedView.SubmitTime.text = GetSubmitTime();
            _cachedView.Duration.text = GameATools.SecondToHour(_record.UsedTime);
            //RefreshView();

        }

        private string GetSubmitTime()
        {
            long time = _record.CreateTime;
            return GameATools.GetYearMonthDayHourMinuteSecondByMilli(time,1);
        }

        }

    }

    //private string JudgeTapName()
    //{
    //    string name=null;
    //    return name;
    //}

   
