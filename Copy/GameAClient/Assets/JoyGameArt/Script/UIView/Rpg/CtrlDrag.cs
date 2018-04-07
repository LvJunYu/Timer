using System;
using DG.Tweening;
using GameA;
using UnityEngine;
using UnityEngine.UI;

public class CtrlDrag : MonoBehaviour
{
    public Transform Canvas;
    private float _beginPos;
    public VerticalLayoutGroup LayoutGroup;
    public RectTransform ItemContent;
    public RectTransform ViewTrs;
    private int _beginIndex;
    private Rect _rangeRect;
    private float _height;
    private DragHelper _dragHelper;
    private RectTransform _selfTras;
    private float _spaceFloat;
    private int _newIndex;
    public ContentSizeFitter SizeFitter;
    public Action OnBeforeDragEndAction;
    public Action OnAfterDragEndAction;
    private bool _setDisable;
    private Vector3 _targetRotation = new Vector3(0.0f, 0.0f, -3.0f);
    private Vector3 _targetRotation2 = new Vector3(0.0f, 0.0f, 3.0f);
    private Sequence _rotationSequence;


    private void Start()
    {
        _selfTras = transform.GetComponent<RectTransform>();
        _beginIndex = _selfTras.GetSiblingIndex();
        _height = _selfTras.rect.height;
        _dragHelper = GetComponent<DragHelper>();
        if (!_setDisable)
        {
            _dragHelper.OnBeginDragAction = DragBegin;
            _dragHelper.OnEndDragAction = DragEnd;
            _dragHelper.OnDragAction = OnDrag;
        }


        _spaceFloat = LayoutGroup.spacing;
        SetTween();
    }

    private void DragBegin()
    {
        _beginPos = _selfTras.anchoredPosition.y;
        _beginIndex = _selfTras.GetSiblingIndex();
        transform.SetParent(Canvas);
        transform.SetAsLastSibling();
        LayoutGroup.enabled = false;
        SizeFitter.enabled = false;
        _newIndex = -1;
        _rotationSequence.Play();
    }

    private void GetRangeRect()
    {
        Vector3 RightUpPos = new Vector3(ViewTrs.rect.width * (1 - ViewTrs.pivot.x),
            ViewTrs.rect.height * (1 - ViewTrs.pivot.y));
        Vector3 LeftDownPos =
            new Vector3(-ViewTrs.rect.width * ViewTrs.pivot.x, -ViewTrs.rect.height * ViewTrs.pivot.y);
        _rangeRect.xMin = ItemContent.InverseTransformPoint(ViewTrs.TransformPoint(LeftDownPos)).x;
        _rangeRect.yMin = ItemContent.InverseTransformPoint(ViewTrs.TransformPoint(LeftDownPos)).y;
        _rangeRect.xMax = ItemContent.InverseTransformPoint(ViewTrs.TransformPoint(RightUpPos)).x;
        _rangeRect.yMax = ItemContent.InverseTransformPoint(ViewTrs.TransformPoint(RightUpPos)).y;
    }

    private void DragEnd()
    {
        if (_tween != null)
        {
            if (_tween.IsPlaying())
            {
                _tween.Complete();
            }
        }

        if (OnBeforeDragEndAction != null)
        {
            OnBeforeDragEndAction.Invoke();
        }

        _selfTras.SetParent(ItemContent);
        if (_newIndex != -1)
        {
            _selfTras.SetSiblingIndex(_newIndex);
        }
        else
        {
            _selfTras.SetSiblingIndex(_beginIndex);
        }


        if (OnAfterDragEndAction != null)
        {
            OnAfterDragEndAction.Invoke();
        }

        for (int i = 0; i < ItemContent.childCount; i++)
        {
            ItemContent.GetChild(i).GetComponent<CtrlDrag>().EndTween();
        }

        _rotationSequence.Complete(false);
        LayoutGroup.enabled = true;
        SizeFitter.enabled = true;
        LayoutGroup.SetLayoutVertical();
    }

    private void OnDrag()
    {
        GetRangeRect();
        if (_rangeRect.Contains(ItemContent.InverseTransformPoint(_selfTras.position)))
        {
            int moveIndex = (int) ((ItemContent.InverseTransformPoint(_selfTras.position).y - _beginPos) /
                                   _height);
//            if (moveIndex <= 0)
//            {
            if (_newIndex != (_beginIndex - moveIndex))
            {
                _newIndex = _beginIndex - moveIndex;
                for (int i = 0; i < ItemContent.childCount; i++)
                {
                    ItemContent.GetChild(i).GetComponent<CtrlDrag>().MoveByIndex(_newIndex);
                }
            }

//            }
        }
        else
        {
            float moveDistanceY = 0.0f;
            float posY = ItemContent.InverseTransformPoint(_selfTras.position).y;
            if (posY > _rangeRect.yMax)
            {
                moveDistanceY = posY - _rangeRect.yMax;
            }

            if (posY < _rangeRect.yMin)
            {
                moveDistanceY = posY - _rangeRect.yMin;
            }

            Vector2 moveDistance = new Vector2(0, moveDistanceY);
            Vector2 targetPos = ItemContent.anchoredPosition - moveDistance;
            if (targetPos.y > 0 && targetPos.y < ItemContent.rect.height - ViewTrs.rect.height)
            {
                ItemContent.anchoredPosition = targetPos;
            }
        }
    }

    private Tween _tween;

    public void MoveByIndex(int judgeindex)
    {
        if (_tween != null)
        {
            if (_tween.IsPlaying())
            {
                _tween.Complete();
            }
        }

        if (_selfTras.GetSiblingIndex() < judgeindex)
        {
            Vector2 targerpos = new Vector2(_selfTras.rect.width / 2,
                -(_selfTras.GetSiblingIndex() + 0.5f) * _height - (_selfTras.GetSiblingIndex()) * _spaceFloat);
            _tween = _selfTras.DOAnchorPosY(targerpos.y, 0.5f);
            _tween.Play();
        }
        else
        {
            Vector2 targerpos = new Vector2(_selfTras.rect.width / 2,
                -(_selfTras.GetSiblingIndex() + 1.5f) * _height - (_selfTras.GetSiblingIndex()) * _spaceFloat);
            _tween = _selfTras.DOAnchorPosY(targerpos.y, 0.5f);
            _tween.Play();
        }
    }

    public void SetDisable()
    {
        if (_dragHelper == null)
        {
            _dragHelper = GetComponent<DragHelper>();
        }

        _dragHelper.OnBeginDragAction = null;
        _dragHelper.OnEndDragAction = null;
        _dragHelper.OnDragAction = null;
        _setDisable = true;
    }

    public void SetCanDrag()
    {
        if (_dragHelper == null)
        {
            _dragHelper = GetComponent<DragHelper>();
        }

        _dragHelper.OnBeginDragAction = DragBegin;
        _dragHelper.OnEndDragAction = DragEnd;
        _dragHelper.OnDragAction = OnDrag;
        _setDisable = false;
    }

    public void EndTween()
    {
        _tween.Complete(true);
    }

    private void SetTween()
    {
        _rotationSequence = DOTween.Sequence();
        _rotationSequence.Append(_selfTras.DOLocalRotate(_targetRotation, 0.2f));
        _rotationSequence.Append(_selfTras.DOLocalRotate(_targetRotation2, 0.2f));
        _rotationSequence.SetAutoKill(false).Pause();
        _rotationSequence.OnComplete(() => { _rotationSequence.Restart(); });

//        _rotationSequence.SetLoops(Int32.MaxValue);
    }
}