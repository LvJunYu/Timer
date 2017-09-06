using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlUnitPropertyEdit : UICtrlInGameAnimationBase<UIViewUnitPropertyEdit>
    {
        #region 常量与字段

        private UnitEditData _originData;
        private Table_Unit _tableUnit;
        private UnitEditData _editData;
        private readonly List<EEditType> _validEditPropertyList = new List<EEditType>();
        private Button[] _activeMenuList;
        private Button[] _forwardMenuList;
        private Button[] _moveDirectionMenuList;
        private Button[] _rotateMenuList;
        private Button[] _rotateEndMenuList;
        private Button[] _triggerDelayMenuList;
        private Button[] _triggerIntervalMenuList;
        #endregion
        
        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            _activeMenuList = _cachedView.ActiveDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _activeMenuList.Length; i++)
            {
                var inx = i;
                _activeMenuList[i].onClick.AddListener(()=>OnActiveMenuClick(inx));
            }
            _forwardMenuList = _cachedView.ForwardDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _forwardMenuList.Length; i++)
            {
                var inx = i;
                _forwardMenuList[i].onClick.AddListener(()=>OnForwardMenuClick(inx));
            }
            _moveDirectionMenuList = _cachedView.MoveDirectionDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _moveDirectionMenuList.Length; i++)
            {
                var inx = i;
                _moveDirectionMenuList[i].onClick.AddListener(()=>OnMoveDirectionMenuClick(inx));
            }
            _rotateMenuList = _cachedView.RotateDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _rotateMenuList.Length; i++)
            {
                var inx = i;
                _rotateMenuList[i].onClick.AddListener(()=>OnRotateMenuClick(inx));
            }
            _rotateEndMenuList = _cachedView.RotateEndDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _rotateEndMenuList.Length; i++)
            {
                var inx = i;
                _rotateEndMenuList[i].onClick.AddListener(()=>OnRotateEndMenuClick(inx));
            }
            _triggerDelayMenuList = _cachedView.TriggerDelayDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _triggerDelayMenuList.Length; i++)
            {
                var inx = i;
                _triggerDelayMenuList[i].onClick.AddListener(()=>OnTriggerDelayMenuClick(inx));
                _triggerDelayMenuList[i].GetComponentInChildren<Text>().text = (i * 0.5f).ToString("F1");
            }
            _triggerIntervalMenuList = _cachedView.TriggerIntervalDock.GetComponentsInChildren<Button>();
            for (int i = 0; i < _triggerIntervalMenuList.Length; i++)
            {
                var inx = i;
                _triggerIntervalMenuList[i].onClick.AddListener(()=>OnTriggerIntervalMenuClick(inx));
                _triggerIntervalMenuList[i].GetComponentInChildren<Text>().text = (i * 0.5f).ToString("F1");
            }
        }

        protected override void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.5f).From());
            _closeSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.5f));
            _openSequence.OnComplete(OnOpenAnimationComplete).SetAutoKill(false).Pause().OnUpdate(OnOpenAnimationUpdate);
            _closeSequence.OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() => _cachedView.Trans.localPosition = Vector3.zero);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _originData = (UnitEditData) parameter;
            _editData = _originData;
            _tableUnit = TableManager.Instance.GetUnit(_originData.UnitDesc.Id);
            RefreshView();
        }

        private void RefreshView()
        {
            _validEditPropertyList.Clear();
            if (_tableUnit.CanEdit(EEditType.Active))
            {
                _validEditPropertyList.Add(EEditType.Active);
                _cachedView.ActiveDock.SetActiveEx(true);
                RefreshAcitveMenu();
            }
            else
            {
                _cachedView.ActiveDock.SetActiveEx(false);
            }
            if (_tableUnit.CanEdit(EEditType.Child))
            {
                _validEditPropertyList.Add(EEditType.Child);
            }
            if (_tableUnit.CanEdit(EEditType.Direction))
            {
                _validEditPropertyList.Add(EEditType.Direction);
                _cachedView.ForwardDock.SetActiveEx(true);
                RefreshForwardMenu();
            }
            else
            {
                _cachedView.ForwardDock.SetActiveEx(false);
            }
            if (_tableUnit.CanEdit(EEditType.MoveDirection))
            {
                _validEditPropertyList.Add(EEditType.MoveDirection);
                _cachedView.MoveDirectionDock.SetActiveEx(true);
                RefreshMoveDirectionMenu();
            }
            else
            {
                _cachedView.MoveDirectionDock.SetActiveEx(false);
            }
            if (_tableUnit.CanEdit(EEditType.Rotate))
            {
                _validEditPropertyList.Add(EEditType.Rotate);
                _cachedView.RotateDock.SetActiveEx(true);
                _cachedView.RotateEndDock.SetActiveEx(true);
                RefreshRotateMenu();
                RefreshRotateEndMenu();
            }
            else
            {
                _cachedView.RotateDock.SetActiveEx(false);
                _cachedView.RotateEndDock.SetActiveEx(false);
            }
            if (_tableUnit.CanEdit(EEditType.Style))
            {
                _validEditPropertyList.Add(EEditType.Style);
            }
            if (_tableUnit.CanEdit(EEditType.Text))
            {
                _validEditPropertyList.Add(EEditType.Text);
            }
            if (_tableUnit.CanEdit(EEditType.Time))
            {
                _validEditPropertyList.Add(EEditType.Time);
                _cachedView.TriggerDelayDock.SetActiveEx(true);
                _cachedView.TriggerIntervalDock.SetActiveEx(true);
                RefreshTriggerDelayMenu();
                RefreshTriggerIntervalMenu();
            }
            else
            {
                _cachedView.TriggerDelayDock.SetActiveEx(false);
                _cachedView.TriggerIntervalDock.SetActiveEx(false);
            }
        }

        private void RefreshAcitveMenu()
        {
            for (int i = 0; i < _activeMenuList.Length; i++)
            {
                _activeMenuList[i].interactable = i != _editData.UnitExtra.Active;
            }
        }
        private void RefreshForwardMenu()
        {
            for (int i = 0; i < _forwardMenuList.Length; i++)
            {
                _forwardMenuList[i].interactable = i != _editData.UnitDesc.Rotation;
            }
        }
        private void RefreshMoveDirectionMenu()
        {
            for (int i = 0; i < _moveDirectionMenuList.Length; i++)
            {
                _moveDirectionMenuList[i].interactable = i != (int) _editData.UnitExtra.MoveDirection;
            }
        }
        private void RefreshRotateMenu()
        {
            for (int i = 0; i < _rotateMenuList.Length; i++)
            {
                _rotateMenuList[i].interactable = i != _editData.UnitExtra.RotateMode;
            }
        }
        private void RefreshRotateEndMenu()
        {
            for (int i = 0; i < _rotateEndMenuList.Length; i++)
            {
                _rotateEndMenuList[i].interactable = i != _editData.UnitExtra.RotateValue;
            }
        }
        private void RefreshTriggerDelayMenu()
        {
            for (int i = 0; i < _triggerDelayMenuList.Length; i++)
            {
                _triggerDelayMenuList[i].interactable = i != _editData.UnitExtra.TimeDelay;
            }
        }
        private void RefreshTriggerIntervalMenu()
        {
            for (int i = 0; i < _triggerIntervalMenuList.Length; i++)
            {
                _triggerIntervalMenuList[i].interactable = i != _editData.UnitExtra.TimeInterval;
            }
        }
        
        


        private void OnActiveMenuClick(int inx)
        {
            _editData.UnitExtra.Active = (byte) inx;
            RefreshAcitveMenu();
        }

        private void OnForwardMenuClick(int inx)
        {
            _editData.UnitDesc.Rotation = (byte) inx;
            RefreshForwardMenu();
        }

        private void OnMoveDirectionMenuClick(int inx)
        {
            _editData.UnitExtra.MoveDirection = (EMoveDirection) inx;
            RefreshMoveDirectionMenu();
        }

        private void OnRotateMenuClick(int inx)
        {
            _editData.UnitExtra.RotateMode = (byte) inx;
            RefreshRotateMenu();
        }

        private void OnRotateEndMenuClick(int inx)
        {
            _editData.UnitExtra.RotateValue = (byte) inx;
            RefreshRotateEndMenu();
        }

        private void OnTriggerDelayMenuClick(int inx)
        {
            _editData.UnitExtra.TimeDelay = (ushort) inx;
            RefreshTriggerDelayMenu();
        }

        private void OnTriggerIntervalMenuClick(int inx)
        {
            _editData.UnitExtra.TimeInterval = (ushort) inx;
            RefreshTriggerIntervalMenu();
        }

        private void OnCloseBtnClick()
        {
            if (_originData != _editData)
            {
                if (UnitDefine.IsMonster(_tableUnit.Id))
                {
                    _editData.UnitExtra.MoveDirection = (EMoveDirection) (_editData.UnitDesc.Rotation + 1);
                    _editData.UnitDesc.Rotation = _originData.UnitDesc.Rotation;
                }
                EditModeState.Global.Instance.ModifyUnitData(_originData.UnitDesc, _originData.UnitExtra,
                    _editData.UnitDesc, _editData.UnitExtra);
            }
            SocialGUIManager.Instance.CloseUI<UICtrlUnitPropertyEdit>();
        }
        #endregion
    }
}
