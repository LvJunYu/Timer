using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4018, Type = typeof(RopeJoint))]
    public class RopeJoint : RigidbodyUnit
    {
        private const int MaxDis = 64;
        private const int MaxSpeed = 400;
        private const int Gravity = 2;
        private const int ForcePower = 12;
        private UnitBase _preJoint;
        private RopeJoint _nextJoint;
        private Rope _rope;
        private WholeRope _wholeRope;
        private int _jointIndex;
        private IntVec2 _expectPos;
        private int _jumpOnTimer;
        private int _jumpAwayTimer;
        private IntVec2 _jumpDir;
        private IntVec2 _jumpAwayDir;

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

        public int RopeIndex
        {
            get { return _rope.RopeIndex; }
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
            if (_nextJoint == null)
            {
                var pos = CenterPos - _wholeRope.TieUnit.CenterDownPos;
                SpeedY -= 32;
                if (Mathf.Abs(pos.x) > 8 * _wholeRope.Length)
                {
                    SpeedX += Mathf.Clamp(32 * pos.x / Mathf.Max(1, Mathf.Abs(pos.y)), -32, 32);
                }
            }

//            int airPower = _jointIndex * AirPower / _wholeRope.Length;
//            if (airPower > 0)
//            {
//                SpeedX = Util.ConstantLerp(SpeedX, 0, airPower);
//            }

            //跳上绳子时的冲力
            if (_jumpOnTimer > 0)
            {
                AddForce(_jumpDir * ForcePower);
                _jumpOnTimer--;
            }

            //跳下时的弹力
            if (_jumpAwayTimer > 0)
            {
                AddForce(-1 * _jumpAwayDir * ForcePower + IntVec2.down * ForcePower);
                _jumpAwayTimer--;
            }

            SpeedX = Mathf.Clamp(SpeedX, -MaxSpeed, MaxSpeed);
            SpeedY = Mathf.Clamp(SpeedY, -MaxSpeed, MaxSpeed);
        }

        public override void UpdateView(float deltaTime)
        {
            _deltaPos = _speed;
            _curPos += _deltaPos;
            _expectPos = _curPos;
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(_colliderPos);
            UpdateTransPos();
            //如果发生碰撞导致预期位置变化，则调整后面关节的速度
            if (_expectPos != _curPos && _nextJoint != null)
            {
                _nextJoint.FixSpeedFromPre(true);
            }
        }

        protected override float GetZ()
        {
            if (_preJoint != null && _preJoint.View != null && _preJoint.Id == UnitDefine.RopeJointId)
            {
                return _preJoint.Trans.position.z - 0.01f;
            }

            return base.GetZ();
        }

        public void Set(Rope rope, int jointIndex, IntVec2 oriPos)
        {
            _rope = rope;
            _jointIndex = jointIndex;
//            _oriPos = oriPos;
        }

        public void Set(WholeRope wholeRope)
        {
            _wholeRope = wholeRope;
        }

        public IntVec2 GetNeighborRelativePos(bool pre)
        {
            if (pre)
            {
                if (_jointIndex == 0) return IntVec2.zero;
                return _preJoint.CenterDownPos - CenterDownPos;
            }

            if (_nextJoint == null) return IntVec2.zero;
            return _nextJoint.CenterDownPos - CenterDownPos;
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

        public void FixSpeedFromPre(bool afterMove = false)
        {
            var target = PreJoint.CurPos;
            if (!afterMove)
            {
                target += PreJoint.Speed;
            }

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

        public void DragRope(IntVec2 dir)
        {
            int force = ForcePower / 2 + ForcePower * (1 + _jointIndex) / _wholeRope.Length / 2;
            AddForce(force * dir);
        }

        public void AddForce(IntVec2 force)
        {
            _wholeRope.AddForce(force, _jointIndex);
        }

        public void CarryPlayer()
        {
//            SpeedY -= 20;
        }

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

        public void JumpAwayRope(EMoveDirection moveDirection)
        {
            _jumpAwayTimer = 20;
            if (moveDirection == EMoveDirection.Left)
            {
                _jumpAwayDir = IntVec2.left;
            }
            else
            {
                _jumpAwayDir = IntVec2.right;
            }
        }
    }
}