using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UGUIGestureRecognizer : EventTrigger
{
    private RectTransform _rectTransform;
    private Stack<TouchData> _touchDataPool;
    private List<TouchData> _touchList;
    private DragEvent _dragEvent;
    private PinchEvent _pinchEvent;

    private Action<DragEvent> _onDragBeginHandler;
    private Action<DragEvent> _onDragUpdateHandler;
    private Action<DragEvent> _onDragEndHandler;

    private Action<PinchEvent> _onPinchBeginHandler;
    private Action<PinchEvent> _onPinchUpdateHandler;
    private Action<PinchEvent> _onPinchEndHandler;

    public Action<DragEvent> OnDragBeginHandler
    {
        get
        {
            return this._onDragBeginHandler;
        }
        set
        {
            _onDragBeginHandler = value;
        }
    }

    public Action<DragEvent> OnDragUpdateHandler
    {
        get
        {
            return this._onDragUpdateHandler;
        }
        set
        {
            _onDragUpdateHandler = value;
        }
    }

    public Action<DragEvent> OnDragEndHandler
    {
        get
        {
            return this._onDragEndHandler;
        }
        set
        {
            _onDragEndHandler = value;
        }
    }

    public Action<PinchEvent> OnPinchBeginHandler
    {
        get
        {
            return this._onPinchBeginHandler;
        }
        set
        {
            _onPinchBeginHandler = value;
        }
    }

    public Action<PinchEvent> OnPinchUpdateHandler
    {
        get
        {
            return this._onPinchUpdateHandler;
        }
        set
        {
            _onPinchUpdateHandler = value;
        }
    }

    public Action<PinchEvent> OnPinchEndHandler
    {
        get
        {
            return this._onPinchEndHandler;
        }
        set
        {
            _onPinchEndHandler = value;
        }
    }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _touchDataPool = new Stack<TouchData>();
        _touchDataPool.Push(new TouchData());
        _touchDataPool.Push(new TouchData());
        _touchList = new List<TouchData>(2);
        _dragEvent = new DragEvent();
        _pinchEvent = new PinchEvent();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
//        Debug.Log("OnPointerDown, Id: " + eventData.pointerId);
        base.OnPointerDown(eventData);
        if(_touchList.Count == 0)
        {
            TouchData td = _touchDataPool.Pop();
            td.TouchId = eventData.pointerId;
            td.StartPosition = td.CurrentPosition = ConvertScreen2UGUI(eventData.position);
            _touchList.Add(td);
            _dragEvent.TouchData = td;
            _dragEvent.Delta = Vector2.zero;
            _dragEvent.TotalDelta = Vector2.zero;
            if(_onDragBeginHandler != null)
            {
                _onDragBeginHandler.Invoke(_dragEvent);
            }
        }
        else if(_touchList.Count == 1)
        {
            _dragEvent.Delta = Vector2.zero;
            if(_onDragEndHandler != null)
            {
                _onDragEndHandler.Invoke(_dragEvent);
            }

            var td0 = _touchList[0];
            td0.StartPosition = td0.CurrentPosition;
            var td1 = _touchDataPool.Pop();
            td1.TouchId = eventData.pointerId;
            td1.StartPosition = td1.CurrentPosition = ConvertScreen2UGUI(eventData.position);
            _touchList.Add(td1);
            _pinchEvent.TouchData0 = td0;
            _pinchEvent.TouchData1 = td1;
            _pinchEvent.TotalDeltaScale = 1;
            _pinchEvent.DeltaScale = 1;
            _pinchEvent.TotalCenterDelta = Vector2.zero;
            _pinchEvent.CenterDelta = Vector2.zero;
            _pinchEvent._startDistance = Mathf.Max(1f, Vector2.Distance(_pinchEvent.TouchData0.StartPosition, _pinchEvent.TouchData1.StartPosition));
            if(_onPinchBeginHandler != null)
            {
                _onPinchBeginHandler.Invoke(_pinchEvent);
            }
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
//        Debug.Log("OnPointerUp, Id: " + eventData.pointerId);
        base.OnPointerUp(eventData);
        int inx = _touchList.FindIndex(td=>td.TouchId == eventData.pointerId);
        if(inx < 0)
        {
            return;
        }
        if(_touchList.Count == 2)
        {
            _pinchEvent.DeltaScale = 1;
            if(_onPinchEndHandler != null)
            {
                _onPinchEndHandler.Invoke(_pinchEvent);
            }
            var td1 = _touchList[inx];
            _touchList.RemoveAt(inx);
            _touchDataPool.Push(td1);
            var td0 = _touchList[0];
            td0.StartPosition = td0.CurrentPosition;
            _dragEvent.TouchData = td0;
            _dragEvent.TotalDelta = Vector2.zero;
            _dragEvent.Delta = Vector2.zero;
            if(_onDragBeginHandler != null)
            {
                _onDragBeginHandler.Invoke(_dragEvent);
            }
        }
        else if(_touchList.Count == 1)
        {
            _dragEvent.Delta = Vector2.zero;
            if(_onDragEndHandler != null)
            {
                _onDragEndHandler.Invoke(_dragEvent);
            }
            var td0 = _touchList[inx];
            _touchList.Clear();
            _touchDataPool.Push(td0);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
//        Debug.Log("OnPointerDrag, Id: " + eventData.pointerId);
        base.OnDrag(eventData);
        int inx = _touchList.FindIndex(td=>td.TouchId == eventData.pointerId);
        if(inx < 0)
        {
            return;
        }
        if(_touchList.Count == 1)
        {
            var td = _touchList[inx];
            td.CurrentPosition = ConvertScreen2UGUI(eventData.position);
            var oldTotalDelta = _dragEvent.TotalDelta;
            _dragEvent.TotalDelta = td.CurrentPosition - td.StartPosition;
            _dragEvent.Delta = _dragEvent.TotalDelta - oldTotalDelta;
            if(_onDragUpdateHandler != null)
            {
                _onDragUpdateHandler.Invoke(_dragEvent);
            }
        }
        else if(_touchList.Count == 2)
        {
            var td = _touchList[inx];
            td.CurrentPosition = ConvertScreen2UGUI(eventData.position);
            var oldTotalDeltaScale = _pinchEvent.TotalDeltaScale;
            _pinchEvent.TotalDeltaScale = Vector2.Distance(_pinchEvent.TouchData0.CurrentPosition,_pinchEvent.TouchData1.CurrentPosition)/_pinchEvent._startDistance;
            _pinchEvent.DeltaScale = _pinchEvent.TotalDeltaScale / oldTotalDeltaScale;
            var oldCenterDelta = _pinchEvent.TotalCenterDelta;
            _pinchEvent.TotalCenterDelta = (_pinchEvent.TouchData0.CurrentPosition + _pinchEvent.TouchData1.CurrentPosition) * 0.5f
                - (_pinchEvent.TouchData0.StartPosition + _pinchEvent.TouchData1.StartPosition) * 0.5f;
            _pinchEvent.CenterDelta = _pinchEvent.TotalCenterDelta - oldCenterDelta;
            if(_onPinchUpdateHandler != null)
            {
                _onPinchUpdateHandler.Invoke(_pinchEvent);
            }
        }
    }

    private Vector2 ConvertScreen2UGUI(Vector2 screen)
    {
        return screen * _rectTransform.rect.width / Screen.width;
    }


    public static UGUIGestureRecognizer GetInstance(GameObject gameObject)
    {
        UGUIGestureRecognizer instance = gameObject.GetComponent<UGUIGestureRecognizer>();
        if(instance == null)
        {
            instance = gameObject.AddComponent<UGUIGestureRecognizer>();
        }
        return instance;
    }

    public class TouchData
    {
        public Vector2 StartPosition {get; internal set;}
        public Vector2 CurrentPosition {get; internal set;}
        public int TouchId {get; internal set;}
    }

    public class DragEvent
    {
        public TouchData TouchData {get; internal set;}
        public Vector2 TotalDelta {get; internal set;}
        public Vector2 Delta {get; internal set;}
    }

    public class PinchEvent
    {
        public TouchData TouchData0 {get; internal set;}
        public TouchData TouchData1 {get; internal set;}
        public float TotalDeltaScale {get; internal set;}
        public float DeltaScale {get; internal set;}
        internal float _startDistance;
        public Vector2 TotalCenterDelta {get; internal set;}
        public Vector2 CenterDelta {get; internal set;}
    }
}
