using GameA.Game;
using SoyEngine;
using UnityEngine;
using System.Collections;

public class ReviveEffect {

    // 死亡位置
    private Vector3 _startPos;
    // 复活位置
    private Vector3 _endPos;
    private Vector3 _curPos;
    // 复活特效飞到复活点后的回调
    private System.Action _callback;

    // 飞行时间
    private int _lifeTime;
    // 计时器
    private int _timer;

    // 最大缩放(飞行距离比较短时最大缩放也小)
    private float _maxScale = 1f;

    private SoyEngine.UnityNativeParticleItem _particle;
    private bool _playing = false;

    public Vector3 Position
    {
        get {
            return _curPos;
        }
    }

    public ReviveEffect (UnityNativeParticleItem particle)
    {
        _particle = particle;
    }

	// Update is called once per frame
	public void Update () {
        //Debug.Log ("ReviveEffect timer: " +_timer + transform.position);
        if (!_playing) return;
        _timer++;
        if (_timer > _lifeTime) {
            if (_callback != null) {
                _callback ();
                if (_particle != null)
                {
                    _particle.Stop();
                }
                _playing = false;
                return;
            }
            _timer = _lifeTime;
        }
        _curPos = Vector3.Lerp(_startPos, _endPos, (float)_timer / _lifeTime);
        if (_particle != null)
        {
            float t = (float)_timer / _lifeTime;
            if (t < 0.25f)
            {
                _particle.Trans.localScale = Vector3.one * _maxScale *
                    4f * t;
            }
            else if (t > 0.75f)
            {
                _particle.Trans.localScale = Vector3.one * _maxScale *
                    (1 - t) * 4f;
            }
            else
            {
                _particle.Trans.localScale = Vector3.one * _maxScale;
            }
            //transform.position = _curve.GetPointAtTime (_timer / _lifeTime);
            _particle.Trans.position = _curPos;
        }
	}

    /// <summary>
    /// Init the specified lifeTime, startPos, endPos and callback.
    /// </summary>
    /// <param name="lifeTime">最少一秒钟</param>
    /// <param name="startPos">Start position.</param>
    /// <param name="endPos">UpdateEnd position.</param>
    /// <param name="callback">Callback.</param>
    public void Play (Vector3 startPos, Vector3 endPos, int speed, System.Action callback)
    {
        if (_particle != null)
        {
            _particle.Trans.position = startPos;
            _particle.Trans.localScale = Vector3.one;
            _particle.Play();
        }
        _startPos = startPos;
        _curPos = _startPos;
        _endPos = endPos;
        _callback = callback;
        _timer = 0;
        float distance = (startPos - endPos).magnitude;
        if (distance < 10f) {
            _maxScale = Mathf.Clamp01( 1f - 1.3f * (10 - distance) * 0.1f );
        } else {
            _maxScale = 1f;
        }
        _lifeTime = (int)(distance * speed);
        _lifeTime = Mathf.Max(1, _lifeTime);
        //
        //controlPoint1 = startPos + (endPos - startPos) * Random.Range(-0.25f, 0.75f) + Vector3.up * distance * Random.Range(0.1f, 0.7f);
        //controlPoint2 = startPos + (endPos - startPos) * Random.Range (0.25f, 1.25f) + Vector3.down * distance * Random.Range (0.1f, 0.7f);
        //_curve = new SoyEngine.Bezier (startPos, controlPoint2, controlPoint1, endPos);
        _playing = true;
    }

    public void Destroy ()
    {
        if (_particle != null)
        {
            _particle.DestroySelf();
        }
    }

    void OnDrawGizmos ()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine (_startPos, _endPos);
    }
}
