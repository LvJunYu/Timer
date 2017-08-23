/********************************************************************
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
	    private readonly Table_Unit[] _selectUnitIdAry = new Table_Unit[(int)EEditorLayer.Max];

        private readonly List<UMCtrlItem> _umItems = new List<UMCtrlItem>();

        private Dictionary<EUIType, Button> _cachedButtonDic;


        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
	        for (int i = 0; i < _cachedView.CategoryButtns.Length; i++)
	        {
		        var inx = i;
		        _cachedView.CategoryButtns[i].onClick.AddListener(()=>OnSelectTab(inx));
	        }
        }

		protected override void OnOpen(object parameter)
	    {
		    base.OnOpen(parameter);
            RefreshView(_selectedUnitType);
        }

	    protected override void OnClose()
	    {
		    base.OnClose();
		    for (int i = 0; i < _umItems.Count; i++)
		    {
			    _umItems[i].Hide();
		    }
		    for (int i = 0; i < _selectUnitIdAry.Length; i++)
		    {
			    _selectUnitIdAry[i] = null;
		    }
	    }

	    public void SelectItem(Table_Unit tableUnit)
	    {
		    _selectUnitIdAry[(int) EditMode.Instance.BoardData.EditorLayer] = tableUnit;
		    Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged,
			    (ushort) PairUnitManager.Instance.GetCurrentId(tableUnit.Id));
		    EditMode.Instance.ChangeSelectUnit(PairUnitManager.Instance.GetCurrentId(tableUnit.Id));
	    }

	    #region ui event

	    private void OnSelectTab(int inx)
	    {
		    RefreshView((EUIType) (inx + 1));
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
	        var editorLayerBefore = EditMode.Instance.BoardData.EditorLayer;
	        EditMode.Instance.ChangeSelectUnitUIType(eUnitType);
	        var editorLayerAfter = EditMode.Instance.BoardData.EditorLayer;
			_selectedUnitType = eUnitType;
            if (!_isViewCreated)
            {
                return;
            }
            for (int i = _umItems.Count - 1; i >= 0; i--)
            {
	            _umItems[i].Hide();
            }
		    _cachedView.ScrollRect.content.anchoredPosition = Vector2.zero;
            List<Table_Unit> items = UnitManager.Instance.GetSameTypeItems(_selectedUnitType);
            if (items != null)
            {
	            if (editorLayerBefore != editorLayerAfter)
	            {
		            var item = _selectUnitIdAry[(int) editorLayerAfter];
		            if (item == null && items.Count > 0)
		            {
			            item = items[0];
		            }
		            if (item != null)
		            {
			            SelectItem(item);
		            }
	            }
                for (int i = 0; i < items.Count; i++)
                {
	                UMCtrlItem umItem;
	                if (i < _umItems.Count)
	                {
		                umItem = _umItems[i];
	                }
	                else
	                {
		                umItem = CreateNewUmCtrlItem();
		                _umItems.Add(umItem);
	                }
                    umItem.Set(items[i], EditMode.Instance.BoardData.CurrentSelectedUnitId == items[i].Id);
	                umItem.Show();
                }
                int itemCount = items.Count;

	            int totalWidth = (int) (itemCount * GM2DUIConstDefine.UICtrlItemWidthPreItem +
	                                    (itemCount - 1) * _cachedView.LayoutGroup.spacing +
	                                    _cachedView.LayoutGroup.padding.left * 2);
				_cachedView.ScrollRect.content.SetWidth(totalWidth);
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

	    private UMCtrlItem CreateNewUmCtrlItem()
	    {
		    var umItem = new UMCtrlItem();
		    umItem.Init(_cachedView.ScrollRect.content);
		    return umItem;
	    }
	    

		#endregion
	}
}