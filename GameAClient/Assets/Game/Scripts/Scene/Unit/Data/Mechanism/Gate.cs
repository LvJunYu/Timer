/********************************************************************
** Filename : BrickUnit
** Author : Dong
** Date : 2016/10/20 星期四 上午 11:52:39
** Summary : BrickUnit
***********************************************************************/

namespace GameA.Game
{
    [Unit(Id = 5013, Type = typeof(Gate))]
    public class Gate : BlockBase
    {
        private bool _opened;
        protected int _timer;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (_opened)
            {
                if (_animation != null)
                {
                    var entry = _animation.PlayOnce("Open");
                    entry.time = entry.endTime;
                }

                SetCross(true);
                SetSortingOrderBackground();
                UpdateTransPos();
            }

            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            _opened = false;
            SetCross(false);
            SetSortingOrderNormal();
            UpdateTransPos();
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            CheckOpen(other, checkOnly);
            if (_opened)
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            CheckOpen(other, checkOnly);
            if (_opened)
            {
                return false;
            }

            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            CheckOpen(other, checkOnly);
            if (_opened)
            {
                return false;
            }

            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            CheckOpen(other, checkOnly);
            if (_opened)
            {
                return false;
            }

            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void CheckOpen(UnitBase other, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsPlayer)
                {
                    if (_enabled && PlayMode.Instance.SceneState.UseKey(other as PlayerBase))
                    {
                        SetEnabled(false);
                        _timer = 50;
                        if (_animation != null)
                        {
                            _animation.PlayOnce("Open");
                        }
                        Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.OnControlFinish(_guid);
//                        RpgTaskManger.Instance.OnControlFinish(_guid);
                    }
                }
            }
        }

        public void DirectOpen()
        {
            SetEnabled(false);
            _timer = 50;
            if (_animation != null)
            {
                _animation.PlayOnce("Open");
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
                if (_timer == 0)
                {
                    _opened = true;
                    SetCross(true);
                    SetSortingOrderBackground();
                    UpdateTransPos();
                }
            }
        }
    }
}