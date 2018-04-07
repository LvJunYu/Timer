using System;
using GameA.Game;
using SoyEngine;
using UnityEngine;

public class ReviveEffect
{
    private UnityNativeParticleItem _particle;

    /// <summary>
    ///     复活特效飞到复活点后的回调
    /// </summary>
    private Action _callback;

    private Vector3 _curPos;

    /// <summary>
    ///     复活位置
    /// </summary>
    private Vector3 _endPos;

    /// <summary>
    ///     飞行时间
    /// </summary>
    private int _lifeTime;

    /// <summary>
    ///     最大缩放(飞行距离比较短时最大缩放也小)
    /// </summary>
    private float _maxScale = 1f;

    private bool _playing;

    /// <summary>
    ///     死亡位置
    /// </summary>
    private Vector3 _startPos;

    /// <summary>
    ///     计时器
    /// </summary>
    private int _timer;

    public Vector3 Position
    {
        get { return _curPos; }
    }

    public void Set(UnityNativeParticleItem particle)
    {
        _particle = particle;
    }

    public void Play(Vector3 startPos, Vector3 endPos, int speed, Action callback)
    {
        if (_particle != null)
        {
            _particle.Trans.position = startPos;
            _particle.Trans.localScale = Vector3.one;
            _particle.Play();
        }
        _startPos = new Vector3(startPos.x, startPos.y, 0);
        _curPos = _startPos;
        _endPos = endPos;
        _callback = callback;
        _timer = 0;
        float distance = (_startPos - endPos).magnitude;
        _maxScale = distance < 10f ? Mathf.Clamp01(1f - 1.3f * (10 - distance) * 0.1f) : 1f;
        _lifeTime = (int)(distance / (speed * ConstDefineGM2D.FixedDeltaTime));
        _lifeTime = Mathf.Max(1, _lifeTime);
        _playing = true;
    }

    public void Update()
    {
        if (!_playing)
        {
            return;
        }
        _timer++;
        if (_timer >= _lifeTime)
        {
            _playing = false;
            if (_callback != null)
            {
                _callback();
            }
            if (_particle != null)
            {
                _particle.Stop();
            }
            return;
        }
        _curPos = Vector3.Lerp(_startPos, _endPos, (float) _timer / _lifeTime);
        if (_particle != null)
        {
            float t = (float) _timer/_lifeTime;
            if (t < 0.25f)
            {
                _particle.Trans.localScale = Vector3.one*_maxScale*4f*t;
            }
            else if (t > 0.75f)
            {
                _particle.Trans.localScale = Vector3.one*_maxScale*(1 - t)*4f;
            }
            else
            {
                _particle.Trans.localScale = Vector3.one*_maxScale;
            }
            _particle.Trans.position = _curPos;
        }
    }

    public void Stop()
    {
        if (_particle != null)
        {
            _particle.Stop();
        }
    }

    public void Free()
    {
        if (_particle != null)
        {
            GameParticleManager.FreeParticleItem(_particle);
            _particle.Stop();
        }
    }
}