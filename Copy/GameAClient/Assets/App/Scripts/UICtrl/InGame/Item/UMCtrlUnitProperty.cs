using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlUnitProperty : UMCtrlBase<UMViewUnitProperty>
    {
        public RectTransform UITran
        {
            get { return _cachedView.Trans; }
        }
        
        public void SetData(ref UnitDesc unitDesc, UnitExtraDynamic unitExtra)
        {
            var table = TableManager.Instance.GetUnit(unitDesc.Id);
            if (table == null)
            {
                return;
            }
            for (var type = EEditType.None + 1; type < EEditType.Max; type++)
            {
                if (table.CanEdit(type))
                {
                    SetPropertyEnable(type, true);
                    SetView(type, ref unitDesc, unitExtra);
                }
                else
                {
                    SetPropertyEnable(type, false);
                }
            }
        }
        
        private void SetPropertyEnable(EEditType editType, bool enable)
        {
            switch (editType)
            {
                case EEditType.None:
                    break;
                case EEditType.Direction:
                    _cachedView.ForwardDirectionDock.SetActiveEx(enable);
                    break;
                case EEditType.MoveDirection:
                    _cachedView.MoveDirectionDock.SetActiveEx(enable);
                    break;
                case EEditType.Active:
                    _cachedView.ActiveStateDock.SetActiveEx(enable);
                    break;
                case EEditType.Child:
                    _cachedView.PayloadDock.SetActiveEx(enable);
                    break;
                case EEditType.Rotate:
                    _cachedView.RotateDock.SetActiveEx(enable);
                    break;
                case EEditType.TimeDelay:
                    _cachedView.DelayDock.SetActiveEx(enable);
                    break;
                case EEditType.TimeInterval:
                    _cachedView.IntervalDock.SetActiveEx(enable);
                    break;
                case EEditType.WoodCase:
                    _cachedView.ItemDock.SetActive(enable);
                    break;
                case EEditType.Text:
                    _cachedView.TextDock.SetActiveEx(enable);
                    break;
                case EEditType.Style:
                    break;
                case EEditType.Max:
                    break;
            }
        }

        private void SetView(EEditType editType, ref UnitDesc unitDesc, UnitExtraDynamic unitExtra)
        {
            switch (editType)
            {
                case EEditType.None:
                    break;
                case EEditType.Direction:
                    var fd = unitDesc.Rotation;
                    if (UnitDefine.IsMonster(unitDesc.Id))
                    {
                        fd = (byte) (unitExtra.MoveDirection -1);
                    }
                    _cachedView.ForwardDirectionFg.rectTransform.localEulerAngles =
                        new Vector3(0, 0, -45) * EditHelper.CalcDirectionVal(fd);
                    break;
                case EEditType.MoveDirection:
                    var md = unitExtra.MoveDirection;
                    var mdFg = _cachedView.MoveDirectionFg;
                    if (md == EMoveDirection.None)
                    {
                        mdFg.sprite = JoyResManager.Instance.GetSprite(SpriteNameDefine.UnitEditMoveDirectionNone);
                        mdFg.SetNativeSize();
                        mdFg.rectTransform.localEulerAngles = Vector3.zero;
                    }
                    else
                    {
                        mdFg.sprite = JoyResManager.Instance.GetSprite(SpriteNameDefine.UnitEditMoveDirectionUp);
                        mdFg.SetNativeSize();
                        mdFg.rectTransform.localEulerAngles = 
                            new Vector3(0, 0, -45) * EditHelper.CalcDirectionVal((byte) (md - 1));
                    }
                    break;
                case EEditType.Active:
                    var aState = (EActiveState) unitExtra.Active;
                    _cachedView.ActiveStateFg.sprite =
                        JoyResManager.Instance.GetSprite(SpriteNameDefine.GetUnitEditActiveStateImage(aState));
                    break;
                case EEditType.Child:
                    _cachedView.PayloadFg.sprite = JoyResManager.Instance.GetSprite(
                            TableManager.Instance.GetEquipment(unitExtra.ChildId).Icon);
                    break;
                case EEditType.Rotate:
                    var rFgNone = _cachedView.RotateFgNone;
                    var rFgView = _cachedView.RotateFgView;
                    bool rShowNone = unitExtra.RotateMode == (int) ERotateMode.None;
                    rFgNone.SetActiveEx(rShowNone);
                    rFgView.SetActiveEx(!rShowNone);
                    if (!rShowNone)
                    {
                        int start, end;
                        if (unitExtra.RotateMode == (int) ERotateMode.Clockwise)
                        {
                            start = EditHelper.CalcDirectionVal(unitDesc.Rotation);
                            end = EditHelper.CalcDirectionVal(unitExtra.RotateValue);
                        }
                        else
                        {
                            end = EditHelper.CalcDirectionVal(unitDesc.Rotation);
                            start = EditHelper.CalcDirectionVal(unitExtra.RotateValue);
                        }
                        int count = (end + 7 - start) % 8 + 1;
                        
                        rFgView.fillAmount = 1f * count / 8;
                        rFgView.rectTransform.localEulerAngles = new Vector3(0, 0, -360f * start / 8);
                    }
                    break;
                case EEditType.TimeDelay:
                    DictionaryTools.SetContentText(_cachedView.DelayText, (unitExtra.TimeDelay * 0.001f).ToString("F1"));
                    break;
                case EEditType.TimeInterval:
                    DictionaryTools.SetContentText(_cachedView.IntervalText, (unitExtra.TimeInterval * 0.001f).ToString("F1"));
                    break;
                case EEditType.WoodCase:
                    _cachedView.ItemSpriteFg.sprite = UMCtrlWoodCaseItem.GetSprite(unitExtra.CommonValue);
                    break;
                case EEditType.Text:
                    break;
                case EEditType.Style:
                    break;
                case EEditType.Max:
                    break;
            }
        }
    }
}