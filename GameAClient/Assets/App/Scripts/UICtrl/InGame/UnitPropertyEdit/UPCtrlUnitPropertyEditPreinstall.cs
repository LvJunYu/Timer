using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlUnitPropertyEditPreinstall : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private USCtrlPreinstallItem[] _preinstallItems;
        private List<UnitPreinstall> _dataList;
        private int _curIndex;
        public bool NeedSave;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SavePreinstallBtn.onClick.AddListener(OnSavePreinstallBtn);
            _cachedView.DeletePreinstallBtn.onClick.AddListener(OnDeletePreinstallBtn);
            _cachedView.CreatePreinstallBtn.onClick.AddListener(OnCreatePreinstallBtn);
            var items = _cachedView.PreinstallToggleGroup.GetComponentsInChildren<USViewPreinstallItem>();
            _preinstallItems = new USCtrlPreinstallItem[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                _preinstallItems[i] = new USCtrlPreinstallItem();
                _preinstallItems[i].Init(items[i]);
                var inx = i;
                _preinstallItems[i].AddListener(() => OnPreinstallBtn(inx));
            }
        }

        public override void Open()
        {
            base.Open();
            _curIndex = -1;
            _dataList = null;
            RequestData();
            RefreshView();
        }

        private void RequestData()
        {
            var list = LocalUser.Instance.UnitPreinstallList;
            list.CheckLocalOrRequest(_mainCtrl.EditData.UnitDesc.Id, () =>
            {
                _dataList = list.PreinstallsCache;
                RefreshView();
            }, () => { });
        }

        public void RefreshView()
        {
            _cachedView.PreinstallBtns.SetActive(_curIndex != -1);
            for (int i = 0; i < _preinstallItems.Length; i++)
            {
                if (_dataList != null && i < _dataList.Count)
                {
                    _preinstallItems[i].SetEnable(true);
                    _preinstallItems[i].SetSelected(i == _curIndex);
                    _preinstallItems[i].Set(_dataList[i]);
                }
                else
                {
                    _preinstallItems[i].SetEnable(false);
                }
            }
        }

        private void OnCreatePreinstallBtn()
        {
            if (_dataList != null && _dataList.Count == _preinstallItems.Length)
            {
                SocialGUIManager.ShowPopupDialog("预设已满，是否替换掉第一个预设？", null,
                    new KeyValuePair<string, Action>("取消", null),
                    new KeyValuePair<string, Action>("确定", () => { DeletePreinstall(0, CreatePreinstall); }));
            }
            else
            {
                CreatePreinstall();
            }
        }

        private void OnDeletePreinstallBtn()
        {
            DeletePreinstall(_curIndex, () =>
            {
                _dataList.RemoveAt(_curIndex);
                _curIndex = -1;
                RefreshView();
            });
        }

        private void OnSavePreinstallBtn()
        {
            if (_curIndex == -1) return;
            Msg_Preinstall msg = _mainCtrl.EditData.UnitExtra.ToUnitPreInstall();
            msg.Name = _dataList[_curIndex].PreinstallData.Name;
            msg.UnitId = _mainCtrl.EditData.UnitDesc.Id;
            RemoteCommands.UpdateUnitPreinstall(_dataList[_curIndex].PreinstallId, msg, unitMsg =>
                {
                    if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
                    {
                        RequestData();
                        NeedSave = false;
                    }
                    //todo
                },
                res => { SocialGUIManager.ShowPopupDialog("保存预设失败"); });
        }

        private void OnPreinstallBtn(int index)
        {
            if (_curIndex == index) return;
            if (NeedSave)
            {
                SocialGUIManager.ShowPopupDialog("读取预设会丢失已有设定，确定要读取预设吗？", null,
                    new KeyValuePair<string, Action>("取消", null),
                    new KeyValuePair<string, Action>("确定", () => { ReadPreinstall(index); }));
            }
            else
            {
                ReadPreinstall(index);
            }
        }

        private void CreatePreinstall()
        {
            var msg = _mainCtrl.EditData.UnitExtra.ToUnitPreInstall();
            int index = 0;
            if (_dataList != null)
            {
                index = _dataList.Count;
            }
            msg.Name = string.Format("预设 {0}", index);
            msg.UnitId = _mainCtrl.EditData.UnitDesc.Id;
            RemoteCommands.CreateUnitPreinstall(msg, unitMsg =>
                {
                    if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
                    {
                        if (_dataList == null)
                        {
                            _dataList = new List<UnitPreinstall>();
                        }
                        _dataList.Add(new UnitPreinstall(unitMsg.UnitPreinstallData));
                        _curIndex = _dataList.Count - 1;
                        NeedSave = false;
                        RefreshView();
                    }
                    //todo
                },
                res => { SocialGUIManager.ShowPopupDialog("创建预设失败"); });
        }

        private void DeletePreinstall(int index, Action successCallBack)
        {
            if (index == -1) return;
            List<long> _deleteList = new List<long>(1);
            _deleteList.Add(_dataList[_curIndex].PreinstallId);
            RemoteCommands.DeleteUnitPreinstall(_deleteList, unitMsg =>
            {
                if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
                {
                    if (successCallBack != null)
                    {
                        successCallBack.Invoke();
                    }
                }
                //todo
            }, res => { SocialGUIManager.ShowPopupDialog("删除预设失败"); });
        }

        private void ReadPreinstall(int index)
        {
            _curIndex = index;
            _mainCtrl.EditData.UnitExtra.Set(_dataList[_curIndex].PreinstallData);
            NeedSave = false;
            RefreshView();
            Messenger.Broadcast(EMessengerType.OnPreinstallRead);
        }
    }
}