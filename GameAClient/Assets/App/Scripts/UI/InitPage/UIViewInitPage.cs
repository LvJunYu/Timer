using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

public class UIViewInitPage : UIViewBase {
    public Image BgImage;
    public Image LoadingImage;
    public Sprite[] LoadingSpriteAry;

	public GameObject UpdateResProcessRoot;
	public Text UpdateResProcessValue;

    private const float SwitchInterval = 0.3f;
    private int _inx;
    private float _leftTime;

    public void ResetState()
    {
        _leftTime = SwitchInterval;
        _inx = 0;
        LoadingImage.sprite = LoadingSpriteAry[_inx];
    }

    private void Update()
    {
        _leftTime -= Time.deltaTime;
        if(_leftTime > 0)
        {
            return;
        }
        _leftTime = SwitchInterval;
        _inx++;
        if(_inx >= LoadingSpriteAry.Length)
        {
            _inx = 0;
        }
        LoadingImage.sprite = LoadingSpriteAry[_inx];
    }
}
