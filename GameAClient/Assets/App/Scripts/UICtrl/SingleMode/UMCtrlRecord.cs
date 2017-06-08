
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

            //_cachedView.PlayBtn.onClick.AddListener(OnPlayBtn);
        }

        protected override void OnDestroy()
        {
            //_cachedView.PlayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }


        private void OnPlayBtn()
        {
        }

        public void Set(object obj,string name)
        {
            _record = obj as Record;
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

   
