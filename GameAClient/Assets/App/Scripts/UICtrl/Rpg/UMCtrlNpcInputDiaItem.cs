﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlNpcInputDiaItem : UMCtrlBase<UMViewNpcInputDiaItem>, IUMPoolable
    {
        public const int MaxCommonUseDiaNum = 20;
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
        private Action<string> _addAction;
        private Action _refresh;

        public void InitItem(RectTransform parent)
        {
            _parent = parent;
            Clear();
            _cachedView.InputField.onEndEdit.AddListener(OnEndSaveDia);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteDia);
            _cachedView.InputField.onValueChanged.AddListener((str) => { _cachedView.AddImage.SetActiveEx(false); });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.InputField);
            _cachedView.ApplyBtn.onClick.AddListener(OnApplyBtn);
            Show();
        }

        private void Clear()
        {
            _idList.Clear();
            _cachedView.DeleteBtn.onClick.RemoveAllListeners();
            _cachedView.InputField.onValueChanged.RemoveAllListeners();
            _cachedView.ApplyBtn.onClick.RemoveAllListeners();
        }

        public void Set(int id, List<UMCtrlNpcInputDiaItem> list, NpcDialogPreinstallList datalist, bool isAdd,
            bool isofficial, List<Action> callbackList, Action<string> addDiAction, Action refresh)
        {
            _list = list;
            _datalist = datalist;
            _id = id;
            _isAdd = isAdd;
            _isofficial = isofficial;
            _callbackList = callbackList;
            _callbackList.Add(NoSelectBtn);

            if (!isAdd)
            {
                _idList.Add(datalist.DataList[id].Id);
                _cachedView.InputField.text = datalist.DataList[id].Data;
            }
            else
            {
                _cachedView.InputField.text = null;
            }
            _addAction = addDiAction;
            _refresh = refresh;
            Refresh();
        }

        private void OnEndSaveDia(string data)
        {
            if (data.Length > 0)
            {
                if (_datalist != null)
                {
                    RemoteCommands.CreateNpcDialogPreinstall(data, code =>
                    {
                        SocialGUIManager.ShowPopupDialog("创建常用对话成功");
                        _cachedView.InputField.text = _datalist.DataList[(int) _id].Data;
                        _isAdd = false;
                        Refresh();
                        _cachedView.ApplyBtn.SetActiveEx(true);
                        OnAddBtn();
                        _refresh.Invoke();
                    }, code => { SocialGUIManager.ShowPopupDialog("创建常用对话失败"); });
                }
            }
            else
            {
                _cachedView.AddImage.SetActiveEx(true);
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
                    _refresh.Invoke();
                },
                code => { SocialGUIManager.ShowPopupDialog("删除常用对话失败"); });
        }

        private void OnAddBtn()
        {
            if (_list.Count == MaxCommonUseDiaNum)
            {
                return;
            }
            _cachedView.AddImage.SetActiveEx(false);
            UMCtrlNpcInputDiaItem item = UMPoolManager.Instance.Get<UMCtrlNpcInputDiaItem>(_parent, EResScenary.Game);
            item.InitItem(_parent);
            item.Set(0, _list, _datalist, true, false, _callbackList, _addAction, _refresh);
            _list.Add(item);
        }

        private void OnApplyBtn()
        {
            for (int i = 0; i < _callbackList.Count; i++)
            {
                _callbackList[i].Invoke();
            }
            _addAction.Invoke(_cachedView.InputField.text);
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
            _cachedView.SelectImage.SetActiveEx(false);
        }

        public void Hide()
        {
            Clear();
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