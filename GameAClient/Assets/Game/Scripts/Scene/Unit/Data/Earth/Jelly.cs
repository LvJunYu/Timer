/********************************************************************
** Filename : JellyUnit
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:11:40
** Summary : Jelly
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4012, Type = typeof(Jelly))]
    public class Jelly : BlockBase
    {
        public static int ExtraSpeedX = 180;
        public static int ExtraSpeedY = 240;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _useCorner = false;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Run");
            return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.SpeedY <= 0)
                {
                    other.SpeedY = 0;
                    if (other.IsMain)
                    {
                        PlayMode.Instance.MainUnit.Step(80);
                        other.ExtraSpeed.y = ExtraSpeedY;
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSpingEffect);
                    }
                    else if (other.IsMonster)
                    {
                        other.ExtraSpeed.y = ExtraSpeedY;
                    }
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Up");
                    }
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.SpeedY >= 0)
                {
                    other.SpeedY = 0;
                    if (other.IsMain)
                    {
                        other.ExtraSpeed.y = -ExtraSpeedY;
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSpingEffect);
                    }
                    else if (other.IsMonster)
                    {
                        other.ExtraSpeed.y = -ExtraSpeedY;
                    }
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Down");
                    }
                }
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsHero)
                {
                    other.Speed = IntVec2.zero;
                    other.ExtraSpeed.x = -ExtraSpeedX;
                    other.ExtraSpeed.y = ExtraSpeedX;
                    other.CurBanInputTime = 20;
                    if (other.IsMain)
                    {
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSpingEffect);
                    }
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Left");
                    }
                }
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsHero)
                {
                    other.Speed = IntVec2.zero;
                    other.ExtraSpeed.x = ExtraSpeedX;
                    other.ExtraSpeed.y = ExtraSpeedX;
                    other.CurBanInputTime = 20;
                    if (other.IsMain)
                    {
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSpingEffect);
                    }
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Right");
                    }
                }
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }
    }
}
