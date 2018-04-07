using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoyEngine
{
    [RequireComponent(typeof(ScrollRect))]
    public class GridDataScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GridLayoutGroup _grid;
        private RectTransform _gridTransform;
        private LayoutElement _layoutElement;

        [SerializeField] private RectTransform _scrollContent;

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
            Vertical
        }

        // public fields //
        [SerializeField] private Movement _moveType = Movement.Horizontal;

        public delegate void OnChange(IDataItemRenderer item, int index);

        private readonly Dictionary<int, IDataItemRenderer> _itemDict = new Dictionary<int, IDataItemRenderer>();
        private readonly Stack<IDataItemRenderer> _itemPool = new Stack<IDataItemRenderer>();
        private OnChange _onChange;
        private Func<RectTransform, IDataItemRenderer> _itemFactory;
        private int _itemCount;
        private int _transCount;
        private int _col;
        private int _row;
        private bool _isDirty;
        private Vector2 _cellSize = Vector3.zero;
        private Vector2 _spacing = Vector3.zero;
        private RectOffset _padding = new RectOffset();
        private int _startIndex;
        private readonly List<int> _tempIndexList = new List<int>();
        private bool _hasInited;
        private bool _preparePool;
        private bool _mouseIn;
        [HideInInspector] public bool ScorllWheelUpOff;
        [HideInInspector] public bool ScorllWheelDownOff;

        public Vector2 ItemSize
        {
            get { return _spacing + _cellSize; }
        }

        public ScrollRect ScrollRect
        {
            get { return _scroller; }
        }

        public GridLayoutGroup GridLayoutGroup
        {
            get { return _grid; }
        }

        public bool MouseIn
        {
            get { return _mouseIn; }
        }

        #region Public Methods

        public void Set(OnChange onChange, Func<RectTransform, IDataItemRenderer> factory, bool preparePool = true)
        {
            _onChange = onChange;
            _itemFactory = factory;
            _preparePool = preparePool;
            PreparePool();
            Init();
        }

        public void RefreshCurrent()
        {
            foreach (var itemEntity in _itemDict)
            {
                if (_onChange != null)
                {
                    _onChange(itemEntity.Value, itemEntity.Key);
                }
            }
        }

        public void SetItemCount(int count)
        {
            if (!_hasInited)
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
            if (_hasInited)
            {
                return;
            }

            _hasInited = true;
            InitGrid();
            InitScroller();
            CalculateParam();
            if (_preparePool)
            {
                PreparePool();
            }
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
            if (_layoutElement == null)
            {
                _layoutElement = _grid.gameObject.AddComponent<LayoutElement>();
            }
        }

        private void CalculateParam()
        {
            Canvas.ForceUpdateCanvases();
            float vWidth = _scroller.viewport.rect.width;
            float vHeight = _scroller.viewport.rect.height;
            LayoutElement le = _scroller.viewport.GetComponent<LayoutElement>();
            if (le != null && le.enabled)
            {
                if (le.preferredHeight >= 0)
                {
                    vHeight = le.preferredHeight;
                }

                if (le.preferredWidth >= 0)
                {
                    vWidth = le.preferredWidth;
                }
            }

            _col = (int) ((vWidth + _spacing.x - _padding.left - _padding.right) / ItemSize.x);
            _row = (int) ((vHeight + _spacing.y - _padding.top - _padding.bottom) / ItemSize.y);
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

        private void PreparePool()
        {
            if (!_hasInited)
            {
                return;
            }

            while (_itemPool.Count < _transCount)
            {
                FreeItem(CreateItem());
            }
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
                if (_row != 0)
                {
                    var widthRow = (_itemCount + _row - 1) / _row;
                    width = widthRow * ItemSize.x;
                }

                if (!IsFloatEqual(_gridTransform.rect.width, width, 10))
                {
                    _gridTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    _layoutElement.minWidth = _layoutElement.preferredWidth = width;
                }
            }
            else
            {
                var height = 0f;
                if (_col != 0)
                {
                    var heightCol = (_itemCount + _col - 1) / _col;
                    height = heightCol * ItemSize.y;
                }

                if (!IsFloatEqual(_gridTransform.rect.height, height, 10))
                {
                    _gridTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    _layoutElement.minHeight = _layoutElement.preferredHeight = height;
                }
            }
        }

        public void Clear()
        {
            _itemPool.Clear();
            _itemDict.Clear();
            _gridTransform.DestroyChildren();
        }

        #endregion

        #region OnValueChanged

        public void OnViewportSizeChanged()
        {
            if (!_hasInited)
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
            int startIndex;
            if (_moveType == Movement.Horizontal)
            {
                float scrollLength = -_scrollContent.anchoredPosition.x - _padding.left;
                int scrollCol = (int) (scrollLength / ItemSize.x);
                startIndex = scrollCol * _row;
            }
            else
            {
                float scrollLength = _scrollContent.anchoredPosition.y - _padding.top;
                int scrollRow = (int) (scrollLength / ItemSize.y);
                startIndex = scrollRow * _col;
            }

            if (_startIndex == startIndex && !_isDirty)
            {
                return;
            }

            ProcessNewStartIndex(Math.Max(0, startIndex));
        }

        private void ProcessNewStartIndex(int startIndex)
        {
            _tempIndexList.Clear();
            foreach (var entity in _itemDict)
            {
                if (
                    entity.Key < startIndex
                    || entity.Key >= startIndex + _transCount
                    || entity.Key >= _itemCount
                )
                {
                    _tempIndexList.Add(entity.Key);
                }
            }

            for (int i = 0; i < _tempIndexList.Count; i++)
            {
                FreeItem(_itemDict[_tempIndexList[i]]);
                _itemDict.Remove(_tempIndexList[i]);
            }

            _tempIndexList.Clear();
            for (int i = startIndex, max = Math.Min(startIndex + _transCount, _itemCount); i < max; i++)
            {
                if (!_itemDict.ContainsKey(i))
                {
                    _tempIndexList.Add(i);
                }
            }

            for (int i = 0; i < _tempIndexList.Count; i++)
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
                var rowPos = index / _row;
                return new Vector2(_padding.left + ItemSize.x * rowPos, -_padding.top - ItemSize.y * (index % _row));
            }

            var colPos = index / _col;
            return new Vector2(_padding.left + ItemSize.x * (index % _col), -_padding.top - ItemSize.y * colPos);
        }

        private void FreeItem(IDataItemRenderer item)
        {
            item.Transform.anchoredPosition = new Vector2(-10000, 0);
            item.Set(null);
            _itemPool.Push(item);
        }

        private IDataItemRenderer GetItem()
        {
            IDataItemRenderer item;
            if (_itemPool.Count > 0)
            {
                item = _itemPool.Pop();
            }
            else
            {
                item = CreateItem();
            }

            return item;
        }

        private IDataItemRenderer CreateItem()
        {
            var item = _itemFactory.Invoke(_gridTransform);
            InitItem(item.Transform);
            return item;
        }

        public static bool IsFloatEqual(float a, float b, int scale)
        {
            return (int) (a * scale) == (int) (b * scale);
        }

        public void SetContentPosY(int inx, int colNum)
        {
            float y = inx / colNum * (_grid.spacing.y + _grid.cellSize.y);
            ContentPosition = new Vector2(ContentPosition.x, -y);
        }

        #endregion

        private void Update()
        {
            if (MouseIn && _scroller.vertical)
            {
                float value = Input.GetAxis("Mouse ScrollWheel");
                if (value > 0 && !ScorllWheelUpOff || value < 0 && !ScorllWheelDownOff)
                {
                    _scrollContent.DOBlendableMoveBy(Vector2.down * value * Time.deltaTime * 40000, 0.3f);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseIn = false;
        }

        public int GetItemIndex(IDataItemRenderer item)
        {
            int index = 0;
            foreach (var dataitem in _itemDict)
            {
                if (dataitem.Value == item)
                {
                    index = dataitem.Key;
                    break;
                }
            }

            return index;
        }

        //只针对竖向
        public int GetItemIndexByPos(Vector2 pos)
        {
            Vector2 calculatePos = new Vector2(Mathf.Clamp(pos.x, 0, ItemSize.x * _col), pos.y);

            int index = 0;
            index = (int) ((-calculatePos.y + _padding.top) / ItemSize.y) * _col;
            if (index < 0)
            {
                index = 0;
            }

            index += (int) ((calculatePos.x - _padding.left) / ItemSize.x);
//            if (index == 1)
//            {
////                Debug.Log("index");
//            }

            return index;
        }

        public Vector2 GetPosByIndex(int index)
        {
            return IndexToPosition(index);
        }

        public void OnItemDragMovePos(int beginindex, int newindex)
        {
            foreach (var item in _itemDict)
            {
                item.Value.MoveByIndex(beginindex, newindex);
            }
        }

        public void EndTween()
        {
            foreach (var item in _itemDict)
            {
                item.Value.EndTween();
            }
        }
    }

    public interface IDataItemRenderer
    {
        RectTransform Transform { get; }
        int Index { get; set; }
        object Data { get; }
        void Set(object data);
        void Unload();
        void MoveByIndex(int beginindex, int newindex);
        void EndTween();
    }
}