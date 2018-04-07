using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoyEngine
{
    [RequireComponent(typeof(ScrollRect)), DisallowMultipleComponent]
    public class TableDataScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const float ShowDistance = 100f;
        private const float FadeDistance = 400f;
        [SerializeField] private LayoutGroup _layoutGroup;

        private RectTransform _layoutTransform;
        private LayoutElement _layoutElement;

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
            Vertical
        }

        // public fields //
        private Movement _moveType = Movement.Horizontal;

        public delegate void OnChange(IDataItemRenderer item, int index);

        private readonly Stack<IDataItemRenderer> _itemPool = new Stack<IDataItemRenderer>();
        private OnChange _onChange;
        private Func<RectTransform, IDataItemRenderer> _itemFactory;
        private readonly List<ItemData> _itemDataList = new List<ItemData>();
        private int _activeStartIndex;
        private int _activeLen;
        private Vector2 _contentSize;
        private Vector2 _spacing = Vector3.zero;
        private RectOffset _padding = new RectOffset();
        private bool _hasInited;
        private int _itemCount;
        private int _poolItemCount;
        private bool _mouseIn;
        [HideInInspector] public bool ScorllWheelUpOff;
        [HideInInspector] public bool ScorllWheelDownOff;

        public ScrollRect ScrollRect
        {
            get { return _scroller; }
        }

        public bool MouseIn
        {
            get { return _mouseIn; }
        }

        #region Public Methods

        public void Set(OnChange onChange, Func<RectTransform, IDataItemRenderer> factory, int poolItemCount = 0)
        {
            _onChange = onChange;
            _itemFactory = factory;
            _poolItemCount = poolItemCount;
            PreparePool();
            Init();
        }

        public void SetEmpty()
        {
            Init();
            _itemCount = 0;
            for (var i = 0; i < _activeLen; i++)
            {
                FreeItem(_itemDataList[_activeStartIndex + i].ItemRenderer);
            }
            _itemDataList.Clear();
            if (_moveType == Movement.Horizontal)
            {
                _contentSize.x = _padding.left + _padding.right;
            }
            else
            {
                _contentSize.y = _padding.top + _padding.bottom;
            }
            RefreshLayoutElementSize();
            _activeStartIndex = 0;
            _activeLen = 0;
        }

        public void RefreshCurrent()
        {
            for (int i = _activeStartIndex; i < _activeStartIndex + _activeLen; i++)
            {
                if (i < _itemDataList.Count)
                {
                    _onChange(_itemDataList[i].ItemRenderer, i);
                }
            }
            RefreshAllSizes();
        }

        public void RefreshAllSizes()
        {
            for (int i = _activeStartIndex; i < _activeStartIndex + _activeLen; i++)
            {
                if (i < _itemDataList.Count)
                {
                    var data = _itemDataList[i];
                    if (data.ItemRenderer == null)
                    {
                        LogHelper.Error("data.ItemRenderer == null");
                        return;
                    }
                    var rect = data.ItemRenderer.Transform.rect;
                    if (_moveType == Movement.Horizontal)
                    {
                        var newSize = Mathf.RoundToInt(rect.size.x);
                        if (newSize != data.Size)
                        {
                            var oldSize = data.Size;
                            data.Size = newSize;
                            for (var j = i + 1; j < _activeStartIndex + _activeLen; j++)
                            {
                                var pos = _itemDataList[j].ItemRenderer.Transform.anchoredPosition;
                                pos.x += newSize - oldSize;
                                _itemDataList[j].ItemRenderer.Transform.anchoredPosition = pos;
                            }
                            _contentSize.x += newSize - oldSize;
                        }
                    }
                    else
                    {
                        var newSize = Mathf.RoundToInt(rect.size.y);
                        if (newSize != data.Size)
                        {
                            var oldSize = data.Size;
                            data.Size = newSize;
                            for (var j = i + 1; j < _activeStartIndex + _activeLen; j++)
                            {
                                var pos = _itemDataList[j].ItemRenderer.Transform.anchoredPosition;
                                pos.y -= newSize - oldSize;
                                _itemDataList[j].ItemRenderer.Transform.anchoredPosition = pos;
                            }
                            _contentSize.y += newSize - oldSize;
                        }
                    }
                    _itemDataList[i] = data;
                }
            }
            RefreshLayoutElementSize();
        }

        public void SetItemCount(int count)
        {
            Init();
            if (_itemCount > count)
            {
                SetEmpty();
            }
            _itemCount = count;
            RefreshView();
            RefreshCurrent();
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
            InitLayoutParam();
            InitScroller();
            SetEmpty();
            PreparePool();
        }

        private void InitScroller()
        {
            // Init Scroller //
            _scroller = GetComponent<ScrollRect>();
//            _scrollerTransform = _scroller.GetComponent<RectTransform>();
            Rect vr = _scroller.viewport.rect;
            if (_moveType == Movement.Horizontal)
            {
                _scroller.vertical = false;
                _scroller.horizontal = true;
                _layoutTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vr.size.y);
            }
            else
            {
                _scroller.vertical = true;
                _scroller.horizontal = false;
                _layoutTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vr.size.x);
            }
            _scrollContent = _scroller.content;
            _scroller.onValueChanged.AddListener(OnValueChanged);
        }

        private void InitLayoutParam()
        {
            VerticalLayoutGroup vlg;
            HorizontalLayoutGroup hlg;
            if (_layoutGroup is HorizontalLayoutGroup)
            {
                _moveType = Movement.Horizontal;
                hlg = _layoutGroup as HorizontalLayoutGroup;
                _spacing = new Vector2(hlg.spacing, 0);
            }
            else
            {
                _moveType = Movement.Vertical;
                vlg = _layoutGroup as VerticalLayoutGroup;
                if (vlg != null)
                {
                    _spacing = new Vector2(0, vlg.spacing);
                }
            }

            _padding = _layoutGroup.padding;
            _layoutGroup.enabled = false;
            _layoutTransform = _layoutGroup.GetComponent<RectTransform>();
            _layoutElement = _layoutGroup.GetComponent<LayoutElement>();
            if (_layoutElement == null)
            {
                _layoutElement = _layoutGroup.gameObject.AddComponent<LayoutElement>();
            }
        }

        private void PreparePool()
        {
            if (!_hasInited)
            {
                return;
            }
            while (_itemPool.Count < _poolItemCount)
            {
                FreeItem(CreateItem());
            }
        }

        private void InitItem(RectTransform rectTrans)
        {
            rectTrans.anchorMax = new Vector2(0, 1);
            rectTrans.anchorMin = new Vector2(0, 1);
            rectTrans.pivot = new Vector2(0, 1);
        }

        public void Clear()
        {
            _itemPool.Clear();
            _itemDataList.Clear();
            _layoutTransform.DestroyChildren();
        }

        #endregion

        #region OnValueChanged

        public void OnValueChanged(Vector2 normalizedPosition)
        {
            RefreshView();
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
            var item = _itemFactory.Invoke(_layoutTransform);
            InitItem(item.Transform);
            return item;
        }

        private void RefreshView()
        {
            bool effect = true;
            while (effect)
            {
                effect = false;
                effect |= ProcessAddUp();
                effect |= ProcessRemoveUp();
                effect |= ProcessAddDown();
                effect |= ProcessRemoveDown();
            }
            RefreshLayoutElementSize();
        }

        private void RefreshLayoutElementSize()
        {
            if (_moveType == Movement.Horizontal)
            {
                _layoutElement.minWidth = _layoutElement.preferredWidth = _contentSize.x;
            }
            else
            {
                _layoutElement.minHeight = _layoutElement.preferredHeight = _contentSize.y;
            }
        }

        /// <summary>
        /// 检查上侧是否需要添加item
        /// </summary>
        private bool ProcessAddUp()
        {
            if (_activeStartIndex == 0)
            {
                return false;
            }
            float upAnchoredVal;
            float upBound = GetItemUpBound(_itemDataList[_activeStartIndex].ItemRenderer, out upAnchoredVal);
            if (_moveType == Movement.Horizontal && upBound < -ShowDistance)
            {
                return false;
            }
            if (_moveType == Movement.Vertical && upBound > ShowDistance)
            {
                return false;
            }
            _activeStartIndex--;
            _activeLen++;
            var item = GetItem();
            _onChange(item, _activeStartIndex);
            Canvas.ForceUpdateCanvases();
            var rect = item.Transform.rect;
            var data = _itemDataList[_activeStartIndex];
            data.ItemRenderer = item;
            if (_moveType == Movement.Horizontal)
            {
                item.Transform.anchoredPosition =
                    new Vector2(upAnchoredVal - _spacing.x - data.Size, -_padding.top);
                var newSize = Mathf.RoundToInt(rect.size.x);
                if (newSize != data.Size)
                {
                    var oldSize = data.Size;
                    data.Size = newSize;
                    for (var i = 1; i < _activeLen; i++)
                    {
                        var pos = _itemDataList[_activeStartIndex + i].ItemRenderer.Transform.anchoredPosition;
                        pos.x += newSize - oldSize;
                        _itemDataList[_activeStartIndex + i].ItemRenderer.Transform.anchoredPosition = pos;
                    }
                    _contentSize.x += newSize - oldSize;
                }
            }
            else
            {
                item.Transform.anchoredPosition =
                    new Vector2(_padding.left, upAnchoredVal + _spacing.y + data.Size);
                var newSize = Mathf.RoundToInt(rect.size.y);
                if (newSize != data.Size)
                {
                    var oldSize = data.Size;
                    data.Size = newSize;
                    for (var i = 1; i < _activeLen; i++)
                    {
                        var pos = _itemDataList[_activeStartIndex + i].ItemRenderer.Transform.anchoredPosition;
                        pos.y -= newSize - oldSize;
                        _itemDataList[_activeStartIndex + i].ItemRenderer.Transform.anchoredPosition = pos;
                    }
                    _contentSize.y += newSize - oldSize;
                }
            }
            _itemDataList[_activeStartIndex] = data;
            return true;
        }

        /// <summary>
        /// 检查上侧是否需要移除item
        /// </summary>
        private bool ProcessRemoveUp()
        {
            if (_activeLen <= 1)
            {
                return false;
            }
            float downAnchoredVal;
            float downBound = GetItemDownBound(_itemDataList[_activeStartIndex].ItemRenderer, out downAnchoredVal);
            if (_moveType == Movement.Horizontal && downBound > -FadeDistance)
            {
                return false;
            }
            if (_moveType == Movement.Vertical && downBound < FadeDistance)
            {
                return false;
            }
            var data = _itemDataList[_activeStartIndex];
            FreeItem(data.ItemRenderer);
            data.ItemRenderer = null;
            _itemDataList[_activeStartIndex] = data;
            _activeStartIndex++;
            _activeLen--;
            return true;
        }

        /// <summary>
        /// 检查下侧是否需要增加item
        /// </summary>
        private bool ProcessAddDown()
        {
            Rect viewRect = _scroller.viewport.rect;
            if (_activeStartIndex + _activeLen >= _itemCount)
            {
                return false;
            }
            float downAnchoredVal;
            float downBound;
            if (_itemDataList.Count == 0)
            {
                downBound = _moveType == Movement.Horizontal ? _padding.left : -_padding.top;
                downAnchoredVal = _moveType == Movement.Horizontal
                    ? _padding.left - _spacing.x
                    : -_padding.top + _spacing.y;
            }
            else
            {
                downBound = GetItemDownBound(_itemDataList[_activeStartIndex + _activeLen - 1].ItemRenderer,
                    out downAnchoredVal);
            }
            if (_moveType == Movement.Horizontal && downBound > ShowDistance + viewRect.size.x)
            {
                return false;
            }
            if (_moveType == Movement.Vertical && downBound < -viewRect.size.y - ShowDistance)
            {
                return false;
            }
            _activeLen++;
            var item = GetItem();
            _onChange(item, _activeStartIndex + _activeLen - 1);
            Canvas.ForceUpdateCanvases();
            var rect = item.Transform.rect;
            if (_itemDataList.Count < _activeStartIndex + _activeLen)
            {
                if (_itemDataList.Count != 0)
                {
                    if (_moveType == Movement.Horizontal)
                    {
                        _contentSize.x += _spacing.x;
                    }
                    else
                    {
                        _contentSize.y += _spacing.y;
                    }
                }
                _itemDataList.Add(new ItemData {Size = 0, ItemRenderer = null});
            }
            var data = _itemDataList[_activeStartIndex + _activeLen - 1];
            data.ItemRenderer = item;
            if (_moveType == Movement.Horizontal)
            {
                item.Transform.anchoredPosition = new Vector2(downAnchoredVal + _spacing.x, -_padding.top);
                var newSize = Mathf.RoundToInt(rect.size.x);
                if (newSize != data.Size)
                {
                    var oldSize = data.Size;
                    data.Size = newSize;
                    _contentSize.x += newSize - oldSize;
                }
            }
            else
            {
                item.Transform.anchoredPosition = new Vector2(_padding.left, downAnchoredVal - _spacing.y);
                var newSize = Mathf.RoundToInt(rect.size.y);
                if (newSize != data.Size)
                {
                    var oldSize = data.Size;
                    data.Size = newSize;
                    _contentSize.y += newSize - oldSize;
                }
            }
            _itemDataList[_activeStartIndex + _activeLen - 1] = data;
            return true;
        }

        /// <summary>
        /// 检查下侧是否需要移除item
        /// </summary>
        private bool ProcessRemoveDown()
        {
            Rect viewRect = _scroller.viewport.rect;
            if (_activeLen <= 1)
            {
                return false;
            }
            float upAnchoredVal;
            float upBound = GetItemUpBound(_itemDataList[_activeStartIndex + _activeLen - 1].ItemRenderer,
                out upAnchoredVal);
            if (_moveType == Movement.Horizontal && upBound < viewRect.size.x + FadeDistance)
            {
                return false;
            }
            if (_moveType == Movement.Vertical && upBound > -viewRect.size.y - FadeDistance)
            {
                return false;
            }
            var data = _itemDataList[_activeStartIndex + _activeLen - 1];
            FreeItem(data.ItemRenderer);
            data.ItemRenderer = null;
            _itemDataList[_activeStartIndex + _activeLen - 1] = data;
            _activeLen--;
            return true;
        }

        private float GetItemUpBound(IDataItemRenderer item, out float realVal)
        {
            if (_moveType == Movement.Horizontal)
            {
                realVal = item.Transform.anchoredPosition.x;
                return _scrollContent.anchoredPosition.x + item.Transform.anchoredPosition.x;
            }
            realVal = item.Transform.anchoredPosition.y;
            return _scrollContent.anchoredPosition.y + item.Transform.anchoredPosition.y;
        }

        private float GetItemDownBound(IDataItemRenderer item, out float realVal)
        {
            Rect rect = item.Transform.rect;
            if (_moveType == Movement.Horizontal)
            {
                realVal = item.Transform.anchoredPosition.x + Mathf.Round(rect.size.x);
                return _scrollContent.anchoredPosition.x + item.Transform.anchoredPosition.x + rect.size.x;
            }
            realVal = item.Transform.anchoredPosition.y - Mathf.Round(rect.size.y);
            return _scrollContent.anchoredPosition.y + item.Transform.anchoredPosition.y - rect.size.y;
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

        private struct ItemData
        {
            public int Size;
            public IDataItemRenderer ItemRenderer;
        }
    }
}

//*/