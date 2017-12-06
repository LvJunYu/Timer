/********************************************************************
** Filename : Lazer
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:45:41
** Summary : Lazer
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5010, Type = typeof(Lazer))]
    public class Lazer : Magic
    {
        protected GridCheck _gridCheck;

        protected UnityNativeParticleItem _lazerEffect;
        protected UnityNativeParticleItem _lazerEffectEnd;

        protected Vector3 _direction;
        protected int _distance;

        protected ERotateMode _eRotateType;
        protected float _endAngle;
        protected float _curAngle;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _gridCheck = new GridCheck(this);
            SetSortingOrderBackground();
            return true;
        }

        public override UnitExtra UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
            _eRotateType = (ERotateMode) unitExtra.RotateMode;
            _endAngle = GM2DTools.GetAngle(unitExtra.RotateValue);
            return unitExtra;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _trans.localEulerAngles = new Vector3(0, 0, -_angle);
            if (_withEffect != null)
            {
                _withEffect.Trans.position += Vector3.back * 0.1f;
            }
            _lazerEffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazer", _trans);
            _lazerEffectEnd = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerStart", _trans);
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _curAngle = _angle;
            _direction = GM2DTools.GetDirection(_curAngle);
            if (_trans != null)
            {
                _trans.localEulerAngles = new Vector3(0, 0, -_angle);
            }
            _gridCheck.Clear();
            if (_lazerEffect != null)
            {
                _lazerEffect.Stop();
            }
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Stop();
            }
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_lazerEffect);
            _lazerEffect = null;
            FreeEffect(_lazerEffectEnd);
            _lazerEffectEnd = null;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            _lazerEffect.SetActiveStateEx(_eActiveState == EActiveState.Active);
            _lazerEffectEnd.SetActiveStateEx(_eActiveState == EActiveState.Active);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _gridCheck.Before();
            //停止
            if (_eActiveState != EActiveState.Active)
            {
                _gridCheck.Clear();
                return;
            }
            _distance = _tableUnit.ValidRange * ConstDefineGM2D.ServerTileScale;
            if (_eRotateType != ERotateMode.None)
            {
                switch (_eRotateType)
                {
                    case ERotateMode.Clockwise:
                        _curAngle += 1;
                        break;
                    case ERotateMode.Anticlockwise:
                        _curAngle += -1;
                        break;
                }
                Util.CorrectAngle360(ref _curAngle);
                if (!Util.IsFloatEqual(_angle, _endAngle))
                {
                    if (Util.IsFloatEqual(_curAngle, _angle) || Util.IsFloatEqual(_curAngle, _endAngle))
                    {
                        _eRotateType = _eRotateType == ERotateMode.Clockwise
                            ? ERotateMode.Anticlockwise
                            : ERotateMode.Clockwise;
                    }
                }
                _direction = GM2DTools.GetDirection(_curAngle);
                if (_trans != null)
                {
                    _trans.localEulerAngles = new Vector3(0, 0, -_curAngle);
                }
            }
            var hits = ColliderScene2D.RaycastAll(CenterPos, _direction, _distance, EnvManager.LazerShootLayer);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    var tableUnit = UnitManager.Instance.GetTableUnit(hit.node.Id);
                    if (tableUnit != null && tableUnit.IsLazerBlock == 1)
                    {
                        if (UnitDefine.IsSameDirectionSwitchTrigger(hit.node, Rotation))
                        {
                            UnitBase switchTrigger;
                            if (ColliderScene2D.Instance.TryGetUnit(hit.node, out switchTrigger))
                            {
                                _gridCheck.Do((SwitchTriggerPress) switchTrigger);
                                _distance = hit.distance + 80;
                                break;
                            }
                        }
                        bool flag = false;
                        var units = ColliderScene2D.GetUnits(hit);
                        for (int j = 0; j < units.Count; j++)
                        {
                            if (units[j] != this && units[j].IsAlive && !units[j].CanCross)
                            {
                                _distance = hit.distance;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (UnitDefine.IsLaserDamage(hit.node.Layer))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
                        {
                            if (unit != null && unit.IsAlive && unit.IsActor)
                            {
                                unit.InLazer();
                            }
                        }
                    }
                }
            }
            UpdateEffect();
            _gridCheck.After();
        }

        private void UpdateEffect()
        {
            if (_lazerEffect != null)
            {
                _lazerEffect.Play();
                var distanceWorld = _distance * ConstDefineGM2D.ClientTileScale;
                _lazerEffect.Trans.localScale = new Vector3(1, distanceWorld, 1);
                if (_lazerEffectEnd != null)
                {
                    _lazerEffectEnd.Play();
                    _lazerEffectEnd.Trans.position =
                        GM2DTools.TileToWorld(CenterPos, _lazerEffect.Trans.position.z - 0.1f) +
                        distanceWorld * _direction;
                }
            }
        }
    }
}