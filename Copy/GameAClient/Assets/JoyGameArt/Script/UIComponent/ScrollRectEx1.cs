using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectEx1 : ScrollRect
{
    private PointEvent _onBeginDragEvent = new PointEvent();
    private PointEvent _onEndDragEvent = new PointEvent();
    private PointEvent _onDragEvent = new PointEvent();

    public PointEvent OnBeginDragEvent
    {
        get { return _onBeginDragEvent; }
    }

    public PointEvent OnEndDragEvent
    {
        get { return _onEndDragEvent; }
    }

    public PointEvent OnDragEvent
    {
        get { return _onDragEvent; }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        _onBeginDragEvent.Invoke(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        _onEndDragEvent.Invoke(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        _onDragEvent.Invoke(eventData);
    }
}