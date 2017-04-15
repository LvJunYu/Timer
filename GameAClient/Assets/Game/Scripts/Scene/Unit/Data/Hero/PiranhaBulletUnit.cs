//using System;
//using SoyEngine;
//using UnityEngine;

//namespace GameA.Game
//{
//    [Serializable]
//    [Unit(Id = 5902, Type = typeof (PiranhaBulletUnit))]
//    public class PiranhaBulletUnit : UnitBase
//    {
//        protected IntVec2 _OrigPos;
//        protected IntVec2 _delta;
//        protected float _deltaLength;
//        protected IntVec2 _mainPlayerPos;
//        protected int _timerMagic;
//        protected int _velocity;

//        protected UnityNativeParticleItem _effect;

//        protected override bool OnInit(SceneNode colliderNode)
//        {
//            if (!base.OnInit(colliderNode))
//            {
//                return false;
//            }
//            _velocity = 40;
//            _mainPlayerPos = PlayMode.Instance.MainUnit.CenterPos;
//            _OrigPos = CenterPos;
//            _delta = _mainPlayerPos - _OrigPos;
//            _deltaLength = Mathf.Sqrt(_delta.x*_delta.x + _delta.y*_delta.y);
//            Speed = IntVec2.zero;
//            _timerMagic = 0;

//            if (_effect == null) {
//                _effect = GameParticleManager.Instance.GetUnityNativeParticleItem ("M1HuoQiuTuoWei", _trans);
//                //_effect.Trans.localPosition = Vector3.up * 0.155f;
//            }
//            return true;
//        }

//        internal override void Reset ()
//        {
//            base.Reset ();
//            _timerMagic = 0;
//            if (_effect != null) {
//                _effect.Stop ();
//            }
//        }

//        internal override void OnObjectDestroy ()
//        {
//            base.OnObjectDestroy ();
//            if (_effect != null) {
//                _effect.DestroySelf ();
//            }
//            _effect = null;
//        }

//        internal override void OnPlay ()
//        {
//            base.OnPlay ();
//            if (_effect != null) {
//                _effect.Play ();
//            }
//        }

//        protected override void OnDead()
//        {
//            if (!_isAlive)
//            {
//                return;
//            }
//            PlayMode.Instance.DestroyUnit(this);
//            GameParticleManager.Instance.Emit ("M1YanMoBeiJi", _trans.position);
//            base.OnDead();
//        }

//        public override void OnShootHit (UnitBase other)
//        {
//            OnDead ();
//        }

//        internal override bool OnHitEarth(UnitBase other, ERotationType eDirectionType)
//        {
//            OnDead();
//            return true;
//        }

//        public override void CheckStart()
//        {
//            _isStart = true;
//        }

//        public override void OnHit(UnitBase other)
//        {
//            if (other.IsMain) {
//                other.OnShootHit (this);
//            }
//        }

//        //public override void BeforeUpdatePhysics(float deltaTime)
//        //{
//        //    if (_isAlive)
//        //    {
//        //        _timerMagic++;
//        //        float t = (_timerMagic*_velocity)/_deltaLength;
//        //        IntVec2 p = _OrigPos + new IntVec2((int) (_delta.x*t), (int) (_delta.y*t));
//        //        CenterPos = p;
//        //    }
//        //}

//        public override void AfterUpdatePhysics(float deltaTime)
//        {
//            if (_isAlive)
//            {
//                if (!DataScene2D.Instance.ValidMapRect.Contains(_curPos))
//                {
//                    OnDead();
//                }
//            }
//        }
//    }
//}