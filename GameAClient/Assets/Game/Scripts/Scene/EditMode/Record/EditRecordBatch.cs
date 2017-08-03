using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class EditRecordBatch
    {
        /// <summary>
        /// 大部分操作都是一条数据，这样节省内存
        /// </summary>
        private EditRecordData _firstRecordData;
        private List<EditRecordData> _listRecordData;

        public bool IsEmpty
        {
            get { return EditRecordData.EAction.None == _firstRecordData.ActionType; }
        }

        public void RecordAddUnit(ref UnitDesc unitDesc, ref UnitExtra unitExtra)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.AddUnit;
            recordData.UnitDesc = unitDesc;
            recordData.UnitExtra = unitExtra;
            AddRecordData(ref recordData);
        }
        
        public void RecordRemoveUnit(ref UnitDesc unitDesc, ref UnitExtra unitExtra)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.RemoveUnit;
            recordData.UnitDesc = unitDesc;
            recordData.UnitExtra = unitExtra;
            AddRecordData(ref recordData);
        }
        
        public void RecordUpdateExtra(ref UnitDesc unitDesc, ref UnitExtra unitExtra, ref UnitExtra oldUnitExtra)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.UpdateExtra;
            recordData.UnitDesc = unitDesc;
            recordData.UnitExtra = unitExtra;
            recordData.UnitExtraOld = oldUnitExtra;
            AddRecordData(ref recordData);
        }
        
        public void RecordAddSwitchConnection(ref IntVec3 switchGuid, ref IntVec3 unitGuid)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.AddSwitchConnection;
            recordData.UnitDesc.Guid = unitGuid;
            recordData.SwitchGuid = switchGuid;
            AddRecordData(ref recordData);
        }
        
        public void RecordRemoveSwitchConnection(ref IntVec3 switchGuid, ref IntVec3 unitGuid)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.RemoveSwitchConnection;
            recordData.UnitDesc.Guid = unitGuid;
            recordData.SwitchGuid = switchGuid;
            AddRecordData(ref recordData);
        }
        
        public void Redo()
        {
            if (EditRecordData.EAction.None != _firstRecordData.ActionType)
            {
                RedoRecordData(_firstRecordData);
            }
            if (null != _listRecordData)
            {
                for (int i = 0; i < _listRecordData.Count; i++)
                {
                    RedoRecordData(_listRecordData[i]);
                }
            }
        }

        public void Undo()
        {
            if (null != _listRecordData)
            {
                for (int i = _listRecordData.Count-1; i >= 0; i--)
                {
                    UndoRecordData(_listRecordData[i]);
                }
            }
            if (EditRecordData.EAction.None != _firstRecordData.ActionType)
            {
                UndoRecordData(_firstRecordData);
            }
        }

        private void RedoRecordData(EditRecordData recordData)
        {
            switch (recordData.ActionType)
            {
                case EditRecordData.EAction.AddUnit:
                    EditMode.Instance.AddUnitWithCheck(recordData.UnitDesc);
                    DataScene2D.Instance.ProcessUnitExtra(recordData.UnitDesc, recordData.UnitExtra);
                    break;
                case EditRecordData.EAction.RemoveUnit:
                    EditMode.Instance.DeleteUnitWithCheck(recordData.UnitDesc);
                    break;
                case EditRecordData.EAction.UpdateExtra:
                    DataScene2D.Instance.ProcessUnitExtra(recordData.UnitDesc, recordData.UnitExtra);
                    break;
                case EditRecordData.EAction.AddSwitchConnection:
                    DataScene2D.Instance.BindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
                case EditRecordData.EAction.RemoveSwitchConnection:
                    DataScene2D.Instance.UnbindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
            }
        }

        private void UndoRecordData(EditRecordData recordData)
        {
            switch (recordData.ActionType)
            {
                case EditRecordData.EAction.AddUnit:
                    EditMode.Instance.DeleteUnitWithCheck(recordData.UnitDesc);
                    break;
                case EditRecordData.EAction.RemoveUnit:
                    EditMode.Instance.AddUnitWithCheck(recordData.UnitDesc);
                    DataScene2D.Instance.ProcessUnitExtra(recordData.UnitDesc, recordData.UnitExtra);
                    break;
                case EditRecordData.EAction.UpdateExtra:
                    DataScene2D.Instance.ProcessUnitExtra(recordData.UnitDesc, recordData.UnitExtraOld);
                    break;
                case EditRecordData.EAction.AddSwitchConnection:
                    DataScene2D.Instance.UnbindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
                case EditRecordData.EAction.RemoveSwitchConnection:
                    DataScene2D.Instance.BindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
            }
        }

        private void AddRecordData(ref EditRecordData editRecordData)
        {
            if (EditRecordData.EAction.None == _firstRecordData.ActionType)
            {
                _firstRecordData = editRecordData;
            }
            else
            {
                if (null == _listRecordData)
                {
                    _listRecordData = new List<EditRecordData>();
                }
                _listRecordData.Add(editRecordData);
            }
        }
        
    }
}