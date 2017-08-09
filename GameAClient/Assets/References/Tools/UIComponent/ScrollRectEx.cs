using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ScrollRectEx : ScrollRect
{
    protected const float VelocityScaleFactor = 1.8f;
    protected bool _dragging;
    protected bool _previousDragging;
    protected readonly Vector3[] _corners = new Vector3[4];
    protected Bounds _viewBounds;
    protected Bounds _contentBounds;
    protected PointEvent _onBeginDragEvent = new PointEvent();
    protected PointEvent _onEndDragEvent = new PointEvent();
	protected PointEvent _onDragEvent = new PointEvent();

    public bool vScrollingNeeded
    {
        get
        {
            this.UpdateBounds();
            if (this._contentBounds.size.y <= this._viewBounds.size.y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

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

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        _dragging = false;
        _previousDragging = false;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!IsActive())
            return;
        _dragging = true;
        _onBeginDragEvent.Invoke(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        _dragging = false;
        _onEndDragEvent.Invoke(eventData);
    }

	public override void OnDrag (PointerEventData eventData)
	{
		base.OnDrag (eventData);
		_onDragEvent.Invoke(eventData);
	}

    protected Bounds GetBounds()
    {
        if (this.content == null)
        {
            return default(Bounds);
        }
        Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Matrix4x4 worldToLocalMatrix = this.viewRect.worldToLocalMatrix;
        this.content.GetWorldCorners(this._corners);
        for (int i = 0; i < 4; i++)
        {
            Vector3 vector3 = worldToLocalMatrix.MultiplyPoint3x4(this._corners[i]);
            vector = Vector3.Min(vector3, vector);
            vector2 = Vector3.Max(vector3, vector2);
        }
        Bounds result = new Bounds(vector, Vector3.zero);
        result.Encapsulate(vector2);
        return result;
    }

    protected void UpdateBounds()
    {
        this._viewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
        this._contentBounds = this.GetBounds();
        if (this.content == null)
        {
            return;
        }
        Vector3 size = this._contentBounds.size;
        Vector3 center = this._contentBounds.center;
        Vector3 vector = this._viewBounds.size - size;
        if (vector.x > 0)
        {
            center.x -= vector.x * (this.content.pivot.x - 0.5f);
            size.x = this._viewBounds.size.x;
        }
        if (vector.y > 0)
        {
            center.y -= vector.y * (this.content.pivot.y - 0.5f);
            size.y = this._viewBounds.size.y;
        }
        this._contentBounds.size = size;
        this._contentBounds.center = center;
    }

    protected override void LateUpdate()
    {
        if (!content)
            return;

        if (!_dragging && _previousDragging && inertia)
        {
            velocity = velocity * VelocityScaleFactor;
        }
        base.LateUpdate();
        _previousDragging = _dragging;
    }


}
public class PointEvent:UnityEvent<PointerEventData>
{

}