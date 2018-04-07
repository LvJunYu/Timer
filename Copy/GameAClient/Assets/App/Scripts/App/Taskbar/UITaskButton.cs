using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UITaskButton : MonoBehaviour
{
    public Button Button;
    public Image NewMessageMark;

    private void Awake()
    {
        NewMessageMark.gameObject.SetActive(false);
    }

    public void MarkUnread()
    {
        NewMessageMark.gameObject.SetActive(true);
    }

    public void MarkAllRead()
    {
        NewMessageMark.gameObject.SetActive(false);
    }
}
