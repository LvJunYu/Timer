namespace GameA.Game
{
    public class ActiveBlockComponent : BlockComponent
    {
        protected EActiveState _curActiveState;

        public EActiveState CurActiveState
        {
            set
            {
                if (_curActiveState == value)
                {
                    return;
                }

                _curActiveState = value;
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }

            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }

            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }

            return base.OnLeftHit(other, ref x, checkOnly);
        }
    }
}