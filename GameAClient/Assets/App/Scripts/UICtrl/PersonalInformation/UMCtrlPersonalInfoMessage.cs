using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA
{
    public class UMCtrlPersonalInfoMessage : UMCtrlBase<UMViewPersonalInfoMessage>, IDataItemRenderer
    {
        public static Action OpenInputCallBack;
        private static string _contentFormat = "<color=orange>{0}</color>:{1}";
        private static string _totalFormat = "共{0}条回复";
        private static string _moreFormat = "更多{0}条回复";
        private const int pageSize = 5;
        private UserMessage _message;
        private UMCtrlPersonalInfoReplyMessage _firstReplay;
        protected EResScenary _resScenary;
        private bool _unfold;
        private bool _openPublishDock;
        private List<UMCtrlPersonalInfoReplyMessage> _umCache = new List<UMCtrlPersonalInfoReplyMessage>(8);
        private List<UserMessageReply> _dataList;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _message; }
        }

        protected override bool Init(RectTransform parent, EResScenary resScenary, Vector3 localpos,
            ResManagedUIRoot uiRoot)
        {
            _resScenary = resScenary;
            return base.Init(parent, resScenary, localpos, uiRoot);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.PraiseBtn.onClick.AddListener(OnPraiseBtn);
            _cachedView.ReplayBtn.onClick.AddListener(OnReplayBtn);
            _cachedView.InputField.onEndEdit.AddListener(OnInputEndEdit);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.MoreBtn.onClick.AddListener(OnMoreBtn);
            _cachedView.FoldBtn.onClick.AddListener(OnFoldBtn);
            _firstReplay = new UMCtrlPersonalInfoReplyMessage();
            _firstReplay.Init(_cachedView.FirstReplyRtf, _resScenary);
        }

        public void Set(object obj)
        {
            _message = obj as UserMessage;
            if (_message == null)
            {
                Unload();
                return;
            }
            _unfold = false;
            _openPublishDock = false;
            _dataList = _message.ReplyList.AllList;
            RefreshView();
        }

        private void RequestData(bool append = false)
        {
            if (_message == null) return;
            if (_message.ReplyList.IsEnd) return;
            TempData();
//            int startInx = 0;
//            if (append)
//            {
//                startInx = _dataList.Count;
//            }
//            _message.ReplyList.Request(_message.Id, startInx, pageSize, () =>
//            {
//                _dataList = _message.ReplyList.AllList;
//                RefreshReplyDock();
//            }, code => { SocialGUIManager.ShowPopupDialog("获取数据失败。"); });
        }

        private void RefreshView()
        {
            _cachedView.PublishDock.SetActive(_openPublishDock);
            _cachedView.PraiseCountTxt.SetActiveEx(_message.LikeNum > 0);
            UserInfoSimple user = _message.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.CreateTime,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_message.CreateTime));
            DictionaryTools.SetContentText(_cachedView.PraiseCountTxt, _message.LikeNum.ToString());
            DictionaryTools.SetContentText(_cachedView.ReplayCountTxt, _message.ReplyCount.ToString());
            DictionaryTools.SetContentText(_cachedView.Content,
                string.Format(_contentFormat, user.NickName, _message.Content));
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultIconTexture);
            RefreshReplyDock();
            Canvas.ForceUpdateCanvases();
        }

        private void RefreshReplyDock(bool Broadcast = false)
        {
            _cachedView.ReplayDock.SetActive(_message.ReplyCount > 0);
            if (_message.ReplyCount > 0)
            {
                _unfold = _dataList.Count > 0;
                _cachedView.FoldBtn.SetActiveEx(_unfold);
                _cachedView.FirstReplyRtf.SetActiveEx(!_unfold);
                if (!_unfold && _firstReplay != null)
                {
                    _firstReplay.Set(_message.FirstReply);
                }
                if (_dataList.Count == 0)
                {
                    ClearCache();
                    _cachedView.MoreTxt.text = string.Format(_totalFormat, _message.ReplyCount);
                    _cachedView.MoreBtn.SetActiveEx(true);
                }
                else
                {
                    int remainCount = _message.ReplyCount - _dataList.Count;
                    if (remainCount < 0)
                    {
                        LogHelper.Error("_message.ReplayCount<_dataList.Count");
                        return;
                    }
                    _cachedView.MoreTxt.text = string.Format(_moreFormat, remainCount);
                    _cachedView.MoreBtn.SetActiveEx(remainCount > 0);
                    for (int i = 0; i < _dataList.Count; i++)
                    {
                        if (i < _umCache.Count)
                        {
                            if (!_umCache[i].IsShow)
                            {
                                _umCache[i].Show();
                            }
                            _umCache[i].Set(_dataList[i]);
                        }
                        else
                        {
                            GetItem().Set(_dataList[i]);
                        }
                    }
                }
            }
            Canvas.ForceUpdateCanvases();
            if (Broadcast)
            {
                Messenger.Broadcast(EMessengerType.OnMessageBoardElementSizeChanged);
            }
        }

        private UMCtrlPersonalInfoReplyMessage GetItem()
        {
            var item = _umCache.Find(p => !p.IsShow);
            if (item == null)
            {
                item = new UMCtrlPersonalInfoReplyMessage();
                item.Init(_cachedView.ReplayRtf, _resScenary);
                _umCache.Add(item);
            }
            else
            {
                item.Show();
            }
            return item;
        }

        private void ClearCache()
        {
            _umCache.ForEach(p => p.Hide());
        }

        private void OnMoreBtn()
        {
            RequestData(_unfold);
        }

        private void OnFoldBtn()
        {
            _dataList.Clear();
            RefreshReplyDock(true);
        }

        private void TempData()
        {
            if (_dataList.Count == _message.ReplyCount) return;
            for (int i = 0; i < pageSize; i++)
            {
                if (_dataList.Count == 0)
                {
                    _dataList.Add(_message.FirstReply);
                }
                else
                {
                    var replay = new UserMessageReply();
                    replay.Content = "测试下拉留言测试下拉留言测试下拉留言测试下拉留言" + i;
                    replay.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis() - 800 + i;
                    replay.Id = i + 2000;
                    replay.MessageId = _message.Id;
                    replay.RelayOther = Random.Range(0, 2) == 0;
                    replay.TargetUserInfoDetail = LocalUser.Instance.User;
                    replay.UserInfoDetail = LocalUser.Instance.User;
                    _dataList.Add(replay);
                }
                if (_dataList.Count == _message.ReplyCount) break;
            }
            RefreshReplyDock(true);
        }

        private void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                _message.Reply(_cachedView.InputField.text);
                //测试
                var reply = new UserMessageReply();
                reply.Content = _cachedView.InputField.text;
                reply.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
                reply.Id = 3000;
                reply.MessageId = _message.Id;
                reply.RelayOther = false;
                reply.UserInfoDetail = LocalUser.Instance.User;
                Messenger<long, UserMessageReply>.Broadcast(EMessengerType.OnReplyMessage, _message.Id, reply);
            }
            SetPublishDock(false);
        }

        private void OnInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendBtn();
            }
        }

        private void OnReplayBtn()
        {
            SetPublishDock(!_openPublishDock);
        }

        private void OnPraiseBtn()
        {
            if (_message.UserLike)
            {
                _message.LikeNum--;
            }
            else
            {
                _message.LikeNum++;
            }
            _message.UserLike = !_message.UserLike;
            RefreshView();
        }

        private void OnHeadBtn()
        {
            if (_message != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_message.UserInfoDetail);
            }
        }

        private void SetPublishDock(bool value)
        {
            if (_message == null) return;
            _openPublishDock = value;
            _cachedView.PublishDock.SetActive(_openPublishDock);
            Canvas.ForceUpdateCanvases();
            Messenger.Broadcast(EMessengerType.OnMessageBoardElementSizeChanged);
            if (_openPublishDock)
            {
                if (OpenInputCallBack != null)
                {
                    OpenInputCallBack.Invoke();
                }
                OpenInputCallBack = () => SetPublishDock(false);
                _cachedView.InputField.text = String.Empty;
                _cachedView.InputField.Select();
            }
            else
            {
                OpenInputCallBack = null;
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.HeadBtn.onClick.RemoveAllListeners();
            _cachedView.PraiseBtn.onClick.RemoveAllListeners();
            _cachedView.ReplayBtn.onClick.RemoveAllListeners();
            _umCache.ForEach(p => p.Destroy());
            base.OnDestroy();
        }

        public void Unload()
        {
            ClearCache();
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultIconTexture);
        }
    }
}