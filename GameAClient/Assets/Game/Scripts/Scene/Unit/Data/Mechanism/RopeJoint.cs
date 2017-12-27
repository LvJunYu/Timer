using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4018, Type = typeof(RopeJoint))]
    public class RopeJoint : RigidbodyUnit
    {
        private const float ForceNormal = 0.1f;
        private const float ForceHard = 0.2f;
        private const float ForceMost = 0.3f;
        private const int AreaNormal = 2;
        private const int AreaHard = 4;
        private const int AreaMost = 6;
        private const int MaxSpeed = 10;
        private UnitBase _preJoint;
        private UnitBase _nextJoint;
        private Vector2 _preJointOffset;
        private Vector2 _nextJointOffset;
        private IntVec2 _preJointForce;
        private IntVec2 _nextJointForce;

        public override void UpdateLogic()
        {
            Speed = IntVec2.zero;
            CaculateGravity();
            _preJointForce = GetNeighborRelativePos(_preJoint, _preJointOffset, true);
            _nextJointForce = GetNeighborRelativePos(_nextJoint, _nextJointOffset, false);
            Speed += _preJointForce;
            Speed += _nextJointForce;

            SpeedX = Mathf.Clamp(SpeedX, -MaxSpeed, MaxSpeed);
            SpeedY = Mathf.Clamp(SpeedY, -MaxSpeed, MaxSpeed);
        }

        public override void UpdateView(float deltaTime)
        {
            _deltaPos = _speed;
            _curPos += _deltaPos;
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(_colliderPos);
            UpdateTransPos();
        }

        private IntVec2 GetNeighborRelativePos(UnitBase unit, Vector2 offset, bool isPre)
        {
            if (unit == null) return IntVec2.zero;
            var relativePos =
                GM2DTools.WorldToTile((Vector2) unit.View.Trans.position - (Vector2) _view.Trans.position + offset);
            float force;
            if (relativePos.SqrMagnitude() < AreaNormal * AreaNormal)
            {
                force = ForceNormal;
            }
            else if (relativePos.SqrMagnitude() < AreaHard * AreaHard)
            {
                force = ForceHard;
            }
            else
            {
                force = ForceMost;
            }
//            if (isPre)
//            {
//                force += 0.3f;
//            }
            return relativePos * force;
        }

        public void SetPreJoint(UnitBase joint)
        {
            _preJoint = joint;
            _preJointOffset = Trans.position - joint.Trans.position;
        }

        public void SetnNextJoint(UnitBase joint)
        {
            _nextJoint = joint;
            _nextJointOffset = Trans.position - joint.Trans.position;
        }

        protected override void CaculateGravity()
        {
            if (_nextJoint == null)
            {
                SpeedY -= 2;
            }
        }
        
        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id)
            {
                return false;
            }
            return base.OnLeftDownHit(other, ref x, ref y, checkOnly);
        }

        public override bool OnRightDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (other.Id == Id)
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
    }
}