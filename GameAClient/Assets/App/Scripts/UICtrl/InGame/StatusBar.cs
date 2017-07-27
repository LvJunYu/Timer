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

        private float hpValue=1;
        private float mpValue=1;

        public float HpValue
        {
            get { return hpValue; }
            set
            {
                hpValue = value;

                Hpfront.localScale = new Vector3(hpValue, 1);

                Hpfront.localPosition = new Vector3((1 - hpValue) * -0.8f, 0);
            }
        }

        public float MpValue
        {
            get { return mpValue; }
            set
            {
                mpValue = value;

                Mpfront.localScale = new Vector3(mpValue, 1);

                Mpfront.localPosition = new Vector3((1 - mpValue) * -0.8f, 0);
            }
        }
        private void OnHPChanged(int currentValue, int maxValue)
        {
            HpValue = currentValue/ maxValue;  
        }

        private void OnMPChanged(int currentValue, int maxValue)
        {
            MpValue = currentValue / maxValue;
        }

        void Start()
        {
            //Debug.Log("_______Hp_Mp__");
            Messenger<int, int>.AddListener(EMessengerType.OnHPChanged, OnHPChanged);
            Messenger<int, int>.AddListener(EMessengerType.OnMPChanged, OnMPChanged);
        }


    }

}
