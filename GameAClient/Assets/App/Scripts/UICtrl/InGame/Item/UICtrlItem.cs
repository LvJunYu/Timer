﻿/********************************************************************
** Filename : UICtrlItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 3:29:49
** Summary : UICtrlItem
***********************************************************************/

using System.Collections.Generic;

using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlItem : UICtrlInGameBase<UIViewItem>
    {
        private EUIType _selectedUnitType;
        private EUIType _lastSelectUnitType;

//        private 

        private List<UMCtrlItem> _umItems = new List<UMCtrlItem>();

//        public const string NormalButtonSpriteName = "ChoiceButton_2";
//        public const string SelectButtonSpriteName = "ChoiceButton_1";

        private Dictionary<EUIType, Button> _cachedButtonDic;


        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
//            _cachedView.Actor.onClick.AddListener(OnActor);
//            _cachedView.Earth.onClick.AddListener(OnEarth);
//            _cachedView.Mechanism.onClick.AddListener(OnMechanism);
//            _cachedView.Monster.onClick.AddListener(OnMonster);
//            _cachedView.Collection.onClick.AddListener(OnCollection);
//            _cachedView.Decoration.onClick.AddListener(OnDecoration);
            _cachedView.CategoryButtns [0].onClick.AddListener (OnActor);
            _cachedView.CategoryButtns [1].onClick.AddListener (OnEarth);
            _cachedView.CategoryButtns [2].onClick.AddListener (OnMechanism);
            _cachedView.CategoryButtns [3].onClick.AddListener (OnCollection);
            _cachedView.CategoryButtns [4].onClick.AddListener (OnDecoration);
            _cachedView.CategoryButtns [5].onClick.AddListener (OnControl);
        }

		protected override void OnOpen(object parameter)
	    {
		    base.OnOpen(parameter);
//			UpdateTabbarState();
//			if (EditMode.Instance.CurEditorLayer == EEditorLayer.Effect)
//			{
//                RefreshView(EUIType.Effect);
//			}
//			else
//			{
//                if (_selectedUnitType == EUIType.Effect)
//				{
//                    _selectedUnitType = EUIType.None;
//				}
//				RefreshView(_selectedUnitType);
//			}
            RefreshView(_selectedUnitType);
        }

	    protected override void InitEventListener()
	    {
		    base.InitEventListener();
			RegisterEvent(EMessengerType.OnEditorLayerChanged, OnEditorLayerChanged);
	    }

	    #region ui event

	    private void OnEditorLayerChanged()
	    {
		    if (IsOpen)
		    {
//				UpdateTabbarState();
                EUIType showType = EditMode.Instance.CurEditorLayer == EEditorLayer.Effect
                    ? EUIType.Effect
				    : _lastSelectUnitType;
				RefreshView(showType);
			}
	    }

		private void OnCollection()
        {
            RefreshView(EUIType.Collection);
        }

        private void OnMonster()
        {
            RefreshView(EUIType.Controller);
        }

        private void OnMechanism()
        {
            RefreshView(EUIType.Mechanism);
        }

        private void OnEarth()
        {
            RefreshView(EUIType.Earth);
        }

        private void OnActor()
        {
            RefreshView(EUIType.Actor);
        }

        private void OnDecoration()
        {
            RefreshView(EUIType.Decoration);
        }
        private void OnControl ()
        {
            RefreshView(EUIType.Controller);
        }
		#endregion

		#region event

		#endregion

		#region private

        private void RefreshView(EUIType eUnitType)
        {
            if (eUnitType == EUIType.None)
		    {
                eUnitType = EUIType.Controller;
            }
//            if (_selectedUnitType == eUnitType)
//            {
//                return;
//            }
			_lastSelectUnitType = _selectedUnitType;
			_selectedUnitType = eUnitType;
            if (!_isViewCreated)
            {
                return;
            }
            for (int i = _umItems.Count - 1; i >= 0; i--)
            {
                PoolFactory<UMCtrlItem>.Free(_umItems[i]);
//                _umItems.RemoveAt(i);
            }
            _umItems.Clear ();
		    _cachedView.ScrollRect.content.anchoredPosition = Vector2.zero;
            List<Table_Unit> items = UnitManager.Instance.GetSameTypeItems(_selectedUnitType);
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var umItem = PoolFactory<UMCtrlItem>.Get ();
                    umItem.Init(_cachedView.ScrollRect.content);
                    umItem.Set(items[i], EditMode.Instance.SelectedItemId == items[i].Id);
                    _umItems.Add(umItem);
                }
                int itemCount = items.Count;

                int totalWidth = (int)(itemCount * GM2DUIConstDefine.UICtrlItemWidthPreItem + (itemCount - 1)*_cachedView.LayoutGroup.spacing + _cachedView.LayoutGroup.padding.left * 2);
				_cachedView.ScrollRect.content.SetWidth(totalWidth);

	   //         {
				//	///显示测试特效
				//	if (_selecetedUnitType == EUnitType.Monster)
				//	{
				//		List<Table_Unit> others = GameDataManager.Instance.GetSameTypeItems(EUnitType.Effect);
				//		for (int i = 0; i < others.Count; i++)
				//		{
				//			var umItem = new UMCtrlItem();
				//			umItem.Init(_cachedView.ScrollRect.content);
				//			umItem.Set(others[i]);
				//			_umItems.Add(umItem);
				//		}
				//		itemCount += others.Count;
				//		totalWidth = (int)(itemCount * GM2DUIConstDefine.UICtrlItemWidthPreItem + (itemCount - 1) * _cachedView.LayoutGroup.spacing + _cachedView.LayoutGroup.padding.left * 2);
				//		_cachedView.ScrollRect.content.SetWidth(totalWidth);
				//	}
				//}
			}

		    UpdateButtonShow();
        }

        private void UpdateButtonShow()
        {
            for (int i = 0; i < _cachedView.SelectedCategorys.Length; i++) {
                if (_selectedUnitType == (EUIType)(i + 1)) {
                    _cachedView.SelectedCategorys [i].SetActive (true);
                } else {
                    _cachedView.SelectedCategorys [i].SetActive (false);
                }
            }
        }

		#endregion
	}
}