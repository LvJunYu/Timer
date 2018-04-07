using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public enum TimeType
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Max
    }

    public class USCtrlTimePick : USCtrlBase<USViewTimePicker>
    {
        private int _time;

        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private int _min;
        private int _max;
        private TimeType _type;
        private Action _refreshAction;
        private DateTime _dateTime;
        private List<int> _timeItem;

        protected override void OnViewCreated()
        {
            _cachedView.AddBtn.onClick.AddListener(OnAddBtn);
            _cachedView.DownBtn.onClick.AddListener(OnLessBtn);

            base.OnViewCreated();
        }

        public void SetType(TimeType type, Action refreshcallback = null)
        {
            _type = type;
            _refreshAction = refreshcallback;
        }

        public void SetOrgTime(List<int> timeItem)
        {
            _timeItem = timeItem;
            _dateTime = OfficalData.GetTimeByItems(_timeItem);
            SetMinAddMax();
        }

        private void OnAddBtn()
        {
            _time = Mathf.Clamp(++_time, _min, _max);
            SetTime();
        }

        private void OnLessBtn()
        {
            _time = Mathf.Clamp(--_time, _min, _max);
            SetTime();
        }

        private void SetMinAddMax()
        {
            DateTime nowtime = DateTimeUtil.GetServerTimeNow();
            switch (_type)
            {
                case TimeType.Year:
                    _min = nowtime.Year;
                    _max = nowtime.Year + 10;
                    break;
                case TimeType.Month:
                    _min = 1;
                    _max = 12;
                    break;
                case TimeType.Day:
                    _min = 1;
                    _max = DateTime.DaysInMonth(_dateTime.Year,
                        _dateTime.Month);
                    break;
                case TimeType.Hour:
                    _min = 1;
                    _max = 23;
                    break;
                case TimeType.Minute:
                    _min = 0;
                    _max = 59;
                    break;
            }

            _time = _timeItem[(int) _type];
            _cachedView.TimeTxt.text = _time.ToString();
        }

        private void SetTime()
        {
            int temp = _timeItem[(int) _type];
            _timeItem[(int) _type] = _time;
            _timeItem[(int) TimeType.Day] =
                Mathf.Clamp(_timeItem[(int) TimeType.Day], 1, DateTime.DaysInMonth(
                    _timeItem[(int) TimeType.Year],
                    _timeItem[(int) TimeType.Month]));
            DateTime date = new DateTime(_timeItem[(int) TimeType.Year],
                _timeItem[(int) TimeType.Month],
                _timeItem[(int) TimeType.Day],
                _timeItem[(int) TimeType.Hour],
                _timeItem[(int) TimeType.Minute],
                0);
            if (date > DateTime.Now)
            {
                _dateTime = date;
            }
            else
            {
                _time = temp;
                _timeItem[(int) _type] = _time;
            }

            _cachedView.TimeTxt.text = _time.ToString();
            if (_refreshAction != null)
            {
                _refreshAction.Invoke();
            }
        }

        public void RefreshTime()
        {
            _time = _timeItem[(int) _type];
            _cachedView.TimeTxt.text = _time.ToString();
        }
    }
}