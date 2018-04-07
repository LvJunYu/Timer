using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 11001, Type = typeof(ProjectileMissile))]
    public class ProjectileMissile : ProjectileBase
    {
        private const string SpriteFormat = "M1Missile_{0}";

        protected override void OnRun()
        {
            if (_view != null)
            {
                _view.ChangeView(GetSpriteName(_skill.Owner.TeamId));
            }
        }

        protected override void SetAngle(float angle)
        {
            _angle = angle;
            _direction = GM2DTools.GetDirection(_angle);
            _speed = new IntVec2((int) (_skill.ProjectileSpeed * _direction.x),
                (int) (_skill.ProjectileSpeed * _direction.y));
            _trans.eulerAngles = new Vector3(0, 0, -_angle - 90);
        }

        public override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            base.Hit(unit, eDirectionType);
            if (_targetUnit != null && _targetUnit.Id == UnitDefine.StoneId)
            {
                ((Stone) _targetUnit).OnChanged();
            }
        }

        public string GetSpriteName(int teamId)
        {
            if (teamId == 0)
            {
                return _tableUnit.Model;
            }

            return string.Format(SpriteFormat, TeamManager.GetTeamColorName(teamId));
        }
    }
}