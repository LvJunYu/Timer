/********************************************************************
  ** Filename : UIImageFrameAnimation.cs
  ** Author : quan
  ** Date : 11/14/2016 10:52 PM
  ** Summary : UIImageFrameAnimation.cs
  ***********************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SoyEngine
{
    public class UIImageFrameAnimation : MonoBehaviour
    {
        [SerializeField]
        private Image _content;
        [SerializeField]
        private Sprite[] _frameSpriteAry;
        [SerializeField]
        private float _frameTime = 0.3f;
        private int _animationInx;
        private float _animationFrameLeftTime;

        private void Awake()
        {
            ResetState();
        }

        private void OnEnable()
        {
            ResetState();
        }
        private void ResetState()
        {
            _frameTime = Mathf.Max(0.02f, _frameTime);
            _animationInx = 0;
            _animationFrameLeftTime = _frameTime;
            if(_frameSpriteAry.Length > 0)
            {
                _content.sprite = _frameSpriteAry[0];
            }
        }

        private void Update()
        {
            _animationFrameLeftTime -= Time.deltaTime;
            if (_animationFrameLeftTime <= 0 && _frameSpriteAry.Length > 1)
            {
                _animationFrameLeftTime = _frameTime;
                _animationInx = (_animationInx + 1) % _frameSpriteAry.Length;
                _content.sprite = _frameSpriteAry[_animationInx];
            }
        }
    }
}
