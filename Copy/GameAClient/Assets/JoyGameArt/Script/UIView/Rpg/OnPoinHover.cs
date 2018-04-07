using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnPoinHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject UpImage;
    private float _hoverTime;
    private bool _startTime;
    public bool _isUp;

    private void Update()
    {
        if (_isUp)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(EnterCoroutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isUp)
        {
            UpImage.SetActive(false);
        }
    }

    IEnumerator EnterCoroutine()
    {
        yield return 5.0f;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            UpImage.SetActive(true);
        }
    }
}