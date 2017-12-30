using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4018, Type = typeof(RopeJoint))]
    public class RopeJoint : RigidbodyUnit
    {
        private const int MaxDis = 64;
        private const int AirPower = 4;
        private const int Gravity = 2;
        private const int ForcePower = 2;
        private UnitBase _preJoint;
        private RopeJoint _nextJoint;
        private Rope _rope;
        private WholeRope _wholeRope;
        private IntVec2 _oriPos;
        private int _jointIndex;
        private IntVec2 _acc;
        private bool _right;
        private int _maxSpeed = 100;

        private IntVec2 _preJointForce;
        private IntVec2 _nextJointForce;

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
            if (_nextJoint == null)
            {
                SpeedY -= 32;
                int airPower = AirPower - (_wholeRope.Length - 1 - _jointIndex);
                if (airPower > 0)
                {
                    SpeedX = Util.ConstantLerp(SpeedX, 0, airPower);
                }
            }

            SpeedY -= Gravity;
            if (_jumpOnTimer > 0)
            {
                AddForce(_jumpDir);
                _jumpOnTimer--;
            }
        }

        public override void UpdateView(float deltaTime)
        {
            _deltaPos = _speed;
            _curPos += _deltaPos;
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(_colliderPos);
            UpdateTransPos();
//            if (!_addForce)
//            {
//                var sqr = Speed.SqrMagnitude();
//                var resistance = Speed * (sqr / (sqr + AirPara));
//                SpeedX = Util.ConstantLerp(SpeedX, 0, Mathf.Abs(resistance.x));
//                SpeedY = Util.ConstantLerp(SpeedY, 0, Mathf.Abs(resistance.y));
//            }
//            SpeedX = Util.ConstantLerp(SpeedX, 0, AirPower);
//            bool right = _curPos.x > _oriPos.x;
//            if (_right != right)
//            {
//                if (SpeedX == 0)
//                {
//                    SpeedX = _oriPos.x - _curPos.x;
//                }
//                _right = right;
//            }
        }

        public void Set(Rope rope, int jointIndex, IntVec2 oriPos)
        {
            _rope = rope;
            _jointIndex = jointIndex;
            _oriPos = oriPos;
        }

        public void Set(WholeRope wholeRope)
        {
            _wholeRope = wholeRope;
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
//            SpeedX = Mathf.Clamp(SpeedX, -_maxSpeed, _maxSpeed);
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

        public void AddForce(IntVec2 forceDir)
        {
            RopeManager.Instance.AddForce(forceDir * ForcePower + IntVec2.down * ForcePower, _rope.RopeIndex,
                JointIndex);
        }

        public void CarryPlayer()
        {
            SpeedY -= 20;
        }

        private int _jumpOnTimer;
        private IntVec2 _jumpDir;

        public void JumpOnRope(EMoveDirection moveDirection)
        {
            _jumpOnTimer = 20;
            if (moveDirection == EMoveDirection.Left)
            {
                _jumpDir = IntVec2.left;
            }
            else
            {
                _jumpDir = IntVec2.right;
            }
        }
    }
}