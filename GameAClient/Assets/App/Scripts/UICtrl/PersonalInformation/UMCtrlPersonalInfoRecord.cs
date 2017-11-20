using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalInfoRecord : UMCtrlBase<UMViewPersonalInfoRecord>, IDataItemRenderer
    {
        private static string _successStr = "成功";
        private static string _failStr = "失败";
        private Record _record;
        public int Index { get; set; }

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
            _cachedView.PlayRecordBtn.onClick.AddListener(OnPlayRecordBtn);
            _cachedView.ProjectBtn.onClick.AddListener(OnProjectBtn);
        }

        protected override void OnDestroy()
        {
            _cachedView.PlayRecordBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnProjectBtn()
        {
            if (_record != null && _record.ProjectData != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(_record.ProjectData);
            }
        }

        private void OnPlayRecordBtn()
        {
            if (_record == null) return;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求进入录像");
            _record.ProjectData.PrepareRes(() =>
            {
                _record.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayRecord(_record.ProjectData, _record);
                    SocialApp.Instance.ChangeToGame();
                }, error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入录像失败");
                });
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("进入录像失败");
            });
        }

        public void Set(object obj)
        {
            _record = obj as Record;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_record == null)
            {
                Unload();
                return;
            }
            _cachedView.RecordNameTxt.text = _record.ProjectData.Name;
            _cachedView.DateTxt.text = GameATools.GetYearMonthDayHourMinuteSecondByMilli(_record.CreateTime, 1);
            _cachedView.SuccessTxt.text = _record.Result == (int) EGameResult.GR_Success ? _successStr : _failStr;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectImg, _record.ProjectData.IconPath,
                _cachedView.DefaultUserIconTexture);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.ProjectImg,
                _cachedView.DefaultUserIconTexture);
        }
    }
}