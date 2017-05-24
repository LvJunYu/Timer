/********************************************************************
** Filename : BrickUnit
** Author : Dong
** Date : 2016/10/20 星期四 上午 11:52:39
** Summary : BrickUnit
***********************************************************************/

using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 5013, Type = typeof (Gate))]
    public class Gate : BlockBase
    {
        private bool _opened;

        protected override void Clear()
        {
            base.Clear();
            if (_opened)
            {
                if (_view != null)
                {
                    _view.ChangeView(_tableUnit.Model);
                }
                _opened = false;
            }
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
                if (other.IsMain)
                {
                    if (_opened == false && PlayMode.Instance.SceneState.UseKey())
                    {
                        _opened = true;
                        SetEnabled(false);
                        if (_view != null)
                        {
                            _view.ChangeView(_tableUnit.Model + "_1");
                        }
                    }
                }
            }
        }
    }
}