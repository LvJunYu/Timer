/********************************************************************
** Filename : GuideboardUnit
** Author : Dong
** Date : 2016/11/6 星期日 下午 2:25:30
** Summary : GuideboardUnit
***********************************************************************/

using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 7002, Type = typeof (GuideboardUnit))]
    public class GuideboardUnit : UnitBase
    {
        private SkeletonAnimation _animation;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _animation = _trans.GetComponent<SkeletonAnimation>();
            return true;
        }

        internal override void OnPlay()
        {
            _animation.state.SetAnimation(0, "Run", true);
            base.OnPlay();
        }

        internal override void Reset()
        {
            _animation.Reset();
            base.Reset();
        }
    }
}