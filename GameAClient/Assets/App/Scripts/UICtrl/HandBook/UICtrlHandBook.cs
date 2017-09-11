using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using NewResourceSolution;
using GameA.Game;

namespace GameA
{
   
    
    [UIAutoSetup]
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
        }
        #region Fields

        private float _TweenTime = 0.5f;
        private string _unitIconName = null;
        private bool _isFirst = true;
        private float _startTime = 0.0f;
        private Sprite _unitIcon;
        private Table_Unit _uint;
        private Tweener _contenTween = null;
        private UMCtrlHandBookItem _curSeleCtrlHandBookItem = null;
        private  List<int> _RoleList =  new List<int>();
        private  List<int> _EarthList =  new List<int>();
        private  List<int> _MechanismList =  new List<int>();
        private  List<int> _ColletionList =  new List<int>();
        private  List<int> _DecorationList =  new List<int>();
        private  List<int> _ControllerList =  new List<int>();
        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
       
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

      
        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlHandBook>();
        }

        public void InitData()
        {
               
                foreach (var item in TableManager.Instance.Table_UnitDic)
                {
                    switch (item.Value.UIType)
                    {
                        case (int) ExplantionIndex.Role:
                            _RoleList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Earth:
                            _EarthList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Mechanism:
                            _MechanismList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Collection:
                            _ColletionList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Decoration:
                            _DecorationList.Add(item.Key);
                            break;
                        case (int) ExplantionIndex.Controller:
                            _ControllerList.Add(item.Key);
                            break;
                    }
                }
        }

        public void InitItemGroup()
        {
            for (int i = 0; i < _RoleList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.RoleRectGroup, Vector3.zero);
                explationItem.IintItem(_RoleList[i], false);
                if (_curSeleCtrlHandBookItem == null)
                {
                    _curSeleCtrlHandBookItem = explationItem;
                    _curSeleCtrlHandBookItem.OnBtn();
                }
            }
            for (int i = 0; i < _EarthList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.EarthRectGroup, Vector3.zero);
                explationItem.IintItem(_EarthList[i], true);
            }
            for (int i = 0; i < _MechanismList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.TrickRectGroup, Vector3.zero);
                explationItem.IintItem(_MechanismList[i], true);
            }
            for (int i = 0; i < _ColletionList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.CollitionRectGroup, Vector3.zero);
                explationItem.IintItem(_ColletionList[i], true);
            }
            for (int i = 0; i < _DecorationList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.DecorationRectGroup, Vector3.zero);
                explationItem.IintItem(_DecorationList[i], true);
            }
            for (int i = 0; i < _ControllerList.Count; i++)
            {
                var explationItem = new UMCtrlHandBookItem();
                explationItem.Init(_cachedView.CtrlRectGroup, Vector3.zero);
                explationItem.IintItem(_ControllerList[i], true);
            }
        }

        public void UpdateDesc(int unitID ,UMCtrlHandBookItem selecCtrlHandBookItem)
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
            if ( ResourcesManager.Instance.TryGetSprite(_unitIconName, out _unitIcon))
            {

                _cachedView.Icon.sprite = _unitIcon;
            }
            _cachedView.Desc.text = _uint.Summary;
            
        }
        public void OnRoleBtn()
        {
            JudgeMove(_cachedView.RoleRect.anchoredPosition.y);
        }

        public void OnEarthBtn()
        {
            JudgeMove(_cachedView.EarthRect.anchoredPosition.y);
        }
        public void OnTrickBtn()
        {
            JudgeMove(_cachedView.TrickRect.anchoredPosition.y);
        }

        public void OnCollitionBtn()
        {   
            JudgeMove(_cachedView.CollitionRect.anchoredPosition.y);
        }
        
        public void OnDecorationBtn()
        {
            JudgeMove(_cachedView.DecorationRect.anchoredPosition.y);
        }

        public void OnCtrlBtn()
        {   
            JudgeMove(_cachedView.CtrlRect.anchoredPosition.y);
        }

        public void JudgeMove( float posY )
        {
            if (_contenTween != null)
            {
                _contenTween.Pause();
            }
            _contenTween =  _cachedView.ContenTransform.DOAnchorPosY(-(posY),_TweenTime,true);
        }

        #endregion
    }
}