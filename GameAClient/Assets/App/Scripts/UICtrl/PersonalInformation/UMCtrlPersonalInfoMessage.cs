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
        protected static string _contentFormat = "<color=orange>{0}</color>: {1}";
        protected static string _totalFormat = "共{0}条回复";
        protected static string _moreFormat = "更多{0}条回复";
        protected EResScenary _resScenary;
        protected bool _unfold;
        protected bool _openPublishDock;
        private const int pageSize = 5;
        private UserMessage _message;
        private UMCtrlPersonalInfoReplyMessage _firstReplay;
        private List<UserMessageReply> _dataList;
        private List<UMCtrlPersonalInfoReplyMessage> _umCache = new List<UMCtrlPersonalInfoReplyMessage>(8);
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public virtual object Data
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
            _cachedView.ReplayBtn.onClick.AddListener(OnReplyBtn);
            _cachedView.InputField.onEndEdit.AddListener(str =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnSendBtn();
                }
            });
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.MoreBtn.onClick.AddListener(OnMoreBtn);
            _cachedView.FoldBtn.onClick.AddListener(OnFoldBtn);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.InputField);
            InitFirstUM();
        }

        protected virtual void InitFirstUM()
        {
            _firstReplay = new UMCtrlPersonalInfoReplyMessage();
            _firstReplay.Init(_cachedView.FirstReplyRtf, _resScenary);
        }

        public virtual void Set(object obj)
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

        protected virtual void RequestData(bool append = false)
        {
            if (_message == null) return;
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }
            _message.ReplyList.Request(_message.Id, startInx, pageSize, () =>
            {
                _dataList = _message.ReplyList.AllList;
                RefreshReplyDock(true);
            }, code => { SocialGUIManager.ShowPopupDialog("获取数据失败。"); });
        }

        protected virtual void RefreshView()
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

        protected virtual void RefreshReplyDock(bool Broadcast = false)
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
                    ClearItem();
                    _cachedView.MoreTxt.text = string.Format(_totalFormat, _message.ReplyCount);
                    _cachedView.MoreBtn.SetActiveEx(true);
                }
                else
                {
                    int remainCount = _message.ReplyCount - _dataList.Count;
                    if (remainCount < 0)
                    {
                        LogHelper.Error("_message.ReplayCount < _dataList.Count");
                        return;
                    }

                    _cachedView.MoreTxt.text = string.Format(_moreFormat, remainCount);
                    _cachedView.MoreBtn.SetActiveEx(remainCount > 0);
                    ClearItem();
                    for (int i = 0; i < _dataList.Count; i++)
                    {
                        GetItem().Set(_dataList[i]);
                    }
                }
            }

            Canvas.ForceUpdateCanvases();
            if (Broadcast)
            {
                Messenger.Broadcast(EMessengerType.OnPublishDockActiveChanged);
            }
        }

        private UMCtrlPersonalInfoReplyMessage GetItem()
        {
            var item = UMPoolManager.Instance.Get<UMCtrlPersonalInfoReplyMessage>(_cachedView.ReplayRtf, _resScenary);
            _umCache.Add(item);
            return item;
        }

        private void ClearItem()
        {
            _umCache.ForEach(p => UMPoolManager.Instance.Free(p));
            _umCache.Clear();
        }

        protected virtual void OnMoreBtn()
        {
            RequestData(_unfold);
        }

        protected virtual void OnFoldBtn()
        {
            _dataList.Clear();
            RefreshReplyDock(true);
        }

        protected virtual void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                _message.Reply(_cachedView.InputField.text);
            }
            SetPublishDock(false);
        }

        protected virtual void OnReplyBtn()
        {
            SetPublishDock(!_openPublishDock);
        }

        protected virtual void OnPraiseBtn()
        {
            _message.LikeChanged(RefreshView);
        }

        protected virtual void OnHeadBtn()
        {
            if (_message != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_message.UserInfoDetail);
            }
        }

        protected virtual void SetPublishDock(bool value)
        {
            _openPublishDock = value;
            _cachedView.PublishDock.SetActive(_openPublishDock);
            Canvas.ForceUpdateCanvases();
            Messenger.Broadcast(EMessengerType.OnPublishDockActiveChanged);
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
            base.OnDestroy();
        }

        public virtual void Unload()
        {
            ClearItem();
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultIconTexture);
        }
    }
}