using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4018, Type = typeof(RopeJoint))]
    public class RopeJoint : RigidbodyUnit
    {
        private const int MaxDis = 74;
        private UnitBase _preJoint;
        private UnitBase _nextJoint;
        private Rope _rope;
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
            SpeedY -= 2;
        }

        public override void UpdateView(float deltaTime)
        {
            _deltaPos = _speed;
            _curPos += _deltaPos;
            CheckPos();
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(_colliderPos);
            UpdateTransPos();
        }

        private void CheckPos()
        {
            if (_preJoint.Id == UnitDefine.RopeJointId)
            {
                var relativePos = GetNeighborRelativePos(_preJoint);
                if (relativePos.x > MaxDis)
                {
                    relativePos.x = MaxDis;
                    SpeedX = 0;
                }
                else if (relativePos.x < -MaxDis)
                {
                    relativePos.x = -MaxDis;
                    SpeedX = 0;
                }
                if (relativePos.y > MaxDis)
                {
                    relativePos.y = MaxDis;
                    SpeedY = 0;
                }
                else if (relativePos.y < -MaxDis)
                {
                    relativePos.y = -MaxDis;
                    SpeedY = 0;
                }
                CenterPos = _preJoint.CenterPos + relativePos;
            }
            //第一个物体固定在原物体下
            else
            {
                CenterUpFloorPos = _preJoint.CenterDownPos;
            }
        }

        public void Set(Rope rope)
        {
            _rope = rope;
        }

        private IntVec2 GetNeighborRelativePos(UnitBase unit)
        {
            if (unit == null) return IntVec2.zero;
            return CenterPos - unit.CenterPos;
        }

        public void SetPreJoint(UnitBase joint)
        {
            _preJoint = joint;
        }

        public void SetnNextJoint(UnitBase joint)
        {
            _nextJoint = joint;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer && other.IsAlive)
            {
                if (_colliderGrid.Intersects(new Grid2D(other.CenterPos, other.CenterPos)))
                {
                    ((PlayerBase) other).CheckRope(this);
                }
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

        public override bool OnLeftDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id || other.IsActor)
            {
                return false;
            }
            return base.OnLeftDownHit(other, ref x, ref y, checkOnly);
        }

        public override bool OnRightDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id || other.IsActor)
            {
                return false;
            }
            return base.OnRightDownHit(other, ref x, ref y, checkOnly);
        }

        protected override void Clear()
        {
            base.Clear();
            _preJoint = _nextJoint = null;
        }

        public void Transmit(IntVec2 acc)
        {
            Speed += acc;
        }
    }
}