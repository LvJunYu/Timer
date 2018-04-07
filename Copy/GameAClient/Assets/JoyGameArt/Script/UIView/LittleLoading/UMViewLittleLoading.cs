  /********************************************************************
  ** Filename : UMViewLittleLoading.cs
  ** Author : quan
  ** Date : 2016/9/8 17:32
  ** Summary : UMViewLittleLoading.cs
  ***********************************************************************/

using System;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewLittleLoading : UMViewResManagedBase
    {
        public Image Image;
        public Text Text;
        public Sprite[] Frames;
        private const float SwitchInterval = 0.3f;
        private int _inx;
        private float _leftTime;

        public void ResetState()
        {
            _leftTime = SwitchInterval;
            _inx = 0;
            Image.sprite = Frames[_inx];
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
            if(_inx >= Frames.Length)
            {
                _inx = 0;
            }
            Image.sprite = Frames[_inx];
        }
    }
}
