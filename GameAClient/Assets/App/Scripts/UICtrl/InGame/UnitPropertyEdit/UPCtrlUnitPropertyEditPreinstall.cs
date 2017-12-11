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
                _preinstallItems[i].SetTogGroup(_cachedView.PreinstallToggleGroup);
            }
        }

        public override void Open()
        {
            base.Open();
            _curIndex = -1;
            RequestData();
            RefreshView();
        }

        private void RequestData()
        {
            var list = new UnitPreinstallList();
            list.Request(_mainCtrl.EditData.UnitDesc.Id, () =>
            {
                _dataList = list.PreinstallList;
                RefreshView();
            }, res => { });
        }

        public void RefreshView()
        {
            for (int i = 0; i < _preinstallItems.Length; i++)
            {
                if (_dataList != null && i < _dataList.Count)
                {
                    _preinstallItems[i].SetEnable(true);
                    _preinstallItems[i].Set(_dataList[i], i);
                }
                else
                {
                    _preinstallItems[i].SetEnable(false);
                }
            }
        }

        private void OnDeletePreinstallBtn()
        {
            throw new System.NotImplementedException();
        }

        private void OnSavePreinstallBtn()
        {
            throw new System.NotImplementedException();
        }

        private void DeletePreinstall()
        {
            if (_curIndex == -1) return;
            List<long> _deleteList = new List<long>(1);
            _deleteList.Add(_dataList[_curIndex].PreinstallId);
            RemoteCommands.DeleteUnitPreinstall(_deleteList, unitMsg =>
            {
                if (unitMsg.ResultCode == (int) EUnitPreinstallOperateResult.UPOR_Success)
                {
                    _deleteList.RemoveAt(_curIndex);
                    RefreshView();
                }
                //todo
            }, res => { SocialGUIManager.ShowPopupDialog("删除预设失败"); });
        }

        private void UpdatePreinstall()
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
                    }
                    //todo
                },
                res => { SocialGUIManager.ShowPopupDialog("保存预设失败"); });
        }

        private void OnCreatePreinstallBtn()
        {
            var msg = _mainCtrl.EditData.UnitExtra.ToUnitPreInstall();
            msg.Name = _dataList[_curIndex].PreinstallData.Name;
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
                        RefreshView();
                        
                    }
                    //todo
                },
                res => { SocialGUIManager.ShowPopupDialog("创建预设失败"); });
        }

        public void OnPreinstallRead(int index)
        {
            if (_dataList == null || index < 0 || index >= _dataList.Count)
            {
                LogHelper.Error("Read preinstall index = {0} failed", index);
                return;
            }
            _curIndex = index;
            _mainCtrl.EditData.UnitExtra.Set(_dataList[_curIndex].PreinstallData);
        }
    }
}