using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlChatBase : UPCtrlBase<UICtrlChat, UIViewChat>
    {
        protected EResScenary _resScenary;
        protected UICtrlChat.EMenu _menu;
        protected List<UMCtrlChatTalkItem> _umCtrlChatTalkItemCache;
        protected List<ChatInfo> _dataList = new List<ChatInfo>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.TableDataScrollers[(int) _menu].Set(OnItemRefresh, GetTalkItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
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
            _umCtrlChatTalkItemCache = null;
        }

        public void RefreshView(bool showLastContent = true)
        {
            if (!_isOpen) return;
            _cachedView.TableDataScrollers[(int) _menu].SetItemCount(_dataList.Count);
            if (showLastContent)
            {
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
                {
                    _cachedView.ScrollRect[(int) _menu].normalizedPosition = Vector2.zero;
                    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
                    {
                        _cachedView.ScrollRect[(int) _menu].normalizedPosition = Vector2.zero;
                    
                    }));
                }));
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

        protected virtual IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlChatTalkItem();
            item.Init(parent, _resScenary);
            if (null == _umCtrlChatTalkItemCache)
            {
                _umCtrlChatTalkItemCache = new List<UMCtrlChatTalkItem>(10);
            }
            _umCtrlChatTalkItemCache.Add(item);
            return item;
        }

        public virtual void AddChatItem(ChatInfo chatInfo)
        {
            _dataList.Add(chatInfo);
            RefreshView();
        }

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlChat.EMenu menu)
        {
            _menu = menu;
        }
    }
}