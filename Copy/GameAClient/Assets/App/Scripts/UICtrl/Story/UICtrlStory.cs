using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlStory : UICtrlResManagedBase<UIViewStory>
    {
        #region 常量与字段

        private Animator[][] _animatorAry;
        private int _curPage;
        private int _curPic;

        private bool _isPlaying;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.AppGameUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            var pageAry = new[] {_cachedView.Page1PicAry, _cachedView.Page2PicAry, _cachedView.Page3PicAry};
            _animatorAry = new Animator[pageAry.Length][];
            for (int i = 0; i < pageAry.Length; i++)
            {
                _cachedView.PageRootAry[i].SetActive(true);
                _animatorAry[i] = new Animator[pageAry[i].Length];
                for (int j = 0; j < pageAry[i].Length; j++)
                {
                    _animatorAry[i][j] = pageAry[i][j].GetComponent<Animator>();
                    pageAry[i][j].SetActive(false);
                }
            }
            _cachedView.NextBtn.onClick.AddListener(OnNextClick);
            _cachedView.SkipBtn.onClick.AddListener(OnSkipClick);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Play();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_isPlaying && _animatorAry[_curPage][_curPic].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                PlayNext();
            }
        }

        private void PlayNext()
        {
            _curPic++;
            if (_curPic >= _animatorAry[_curPage].Length)
            {
                _isPlaying = false;
                _cachedView.NextBtn.SetActiveEx(true);
                _cachedView.NextBtn.GetComponentInChildren<Animator>().Play(0);
            }
            else
            {
                Play();
            }
        }

        private void Play()
        {
            _isPlaying = true;
            _animatorAry[_curPage][_curPic].SetActiveEx(true);
            _animatorAry[_curPage][_curPic].Play(0);
        }

        private void OnSkipClick()
        {
            _isPlaying = false;
            SocialGUIManager.Instance.CloseUI<UICtrlStory>();
            if (LocalUser.Instance.User.LoginCount == 1)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlSingleMode>();
            }
        }

        private void OnNextClick()
        {
            for (int i = 0; i < _animatorAry[_curPage].Length; i++)
            {
                _animatorAry[_curPage][i].SetActiveEx(false);
            }
            _cachedView.NextBtn.SetActiveEx(false);
            _curPage++;
            _curPic = 0;
            if (_curPage < _animatorAry.Length)
            {
                Play();
            }
            else
            {
                OnSkipClick();
            }
        }

        #endregion
    }
}