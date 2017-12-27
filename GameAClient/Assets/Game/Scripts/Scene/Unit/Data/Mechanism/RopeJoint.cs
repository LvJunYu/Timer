using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4018, Type = typeof(RopeJoint))]
    public class RopeJoint : RigidbodyUnit
    {
        private const int MaxOffset = 10;
        private const int MaxSpeed = 200;
        private UnitBase _preJoint;
        private UnitBase _nextJoint;
        private Vector2 _preJointOffset;
        private Vector2 _nextJointOffset;
        private IntVec2 _preJointForce;
        private IntVec2 _nextJointForce;

        public override void UpdateLogic()
        {
            CaculateGravity();
            _preJointForce = GetNeighborForce(_preJoint, _preJointOffset);
            _nextJointForce = GetNeighborForce(_nextJoint, _nextJointOffset);
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

        private IntVec2 GetNeighborForce(UnitBase unit, Vector2 offset)
        {
            if (unit == null) return IntVec2.zero;
            var relativePos = GM2DTools.WorldToTile((Vector2)unit.View.Trans.position - (Vector2)_view.Trans.position + offset);
            return relativePos;
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

        protected override void Clear()
        {
            base.Clear();
            _preJoint = _nextJoint = null;
        }
    }
}