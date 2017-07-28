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
        ///目标物体的高度 
        /// </summary>
        private float _targetHeight;
        /// <summary>
        ///蓝条能量数 
        /// </summary>
        private float _energyGrid;
        public Transform Hpfront;
        public Transform Mpfront;
        public Transform SuspensionPoint;
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

        public void SetMp(bool mpVisible)
        {
            Mpfront.gameObject.SetActiveEx(mpVisible);
        }

        public void SetHp(bool hpVisible)
        {
            Mpfront.gameObject.SetActiveEx(hpVisible);
        }

        /// <summary>
        ///能量条闪烁 
        /// </summary>
        public void EnergyFlash()
        {
            
        }

        /// <summary>
        ///能量减少 
        /// </summary>
        public void UseEnergy(int consumptionBySkill)
        {

        }

        /// <summary>
        ///能量回复 
        /// </summary>
        public void EnergyRecover(float percentagePerFrame)
        {

        }

        public void SetBarPosition(float Hight)
        {
            SuspensionPoint.localPosition= new Vector3(0, Hight, 0);
        }


    }

}
