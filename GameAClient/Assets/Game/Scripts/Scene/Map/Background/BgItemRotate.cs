///********************************************************************
//** Filename : BgItemRotate
//** Author : Dong
//** Date : 2016/12/1 星期四 下午 9:01:58
//** Summary : BgItemRotate
//***********************************************************************/

//using System;
//using System.Collections;
//using SoyEngine;
//using UnityEngine;
//using Random = UnityEngine.Random;

//namespace GameA.Game
//{
//    public class BgItemRotate : MonoBehaviour
//    {
//        [SerializeField]
//        private float _minIntervalTime = 5;
//        [SerializeField]
//        private float _maxIntervalTime = 18;
//        private Transform _trans;
//        private Table_Background _tableBg;
//        [SerializeField]
//        private float _currentAngle;
//        [SerializeField]
//        private float _finalAngle;

//        public void Init(Table_Background tableBackground)
//        {
//            _tableBg = tableBackground;
//            _trans = GetComponent<Transform>();
//            StartCoroutine("Rotate");
//        }

//        private IEnumerator Rotate()
//        {
//            while (true)
//            {
//                if (_trans == null || _tableBg == null)
//                {
//                    yield break;
//                }
//                yield return new WaitForSeconds(Random.Range(_minIntervalTime, _maxIntervalTime));
//                var v = Random.Range(0, 10);
//                _finalAngle = _currentAngle;
//                _finalAngle += v >= 5 ? _tableBg.RotateAngle : -_tableBg.RotateAngle;
//                while (true)
//                {
//                    _currentAngle = Util.ConstantLerp(_currentAngle, _finalAngle, _tableBg.RotateSpeed * ConstDefineGM2D.FixedDeltaTime);
//                    _trans.eulerAngles = new Vector3(0, 0, _currentAngle);
//                    if (Util.IsFloatEqual(_currentAngle, _finalAngle))
//                    {
//                        break;
//                    }
//                    yield return null;
//                }
//            }
//        }

//        private void OnDestroy()
//        {
//            StopCoroutine("Rotate");
//        }
//    }
//}
