using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlMailBase : UPCtrlBase<UICtrlMail, UIViewMail>
    {
        protected EResScenary _resScenary;
        protected UICtrlMail.EMenu _menu;
        protected List<Mail> _dataList;
        protected int _startIndex = 0;
        protected int _maxCount = 50;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetTalkItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RefreshData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _dataList = null;
        }

        public void RefreshData()
        {
            LocalUser.Instance.Mail.Request(_startIndex, _maxCount, () =>
                {
                    _dataList = LocalUser.Instance.Mail.DataList;
                    RefreshView();
                },
                code =>
                {
                    TempData();
//                    SocialGUIManager.ShowPopupDialog("请求邮箱数据失败。");
                });
        }

        private void TempData()
        {
            if (_dataList != null && _dataList.Count != 0) return;
            _dataList = new List<Mail>(10);
            for (int i = 0; i < 10; i++)
            {
                Msg_Mail mail = new Msg_Mail();
                mail.Type = i % 2 == 0 ? EMailType.EMailT_Gift : EMailType.EMailT_ShadowBattleHelp;
                mail.UserInfo = new Msg_SC_DAT_UserInfoSimple();
                mail.UserInfo.UserId = 3400 + i;
                mail.UserInfo.NickName = "赵四" + i;
                mail.CreateTime = 9000000000000 + i * 10000000;
                mail.Title = "请赐予我力量！";
                mail.Content = "你好，这是一封测试邮件！";
                mail.AttachItemList = new Msg_Reward();
                
                _dataList.Add(new Mail(mail));
            }
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
            }
            else
            {
                _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_dataList.Count);
            }
        }

        protected void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _dataList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_dataList[inx]);
        }

        protected IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlMail();
            item.Init(parent, _resScenary);
            return item;
        }

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlMail.EMenu menu)
        {
            _menu = menu;
        }
    }
}