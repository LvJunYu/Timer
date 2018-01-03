using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA
{
    public class UPCtrlPersonalInfoMessageBoard : UPCtrlPersonalInfoBase
    {
        private const int PageSize = 10;
        private List<UserMessage> _dataList;
        private UserMessageData _data = new UserMessageData();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MessageTableDataScroller.Set(OnItemRefresh, GetItemRenderer);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.InputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        private void RequestData(bool append = false)
        {
            if (_mainCtrl.UserInfoDetail == null) return;
            TempData();
//            int startInx = 0;
//            if (append)
//            {
//                startInx = _dataList.Count;
//            }
//            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, startInx, PageSize, () =>
//            {
//                _dataList = _data.AllList;
//                if (_isOpen)
//                {
//                    RefreshView();
//                }
//            }, code =>
//            {
//                SocialGUIManager.ShowPopupDialog("获取留言数据失败。");
//            });
        }

        private void TempData()
        {
            if (_dataList != null) return;
            _dataList = new List<UserMessage>();
            for (int i = 0; i < _mainCtrl.MessageCount; i++)
            {
                UserMessage userMessage = new UserMessage();
                userMessage.Content = "测试留言测试留言测试留言测试留言" + i;
                userMessage.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis() - 4000;
                userMessage.Id = i + 100;
                userMessage.UserInfoDetail = _mainCtrl.UserInfoDetail;
                userMessage.LikeNum = Random.Range(0, 5);
                userMessage.UserLike = false;
                userMessage.ReplyCount = Random.Range(0, 15);
                userMessage.FirstReply = new UserMessageReply();
                userMessage.FirstReply.Content = "测试第一条回复测试第一条回复测试第一条回复测试第一条回复";
                userMessage.FirstReply.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis() - 1000 + i;
                userMessage.FirstReply.Id = i + 1000;
                userMessage.FirstReply.MessageId = userMessage.Id;
                bool relayOther = Random.Range(0, 2) == 0;
                if (relayOther)
                {
                    userMessage.FirstReply.TargetUserInfoDetail = _mainCtrl.UserInfoDetail;
                }
                else
                {
                    userMessage.FirstReply.TargetUserInfoDetail = null;
                }
                userMessage.FirstReply.UserInfoDetail = _mainCtrl.UserInfoDetail;
                _dataList.Add(userMessage);
            }
        }

        public override void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.EmptyObj.SetActive(true);
                _cachedView.MessageTableDataScroller.SetEmpty();
                return;
            }
            _cachedView.EmptyObj.SetActive(_dataList.Count == 0);
            _cachedView.MessageTableDataScroller.SetItemCount(_dataList.Count);
        }

        private void OnInputFieldEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendBtn();
            }
        }

        private void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                //测试
                var message = new UserMessage();
                message.Content = _cachedView.InputField.text;
                message.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
                message.Id = Random.Range(10000, 20000);
                message.UserInfoDetail = _mainCtrl.UserInfoDetail;
                _dataList.Add(message);
                _dataList.Sort((r1, r2) => -r1.CreateTime.CompareTo(r2.CreateTime));
                RefreshView();
                _mainCtrl.MessageCount++;
                _mainCtrl.RefreshMessageNum(_mainCtrl.MessageCount);
            }
            _cachedView.InputField.text = String.Empty;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _dataList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_dataList[inx]);
            if (!_data.IsEnd)
            {
                if (inx > _dataList.Count - 2)
                {
                    RequestData(true);
                }
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalInfoMessage();
            item.Init(parent, _resScenary);
            return item;
        }

        public override void Clear()
        {
            base.Clear();
            _dataList = null;
            _cachedView.MessageTableDataScroller.ContentPosition = Vector2.zero;
        }

        public void OnReplyMessage(long messageId, UserMessageReply reply)
        {
            var message = _dataList.Find(p => p.Id == messageId);
            if (message != null)
            {
                message.LocalAddReply(reply);
                _cachedView.MessageTableDataScroller.RefreshCurrent();
            }
        }
    }
}