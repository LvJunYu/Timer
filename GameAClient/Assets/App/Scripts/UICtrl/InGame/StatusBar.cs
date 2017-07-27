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

        public Transform Hpfront;
        public Transform Mpfront;

        private float Hp_value=1;
        private float Mp_value=1;

        public float HpValue
        {
            get { return Hp_value; }
            set
            {
                Hp_value = value;

                Hpfront.localScale = new Vector3(Hp_value, 1);

                Hpfront.localPosition = new Vector3((1 - Hp_value) * -0.8f, 0);
            }
        }

        public float MpValue
        {
            get { return Mp_value; }
            set
            {
                Mp_value = value;

                Mpfront.localScale = new Vector3(Mp_value, 1);

                Mpfront.localPosition = new Vector3((1 - Mp_value) * -0.8f, 0);
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
            Messenger<int, int>.AddListener(EMessengerType.OnHPChanged, OnHPChanged);
            Messenger<int, int>.AddListener(EMessengerType.OnMPChanged, OnMPChanged);
        }


    }

}
