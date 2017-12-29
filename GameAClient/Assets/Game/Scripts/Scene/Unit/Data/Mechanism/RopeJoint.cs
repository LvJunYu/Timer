using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4018, Type = typeof(RopeJoint))]
    public class RopeJoint : RigidbodyUnit
    {
        private const int MaxDis = 64;
        private const int MaxSpeed = 200;
        private const float AirPara = 1000f;
        private const int Gravity = 10;
        private const int ForcePower = 4;
        private UnitBase _preJoint;
        private RopeJoint _nextJoint;
        private Rope _rope;
        private int _jointIndex;
        private IntVec2 _acc;

        private IntVec2 _preJointForce;
        private IntVec2 _nextJointForce;

        public int MaxHeight
        {
            get { return RopeManager.Instance.GetMaxHeight(_rope.RopeIndex); }
        }

        public int MinHeight
        {
            get { return RopeManager.Instance.GetMinHeight(_rope.RopeIndex); }
        }

        public UnitBase PreJoint
        {
            get { return _preJoint; }
        }

        public RopeJoint NextJoint
        {
            get { return _nextJoint; }
        }

        public int JointIndex
        {
            get { return _jointIndex; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderBackground();
            return true;
        }

        public override void UpdateLogic()
        {
            SpeedY -= Gravity;
        }

        public override void UpdateView(float deltaTime)
        {
//            SpeedX = Mathf.Clamp(SpeedX, -MaxSpeed, MaxSpeed);
//            SpeedY = Mathf.Clamp(SpeedY, -MaxSpeed, MaxSpeed);
            _deltaPos = _speed;
            _curPos += _deltaPos;
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(_colliderPos);
            UpdateTransPos();
        }

        public void Set(Rope rope, int jointIndex)
        {
            _rope = rope;
            _jointIndex = jointIndex;
        }

        public IntVec2 GetNeighborRelativePos(bool pre)
        {
            if (pre)
            {
                if (JointIndex == 0) return IntVec2.zero;
                return PreJoint.CenterDownPos - CenterDownPos;
            }
            if (NextJoint == null) return IntVec2.zero;
            return NextJoint.CenterDownPos - CenterDownPos;
        }

        public void SetPreJoint(UnitBase joint)
        {
            _preJoint = joint;
        }

        public void SetnNextJoint(RopeJoint joint)
        {
            _nextJoint = joint;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer)
            {
                ((PlayerBase) other).CheckRope(this);
            }
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id || other.IsActor)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id || other.IsActor)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.Id == Id || other.IsActor)
            {
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.Id == Id || other.IsActor)
            {
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        protected override void Clear()
        {
            base.Clear();
            _preJoint = _nextJoint = null;
        }

        public void FixSpeedFromPre()
        {
            var target = PreJoint.CurPos + PreJoint.Speed;
            var from = CurPos + Speed;
            if (!CheckDis(ref from, ref target, false))
            {
                Speed = from - CurPos;
                if (NextJoint != null)
                {
                    NextJoint.FixSpeedFromPre();
                }
            }
        }

        public void CheckPos()
        {
//            if (!_addForce)
//            {
//                var sqr = Speed.SqrMagnitude();
//                var resistance = Speed * (sqr / (sqr + AirPara));
//                SpeedX = Util.ConstantLerp(SpeedX, 0, Mathf.Abs(resistance.x));
//                SpeedY = Util.ConstantLerp(SpeedY, 0, Mathf.Abs(resistance.y));
//            }
            if (PreJoint.Id == UnitDefine.RopeJointId)
            {
                var target = PreJoint.CurPos + PreJoint.Speed;
                var from = CurPos + Speed;
                if (!CheckDis(ref from, ref target))
                {
                    PreJoint.Speed = target - PreJoint.CurPos;
                }
            }
            //第一个物体固定在原物体下
            else
            {
                Speed = PreJoint.CenterDownPos - CenterUpFloorPos;
                if (NextJoint != null)
                {
                    NextJoint.FixSpeedFromPre();
                }
            }
        }

        public bool CheckDis(ref IntVec2 from, ref IntVec2 target, bool changeTarget = true)
        {
            var relative = target - from;
            var sqr = relative.SqrMagnitude();
            if (sqr > MaxDis * MaxDis)
            {
                var distance = Mathf.Pow(sqr, 0.5f);
                var delta = 1 - MaxDis / distance;
                if (changeTarget)
                {
                    target -= relative * delta;
                }
                else
                {
                    from += relative * delta;
                }
                return false;
            }
            return true;
        }

        public static bool _addForce;

        public void AddForce(IntVec2 forceDir)
        {
            RopeManager.Instance.Transmit(forceDir * ForcePower, _rope.RopeIndex, JointIndex);
            _addForce = true;
        }
    }
}