using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlInfoNotificationBase : UPCtrlBase<UICtrlInfoNotification, UIViewInfoNotification>
    {
        protected const int MaxCount = 20;
        protected EResScenary _resScenary;
        protected UICtrlInfoNotification.EMenu _menu;
        protected List<NotificationDataItem> _dataList = new List<NotificationDataItem>();
        protected NotificationData _notificationData = new NotificationData();
        protected bool _isEnd;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.TableDataScrollers[(int) _menu].Set(OnItemRefresh, GetTalkItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RequestData();
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

        protected virtual void RequestData(bool append = false)
        {
            int startIndex = 0;
            if (append)
            {
                if (_isEnd)
                {
                    return;
                }

                startIndex = _dataList.Count;
            }

            _notificationData.Request(InfoNotificationManager.GetMask(_menu), startIndex, MaxCount, () =>
            {
                if (!append)
                {
                    _dataList.Clear();
                }

                _dataList.AddRange(_notificationData.DataList);
                _isEnd = _notificationData.DataList.Count < MaxCount;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { LogHelper.Error("NotificationData Request fail, code = {0}", code); });
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.TableDataScrollers[(int) _menu].SetEmpty();
                _cachedView.EmptyObj.SetActive(true);
                _cachedView.ClearBtn.SetActiveEx(false);
            }
            else
            {
                _cachedView.ClearBtn.SetActiveEx(_dataList.Count != 0);
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
            if (!_isEnd)
            {
                if (inx > _dataList.Count - 2)
                {
                    RequestData(true);
                }
            }
        }

        protected IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlInfoNotification();
            item.MainCtrl = this;
            item.Init(parent, _resScenary);
            return item;
        }

        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlInfoNotification.EMenu menu)
        {
            _menu = menu;
        }

        public void SetReplyPannel(bool b, NotificationDataItem data)
        {
            _mainCtrl.SetReplyPannel(b, data);
        }

        public void OnMarkRead(NotificationDataItem data)
        {
            if (_dataList.Contains(data))
            {
                _dataList.Remove(data);
                RefreshView();
            }
        }

        public void ClearData()
        {
            InfoNotificationManager.Instance.MarkReadBatch(_menu);
            _dataList.Clear();
            RefreshView();
        }
    }
}