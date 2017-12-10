using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlSliderSetting : USCtrlBase<USViewSliderSetting>
    {
        private int _min;
        private int _max;
        private int _cur;
        private Action<int> _callBack;
        private string _numFormat;
        private int _delta;

        public int Cur
        {
            get { return _cur; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LeftBtn.onClick.AddListener(OnLeftBtn);
            _cachedView.RightBtn.onClick.AddListener(OnRightBtn);
            _cachedView.Slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void Set(int min, int max, Action<int> callBack, int delta = 1, string numFormat = "{0}")
        {
            if (min >= max)
            {
                LogHelper.Error("min is equal or bigger than max");
                return;
            }
            if (delta < 1) delta = 1;
            _min = min;
            _max = max;
            _delta = delta;
            _callBack = callBack;
            _cachedView.Slider.minValue = _min;
            _cachedView.Slider.maxValue = _max;
            _numFormat = numFormat;
        }

        public void SetCur(int cur, bool init = true, int min = -1)
        {
            //用于更新最小值
            if (min > -1)
            {
                if (min >= _max)
                {
                    LogHelper.Error("min is equal or bigger than max");
                }
                else
                {
                    _min = min;
                    _cachedView.Slider.minValue = _min;
                }
            }
            cur = Mathf.Clamp(cur, _min, _max);
            //若是初始化，则只改变Slider.value，不走回调
            if (init)
            {
                _cur = cur;
            }
            _cachedView.Slider.value = cur;
            _cachedView.Num.text = string.Format(_numFormat, _cur);
        }

        private void OnRightBtn()
        {
            SetCur((_cur / _delta + 1) * _delta, false);
        }

        private void OnLeftBtn()
        {
            SetCur((_cur - 1) / _delta * _delta, false);
        }

        private void OnSliderValueChanged(float value)
        {
            var cur = (int) value;
            if (cur == _cur) return;
            _cur = cur;
            if (_numFormat != null)
            {
                _cachedView.Num.text = string.Format(_numFormat, _cur);
            }
            if (_callBack != null)
            {
                _callBack.Invoke((int) value);
            }
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }
    }
}