/********************************************************************
** Filename : GridCheck
** Author : Dong
** Date : 2017/1/13 星期五 下午 3:58:22
** Summary : GridCheck
***********************************************************************/

namespace GameA.Game
{
    public struct GridCheck
    {
        private UnitBase _self;
        private SwitchTriggerPress _hit;
        private bool _isStay;

        public GridCheck(UnitBase self)
        {
            _self = self;
            _hit = null;
            _isStay = false;
        }

        public void Clear()
        {
            After();
            _hit = null;
            _isStay = false;
        }

        public void Before()
        {
            _isStay = false;
        }

        public void Do(SwitchTriggerPress hit)
        {
            if (hit != null)
            {
                _isStay = true;
                if (_hit == null)
                {
                    _hit = hit;
                    _hit.OnGridCheckEnter(_self);
                }
            }
            else
            {
                if (_hit != null)
                {
                    _hit.OnGridCheckExit(_self);
                    _hit = null;
                }
            }
        }

        public void After()
        {
            if (!_isStay)
            {
                Do(null);
            }
        }

        public void OnDestroySelf(UnitBase unit)
        {
            if (_self == unit)
            {
                Do(null);
            }
        }
    }
}
