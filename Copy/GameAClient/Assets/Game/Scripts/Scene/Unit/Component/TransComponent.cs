using SoyEngine;

namespace GameA.Game
{
    public class TransComponent : UnitComponent
    {
        public IntVec2 CurPos;
        public IntVec2 Speed;
        public IntVec2 DeltaPos;
        public IntVec2 ExtraDeltaPos;
        public IntVec2 DeltaImpactPos;
        public IntVec2 LastExtraDeltaPos;
        protected ColliderComponent _colliderCom;
        protected IntVec2 _dataSize;

        public int SpeedX
        {
            get { return Speed.x; }
            set { Speed.x = value; }
        }

        public int SpeedY
        {
            get { return Speed.y; }
            set { Speed.y = value; }
        }

        public int DeltaPosX
        {
            get { return DeltaPos.x; }
            set { DeltaPos.x = value; }
        }

        public int DeltaPosY
        {
            get { return DeltaPos.y; }
            set { DeltaPos.y = value; }
        }

        public IntVec2 CenterDownPos
        {
            get { return new IntVec2(CurPos.x + _dataSize.x / 2, CurPos.y); }
            set { CurPos = new IntVec2(value.x - _dataSize.x / 2, value.y); }
        }

        public IntVec2 CenterPos
        {
            get { return new IntVec2(CurPos.x + _dataSize.x / 2, CurPos.y + _dataSize.y / 2); }
            set { CurPos = new IntVec2(value.x - _dataSize.x / 2, value.y - _dataSize.y / 2); }
        }

        public IntVec2 CenterUpFloorPos
        {
            get { return new IntVec2(CurPos.x + _dataSize.x / 2, CurPos.y + _dataSize.y + 1); }
            set { CurPos = new IntVec2(value.x - _dataSize.x / 2, value.y - _dataSize.y - 1); }
        }

        public IntVec2 CenterLeftPos
        {
            get { return new IntVec2(CurPos.x, CurPos.y + _dataSize.x / 2); }
        }

        public IntVec2 CenterRightPos
        {
            get { return new IntVec2(CurPos.x + _dataSize.x, CurPos.y + _dataSize.y / 2); }
        }

        public IntVec2 CenterUpPos
        {
            get { return new IntVec2(CurPos.x + _dataSize.x / 2, CurPos.y + _dataSize.y); }
        }

        public override void Reset()
        {
            base.Reset();
            CurPos = new IntVec2(Unit.Guid.x, Unit.Guid.y);
            Unit.UpdateTransPos(CurPos);
        }

        public override void Clear()
        {
            base.Clear();
            Speed = IntVec2.zero;
            DeltaPos = ExtraDeltaPos = IntVec2.zero;
            DeltaImpactPos = LastExtraDeltaPos = IntVec2.zero;
        }

        public void UpdatePos()
        {
            DeltaPos = Speed + ExtraDeltaPos;
            CurPos += DeltaPos;
            _colliderCom.UpdateCollider(Unit.GetColliderPos(CurPos));
            CurPos = Unit.GetPos(_colliderCom.ColliderPos);
            Unit.UpdateTransPos(CurPos);
            if (GameModeBase.DebugEnable())
            {
                GameModeBase.WriteDebugData(string.Format("Type = {2}, Guid == {0} _trans.position = {1} ",
                    Unit.Guid, Unit.Trans.position, Unit.GetType().Name));
            }
        }

        public override void Dispose()
        {
            _colliderCom = null;
            base.Dispose();
        }

        public override void Awake()
        {
            base.Awake();
            _colliderCom = Unit.GetComponent<ColliderComponent>();
            _dataSize = Unit.GetDataSize();
            CurPos = new IntVec2(Unit.Guid.x, Unit.Guid.y);
        }
    }
}