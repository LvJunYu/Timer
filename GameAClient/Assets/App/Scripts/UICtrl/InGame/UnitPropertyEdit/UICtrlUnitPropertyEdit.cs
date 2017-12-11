using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlUnitPropertyEdit : UICtrlInGameAnimationBase<UIViewUnitPropertyEdit>
    {
        private const float MenuPosRadius = 295;
        private const float MenuOptionsPosRadius = 180;
        private const float RotateEndOptionsPosRadius = 195;
        private const float OptionArrowRadius = 190;
        private const float MenuArrowRadius = 18;
        public const int MessageStringCountMax = 45;
        private UPCtrlUnitPropertyEditAdvance _upCtrlUnitPropertyEditAdvance;
        private UPCtrlUnitPropertyEditPreinstall _upCtrlUnitPropertyEditPreinstall;
        private UnitEditData _originData;
        private Table_Unit _tableUnit;
        public UnitEditData EditData;
        private GameObject[] _rootArray = new GameObject[(int) EEditType.Max];
        private USCtrlUnitPropertyEditButton[] _menuButtonArray = new USCtrlUnitPropertyEditButton[(int) EEditType.Max];
        private readonly List<EEditType> _validEditPropertyList = new List<EEditType>();
        private USCtrlUnitPropertyEditButton[] _activeMenuList;
        private USCtrlUnitPropertyEditButton[] _forwardMenuList;
        private USCtrlUnitPropertyEditButton[] _payloadMenuList;
        private USCtrlUnitPropertyEditButton[] _moveDirectionMenuList;
        private USCtrlUnitPropertyEditButton[] _rotateMenuList;
        private USCtrlUnitPropertyEditButton[] _rotateEndMenuList;
        private USCtrlUnitPropertyEditButton[] _triggerDelayMenuList;
        private USCtrlUnitPropertyEditButton[] _triggerIntervalMenuList;
        private USCtrlUnitPropertyEditButton[] _campMenuList;
        private Image[] _optionRotateArrowList;
        private Image[] _menuRotateArrowList;
        private float _posTweenFactor;
        private EEditType _curEditType;

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<int>(EMessengerType.OnPreinstallRead, OnPreinstallRead);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void CreateSequences()
        {
            const float tweenTime = 0.2f;

            var openTweenerPos = DOTween.To(() => _posTweenFactor, v =>
            {
                _posTweenFactor = v;
                _cachedView.ContentDock.anchoredPosition = Vector2.Lerp(_startPos, Vector2.zero, v);
            }, 1f, tweenTime).SetEase(Ease.OutBack);
            var closeTweenerPos = DOTween.To(() => _posTweenFactor, v =>
            {
                _posTweenFactor = v;
                _cachedView.ContentDock.anchoredPosition = Vector2.Lerp(_startPos, Vector2.zero, v);
            }, 0f, tweenTime);
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(_cachedView.ContentDock.DOScale(Vector3.zero, tweenTime).From().SetEase(Ease.OutBack))
                .Join(_cachedView.CloseBtn.targetGraphic.DOFade(0, tweenTime).From())
                .Join(openTweenerPos)
                .OnComplete(OnOpenAnimationComplete)
                .SetAutoKill(false)
                .Pause()
                .OnUpdate(OnOpenAnimationUpdate);
            _closeSequence.Append(_cachedView.ContentDock.DOScale(Vector3.zero, tweenTime))
                .Join(_cachedView.CloseBtn.targetGraphic.DOFade(0, tweenTime))
                .Join(closeTweenerPos)
                .OnComplete(OnCloseAnimationComplete)
                .SetAutoKill(false)
                .Pause();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            _upCtrlUnitPropertyEditAdvance = new UPCtrlUnitPropertyEditAdvance();
            _upCtrlUnitPropertyEditAdvance.Init(this, _cachedView);
            _upCtrlUnitPropertyEditPreinstall = new UPCtrlUnitPropertyEditPreinstall();
            _upCtrlUnitPropertyEditPreinstall.Init(this, _cachedView);

            _rootArray[(int) EEditType.Active] = _cachedView.ActiveDock;
            _rootArray[(int) EEditType.Direction] = _cachedView.ForwardDock;
            _rootArray[(int) EEditType.Child] = _cachedView.PayloadDock;
            _rootArray[(int) EEditType.MoveDirection] = _cachedView.MoveDirectionDock;
            _rootArray[(int) EEditType.Rotate] = _cachedView.RotateDock;
            _rootArray[(int) EEditType.TimeDelay] = _cachedView.TriggerDelayDock;
            _rootArray[(int) EEditType.TimeInterval] = _cachedView.TriggerIntervalDock;
            _rootArray[(int) EEditType.Text] = _cachedView.TextDock;
            _rootArray[(int) EEditType.Camp] = _cachedView.CampDock;

            for (var type = EEditType.None + 1; type < EEditType.Max; type++)
            {
                _menuButtonArray[(int) type] = new USCtrlUnitPropertyEditButton();
            }

            _menuButtonArray[(int) EEditType.Active].Init(_cachedView.ActiveStateMenu);
            _menuButtonArray[(int) EEditType.Direction].Init(_cachedView.DirectionMenu);
            _menuButtonArray[(int) EEditType.Child].Init(_cachedView.PayloadMenu);
            _menuButtonArray[(int) EEditType.MoveDirection].Init(_cachedView.MoveDirectionMenu);
            _menuButtonArray[(int) EEditType.Rotate].Init(_cachedView.RotateStateMenu);
            _menuButtonArray[(int) EEditType.TimeDelay].Init(_cachedView.TimeDelayMenu);
            _menuButtonArray[(int) EEditType.TimeInterval].Init(_cachedView.TimeIntervalMenu);
            _menuButtonArray[(int) EEditType.Text].Init(_cachedView.TextMenu);
            _menuButtonArray[(int) EEditType.Camp].Init(_cachedView.CampMenu);

            for (var type = EEditType.None + 1; type < EEditType.Max; type++)
            {
                var t = type;
                if (!_menuButtonArray[(int) type].HasInited)
                {
                    continue;
                }
                _menuButtonArray[(int) type].AddClickListener(() => OnEditTypeMenuClick(t));
            }

            var list = _cachedView.ActiveDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _activeMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _activeMenuList[i] = button;
                _activeMenuList[i].AddClickListener(() => OnActiveMenuClick(inx));
            }
            list = _cachedView.ForwardDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _forwardMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _forwardMenuList[i] = button;
                _forwardMenuList[i].AddClickListener(() => OnForwardMenuClick(inx));

                if (i < 4)
                {
                    button.SetPosAngle(90 * i, MenuOptionsPosRadius);
                    button.SetFgImageAngle(90 * i);
                    button.SetBgImageAngle(90 * i);
                }
                else
                {
                    button.SetPosAngle(45 + 90 * (i - 4), MenuOptionsPosRadius);
                    button.SetFgImageAngle(45 + 90 * (i - 4));
                    button.SetBgImageAngle(45 + 90 * (i - 4));
                }
            }
            list = _cachedView.PayloadDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _payloadMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _payloadMenuList[i] = button;
                _payloadMenuList[i].AddClickListener(() => OnPayloadMenuClick(inx));
            }
            list = _cachedView.MoveDirectionDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _moveDirectionMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _moveDirectionMenuList[i] = button;
                _moveDirectionMenuList[i].AddClickListener(() => OnMoveDirectionMenuClick(inx));
                if (i == 0)
                {
                }
                else if (i < 5)
                {
                    button.SetPosAngle(90 * (i - 1), MenuOptionsPosRadius);
                    button.SetFgImageAngle(90 * (i - 1));
                    button.SetBgImageAngle(90 * (i - 1));
                }
            }
            list = _cachedView.RotateModeDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _rotateMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _rotateMenuList[i] = button;
                _rotateMenuList[i].AddClickListener(() => OnRotateMenuClick(inx));
            }
            list = _cachedView.RotateEndDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _rotateEndMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _rotateEndMenuList[i] = button;
                _rotateEndMenuList[i].AddClickListener(() => OnRotateEndMenuClick(inx));
                if (i < 4)
                {
                    button.SetPosAngle(90 * i, RotateEndOptionsPosRadius);
                }
                else
                {
                    button.SetPosAngle(45 + 90 * (i - 4), RotateEndOptionsPosRadius);
                }
            }
            list = _cachedView.TriggerDelayDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _triggerDelayMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            var da = 360f / list.Length;
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _triggerDelayMenuList[i] = button;
                _triggerDelayMenuList[i].AddClickListener(() => OnTriggerDelayMenuClick(inx));
                _triggerDelayMenuList[i].SetText((i * 0.5f).ToString("F1"));
                button.SetPosAngle(da * i, MenuOptionsPosRadius);
                button.SetBgImageAngle(da * i);
            }
            list = _cachedView.TriggerIntervalDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _triggerIntervalMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            da = 360f / list.Length;
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _triggerIntervalMenuList[i] = button;
                _triggerIntervalMenuList[i].AddClickListener(() => OnTriggerIntervalMenuClick(inx));
                _triggerIntervalMenuList[i].SetText((i * 0.5f).ToString("F1"));
                button.SetPosAngle(da * i, MenuOptionsPosRadius);
                button.SetBgImageAngle(da * i);
            }
            _optionRotateArrowList = _cachedView.RotateArrowDock.GetComponentsInChildren<Image>();
            _menuRotateArrowList = _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.RotateArrows
                .GetComponentsInChildren<Image>();
            float arrowDeltaAngle = Mathf.PI * 2 / 8;
            float arrowBaseAngle = arrowDeltaAngle / 2;
            for (int i = 0; i < _optionRotateArrowList.Length; i++)
            {
                var arrow = _optionRotateArrowList[i];
                var menuArrow = _menuRotateArrowList[i];
                var rad = arrowBaseAngle + arrowDeltaAngle * i;
                arrow.rectTransform.anchoredPosition = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * OptionArrowRadius;
                arrow.rectTransform.localEulerAngles = new Vector3(0, 0, -45 * i);
                menuArrow.rectTransform.anchoredPosition =
                    new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * MenuArrowRadius;
                menuArrow.rectTransform.localEulerAngles = new Vector3(0, 0, -45 * i);
            }

            list = _cachedView.CampDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _campMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            da = 360f / TeamManager.MaxTeamCount;
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _campMenuList[i] = button;
                _campMenuList[i].AddClickListener(() => OnCampMenuClick(inx));
                if (inx > 0 && inx < 8)
                {
                    button.SetPosAngle(da * i, MenuOptionsPosRadius);
                    button.SetBgImageAngle(da * i);
                }
            }
        }

        protected override void OnOpen(object parameter)
        {
            var pos = Input.mousePosition;
            _startPos = SocialGUIManager.ScreenToRectLocal(pos, _cachedView.Trans);
            base.OnOpen(parameter);
            _originData = (UnitEditData) parameter;
            EditData = _originData;
            _tableUnit = TableManager.Instance.GetUnit(_originData.UnitDesc.Id);
            if (UnitDefine.IsMonster(_tableUnit.Id))
            {
                EditData.UnitDesc.Rotation = (byte) (EditData.UnitExtra.MoveDirection - 1);
            }
            RefreshView();
            OnEditTypeMenuClick(_validEditPropertyList[0]);
            _upCtrlUnitPropertyEditPreinstall.Open();
        }

        protected override void OnClose()
        {
            _upCtrlUnitPropertyEditPreinstall.Close();
            base.OnClose();
        }

        protected override void OnOpenAnimationComplete()
        {
            base.OnOpenAnimationComplete();
            if (_curEditType == EEditType.Camp)
            {
                _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.ActorSetting);
            }
            else if (_curEditType == EEditType.Child)
            {
                _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.WeaponSetting);
            }
        }

        private void RefreshView()
        {
            _validEditPropertyList.Clear();

            if (_tableUnit.CanEdit(EEditType.Active))
            {
                _validEditPropertyList.Add(EEditType.Active);
                _menuButtonArray[(int) EEditType.Active].SetEnable(true);
                RefreshAcitveMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Active].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.Child))
            {
                _validEditPropertyList.Add(EEditType.Child);
                _menuButtonArray[(int) EEditType.Child].SetEnable(true);
                var da = 360f / _tableUnit.ChildState.Length;
                for (int i = 0; i < _tableUnit.ChildState.Length; i++)
                {
                    _payloadMenuList[i]
                        .SetFgImage(JoyResManager.Instance.GetSprite(TableManager.Instance
                            .GetEquipment(_tableUnit.ChildState[i]).Icon));
                    _payloadMenuList[i].SetPosAngle(da * i, MenuOptionsPosRadius);
                }
                RefreshPayloadMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Child].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.Camp))
            {
                _validEditPropertyList.Add(EEditType.Camp);
                _menuButtonArray[(int) EEditType.Camp].SetEnable(true);
                RefreshCampMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Camp].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.Direction))
            {
                _validEditPropertyList.Add(EEditType.Direction);
                _menuButtonArray[(int) EEditType.Direction].SetEnable(true);
                RefreshForwardMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Direction].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.MoveDirection))
            {
                _validEditPropertyList.Add(EEditType.MoveDirection);
                _menuButtonArray[(int) EEditType.MoveDirection].SetEnable(true);
                RefreshMoveDirectionMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.MoveDirection].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.Rotate))
            {
                _validEditPropertyList.Add(EEditType.Rotate);
                _menuButtonArray[(int) EEditType.Rotate].SetEnable(true);
                RefreshRotateModeMenu();
                RefreshRotateEndMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Rotate].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.Text))
            {
                _validEditPropertyList.Add(EEditType.Text);
                _menuButtonArray[(int) EEditType.Text].SetEnable(true);
                RefreshTextDock();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Text].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.TimeDelay))
            {
                _validEditPropertyList.Add(EEditType.TimeDelay);
                _menuButtonArray[(int) EEditType.TimeDelay].SetEnable(true);
                RefreshTriggerDelayMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.TimeDelay].SetEnable(false);
            }
            if (_tableUnit.CanEdit(EEditType.TimeInterval) && _tableUnit.ChildState == null)
            {
                _validEditPropertyList.Add(EEditType.TimeInterval);
                _menuButtonArray[(int) EEditType.TimeInterval].SetEnable(true);
                RefreshTriggerIntervalMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.TimeInterval].SetEnable(false);
            }

            const int menuAngle = 20;
            var totalAngle = menuAngle * _validEditPropertyList.Count;
            var baseAngle = 180 + totalAngle / 2 - menuAngle / 2;
            for (int i = 0; i < _validEditPropertyList.Count; i++)
            {
                var angle = baseAngle - i * menuAngle;
                _menuButtonArray[(int) _validEditPropertyList[i]].SetBgImageAngle(angle - 180);
                _menuButtonArray[(int) _validEditPropertyList[i]].SetPosAngle(angle, MenuPosRadius);
            }
        }

        private void RefreshAcitveMenu()
        {
            var val = Mathf.Clamp(EditData.UnitExtra.Active - 1, 0, 1);
            for (int i = 0; i < _activeMenuList.Length; i++)
            {
                _activeMenuList[i].SetSelected(i == val);
            }
            _menuButtonArray[(int) EEditType.Active]
                .SetFgImage(_activeMenuList[val].View.FgImage.sprite);
        }

        private void RefreshForwardMenu()
        {
            var val = Mathf.Clamp(EditData.UnitDesc.Rotation, 0, _forwardMenuList.Length - 1);
            for (byte i = 0; i < _forwardMenuList.Length; i++)
            {
                if (EditHelper.CheckMask(i, _tableUnit.DirectionMask))
                {
                    _forwardMenuList[i].SetEnable(true);
                    _forwardMenuList[i].SetSelected(i == val);
                }
                else
                {
                    _forwardMenuList[i].SetEnable(false);
                }
            }
            _menuButtonArray[(int) EEditType.Direction].View.FgImage.rectTransform.localEulerAngles =
                _forwardMenuList[val].View.FgImage.rectTransform.localEulerAngles;

            if (_tableUnit.CanEdit(EEditType.Rotate))
            {
                RefreshRotateEndMenu();
            }
        }

        private void RefreshPayloadMenu()
        {
            var table = TableManager.Instance.GetEquipment(EditData.UnitExtra.ChildId);
            if (table != null)
            {
                _menuButtonArray[(int) EEditType.Child].SetFgImage(
                    JoyResManager.Instance.GetSprite(table.Icon));
            }
            else
            {
                EditData.UnitExtra.ChildId = (ushort) _tableUnit.ChildState[0];
                EditData.UnitExtra.UpdateFromChildId();
            }
            var totalCount = _tableUnit.ChildState.Length;
            var da = 360f / totalCount;
            for (int i = 0; i < _payloadMenuList.Length; i++)
            {
                if (i < _tableUnit.ChildState.Length)
                {
                    _payloadMenuList[i].SetEnable(true);
                    _payloadMenuList[i].SetFgImage(JoyResManager.Instance.GetSprite(
                        TableManager.Instance.GetEquipment(_tableUnit.ChildState[i]).Icon));
                    _payloadMenuList[i].SetSelected(_tableUnit.ChildState[i] == EditData.UnitExtra.ChildId);
                    _payloadMenuList[i].SetBgImageAngle(da * i);
                }
                else
                {
                    _payloadMenuList[i].SetEnable(false);
                }
            }
        }

        private void RefreshMoveDirectionMenu()
        {
            var val = Mathf.Clamp((int) EditData.UnitExtra.MoveDirection, 0, _moveDirectionMenuList.Length - 1);
            var defaultVal = -1;
            for (int i = 0; i < _moveDirectionMenuList.Length; i++)
            {
                var checkVal = (i + 4) % 5;
                if (EditHelper.CheckMask(checkVal, _tableUnit.MoveDirectionMask))
                {
                    _moveDirectionMenuList[i].SetEnable(true);
                    _moveDirectionMenuList[i].SetSelected(i == val);
                    defaultVal = i;
                }
                else
                {
                    _moveDirectionMenuList[i].SetEnable(false);
                    if (val == i)
                    {
                        val = -1;
                    }
                }
            }
            if (val == -1)
            {
                val = defaultVal;
                if (val != -1)
                {
                    _moveDirectionMenuList[val].SetSelected(true);
                }
            }
            var menuBtn = _menuButtonArray[(int) EEditType.MoveDirection];
            var optionBtn = _moveDirectionMenuList[val];

            menuBtn.SetFgImage(optionBtn.View.FgImage.sprite);
            menuBtn.View.FgImage.SetNativeSize();
            menuBtn.View.FgImage.rectTransform.localEulerAngles = optionBtn.View.FgImage.rectTransform.localEulerAngles;
        }

        private void RefreshRotateModeMenu()
        {
            var val = Mathf.Clamp(EditData.UnitExtra.RotateMode, 0, _rotateMenuList.Length - 1);
            for (int i = 0; i < _rotateMenuList.Length; i++)
            {
                _rotateMenuList[i].SetSelected(i == val);
            }
            RefreshRotateEndMenu();
        }

        private void RefreshRotateEndMenu()
        {
            ERotateMode rotateMode = (ERotateMode) EditData.UnitExtra.RotateMode;
            if (rotateMode == ERotateMode.None)
            {
                _cachedView.RotateEndDock.SetActiveEx(false);
                _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.RotateDock.SetActiveEx(false);
                _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.FgImage.SetActiveEx(true);
                return;
            }
            _cachedView.RotateEndDock.SetActiveEx(true);
            _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.FgImage.SetActiveEx(false);
            _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.RotateDock.SetActiveEx(true);
            var forwardBg = JoyResManager.Instance.GetSprite(SpriteNameDefine.UnitEditRotateEndBgForward);
            var normalBg = JoyResManager.Instance.GetSprite(SpriteNameDefine.UnitEditRotateEndBgNormal);
            int start, end;
            if (rotateMode == ERotateMode.Clockwise)
            {
                start = EditHelper.CalcDirectionVal(EditData.UnitDesc.Rotation);
                end = EditHelper.CalcDirectionVal(EditData.UnitExtra.RotateValue);
                _cachedView.RotateArrowDock.transform.localEulerAngles = Vector3.zero;
                _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.RotateArrows.transform.localEulerAngles =
                    Vector3.zero;
            }
            else
            {
                end = EditHelper.CalcDirectionVal(EditData.UnitDesc.Rotation);
                start = EditHelper.CalcDirectionVal(EditData.UnitExtra.RotateValue);
                _cachedView.RotateArrowDock.transform.localEulerAngles = new Vector3(0, 180, 0);
                _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.RotateArrows.transform.localEulerAngles =
                    new Vector3(0, 180, 0);
            }
            int count = (end + 7 - start) % 8 + 1;

            for (int i = 0; i < _rotateEndMenuList.Length; i++)
            {
                var btn = _rotateEndMenuList[i];
                btn.SetBgImage(i == EditData.UnitDesc.Rotation ? forwardBg : normalBg);
                btn.SetSelected(i == EditData.UnitExtra.RotateValue);
                var inx = rotateMode == ERotateMode.Clockwise ? (start + i) % 8 : 7 - (start + i) % 8;
                _optionRotateArrowList[inx].SetActiveEx(i < count);
                _menuRotateArrowList[inx].SetActiveEx(i < count);
            }
            _cachedView.RotateViewImage.fillAmount = 1f * count / 8;
            _cachedView.RotateViewImage.rectTransform.localEulerAngles = new Vector3(0, 0, -360f * start / 8);
            var menuRotateViewImage = _menuButtonArray[(int) EEditType.Rotate].RotateMenuView.RotateView;
            menuRotateViewImage.fillAmount = 1f * count / 8;
            menuRotateViewImage.rectTransform.localEulerAngles = new Vector3(0, 0, -360f * start / 8);
        }

        private void RefreshTriggerDelayMenu()
        {
            for (int i = 0; i < _triggerDelayMenuList.Length; i++)
            {
                _triggerDelayMenuList[i].SetSelected(i * 500 == EditData.UnitExtra.TimeDelay);
            }
            _menuButtonArray[(int) EEditType.TimeDelay]
                .SetText2((EditData.UnitExtra.TimeDelay * 0.001f).ToString("F1"));
        }

        private void RefreshTriggerIntervalMenu()
        {
            for (int i = 0; i < _triggerIntervalMenuList.Length; i++)
            {
                _triggerIntervalMenuList[i].SetSelected(i * 500 == EditData.UnitExtra.TimeInterval);
            }
            _menuButtonArray[(int) EEditType.TimeInterval]
                .SetText2((EditData.UnitExtra.TimeInterval * 0.001f).ToString("F1"));
        }

        private void RefreshCampMenu()
        {
            var val = Mathf.Clamp(EditData.UnitExtra.TeamId, 0, TeamManager.MaxTeamCount);
            for (int i = 0; i < _campMenuList.Length; i++)
            {
                _campMenuList[i].SetSelected(i == val);
            }
        }

        private void RefreshTextDock()
        {
            _cachedView.TextInput.text = _originData.UnitExtra.Msg;
            _cachedView.TextInput.characterLimit = MessageStringCountMax;
        }

        private void OnActiveMenuClick(int inx)
        {
            EditData.UnitExtra.Active = (byte) (inx + 1);
            RefreshAcitveMenu();
        }

        private void OnForwardMenuClick(int inx)
        {
            EditData.UnitDesc.Rotation = (byte) inx;
            RefreshForwardMenu();
        }

        private void OnPayloadMenuClick(int inx)
        {
            if (EditData.UnitExtra.ChildId != _tableUnit.ChildState[inx])
            {
                EditData.UnitExtra.ChildId = (ushort) _tableUnit.ChildState[inx];
                EditData.UnitExtra.UpdateFromChildId();
                _upCtrlUnitPropertyEditAdvance.OnChildIdChanged();
            }
            RefreshPayloadMenu();
        }

        private void OnMoveDirectionMenuClick(int inx)
        {
            EditData.UnitExtra.MoveDirection = (EMoveDirection) inx;
            RefreshMoveDirectionMenu();
            _upCtrlUnitPropertyEditAdvance.Close();
        }

        private void OnRotateMenuClick(int inx)
        {
            EditData.UnitExtra.RotateMode = (byte) inx;
            RefreshRotateModeMenu();
        }

        private void OnRotateEndMenuClick(int inx)
        {
            EditData.UnitExtra.RotateValue = (byte) inx;
            RefreshRotateEndMenu();
        }

        private void OnTriggerDelayMenuClick(int inx)
        {
            EditData.UnitExtra.TimeDelay = (ushort) (inx * 500);
            RefreshTriggerDelayMenu();
        }

        private void OnTriggerIntervalMenuClick(int inx)
        {
            EditData.UnitExtra.TimeInterval = (ushort) (inx * 500);
            RefreshTriggerIntervalMenu();
        }

        private void OnCampMenuClick(int inx)
        {
            EditData.UnitExtra.TeamId = (byte) inx;
            RefreshCampMenu();
        }

        private void OnCloseBtnClick()
        {
            if (_openSequence.IsPlaying() || _closeSequence.IsPlaying())
            {
                return;
            }
            if (_tableUnit.CanEdit(EEditType.Text))
            {
                if (_cachedView.TextInput.text != EditData.UnitExtra.Msg)
                {
                    EditData.UnitExtra.Msg = _cachedView.TextInput.text;
                }
            }
            if (_originData != EditData)
            {
                if (UnitDefine.IsMonster(_tableUnit.Id))
                {
                    EditData.UnitExtra.MoveDirection = (EMoveDirection) (EditData.UnitDesc.Rotation + 1);
                    EditData.UnitDesc.Rotation = 0;
                }
                EditHelper.CompleteEditUnitData(_originData, EditData);
            }
            _upCtrlUnitPropertyEditAdvance.CheckClose();
            SocialGUIManager.Instance.CloseUI<UICtrlUnitPropertyEdit>();
        }

        private void OnEditTypeMenuClick(EEditType editType)
        {
            _curEditType = editType;
            for (var type = EEditType.None + 1; type < EEditType.Max; type++)
            {
                if (!_menuButtonArray[(int) type].HasInited)
                {
                    continue;
                }
                _menuButtonArray[(int) type].SetSelected(type == editType);
                _rootArray[(int) type].SetActiveEx(type == editType);
            }
            if (editType == EEditType.Camp)
            {
                if (!_openSequence.IsPlaying())
                {
                    _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.ActorSetting);
                }
            }
            else if (editType == EEditType.Child)
            {
                if (!_openSequence.IsPlaying())
                {
                    _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.WeaponSetting);
                }
            }
            else
            {
                _upCtrlUnitPropertyEditAdvance.Close();
            }
        }

        private void OnPreinstallRead(int index)
        {
            _upCtrlUnitPropertyEditPreinstall.OnPreinstallRead(index);
            RefreshView();
        }
    }
}