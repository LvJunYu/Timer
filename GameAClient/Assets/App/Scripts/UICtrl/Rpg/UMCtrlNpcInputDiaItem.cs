using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlNpcInputDiaItem : UMCtrlBase<UMViewNpcInputDiaItem>, IUMPoolable
    {
        public bool IsShow { get; private set; }
        private List<UMCtrlNpcInputDiaItem> _list;
        private NpcDialogPreinstallList _datalist;
        private List<long> _idList = new List<long>();
        private long _id;
        private bool _isAdd;
        private RectTransform _parent;
        private UIRoot _uiRoot;
        private Vector3 _pos;
        private bool _isofficial;
        private List<Action> _callbackList;

        protected override bool Init(RectTransform parent, Vector3 localpos, UIRoot uiRoot)
        {
            _parent = parent;
            _uiRoot = uiRoot;
            _pos = localpos;
            _cachedView.InputField.onEndEdit.AddListener(OnEndSaveDia);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteDia);
            _cachedView.AddBtn.onClick.AddListener(OnAddBtn);
            _cachedView.InputField.onValueChanged.AddListener((str) => { _cachedView.AddBtn.SetActiveEx(false); });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.InputField);
            _cachedView.ApplyBtn.onClick.AddListener(OnApplyBtn);
            return base.Init(parent, localpos, uiRoot);
        }

        public void Set(int id, List<UMCtrlNpcInputDiaItem> list, NpcDialogPreinstallList datalist, bool isAdd,
            bool isofficial, List<Action> callbackList)
        {
            _list = list;
            _datalist = datalist;
            _id = id;
            _isAdd = isAdd;
            _isofficial = isofficial;
            _callbackList = callbackList;
            _callbackList.Add(NoSelectBtn);
            _idList.Add(id);

            Refresh();
        }

        private void OnEndSaveDia(string data)
        {
            if (_datalist == null)
            {
                RemoteCommands.CreateNpcDialogPreinstall(data, code =>
                {
                    SocialGUIManager.ShowPopupDialog("创建常用对话成功");
                    _cachedView.InputField.text = _datalist.DataList[(int) _id].Data;
                    _isAdd = false;
                    Refresh();
                }, code => { SocialGUIManager.ShowPopupDialog("创建常用对话失败"); });
            }
        }

        private void OnDeleteDia()
        {
            RemoteCommands.DeleteNpcDialogPreinstall(_idList, code =>
                {
                    _list.RemoveAt((int) _id);
                    _callbackList.Remove(NoSelectBtn);
                    UMPoolManager.Instance.Free(this);
                    SocialGUIManager.ShowPopupDialog("删除常用对话成功");
                },
                code => { SocialGUIManager.ShowPopupDialog("删除常用对话失败"); });
        }

        private void OnAddBtn()
        {
            _cachedView.AddBtn.SetActiveEx(false);
            UMCtrlNpcInputDiaItem item = new UMCtrlNpcInputDiaItem();
            item.Init(_parent, _pos, _uiRoot);
            item.Set(0, _list, _datalist, true, false, _callbackList);
            _list.Add(item);
        }

        private void OnApplyBtn()
        {
            for (int i = 0; i < _callbackList.Count; i++)
            {
                _callbackList[i].Invoke();
            }
            _cachedView.SelectImage.SetActiveEx(true);
        }

        private void NoSelectBtn()
        {
            _cachedView.SelectImage.SetActiveEx(false);
        }

        private void Refresh()
        {
            if (_isofficial)
            {
                _cachedView.DeleteBtn.SetActiveEx(false);
            }
            else
            {
                _cachedView.DeleteBtn.SetActiveEx(true);
            }
            if (_isAdd)
            {
                _cachedView.DeleteBtn.SetActiveEx(false);
                _cachedView.ApplyBtn.SetActiveEx(false);
            }
            else
            {
                _cachedView.DeleteBtn.SetActiveEx(true);
                _cachedView.ApplyBtn.SetActiveEx(true);
            }
        }

        public void Hide()
        {
            IsShow = false;
            _cachedView.Trans.anchoredPosition = new Vector2(100000, 0);
        }

        public void Show()
        {
            IsShow = true;
            _cachedView.Trans.anchoredPosition = Vector2.zero;
        }

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.SetParent(rectTransform);
        }
    }
}