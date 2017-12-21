using Spine;
using UnityEngine;

namespace GameA.Game
{
    public class RotateGun
    {
        protected PlayerBase _player;
        protected string _lastModelName;
        protected const float _duration = 0.5f;
        protected const float _mixDuration = 0.2f;
        protected Bone _bone;
        protected float _timer;
        protected float _originalDir = -1;
        protected TrackEntry _track;
        protected float _dir;

        protected RotateGun(PlayerBase player)
        {
            _player = player;
        }

        public void Revive()
        {
            if (_lastModelName != null)
            {
                SetGun(_lastModelName, null);
            }
        }

        public void SetGun(Table_Equipment tableEquipment, EShootDirectionType? eShootDir)
        {
            SetGun(tableEquipment.ModelExtra, eShootDir);
        }

        protected virtual void SetGun(string mobleName, EShootDirectionType? eShootDir)
        {
        }

        protected void SetBoneDir(string boneName, EShootDirectionType eShootDir)
        {
            if (_bone == null)
            {
                _bone = _player.Skeleton.FindBone(boneName);
                _originalDir = _bone.data.rotation;
            }
            if (_bone == null) return;
            _bone.data.rotation = _dir = GetDir(eShootDir);
            //保证从前方旋转
            if (_dir < _originalDir)
            {
                _dir += 360;
            }
            _timer = _duration;
        }

        protected virtual int GetDir(EShootDirectionType eShootDir)
        {
            int dir = 0;
            if (_player.MoveDirection == EMoveDirection.Left)
            {
                dir = (int) eShootDir;
            }
            else
            {
                switch (eShootDir)
                {
                    case EShootDirectionType.Up:
                        dir = 0;
                        break;
                    case EShootDirectionType.Right:
                        dir = 270;
                        break;
                    case EShootDirectionType.Down:
                        dir = 180;
                        break;
                    case EShootDirectionType.Left:
                        dir = 90;
                        break;
                    case EShootDirectionType.RightUp:
                        dir = 315;
                        break;
                    case EShootDirectionType.RightDown:
                        dir = 225;
                        break;
                    case EShootDirectionType.LeftDown:
                        dir = 135;
                        break;
                    case EShootDirectionType.LeftUp:
                        dir = 45;
                        break;
                }
            }
            if (_player.Animation.IsPlaying(_player.Run))
            {
                dir += 25;
            }
            if (_player.ClimbState == EClimbState.Up)
            {
                dir -= 90;
            }
            return dir;
        }

        private void Reset()
        {
            if (_bone == null) return;
            _bone.data.rotation = _originalDir;
            _timer = -1;
        }

        public void Stop()
        {
            Reset();
        }

        public void OnObjectDestroy()
        {
            Reset();
        }

        public void UpdateView(float deltaTime)
        {
            if (_timer > 0)
            {
                _timer -= deltaTime;
                if (_timer < _mixDuration)
                {
                    _track.mix = _timer / _mixDuration;
                    _bone.data.rotation = Mathf.Lerp(_dir, _originalDir, 1 - _timer / _mixDuration);
                }
            }
            else if (_timer > -1)
            {
                Reset();
            }
        }
    }

    public class JetGun : RotateGun
    {
        public JetGun(PlayerBase player) : base(player)
        {
        }

        protected override void SetGun(string mobleName, EShootDirectionType? eShootDir)
        {
            if (_player.Skeleton == null) return;
            _player.Skeleton.SetAttachment("SMainBoy0/s_BeiBao", "0/EWai/WuQiBao");
            _player.Skeleton.SetAttachment("SMainBoy0/s_PenGuan", "0/EWai/WuQi");
            _lastModelName = "0/EWai/WuQiBao";
            if (eShootDir != null)
            {
                SetBoneDir("SMainBoy0/PenGuan", eShootDir.Value);
                _player.Animation.ClearTrack(1);
                _track = _player.Animation.PlayOnce("PenGuanHit", 1, 1);
                _track.endTime = _duration;
            }
        }
    }

    public class ProjectileGun : RotateGun
    {
        public ProjectileGun(PlayerBase player) : base(player)
        {
        }

        protected override void SetGun(string mobleName, EShootDirectionType? eShootDir)
        {
            if (_player.Skeleton == null) return;
            _player.Skeleton.SetAttachment("SMainBoy0/s_nu", mobleName);
            _lastModelName = mobleName;
            if (eShootDir != null)
            {
                SetBoneDir("SMainBoy0/YouDaBi", eShootDir.Value);
                _player.Animation.ClearTrack(2);
                _track = _player.Animation.PlayOnce("RunHit", 1, 2);
                _track.endTime = _duration;
            }
        }
    }
}