using SoyEngine;

namespace GameA
{
    public class UMCtrlOfficialProject : UMCtrlBase<UMViewOfficialProject>
    {
        private Project _project;
        private const string _descFormat = "简介：{0}";
        private const string _playerCountFormat = "游戏人数：{0}";

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Button.onClick.AddListener(OnBtn);
        }

        private void OnBtn()
        {
            if (_project != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(_project);
            }
        }

        protected override void OnDestroy()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
            base.OnDestroy();
        }

        public void Set(Project project)
        {
            _project = project;
            if (_project == null) return;
            _cachedView.TitleTxt.text = _project.Name;
            if (_project.NetData != null)
            {
                _cachedView.PlayerCountTxt.text = string.Format(_playerCountFormat, _project.NetData.PlayerCount);
            }
            _cachedView.DescTxt.text = string.Format(_descFormat, _project.Summary);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCoverTexture);
        }
    }
}