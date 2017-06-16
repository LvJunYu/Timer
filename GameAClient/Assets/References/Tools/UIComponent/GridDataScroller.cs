using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SoyEngine
{
    [RequireComponent(typeof(ScrollRect))]
	public class GridDataScroller : MonoBehaviour {

        [SerializeField]
        private GridLayoutGroup _grid;
        private RectTransform _gridTransform;
        private LayoutElement _layoutElement;

        [SerializeField]
        private RectTransform _scrollContent;

        // private UI elements //
        private ScrollRect _scroller;
//        private RectTransform _scrollerTransform;

        public Vector2 ContentPosition
        {
            get { return _scrollContent.anchoredPosition; }
            set { _scrollContent.anchoredPosition = value; }
        }

        // define //
        public enum Movement
        {
            Horizontal,
            Vertical,
        }

        // public fields //
        [SerializeField]
        private Movement _moveType = Movement.Horizontal;
        
        public delegate void OnChange(IDataItemRenderer item, int index);

        private Dictionary<int, IDataItemRenderer> _itemDict = new Dictionary<int, IDataItemRenderer>();
        private Stack<IDataItemRenderer> _itemPool = new Stack<IDataItemRenderer>();
        private OnChange _onChange;
        private Func<RectTransform, IDataItemRenderer> _itemFactory;
        private int _itemCount = 0;
        private int _transCount = 0;
        private int _col = 0;
        private int _row = 0;
        private bool _isDirty = false;
        private Vector2 _cellSize = Vector3.zero;
        private Vector2 _spacing = Vector3.zero;
        private RectOffset _padding = new RectOffset();
        private int _startIndex = 0;
        private List<int> _tempIndexList = new List<int>();
        private bool _hasInited = false;

        public Vector2 ItemSize
        {
            get
            {
                return _spacing + _cellSize;
            }
        }

        public ScrollRect ScrollRect
        {
            get { return _scroller; }
        }

        public GridLayoutGroup GridLayoutGroup
        {
            get { return _grid; }
        }
        #region Public Methods

        public void SetCallback(OnChange onChange, Func<RectTransform, IDataItemRenderer> factory)
        {
            _onChange = onChange;
            _itemFactory = factory;
        }

        public void RefreshCurrent()
        {
            foreach (var itemEntity  in _itemDict)
            {
                if (_onChange != null)
                {
                    _onChange(itemEntity.Value, itemEntity.Key);
                }
            }
        }

        public void SetItemCount(int count)
        {
            if(!_hasInited)
            {
                Init();
            }
            _isDirty = true;
            _itemCount = count;
            ResetRectTransformSize();
            ProcessNewStartIndex(_startIndex);
            RefreshCurrent();
        }

        public void SetEmpty()
        {
            SetItemCount(0);
        }
        #endregion

        #region Init
        void Start()
        {
            Init();
        }

        private void Init()
        {
            if(_hasInited)
            {
                return;
            }
            _hasInited = true;
            InitGrid();
            InitScroller();
            CalculateParam();
        }

        private void InitScroller()
        {
            // Init Scroller //
            _scroller = GetComponent<ScrollRect>();
//            _scrollerTransform = _scroller.GetComponent<RectTransform>();

            if (_moveType == Movement.Horizontal)
            {
                _scroller.vertical = false;
                _scroller.horizontal = true;
//                _gridTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _scrollerRect.height);
            }
            else
            {
                _scroller.vertical = true;
                _scroller.horizontal = false;
//                _gridTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _scrollerRect.width);
            }
            _scroller.onValueChanged.AddListener(OnValueChanged);
        }

        private void InitGrid()
        {
            _cellSize = _grid.cellSize;
            _spacing = _grid.spacing;
            _padding = _grid.padding;
            _grid.enabled = false;
            _gridTransform = _grid.GetComponent<RectTransform>();
            _layoutElement = _grid.GetComponent<LayoutElement>();
            if(_layoutElement == null)
            {
                _layoutElement = _grid.gameObject.AddComponent<LayoutElement>();
            }
        }

        private void CalculateParam()
        {
            Canvas.ForceUpdateCanvases ();
            float vWidth = _scroller.viewport.rect.width;
            float vHeight = _scroller.viewport.rect.height;
            LayoutElement le = _scroller.viewport.GetComponent<LayoutElement>();
            if(le != null && le.enabled)
            {
                if(le.preferredHeight >= 0)
                {
                    vHeight = le.preferredHeight;
                }
                if(le.preferredWidth >= 0)
                {
                    vWidth = le.preferredWidth;
                }
            }
            _col = (int)((vWidth + _spacing.x - _padding.left - _padding.right) / ItemSize.x);
            _row = (int)((vHeight + _spacing.y - _padding.top - _padding.bottom) / ItemSize.y);
            if (_moveType == Movement.Horizontal)
            {
                _col += 2;
            }
            else
            {
                _row += 2;
            }
            _transCount = _col * _row;
        }
   
        private void InitItem(RectTransform rectTrans)
        {
            rectTrans.anchorMax = new Vector2(0, 1);
            rectTrans.anchorMin = new Vector2(0, 1);
            rectTrans.pivot = new Vector2(0, 1);
            rectTrans.sizeDelta = _cellSize;
        }

        private void ResetRectTransformSize()
        {
            if (_moveType == Movement.Horizontal)
            {
                var width = 0f;
                if(_row != 0)
                {
                    width =  ((_itemCount + _row - 1) / _row) * ItemSize.x;
                }
                if(!Util.IsFloatEqual(_gridTransform.GetWidth(), width, 10))
                {
                    _gridTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    _layoutElement.minWidth = _layoutElement.preferredWidth = width;
                }
            }
            else
            {
                var height = 0f;
                if(_col != 0)
                {
                    height = ((_itemCount + _col - 1) / _col) * ItemSize.y;
                }
                if(!Util.IsFloatEqual(_gridTransform.GetHeight(), height, 10))
                {
                    _gridTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    _layoutElement.minHeight = _layoutElement.preferredHeight = height;
                }
            }
        }

        private void Clear()
        {
            _itemPool.Clear();
            _itemDict.Clear();
            _gridTransform.DestroyChildren();
        }

        #endregion

        #region OnValueChanged

        public void OnViewportSizeChanged()
        {
            if(!_hasInited)
            {
                Init();
            }
            else
            {
                CalculateParam();
            }
            _isDirty = true;
            OnValueChanged(_scroller.normalizedPosition);
        }

        public void OnValueChanged(Vector2 normalizedPosition)
        {
            int startIndex = 0;
            if (_moveType == Movement.Horizontal)
            {
                float scrollLength = - _scrollContent.anchoredPosition.x - _padding.left;
                int scrollCol = (int)(scrollLength / ItemSize.x);
                startIndex = scrollCol * _row;
            }
            else
            {
                float scrollLength = _scrollContent.anchoredPosition.y - _padding.top;
                int scrollRow = (int)(scrollLength / ItemSize.y);
                startIndex = scrollRow * _col;
            }
            if(_startIndex == startIndex && !_isDirty)
            {
                return;
            }

            ProcessNewStartIndex(Math.Max(0, startIndex));
        }

        private void ProcessNewStartIndex(int startIndex)
        {
            _tempIndexList.Clear();
            foreach(var entity in _itemDict)
            {
                if(
                    entity.Key < startIndex
                    || entity.Key >= startIndex + _transCount
                    || entity.Key >= _itemCount
                )
                {
                    _tempIndexList.Add(entity.Key);
                }
            }
            for(int i=0; i<_tempIndexList.Count; i++)
            {
                FreeItem(_itemDict[_tempIndexList[i]]);
                _itemDict.Remove(_tempIndexList[i]);
            }
            _tempIndexList.Clear();
            for(int i=startIndex, max=Math.Min(startIndex + _transCount, _itemCount); i<max; i++)
            {
                if(!_itemDict.ContainsKey(i))
                {
                    _tempIndexList.Add(i);
                }
            }

            for(int i=0; i<_tempIndexList.Count; i++)
            {
                CreateItemForIndex(_tempIndexList[i]);
            }
            _startIndex = startIndex;
            _isDirty = false;
        }

        private void CreateItemForIndex(int inx)
        {
            IDataItemRenderer item = GetItem();
            item.Transform.anchoredPosition = IndexToPosition(inx);
            _itemDict.Add(inx, item);
            _onChange(item, inx);
        }

        private Vector2 IndexToPosition(int index)
        {
            if (_moveType == Movement.Horizontal)
	        {
                return new Vector2(_padding.left + ItemSize.x * (index / _row), -_padding.top - ItemSize.y * (index % _row));
	        }else
	        {
                return new Vector2(_padding.left + ItemSize.x * (index % _col), -_padding.top - ItemSize.y * (index / _col));
	        }
        }

        private void FreeItem(IDataItemRenderer item)
        {
            item.Transform.anchoredPosition = new Vector2(-10000,0);
            item.Set(null);
            _itemPool.Push(item);
        }

        private IDataItemRenderer GetItem()
        {
            IDataItemRenderer item = null;
            if(_itemPool.Count > 0)
            {
                item = _itemPool.Pop();
            }
            else
            {
                item = _itemFactory.Invoke(_gridTransform);
                InitItem(item.Transform);
            }
            return item;
        }

        #endregion
    }

    public interface IDataItemRenderer
    {
        RectTransform Transform {get;}
        int Index { get; set; }
        object Data { get; }
        void Set(object data);
        void Unload();
    }
}

