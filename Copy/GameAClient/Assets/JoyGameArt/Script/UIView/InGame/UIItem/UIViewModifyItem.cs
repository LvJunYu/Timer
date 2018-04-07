using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewModifyItem : MonoBehaviour {

	public Button DelBtn;
    public Button IconBtn;
	public Image Empty;
	public Image Icon;

	public int id;

	public System.Action<int> DelBtnCb;
    public System.Action<int> IconBtnCb;

	public void SetEmpty () {
		DelBtn.gameObject.SetActive (false);
		Empty.gameObject.SetActive (true);
		Icon.gameObject.SetActive (false);
	}

	public void SetItem (Sprite sprite) {
		DelBtn.gameObject.SetActive (true);
		Empty.gameObject.SetActive (false);
		Icon.gameObject.SetActive (true);
		Icon.sprite = sprite;
	}

	void Awake () {
		DelBtn.onClick.AddListener (OnDelBtn);
        IconBtn.onClick.AddListener (OnIconBtn);
	}

	void OnDelBtn () {
		if (DelBtnCb != null) {
			DelBtnCb.Invoke (id);
		}
	}
    void OnIconBtn () {
        if (IconBtnCb != null) {
            IconBtnCb.Invoke (id);
        }
    }
}
