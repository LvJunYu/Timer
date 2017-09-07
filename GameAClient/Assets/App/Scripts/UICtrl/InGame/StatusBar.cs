﻿using System.Collections;
using System;
using SoyEngine.Proto;
using System.Collections.Generic;
using System.Net;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class StatusBar : MonoBehaviour
    {
//        public int TestCurrent = 100;
//        public int TestMax = 100;
//        public int TestMPGrids = 10;

        void Awake()
        {
            _trans = transform;
            var srs = _trans.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].sortingOrder = (int) ESortingOrder.DragingItem;
            }
            SetHPActive(false);
        }

        void Update()
        {
            _trans.rotation = Quaternion.identity;
//            if (Input.GetKeyDown(KeyCode.H))
//            {
//                TestCurrent -= UnityEngine.Random.Range(5, 10);
//                SetHP(EHPModifyCase.Hit, TestCurrent, TestMax);
//            }
//            if (Input.GetKeyDown(KeyCode.J))
//            {
//                TestCurrent += UnityEngine.Random.Range(5, 10);
//                SetHP(EHPModifyCase.Heal, TestCurrent, TestMax);
//            }
//            if (Input.GetKeyDown(KeyCode.G))
//            {
//                SetMPGrids(TestMPGrids);
//            }
//            if (Input.GetKeyDown(KeyCode.M))
//            {
//                SetMP(TestCurrent, TestMax);
//            }
            switch (_hpState)
            {
                case EHPShowState.Normal:
                    break;
                case EHPShowState.BeingHit:
                    HPBeforeHitRenderer.color = new Color(1f, 1f, 1f, _stateTimer / s_hittingTime);
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer < 0)
                    {
                        _hpState = EHPShowState.Normal;
                        HPBeforeHitTrans.gameObject.SetActive(false);
                    }
                    break;
                case EHPShowState.beingHeal:
                    HPAfterHealRenderer.color = new Color(0f, 1f, 0f, _stateTimer / s_healingTime);
                    _stateTimer -= Time.deltaTime;
                    if (_stateTimer < 0)
                    {
                        _hpState = EHPShowState.Normal;
                        HPAfterHealTrans.gameObject.SetActive(false);
                    }
                    break;
            }
        }

        #region fields

        /// <summary>
        /// 被攻击状态显示持续时间
        /// </summary>
        private static float s_hittingTime = 0.5f;

        /// <summary>
        /// 被治疗状态显示持续时间
        /// </summary>
        private static float s_healingTime = 0.3f;

        private Transform _trans;
        public GameObject HPRoot;

        public Transform CurrentHPTrans;
        public Transform HPBeforeHitTrans;
        public SpriteRenderer HPBeforeHitRenderer;
        public Transform HPAfterHealTrans;
        public SpriteRenderer HPAfterHealRenderer;

        /// <summary>
        ///目标物体的高度 
        /// </summary>
        private float _targetHeight;

        private float _hpPerccentage = 1;

        /// <summary>
        /// 当前hp显示状态
        /// </summary>
        [SerializeField] private EHPShowState _hpState;

        [SerializeField] private float _stateTimer;

        #endregion

        #region properties

        private float HpPerccentage
        {
            set
            {
                _hpPerccentage = value;
                CurrentHPTrans.localScale = new Vector3(_hpPerccentage, 1, 1);
            }
        }

        #endregion

        #region methods

        public void SetHPActive(bool value)
        {
            HPRoot.SetActive(value);
        }

        public void SetHP(EHPModifyCase modifyCase, int current, int max)
        {
            current = Mathf.Clamp(current, 0, max);
            switch (modifyCase)
            {
                case EHPModifyCase.Set:
                    _hpState = EHPShowState.Normal;
                    break;
                case EHPModifyCase.Hit:
                    _stateTimer = s_hittingTime;
                    if (EHPShowState.BeingHit != _hpState)
                    {
                        _hpState = EHPShowState.BeingHit;
                        HPBeforeHitTrans.localScale = new Vector3(_hpPerccentage, 1, 1);
                        HPBeforeHitTrans.gameObject.SetActive(true);
                        HPAfterHealTrans.gameObject.SetActive(false);
                        HPBeforeHitRenderer.color = Color.white;
                    }
                    break;
                case EHPModifyCase.Heal:
                    _stateTimer = s_healingTime;
                    if (EHPShowState.beingHeal != _hpState)
                    {
                        _hpState = EHPShowState.beingHeal;
                        HPAfterHealTrans.gameObject.SetActive(true);
                        HPBeforeHitTrans.gameObject.SetActive(false);
                        HPAfterHealRenderer.color = Color.green;
                    }
                    float healingPercentage = current / (float) max - _hpPerccentage;
                    // 因为healing是放在current下的子物体，所以缩放要除以父物体的缩放
                    HPAfterHealTrans.localScale = new Vector3(healingPercentage * max / (float) current, 1, 1);
                    break;
            }
            HpPerccentage = current / (float) max;
        }

        #endregion
    }

    /// <summary>
    /// HP显示状态
    /// </summary>
    public enum EHPShowState
    {
        Normal, // 正常
        BeingHit, // 被攻击中
        beingHeal, // 被治疗中
    }

    /// <summary>
    /// HP设置原因
    /// </summary>
    public enum EHPModifyCase
    {
        Set, // 指定
        Hit, // 被攻击
        Heal, // 被治疗
    }
}