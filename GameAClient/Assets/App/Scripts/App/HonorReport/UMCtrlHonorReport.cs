using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlHonorReport : UMCtrlBase<UMViewHonorReport>
    {
        private UserInfoDetail _user;
        private const float _speed = 70;
        public bool IsShow;
        private float _width;

        public float RightPosX
        {
            get
            {
                return _cachedView.rectTransform().anchoredPosition.x +
                       _cachedView.rectTransform().rect.width;
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
        }

        public void SetDisplay(bool value)
        {
            if (IsShow == value) return;
            _cachedView.SetActiveEx(value);
            IsShow = value;
        }

        public void Set(UserInfoDetail user, Vector2 pos)
        {
            _cachedView.rectTransform().anchoredPosition = pos;
            _user = user;
            _cachedView.ContentTxt.text = string.Format("{0}完成了惊人的成就，大家都来祝贺他吧！", _user.UserInfoSimple.NickName);
            Canvas.ForceUpdateCanvases();
            _width = _cachedView.rectTransform().rect.width;
        }

        public void OnUpdate()
        {
            if (!IsShow) return;
            if (_cachedView.rectTransform().anchoredPosition.x < -UICtrlHonorReport.BgWidth - _width)
            {
                SetDisplay(false);
            }
            else
            {
                _cachedView.rectTransform().anchoredPosition += Vector2.left * _speed * Time.deltaTime;
            }
        }
    }
}