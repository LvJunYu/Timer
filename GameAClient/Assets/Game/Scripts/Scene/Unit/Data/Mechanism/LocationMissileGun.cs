using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5029, Type = typeof(LocationMissileGun))]
    public class LocationMissileGun : UnitBase
    {
        private const string SpriteFormat = "M1LocationMissile/M1LocationMissile_{0}";
        private const string SlotName = "M1LocationMissile/M1LocationMissile_0";

        public void ChangeView(int teamId)
        {
            if (Skeleton != null)
            {
                Skeleton.SetAttachment(SlotName, GetLocationMissileSprite(teamId));
            }
        }

        public void RefreshGunDir(float curAngle)
        {
            if (_view != null)
            {
                _trans.localEulerAngles = new Vector3(0, 0, 180 - curAngle);
            }
        }

        public void PlayAttackAnim()
        {
            if (_animation != null)
            {
                _animation.PlayOnce("Hit");
            }
        }

        public static string GetLocationMissileSprite(int teamId)
        {
            if (teamId == 0)
            {
                return string.Format(SpriteFormat, 0);
            }

            return string.Format(SpriteFormat, TeamManager.GetTeamColorName(teamId));
        }
    }
}