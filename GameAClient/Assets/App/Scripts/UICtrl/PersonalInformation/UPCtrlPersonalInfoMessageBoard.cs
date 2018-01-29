using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

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
            _cachedView.InputField.onEndEdit.AddListener(str =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnSendBtn();
                }
            });
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
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }

            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, startInx, PageSize, () =>
            {
                _dataList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { SocialGUIManager.ShowPopupDialog("获取留言数据失败"); });
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

        private void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                string content = _cachedView.InputField.text;
                var testRes = CheckTools.CheckMessage(content);
                if (testRes == CheckTools.ECheckMessageResult.Success)
                {
                    RemoteCommands.PublishUserMessage(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, content, msg =>
                    {
                        if (msg.ResultCode == (int) EPublishUserMessageCode.PUMC_Success)
                        {
                            _cachedView.InputField.text = string.Empty;
                            _dataList.Add(new UserMessage(msg.Data));
                            _dataList.Sort((r1, r2) => -r1.CreateTime.CompareTo(r2.CreateTime));
                            RefreshView();
                            _mainCtrl.OnPublishUserMessage();
                        }
                        else
                        {
                            SocialGUIManager.ShowPopupDialog("发送失败");
                            LogHelper.Error("发布留言失败，ResultCode：{0}", msg.ResultCode);
                        }
                    }, code =>
                    {
                        SocialGUIManager.ShowPopupDialog("发送失败");
                        LogHelper.Error("发布留言失败，code：{0}", code);
                    });
                }
                else
                {
                    SocialGUIManager.ShowCheckMessageRes(testRes);
                }
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

        public void OnDeleteUserMessage(UserMessage message)
        {
            if (_data.AllList.Contains(message))
            {
                _data.AllList.Remove(message);
                _dataList = _data.AllList;
                RefreshView();
            }
        }
    }
}