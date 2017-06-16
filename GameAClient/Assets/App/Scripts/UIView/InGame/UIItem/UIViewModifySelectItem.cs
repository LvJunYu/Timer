using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewModifySelectItem : MonoBehaviour {

	public Button Btn;
	public Image Icon;
    public Text Number;
    public Transform IconTrans;
    public Transform ShadowTrans;

	public int id;

	public System.Action<int> BtnCb;


    public void SetItem (Sprite sprite, int number) {
		Icon.gameObject.SetActive (true);
		Icon.sprite = sprite;
        Number.text = number.ToString ();
	}

    public void SetSelected (bool selected) {
        if (selected) {
            IconTrans.localPosition = Vector3.up * 15;
//            IconTrans.localScale = Vector3.one * 1.1f;
            ShadowTrans.localScale = Vector3.one * 0.7f;
        } else {
            IconTrans.localPosition = Vector3.zero;
//            IconTrans.localScale = Vector3.one;
            ShadowTrans.localScale = Vector3.one;
        }
    }

	void Awake () {
        Btn.onClick.AddListener (OnBtn);
	}

	void OnBtn () {
        if (BtnCb != null) {
            BtnCb.Invoke (id);
		}
	}
}
