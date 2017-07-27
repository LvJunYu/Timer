using System.Collections;
using System;
using SoyEngine.Proto;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{

    public class StatusBar: MonoBehaviour
    {
        /// <summary>
        ///获取目标物体的高度 
        /// </summary>
        
        public Transform Hpfront;
        public Transform Mpfront;

        private float _hpValue=1;
        private float _mpValue=1;

        public float HpValue
        {
            get { return _hpValue; }
            set
            {
                _hpValue = value;

                Hpfront.localScale = new Vector3(_hpValue, 1);

                Hpfront.localPosition = new Vector3((1 - _hpValue) * -0.8f, 0);
            }
        }

        public float MpValue
        {
            get { return _mpValue; }
            set
            {
                _mpValue = value;

                Mpfront.localScale = new Vector3(_mpValue, 1);

                Mpfront.localPosition = new Vector3((1 - _mpValue) * -0.8f, 0);
            }
        }
        private void OnHPChanged(int currentValue, int maxValue)
        {
            HpValue = currentValue/maxValue;  
        }

        private void OnMPChanged(int currentValue, int maxValue)
        {
            MpValue = currentValue/maxValue;
        }

        void Start()
        {
            //Debug.Log("_______Hp_Mp__");
            Messenger<int, int>.AddListener(EMessengerType.OnHPChanged, OnHPChanged);
            Messenger<int, int>.AddListener(EMessengerType.OnMPChanged, OnMPChanged);
        }


    }

}
