using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
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
        private const int MessageStringCountMax = 45;
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
        private USCtrlUnitPropertyEditButton[] _npcTypeMenuList;
        private USCtrlUnitPropertyEditButton[] _monsterCaveMenuList;
        private USCtrlUnitPropertyEditButton[] _spawnMenuList;
        private Image[] _optionRotateArrowList;
        private Image[] _menuRotateArrowList;
        private float _posTweenFactor;
        private EEditType _curEditType;
        private EEnterType _curEnterType;
        private int _curSelectedPlayerIndex;
        private Project _project;
        private UnitExtraDynamic _curUnitExtra;
        private int _curId;

        //npc 之间的引用数据类型
        public List<NpcTaskDynamic> NpcTaskDatas;

        public EEnterType CurEnterType
        {
            get { return _curEnterType; }
        }

        public int CurSelectedPlayerIndex
        {
            get { return _curSelectedPlayerIndex; }
        }

        public Project Project
        {
            get { return _project; }
        }

        public EEditType CurEditType
        {
            get { return _curEditType; }
        }

        public int CurId
        {
            get { return _curId; }
        }

        private UPCtrlUnitPropertyEditNpcDiaType _upCtrlUnitPropertyEditNpcDiaType;
        public UPCtrlUnitPropertyEditNpcTaskType UpCtrlUnitPropertyEditNpcTaskType;
        public UPCtrlUnitPropertyEditNpcTaskAdvance UpCtrlUnitPropertyEditNpcTaskAdvance;

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
            // npc对话类
            _upCtrlUnitPropertyEditNpcDiaType = new UPCtrlUnitPropertyEditNpcDiaType();
            _upCtrlUnitPropertyEditNpcDiaType.Init(this, _cachedView);
            //任务类
            UpCtrlUnitPropertyEditNpcTaskType = new UPCtrlUnitPropertyEditNpcTaskType();
            UpCtrlUnitPropertyEditNpcTaskType.Init(this, _cachedView);
            //任务细分面板
            UpCtrlUnitPropertyEditNpcTaskAdvance = new UPCtrlUnitPropertyEditNpcTaskAdvance();
            UpCtrlUnitPropertyEditNpcTaskAdvance.Init(this, _cachedView);

            _rootArray[(int) EEditType.Active] = _cachedView.ActiveDock;
            _rootArray[(int) EEditType.Direction] = _cachedView.ForwardDock;
            _rootArray[(int) EEditType.Child] = _cachedView.PayloadDock;
            _rootArray[(int) EEditType.MoveDirection] = _cachedView.MoveDirectionDock;
            _rootArray[(int) EEditType.Rotate] = _cachedView.RotateDock;
            _rootArray[(int) EEditType.TimeDelay] = _cachedView.TriggerDelayDock;
            _rootArray[(int) EEditType.TimeInterval] = _cachedView.TriggerIntervalDock;
            _rootArray[(int) EEditType.Text] = _cachedView.TextDock;
            _rootArray[(int) EEditType.Camp] = _cachedView.CampDock;

            _rootArray[(int) EEditType.NpcType] = _cachedView.NpcTypeDock;
            _rootArray[(int) EEditType.NpcTask] = _cachedView.NpcDiaLogDock;
            _rootArray[(int) EEditType.MonsterCave] = _cachedView.MonsterCaveDock;
            _rootArray[(int) EEditType.Spawn] = _cachedView.SpawnDock;

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

            _menuButtonArray[(int) EEditType.NpcType].Init(_cachedView.NpcTypeMenu);
            _menuButtonArray[(int) EEditType.NpcTask].Init(_cachedView.NpcTaskSettingMenu);
            _menuButtonArray[(int) EEditType.MonsterCave].Init(_cachedView.MonsterCaveMenu);
            _menuButtonArray[(int) EEditType.Spawn].Init(_cachedView.SpawnMenu);

            for (var type = EEditType.None + 1; type < EEditType.Max; type++)
            {
                var t = type;
                if (!_menuButtonArray[(int) type].HasInited)
                {
                    continue;
                }

                _menuButtonArray[(int) type].AddClickListener(() => OnEditTypeMenuClick(t));
            }

            //激活
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

            //Npc类型
            list = _cachedView.NpcTypeDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _npcTypeMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _npcTypeMenuList[i] = button;
                _npcTypeMenuList[i].AddClickListener(() => OnNpcTypeMenuClick(inx));
            }

            //朝向
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

            // 装弹
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

            // 移动方向
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

            // 旋转方向
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

            // 旋转结束方向
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

            //出发延迟
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

            // 触发间隔
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

            //阵营  
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
                if (inx > 0 && inx < 7)
                {
                    button.SetPosAngle(da * i, MenuOptionsPosRadius);
                    button.SetBgImageAngle(da * i);
                }
            }

            //怪物洞穴
            list = _cachedView.MonsterCaveDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _monsterCaveMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            da = 360f / UnitDefine.MonstersInCave.Length;
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _monsterCaveMenuList[i] = button;
                _monsterCaveMenuList[i].AddClickListener(() => OnMonsterCaveMenuClick(inx));
                button.SetPosAngle(da * i, MenuOptionsPosRadius);
                button.SetBgImageAngle(da * i);
            }

            //出生点
            list = _cachedView.SpawnDock.GetComponentsInChildren<USViewUnitPropertyEditButton>();
            _spawnMenuList = new USCtrlUnitPropertyEditButton[list.Length];
            da = 360f / list.Length;
            for (int i = 0; i < list.Length; i++)
            {
                var inx = i;
                var button = new USCtrlUnitPropertyEditButton();
                button.Init(list[i]);
                _spawnMenuList[i] = button;
                _spawnMenuList[i].AddClickListener(() => OnSpawnMenuClick(inx));
                _spawnMenuList[i].AddDeleteBtnListener(() => OnSpawnMenuDelete(inx));
                button.SetPosAngle(da * i, MenuOptionsPosRadius);
            }
        }

        protected override void OnOpen(object parameter)
        {
            var pos = Input.mousePosition;
            _startPos = SocialGUIManager.ScreenToRectLocal(pos, _cachedView.Trans);
            base.OnOpen(parameter);
            _originData = (UnitEditData) parameter;
            EditData = _originData;
            EditData.UnitExtra = _originData.UnitExtra.Clone();
            _project = GM2DGame.Instance.GameMode.Project;
//            _project.ProjectType = EProjectType.PS_Compete; //todo 临时
            if (_project == null)
            {
                LogHelper.Error("RefreshSpawmMenu, but project is null");
                return;
            }

            Reset();
            _upCtrlUnitPropertyEditPreinstall.Open();
        }

        protected override void OnClose()
        {
            _upCtrlUnitPropertyEditAdvance.Close();
            _upCtrlUnitPropertyEditNpcDiaType.Close();
            UpCtrlUnitPropertyEditNpcTaskAdvance.Close();
            UpCtrlUnitPropertyEditNpcTaskType.Close();
            _upCtrlUnitPropertyEditPreinstall.Close();
            base.OnClose();
        }

        protected override void OnOpenAnimationComplete()
        {
            base.OnOpenAnimationComplete();
            CheckOpenAdvanceEdit();
        }

        private void RefreshView()
        {
            _validEditPropertyList.Clear();

            if (_tableUnit.CanEdit(EEditType.MonsterCave))
            {
                _validEditPropertyList.Add(EEditType.MonsterCave);
                _menuButtonArray[(int) EEditType.MonsterCave].SetEnable(true);
                RefreshMonsterCaveMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.MonsterCave].SetEnable(false);
            }

            if (_tableUnit.CanEdit(EEditType.Spawn))
            {
                _validEditPropertyList.Add(EEditType.Spawn);
                _menuButtonArray[(int) EEditType.Spawn].SetEnable(true);
                RefreshSpawmMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Spawn].SetEnable(false);
            }

            if (_tableUnit.CanEdit(EEditType.Active) && _curEnterType == EEnterType.Normal)
            {
                _validEditPropertyList.Add(EEditType.Active);
                _menuButtonArray[(int) EEditType.Active].SetEnable(true);
                RefreshAcitveMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Active].SetEnable(false);
            }

            //能编辑Npc/
            if (_tableUnit.CanEdit((EEditType.NpcType)))
            {
                //类型
                _validEditPropertyList.Add(EEditType.NpcType);
                _menuButtonArray[(int) EEditType.NpcType].SetEnable(true);
                RefreshNpcTypeMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.NpcType].SetEnable(false);
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

            if (_tableUnit.CanEdit(EEditType.Direction) && _curEnterType == EEnterType.Normal)
            {
                _validEditPropertyList.Add(EEditType.Direction);
                _menuButtonArray[(int) EEditType.Direction].SetEnable(true);
                RefreshForwardMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.Direction].SetEnable(false);
            }

            if (_tableUnit.CanEdit(EEditType.MoveDirection) && _curEnterType == EEnterType.Normal)
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

            //能编辑Npc/
            if (_tableUnit.CanEdit((EEditType.NpcType)))
            {
                //任务
                _validEditPropertyList.Add(EEditType.NpcTask);
                _menuButtonArray[(int) EEditType.NpcTask].SetEnable(true);
                RefreshNpcTypeMenu();
            }
            else
            {
                _menuButtonArray[(int) EEditType.NpcTask].SetEnable(false);
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
            var table = TableManager.Instance.GetEquipment(GetCurUnitExtra().ChildId);
            if (table != null)
            {
                _menuButtonArray[(int) EEditType.Child].SetFgImage(
                    JoyResManager.Instance.GetSprite(table.Icon));
            }
            else
            {
                GetCurUnitExtra().ChildId = (ushort) _tableUnit.ChildState[0];
                GetCurUnitExtra().UpdateDefaultValueFromChildId();
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
                    _payloadMenuList[i].SetSelected(_tableUnit.ChildState[i] == GetCurUnitExtra().ChildId);
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
            var teamId = Mathf.Clamp(EditData.UnitExtra.TeamId, 0, TeamManager.MaxTeamCount);
            _menuButtonArray[(int) EEditType.Camp].SetFgImage(TeamManager.GetSpawnSprite(teamId));
            bool isMulti = GM2DGame.Instance.GameMode.IsMulti;
            bool isSpawn = UnitDefine.IsSpawn(EditData.UnitDesc.Id);
            for (int i = 0; i < _campMenuList.Length; i++)
            {
                _campMenuList[i].SetSelected(i == teamId);
                if (i == 0)
                {
                    _campMenuList[i].SetEnable(!isSpawn); //玩家没有Team0
                }
                else if (i > 1)
                {
                    _campMenuList[i].SetEnable(isMulti || !isSpawn); //玩家多人只有Team1
                }
            }

            if (isMulti || !isSpawn)
            {
                var da = 360f / TeamManager.MaxTeamCount;
                _campMenuList[1].SetPosAngle(da * 1, MenuOptionsPosRadius);
                _campMenuList[1].SetBgImageAngle(da * 1);
            }
            else
            {
                _campMenuList[1].SetPosAngle(0, 0);
                _campMenuList[1].SetBgImageAngle(0);
            }
        }

        private void RefreshMonsterCaveMenu()
        {
            var table = TableManager.Instance.GetUnit(EditData.UnitExtra.MonsterId);
            if (table != null)
            {
                _menuButtonArray[(int) EEditType.MonsterCave].SetFgImage(JoyResManager.Instance.GetSprite(table.Icon));
            }
            else
            {
                EditData.UnitExtra.MonsterId = (ushort) UnitDefine.MonstersInCave[0];
                EditData.UnitExtra.UpdateFromMonsterId();
            }

            var monsterId = EditData.UnitExtra.MonsterId;
            for (int i = 0; i < _monsterCaveMenuList.Length; i++)
            {
                _monsterCaveMenuList[i]
                    .SetSelected(i < UnitDefine.MonstersInCave.Length && UnitDefine.MonstersInCave[i] == monsterId);
            }
        }

        private void RefreshSpawmMenu()
        {
            var projectType = _project.ProjectType;
            var playersDic = TeamManager.Instance.PlayerUnitExtraDic;
            for (int i = 0; i < _spawnMenuList.Length; i++)
            {
                _spawnMenuList[i].SetEnable(projectType != EProjectType.PT_Single);
                if (projectType != EProjectType.PT_Single)
                {
                    _spawnMenuList[i].SetSelected(i == _curSelectedPlayerIndex);
                    bool hasSet;
                    var playerUnitExtra = EditData.UnitExtra.InternalUnitExtras.Get<UnitExtraDynamic>(i);
                    if (playerUnitExtra == null)
                    {
                        hasSet = playersDic.ContainsKey(i);
                        if (hasSet)
                        {
                            _spawnMenuList[i].SetColor(TeamManager.GetTeamColor(playersDic[i].TeamId));
                        }
                        else
                        {
                            _spawnMenuList[i].SetColor(Color.white);
                        }

                        _spawnMenuList[i].SetBtnInteractable(!hasSet);
                        _spawnMenuList[i].SetDeleteBtnActive(false);
                    }
                    else
                    {
                        hasSet = true;
                        _spawnMenuList[i].SetColor(TeamManager.GetTeamColor(playerUnitExtra.TeamId));
                        _spawnMenuList[i].SetBtnInteractable(true);
                        _spawnMenuList[i].SetDeleteBtnActive(true);
                        _spawnMenuList[i].SetFgImage(JoyResManager.Instance.GetSprite("SMainBoy0Icon"));
                    }

                    _spawnMenuList[i]
                        .SetFgImage(JoyResManager.Instance.GetSprite(hasSet ? "SMainBoy0Icon" : "icon_add"));
                }
            }

            _cachedView.CharacterRawImage.texture =
                SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().AvatarRenderTexture;
        }

        private void RefreshTextDock()
        {
            _cachedView.TextInput.text = _originData.UnitExtra.Msg;
            _cachedView.TextInput.characterLimit = MessageStringCountMax;
        }

        private void RefreshNpcTypeMenu()
        {
            var val = Mathf.Clamp(EditData.UnitExtra.NpcType, 0, 1);
            for (int i = 0; i < _npcTypeMenuList.Length; i++)
            {
                _npcTypeMenuList[i].SetSelected(i == val);
            }

            _menuButtonArray[(int) EEditType.NpcType]
                .SetFgImage(_activeMenuList[val].View.FgImage.sprite);
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
            var curUnitExtra = GetCurUnitExtra();
            if (curUnitExtra.ChildId != _tableUnit.ChildState[inx])
            {
                curUnitExtra.ChildId = (ushort) _tableUnit.ChildState[inx];
                curUnitExtra.UpdateDefaultValueFromChildId();
                _upCtrlUnitPropertyEditAdvance.OnChildIdChanged();
            }

            RefreshPayloadMenu();
        }

        private void OnMoveDirectionMenuClick(int inx)
        {
            EditData.UnitExtra.MoveDirection = (EMoveDirection) inx;
            RefreshMoveDirectionMenu();
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

        private void OnMonsterCaveMenuClick(int inx)
        {
            if (inx < UnitDefine.MonstersInCave.Length)
            {
                ushort monsterId = (ushort) UnitDefine.MonstersInCave[inx];
                if (EditData.UnitExtra.MonsterId == monsterId)
                {
                    return;
                }

                EditData.UnitExtra.MonsterId = monsterId;
                if (UnitDefine.MonsterDragonId == monsterId)
                {
                    EditData.UnitExtra.ChildId = (ushort) TableManager.Instance.GetUnit(monsterId).ChildState[0];
                    EditData.UnitExtra.UpdateFromMonsterId();
                    EditData.UnitExtra.UpdateDefaultValueFromChildId();
                }
                else
                {
                    EditData.UnitExtra.UpdateFromMonsterId();
                }

                RefreshMonsterCaveMenu();
            }
        }

        private void OnSpawnMenuClick(int inx)
        {
            if (inx == -1 || !_spawnMenuList[inx].GetBtnInteractable())
            {
                return;
            }

            var playerUnitExtra = EditData.UnitExtra.InternalUnitExtras.Get<UnitExtraDynamic>(inx);
            if (playerUnitExtra == null)
            {
                playerUnitExtra = UnitExtraDynamic.GetDefaultPlayerValue(inx, _project.ProjectType);
                EditData.UnitExtra.InternalUnitExtras.Set(playerUnitExtra, inx);
                TeamManager.Instance.SetPlayerUnitExtra(inx, playerUnitExtra);
            }

            _curSelectedPlayerIndex = inx;
            _curUnitExtra = playerUnitExtra;
            _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.ActorSetting);
            RefreshSpawmMenu();
        }

        private void OnSpawnMenuDelete(int inx)
        {
            EditData.UnitExtra.InternalUnitExtras.Set<UnitExtraDynamic>(null, inx);
            TeamManager.Instance.SetPlayerUnitExtra(inx, null);
            if (inx == _curSelectedPlayerIndex)
            {
                _curSelectedPlayerIndex = -1;
                _upCtrlUnitPropertyEditAdvance.Close();
            }

            RefreshSpawmMenu();
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
            UpCtrlUnitPropertyEditNpcTaskAdvance.CheckClose();
            SocialGUIManager.Instance.CloseUI<UICtrlUnitPropertyEdit>();
        }

        private void OnNpcTypeMenuClick(int inx)
        {
            EditData.UnitExtra.NpcType = (byte) inx;
            RefreshNpcTypeMenu();
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

            if (!_openSequence.IsPlaying())
            {
                if (!CheckOpenAdvanceEdit())
                {
                    _upCtrlUnitPropertyEditAdvance.Close();
                }
            }

            //任务型npc
            UpCtrlUnitPropertyEditNpcTaskAdvance.Close();
            if (editType == EEditType.NpcTask && (ENpcType) EditData.UnitExtra.NpcType == ENpcType.Task)
            {
                _upCtrlUnitPropertyEditNpcDiaType.Close();
                UpCtrlUnitPropertyEditNpcTaskType.Open();
            }
            else
            {
                UpCtrlUnitPropertyEditNpcTaskType.Close();
            }
        }

        public void ReadPreinstall()
        {
            if (_curEnterType != EEnterType.Normal)
            {
                Reset();
            }
            else
            {
                RefreshView();
            }

            _upCtrlUnitPropertyEditAdvance.RefreshView();
            _upCtrlUnitPropertyEditNpcDiaType.RefreshView();
        }

        public void Reset()
        {
            Enter(EEnterType.Normal, EditData.UnitDesc.Id);
        }

        public void OnMonsterSettingBtn()
        {
            Enter(EEnterType.MonsterSettingFromMonsterCave, EditData.UnitExtra.MonsterId);
        }

        public void OnPlayerWeaponSettingBtn(int index)
        {
            var playerUnitExtra = EditData.UnitExtra.InternalUnitExtras.Get<UnitExtraDynamic>(_curSelectedPlayerIndex);
            var curWeaponExtra = playerUnitExtra.InternalUnitExtras.Get<UnitExtraDynamic>(index);
            if (curWeaponExtra == null)
            {
                curWeaponExtra = new UnitExtraDynamic();
                curWeaponExtra.ChildId = UnitDefine.WaterGun;
                curWeaponExtra.UpdateDefaultValueFromChildId();
                EditData.UnitExtra.InternalUnitExtras.Set(curWeaponExtra, _curSelectedPlayerIndex,
                    UnitExtraDynamic.FieldTag.InternalUnitExtras, index);
            }

            _curUnitExtra = curWeaponExtra;
            Enter(EEnterType.WeaponSettingFromSpawn, UnitDefine.EnergyPoolId);
        }

        private void Enter(EEnterType eEnterType, int unitId)
        {
            _curEnterType = eEnterType;
            _curId = unitId;
            _tableUnit = TableManager.Instance.GetUnit(unitId);
            if (eEnterType == EEnterType.Normal && UnitDefine.IsMonster(_tableUnit.Id))
            {
                EditData.UnitDesc.Rotation = (byte) (EditData.UnitExtra.MoveDirection - 1);
            }

            RefreshView();
            OnEditTypeMenuClick(_validEditPropertyList[0]);
        }

        public void OnTeamChanged(int teamId)
        {
            _curUnitExtra.TeamId = (byte) teamId;
            _spawnMenuList[_curSelectedPlayerIndex].SetColor(TeamManager.GetTeamColor(teamId));
        }
        
        public UnitExtraDynamic GetCurUnitExtra()
        {
            if (_curEditType == EEditType.Spawn || _curEnterType == EEnterType.WeaponSettingFromSpawn)
            {
                return _curUnitExtra;
            }

            return EditData.UnitExtra;
        }

        private bool CheckOpenAdvanceEdit()
        {
            if (_curEditType == EEditType.Camp)
            {
                _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.ActorSetting);
                return true;
            }

            if (_curEditType == EEditType.Child)
            {
                _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.WeaponSetting);
                return true;
            }

            if (_curEditType == EEditType.MonsterCave)
            {
                _upCtrlUnitPropertyEditAdvance.OpenMenu(UPCtrlUnitPropertyEditAdvance.EMenu.MonsterCave);
                return true;
            }

            if (_curEditType == EEditType.Spawn)
            {
                OnSpawnMenuClick(_curSelectedPlayerIndex);
                return true;
            }

            return false;
        }

        public enum EEnterType
        {
            Normal,
            MonsterSettingFromMonsterCave,
            WeaponSettingFromSpawn
        }
    }
}