using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlSliderSetting : USCtrlBase<USViewSliderSetting>
    {
        private int _min;
        private int _max;
        private int _duration;
        private int _cur;
        private Action<int> _callBack;
        private string _numFormat;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LeftBtn.onClick.AddListener(OnLeftBtn);
            _cachedView.RightBtn.onClick.AddListener(OnRightBtn);
            _cachedView.Slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void Set(int min, int max, Action<int> callBack, string numFormat = "{0}")
        {
            if (min >= max)
            {
                LogHelper.Error("min is equal or bigger than max");
                return;
            }
            _min = min;
            _max = max;
            _callBack = callBack;
            _duration = _max - _min;
            _cachedView.Slider.minValue = _min;
            _cachedView.Slider.maxValue = _max;
            _numFormat = numFormat;
        }

        public void Set(int cur)
        {
            cur = Mathf.Clamp(cur, _min, _max);
            if (cur == _cur) return;
            _cur = cur;
            _cachedView.Slider.value = (cur - _min) / (float) _duration;
            _cachedView.Num.text = string.Format(_numFormat, _cur);
        }

        private void OnRightBtn()
        {
            Set(++_cur);
        }

        private void OnLeftBtn()
        {
            Set(--_cur);
        }

        private void OnSliderValueChanged(float value)
        {
            var cur = (int) value;
            if (cur == _cur) return;
            _cur = cur;
            _cachedView.Num.text = cur.ToString();
            if (_callBack != null)
            {
                _callBack.Invoke((int) value);
            }
        }
    }
}