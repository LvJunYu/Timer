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

        public void RecordUpdateExtra(ref UnitDesc oldUnitDesc, ref UnitExtra oldUnitExtra,
            ref UnitDesc newUnitDesc, ref UnitExtra newUnitExtra)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.UpdateExtra;
            recordData.UnitDesc = newUnitDesc;
            recordData.UnitExtra = newUnitExtra;
            recordData.UnitDescOld = oldUnitDesc;
            recordData.UnitExtraOld = oldUnitExtra;
            AddRecordData(ref recordData);
        }

        public void RecordChangeMapRect(bool add, bool horizontal)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.ChangeMapRect;
            int x = horizontal ? add ? 1 : -1 : 0;
            int y = horizontal ? 0 : add ? 1 : -1;
            recordData.SwitchGuid = new IntVec3(x, y, 0);
            AddRecordData(ref recordData);
        }

        public void RecordAddSwitchConnection(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            EditRecordData recordData = new EditRecordData();
            recordData.ActionType = EditRecordData.EAction.AddSwitchConnection;
            recordData.UnitDesc.Guid = unitGuid;
            recordData.SwitchGuid = switchGuid;
            AddRecordData(ref recordData);
        }

        public void RecordRemoveSwitchConnection(IntVec3 switchGuid, IntVec3 unitGuid)
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

            // 开关模式不健壮，需要重刷
            if (EditMode.Instance.IsInState(EditModeState.Switch.Instance))
            {
                EditMode.Instance.StopSwitch();
                EditMode.Instance.StartSwitch();
            }
        }

        public void Undo()
        {
            if (null != _listRecordData)
            {
                for (int i = _listRecordData.Count - 1; i >= 0; i--)
                {
                    UndoRecordData(_listRecordData[i]);
                }
            }

            if (EditRecordData.EAction.None != _firstRecordData.ActionType)
            {
                UndoRecordData(_firstRecordData);
            }

            // 开关模式不健壮，需要重刷
            if (EditMode.Instance.IsInState(EditModeState.Switch.Instance))
            {
                EditMode.Instance.StopSwitch();
                EditMode.Instance.StartSwitch();
            }
        }

        private void RedoRecordData(EditRecordData recordData)
        {
            switch (recordData.ActionType)
            {
                case EditRecordData.EAction.AddUnit:
                    EditMode.Instance.AddUnitWithCheck(recordData.UnitDesc, recordData.UnitExtra);
                    break;
                case EditRecordData.EAction.RemoveUnit:
                    EditMode.Instance.DeleteUnitWithCheck(recordData.UnitDesc);
                    break;
                case EditRecordData.EAction.UpdateExtra:
                    DataScene2D.CurScene.ProcessUnitExtra(recordData.UnitDesc, recordData.UnitExtra);
                    break;
                case EditRecordData.EAction.AddSwitchConnection:
                    DataScene2D.CurScene.BindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
                case EditRecordData.EAction.RemoveSwitchConnection:
                    DataScene2D.CurScene.UnbindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
                case EditRecordData.EAction.ChangeMapRect:
                    bool horizontal = recordData.SwitchGuid.x != 0;
                    bool add;
                    if (horizontal)
                    {
                        add = recordData.SwitchGuid.x > 0;
                    }
                    else
                    {
                        add = recordData.SwitchGuid.y > 0;
                    }
                    DataScene2D.CurScene.ChangeMapRect(add, horizontal, false);
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
                    EditMode.Instance.AddUnitWithCheck(recordData.UnitDesc, recordData.UnitExtra);
                    break;
                case EditRecordData.EAction.UpdateExtra:
                    DataScene2D.CurScene.ProcessUnitExtra(recordData.UnitDescOld, recordData.UnitExtraOld);
                    break;
                case EditRecordData.EAction.AddSwitchConnection:
                    DataScene2D.CurScene.UnbindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
                case EditRecordData.EAction.RemoveSwitchConnection:
                    DataScene2D.CurScene.BindSwitch(recordData.SwitchGuid, recordData.UnitDesc.Guid);
                    break;
                case EditRecordData.EAction.ChangeMapRect:
                    bool horizontal = recordData.SwitchGuid.x != 0;
                    bool add;
                    if (horizontal)
                    {
                        add = recordData.SwitchGuid.x > 0;
                    }
                    else
                    {
                        add = recordData.SwitchGuid.y > 0;
                    }
                    DataScene2D.CurScene.ChangeMapRect(!add, horizontal, false);
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