using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashController : MonoBehaviour {

//	/// <summary>
//	/// 不显示的区域1
//	/// </summary>
//	public Vector2 _region1;
//	/// <summary>
//	/// 不显示的区域2
//	/// </summary>
//	public Vector2 _region2;
	private float[] _region = new float[10];

	/// <summary>
	/// 总体透明度
	/// </summary>
	public float _mainAlpha = 1.0f;
	/// <summary>
	/// UV动画1的移动速度
	/// </summary>
	public float _drop1Speed = 0.05f;
	/// <summary>
	/// UV动画1的alpha循环速度
	/// </summary>
	public float _drop1AlphaSpeed = 1f;
	/// <summary>
	/// UV动画2的移动速度
	/// </summary>
	public float _drop2Speed = 0.05f;
	/// <summary>
	/// UV动画2的alpha循环速度
	/// </summary>
	public float _drop2AlphaSpeed = 1f;
	/// <summary>
	/// UV动画1和2的alpha循环相位差
	/// </summary>
	public float _drop2AlphaPhaseOffset = 0.1f;
	/// <summary>
	/// UV动画2的U偏移
	/// </summary>
	public float _drop2TileXOffset = 0.5f;
	/// <summary>
	/// UV动画的最小透明度
	/// </summary>
	public float _dropAlphaMin = 0;
	/// <summary>
	/// UV动画的最大透明度
	/// </summary>
	public float _dropAlphaMax = 1f;
	private Material _mat; 
	// Use this for initialization
	void Awake () {
		Renderer r = GetComponent<Renderer> ();
		_mat = r.material;
	}
	
	// Update is called once per frame
	void Update () {
		_mat.SetFloat ("_drop1Pos", Time.time * _drop1Speed);
		_mat.SetFloat ("_drop1Alpha", Mathf.Clamp01(Mathf.Lerp(_dropAlphaMin, _dropAlphaMax, (Mathf.Sin(Time.time * _drop1AlphaSpeed) + 1f) * 0.5f)));
		_mat.SetFloat ("_drop2Pos", Time.time * _drop2Speed);
		_mat.SetFloat ("_drop2Alpha", Mathf.Clamp01(Mathf.Lerp(_dropAlphaMin, _dropAlphaMax, (Mathf.Sin((Time.time + _drop2AlphaPhaseOffset) * _drop2AlphaSpeed) + 1f) * 0.5f)));
		_mat.SetFloat ("_drop2Offset", _drop2TileXOffset);

		_mat.SetFloat ("_mainAlpha", _mainAlpha);
	}

	public void SetSplashRegion (List<Vector2> regions) {
		Debug.Log ("____________SetSplashRegion " + regions.Count);
        int i = 0;
        for (; i < 5 && i < regions.Count; i++)
        {
            _region[2 * i] = regions[i].x;
            _region[2 * i + 1] = regions[i].y;
        }
        for (; i < 5; i++)
        {
            _region[2 * i] = -1;
            _region[2 * i + 1] = -1;
        } 

		_mat.SetFloat ("_visibleStart1", _region[0]);
		_mat.SetFloat ("_visibleEnd1", _region [1]);
		_mat.SetFloat ("_visibleStart2", _region[2]);
		_mat.SetFloat ("_visibleEnd2", _region [3]);
		_mat.SetFloat ("_visibleStart3", _region[4]);
		_mat.SetFloat ("_visibleEnd3", _region[5]);
		_mat.SetFloat ("_visibleStart4", _region[6]);
		_mat.SetFloat ("_visibleEnd4", _region[7]);
		_mat.SetFloat ("_visibleStart5", _region[8]);
		_mat.SetFloat ("_visibleEnd5", _region[9]);
	}
}
