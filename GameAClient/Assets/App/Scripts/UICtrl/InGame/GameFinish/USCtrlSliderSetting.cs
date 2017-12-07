using SoyEngine;

namespace GameA
{
    public class USCtrlSliderSetting : USCtrlBase<USViewSliderSetting>
    {
        private int _min;
        private int _max;
        private int _duration;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LeftBtn.onClick.AddListener(OnLeftBtn);
            _cachedView.RightBtn.onClick.AddListener(OnRightBtn);
            _cachedView.Slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void Set(int min, int max)
        {
            if (min >= max)
            {
                LogHelper.Error("min is equal or bigger than max");
                return;
            }
            _min = min;
            _max = max;
            _duration = _max - _min;
            _cachedView.Slider.minValue = _min;
            _cachedView.Slider.maxValue = _max;
        }

        public void Set(int cur)
        {
            _cachedView.Num.text = cur.ToString();
            _cachedView.Slider.value = (cur - _min) / (float) _duration;
        }

        private void OnRightBtn()
        {
        }

        private void OnLeftBtn()
        {
        }
        
        private void OnSliderValueChanged(float arg0)
        {
        }
    }
}