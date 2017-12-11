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
    public class Jelly : SkillBlock
    {
        public static int ExtraSpeedX = 180;
        public static int ExtraSpeedY = 264;

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

        protected override void CheckSkillHit(UnitBase other, Grid2D grid, EDirectionType eDirectionType)
        {
//            if (_colliderGrid.Intersects(grid))
            {
                PlayAnimation(other, eDirectionType);
                OnEffect(other, eDirectionType);
            }
        }

        private void PlayAnimation(UnitBase other, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                    if (other.SpeedY <= 0)
                    {
                        if (_animation != null)
                        {
                            _animation.PlayOnce("Up");
                        }
                    }
                    break;
                case EDirectionType.Down:
                    if (other.SpeedY >= 0)
                    {
                        if (_animation != null)
                        {
                            _animation.PlayOnce("Down");
                        }
                    }
                    break;
                case EDirectionType.Left:
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Left");
                    }
                    break;
                case EDirectionType.Right:
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Right");
                    }
                    break;
            }
        }

        public static void OnEffect(UnitBase other, EDirectionType eDirectionType)
        {
            if (other.IsInState(EEnvState.Ice))
            {
                return;
            }
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                    if (other.SpeedY <= 0)
                    {
                        other.SpeedY = 0;
                        if (other.IsPlayer)
                        {
                            ((PlayerBase) other).Step();
                            other.ExtraSpeed.y = ExtraSpeedY;
                            if (other.IsMain)
                            {
                                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Sping);
                            }
                        }
                        else if (other.IsMonster)
                        {
                            other.ExtraSpeed.y = ExtraSpeedY;
                        }
                    }
                    break;
                case EDirectionType.Down:
                    if (other.SpeedY >= 0)
                    {
                        other.SpeedY = 0;
                        if (other.IsPlayer)
                        {
                            other.ExtraSpeed.y = -ExtraSpeedY;
                            if (other.IsMain)
                            {
                                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Sping);
                            }
                        }
                        else if (other.IsMonster)
                        {
                            other.ExtraSpeed.y = -ExtraSpeedY;
                        }
                    }
                    break;
                case EDirectionType.Left:
                    other.Speed = IntVec2.zero;
                    other.ExtraSpeed.x = -ExtraSpeedX;
                    other.ExtraSpeed.y = ExtraSpeedX;
                    if (other.IsPlayer)
                    {
                        ((PlayerBase) other).Step();
                        if (other.IsMain)
                        {
                            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Sping);
                        }
                    }
                    break;
                case EDirectionType.Right:
                    other.Speed = IntVec2.zero;
                    other.ExtraSpeed.x = ExtraSpeedX;
                    other.ExtraSpeed.y = ExtraSpeedX;
                    if (other.IsMain)
                    {
                        ((PlayerBase) other).Step();
                        if (other.IsMain)
                        {
                            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Sping);
                        }
                    }
                    break;
            }
        }
    }
}