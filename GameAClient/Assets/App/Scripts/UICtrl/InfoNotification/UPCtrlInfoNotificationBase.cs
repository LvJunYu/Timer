using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlInfoNotificationBase : UPCtrlBase<UICtrlInfoNotification, UIViewInfoNotification>
    {
        protected EResScenary _resScenary;
        protected UICtrlMail.EMenu _menu;
        protected List<Mail> _dataList;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.TableDataScrollers[(int) _menu].Set(OnItemRefresh, GetTalkItemRenderer);
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

        protected virtual void RefreshData()
        {
        }

        protected void TempData()
        {
            if (_dataList != null && _dataList.Count != 0) return;
            _dataList = new List<Mail>(10);
            for (int i = 0; i < 10; i++)
            {
                Msg_Mail mail = new Msg_Mail();
                mail.FuncType = i % 2 == 0 ? EMailFuncType.MFT_Reward : EMailFuncType.MFT_ShareProject;
                mail.UserInfo = new Msg_SC_DAT_UserInfoSimple();
                mail.UserInfo.UserId = 3400 + i;
                mail.UserInfo.NickName = "赵四" + i;
                mail.CreateTime = 9000000000000 + i * 10000000;
                mail.Title = "请赐予我力量！";
                mail.Content = "你被神秘力量看中，快，拯救世界的使命就交给你了！";
                mail.AttachItemList = new Msg_Reward();

                _dataList.Add(new Mail(mail));
            }
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.TableDataScrollers[(int) _menu].SetEmpty();
                _cachedView.EmptyObj.SetActive(true);
            }
            else
            {
                _cachedView.EmptyObj.SetActive(_dataList.Count == 0);
                _cachedView.TableDataScrollers[(int) _menu].SetItemCount(_dataList.Count);
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

        public void OnMailListChanged()
        {
            RefreshData();
        }
    }
}