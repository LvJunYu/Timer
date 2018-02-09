using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlChatInGameQuickChat : UPCtrlBase<UICtrlChatInGame, UIViewChatInGame>
    {
        private USCtrlPreinstallChat[] _usCtrlPreinstallChats;
        private List<RoomChatPreinstall> _dataList;
        private RoomChatPreinstallList _data;
        private bool _hasRequested;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddNewPreinstallChatBtn.onClick.AddListener(() => SetNewPreinstallInputField(true));
            _cachedView.NewPreinstallInputField.onEndEdit.AddListener(str =>
            {
                OnSavePreinstall();
                _cachedView.NewPreinstallInputField.text = string.Empty;
                SetNewPreinstallInputField(false);
            });
            var list = _cachedView.QuickChatPannel.GetComponentsInChildren<USViewPreinstallChat>();
            _usCtrlPreinstallChats = new USCtrlPreinstallChat[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                _usCtrlPreinstallChats[i] = new USCtrlPreinstallChat();
                _usCtrlPreinstallChats[i].Init(list[i]);
                var inx = i;
                _usCtrlPreinstallChats[i].AddDeleteBtnListener(() => OnDeleteBtnClick(inx));
                _usCtrlPreinstallChats[i].AddBtnListener(() => OnSendQuickChatBtnClick(inx));
            }
        }

        private void OnSendQuickChatBtnClick(int inx)
        {
            if (_dataList == null) return;
            if (inx < _dataList.Count)
            {
                _mainCtrl.Chat.SendChat(_dataList[inx].Data);
                _mainCtrl.OnCloseBtn();
            }
        }

        private void OnDeleteBtnClick(int i)
        {
            if (_dataList == null) return;
            if (i < _dataList.Count)
            {
                var list = new List<long>();
                list.Add(_dataList[i].Id);
                RemoteCommands.DeleteRoomChatPreinstall(list, msg =>
                {
                    if (msg.ResultCode == (int) ERoomChatPreinstallOperateResult.RCPOR_Success)
                    {
                        _dataList.RemoveAt(i);
                        RefreshView();
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("删除失败");
                    }
                }, code =>
                {
                    LogHelper.Error("DeleteRoomChatPreinstall fail, code = {0}", code);
                    SocialGUIManager.ShowPopupDialog("删除失败");
                });
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.QuickChatPannel.SetActive(true);
            if (!_hasRequested)
            {
                RequestData();
            }

            RefreshView();
        }

        public override void Close()
        {
            _cachedView.QuickChatPannel.SetActive(false);
            base.Close();
        }

        private void RequestData()
        {
            _data = LocalUser.Instance.MultiBattleData.ChatPreinstallList;
            _data.RequestList(() =>
            {
                _dataList = _data.DataList;
                RefreshView();
                _hasRequested = true;
            });
        }

        private void RefreshView()
        {
            for (int i = 0; i < _usCtrlPreinstallChats.Length; i++)
            {
                if (_dataList != null && i < _dataList.Count)
                {
                    _usCtrlPreinstallChats[i].Set(_dataList[i]);
                }
                else
                {
                    _usCtrlPreinstallChats[i].Set(null);
                }
            }

            _cachedView.AddNewPannel.SetActive(_dataList == null || _dataList.Count < _usCtrlPreinstallChats.Length);
            SetNewPreinstallInputField(false);
        }

        private void SetNewPreinstallInputField(bool value)
        {
            _cachedView.NewPreinstallInputField.SetActiveEx(value);
            _cachedView.AddNewPreinstallChatBtn.SetActiveEx(!value);
        }

        private void OnSavePreinstall()
        {
            var str = _cachedView.NewPreinstallInputField.text;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            LocalUser.Instance.MultiBattleData.ChatPreinstallList.CreateRoomChatPreinstall(str, chat =>
            {
                if (_dataList != null)
                {
                    _dataList.Add(chat);
                    RefreshView();
                }
            });
        }
    }
}