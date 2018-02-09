using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlHandBook : UICtrlAnimationBase<UIViewHandBook>
    {
        public enum ExplantionIndex
        {
            Role = 1,
            Earth = 2,
            Mechanism = 3,
            Collection = 4,
            Decoration = 5,
            Controller = 6,
            Effect = 7
        }

        #region Fields

        private float _TweenTime = 0.5f;
        private float _height = 450;
        private string _unitIconName;
        private bool _isFirst = true;
        private float _startTime;
        private Sprite _unitIcon;
        private Table_Unit _uint;
        private Tweener _contenTween;
        private UMCtrlHandBookItem _curSeleCtrlHandBookItem;
        private readonly List<int> _roleList = new List<int>();
        private readonly List<int> _earthList = new List<int>();
        private readonly List<int> _mechanismList = new List<int>();
        private readonly List<int> _colletionList = new List<int>();
        private readonly List<int> _decorationList = new List<int>();
        private readonly List<int> _controllerList = new List<int>();
        private readonly List<int> _effectList = new List<int>();

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitData();
            InitItemGroup();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.RoleBtn.onClick.AddListener(OnRoleBtn);
            _cachedView.EarthBtn.onClick.AddListener(OnEarthBtn);
            _cachedView.TrickBtn.onClick.AddListener(OnTrickBtn);
            _cachedView.CollitionBtn.onClick.AddListener(OnCollitionBtn);
            _cachedView.DecorationBtn.onClick.AddListener(OnDecorationBtn);
            _cachedView.CtrlBtn.onClick.AddListener(OnCtrlBtn);
            _cachedView.EffectBtn.onClick.AddListener(OnEffectBtn);
            _cachedView.ScrollRect.onValueChanged.AddListener(OnScrollVaLueChange);
            _isFirst = true;
            _startTime = 0.0f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_isFirst)
            {
                _startTime += Time.deltaTime;
                _cachedView.ContenTransform.anchoredPosition = Vector3.zero;
                if (_startTime > 1.5f)
                {
                    _isFirst = false;
                }
            }
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            OnRoleBtn();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlHandBook>();
        }

        public void InitData()
        {
            foreach (var item in TableManager.Instance.Table_UnitDic)
            {
                if (item.Value.Use == 1)
                {
                    switch (item.Value.UIType)
                    {
                        case (int) ExplantionIndex.Role:
                            _roleList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Earth:
                            _earthList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Mechanism:
                            _mechanismList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Collection:
                            _colletionList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Decoration:
                            _decorationList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Controller:
                            _controllerList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Effect:
                            _effectList.Add(item.Key);
                            break;
                    }
                }
            }
        }

        public void InitItemGroup()
        {
            var resScenary = ResScenary;
            for (int i = 0; i < _roleList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.RoleRectGroup, resScenary);
                explationItem.IintItem(_roleList[i], true);
                if (_curSeleCtrlHandBookItem == null)
                {
                    _curSeleCtrlHandBookItem = explationItem;
                    _curSeleCtrlHandBookItem.OnBtn();
                }
            }
            for (int i = 0; i < _earthList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.EarthRectGroup, resScenary);
                explationItem.IintItem(_earthList[i], true);
            }
            for (int i = 0; i < _mechanismList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.TrickRectGroup, resScenary);
                explationItem.IintItem(_mechanismList[i], true);
            }
            for (int i = 0; i < _colletionList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.CollitionRectGroup, resScenary);
                explationItem.IintItem(_colletionList[i], true);
            }
            for (int i = 0; i < _decorationList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.DecorationRectGroup, resScenary);
                explationItem.IintItem(_decorationList[i], true);
            }
            for (int i = 0; i < _controllerList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.CtrlRectGroup, resScenary);
                explationItem.IintItem(_controllerList[i], true);
            }
            for (int i = 0; i < _effectList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.EffecRectGroup, resScenary);
                explationItem.IintItem(_effectList[i], true);
            }
        }

        private void OnScrollVaLueChange(Vector2 pos)
        {
            float y = (1 - pos.y) * _cachedView.ScrollRect.content.GetHeight();
            if (y >= -_cachedView.RoleRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.RoleBtnSlect.SetActiveEx(true);
            }
            if (y >= -_cachedView.EarthRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.EarthBtnSlect.SetActiveEx(true);
            }
            if (y >= -_cachedView.TrickRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.TrickBtnSlect.SetActiveEx(true);
            }
            if (y >= -_cachedView.CollitionRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.CollitionBtnSlect.SetActiveEx(true);
            }
            if (y >= -_cachedView.DecorationRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.DecorationBtnSlect.SetActiveEx(true);
            }
            if (y >= -_cachedView.CtrlRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.CtrlBtnSlect.SetActiveEx(true);
            }
            if (y >= -_cachedView.EffecRect.anchoredPosition.y)
            {
                DisableSelectBtn();
                _cachedView.EffectBtnSlect.SetActiveEx(true);
            }
        }

        public void UpdateDesc(int unitID, UMCtrlHandBookItem selecCtrlHandBookItem)
        {
            if (_curSeleCtrlHandBookItem == null)
            {
                _curSeleCtrlHandBookItem = selecCtrlHandBookItem;
            }
            else
            {
                _curSeleCtrlHandBookItem.OnSelectDisable();
                _curSeleCtrlHandBookItem = selecCtrlHandBookItem;
                _curSeleCtrlHandBookItem.OnSelect();
            }
            _uint = TableManager.Instance.GetUnit(unitID);
            _cachedView.Name.text = _uint.Name;
            _unitIconName = _uint.Icon;
            if (JoyResManager.Instance.TryGetSprite(_unitIconName, out _unitIcon))
            {
                _cachedView.Icon.sprite = _unitIcon;
            }
            if (_uint.Summary != null)
            {
                _cachedView.Desc.text = String.Format("<size=22>简介</size>\n{0}", _uint.Summary);
            }
            else
            {
                _cachedView.Desc.text = "";
            }
            if (_uint.EffectSummary != null)
            {
                _cachedView.EffectText.text = String.Format("<size=22>效果</size>\n{0}", _uint.EffectSummary);
            }
            else
            {
                _cachedView.EffectText.text = "";
            }
        }

        public void OnRoleBtn()
        {
            DisableSelectBtn();
            _cachedView.RoleBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.RoleRect.anchoredPosition.y);
        }

        public void OnEarthBtn()
        {
            DisableSelectBtn();
            _cachedView.EarthBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.EarthRect.anchoredPosition.y);
        }

        public void OnTrickBtn()
        {
            DisableSelectBtn();
            _cachedView.TrickBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.TrickRect.anchoredPosition.y);
        }

        public void OnCollitionBtn()
        {
            DisableSelectBtn();
            _cachedView.CollitionBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.CollitionRect.anchoredPosition.y);
        }

        public void OnDecorationBtn()
        {
            DisableSelectBtn();
            _cachedView.DecorationBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.DecorationRect.anchoredPosition.y);
        }

        public void OnCtrlBtn()
        {
            DisableSelectBtn();
            _cachedView.CtrlBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.CtrlRect.anchoredPosition.y);
        }

        public void OnEffectBtn()
        {
            DisableSelectBtn();
            _cachedView.EffectBtnSlect.SetActiveEx(true);
            JudgeMove(_cachedView.EffecRect.anchoredPosition.y);
        }

        public void JudgeMove(float posY)
        {
            if (_contenTween != null)
            {
                _contenTween.Pause();
            }
            if (-posY > (_cachedView.ContenTransform.GetHeight() - _height))
            {
                posY = -(_cachedView.ContenTransform.GetHeight() - _height);
            }
            _cachedView.ScrollRect.onValueChanged.RemoveListener(OnScrollVaLueChange);
            _contenTween = _cachedView.ContenTransform.DOAnchorPosY(-(posY), _TweenTime, true);
            _contenTween.OnComplete(
                () => { _cachedView.ScrollRect.onValueChanged.AddListener(OnScrollVaLueChange); });
        }

        public void DisableSelectBtn()
        {
            _cachedView.RoleBtnSlect.SetActiveEx(false);
            _cachedView.EarthBtnSlect.SetActiveEx(false);
            _cachedView.TrickBtnSlect.SetActiveEx(false);
            _cachedView.CollitionBtnSlect.SetActiveEx(false);
            _cachedView.DecorationBtnSlect.SetActiveEx(false);
            _cachedView.CtrlBtnSlect.SetActiveEx(false);
            _cachedView.EffectBtnSlect.SetActiveEx(false);
        }

        #endregion
    }
}