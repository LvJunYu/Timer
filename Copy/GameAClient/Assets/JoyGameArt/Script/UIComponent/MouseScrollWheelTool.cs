using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseScrollWheelTool : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScrollRect ScrollRect;
    private bool _mouseIn;
    [HideInInspector]
    public bool ScorllWheelUpOff;
    [HideInInspector]
    public bool ScorllWheelDownOff;

    private void Update()
    {
        if (_mouseIn && ScrollRect.vertical)
        {
            float value = Input.GetAxis("Mouse ScrollWheel");
            if (value > 0 && !ScorllWheelUpOff || value < 0 && !ScorllWheelDownOff)
            {
                ScrollRect.content.DOBlendableMoveBy(Vector2.down * value * Time.deltaTime * 40000, 0.3f);
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
}