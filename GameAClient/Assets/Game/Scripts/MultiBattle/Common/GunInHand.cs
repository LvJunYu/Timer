using Spine;
using UnityEngine;

namespace GameA.Game
{
    public class GunInHand
    {
        protected PlayerBase _player;
        protected string _lastModelName;
        protected const float _duration = 0.5f;
        protected const float _mixDuration = 0.2f;
        protected Bone _bone;
        protected Bone _headBone;
        protected TrackEntry _track;
        protected float _dir;
        protected float _headDir;
        protected float _timer;

        public GunInHand(PlayerBase player)
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

        protected void SetGun(string mobleName, EShootDirectionType? eShootDir)
        {
            if (_player.Skeleton == null) return;
            if (_lastModelName != mobleName || eShootDir == null)
            {
                _player.Skeleton.SetAttachment("SMainBoy0/s_BeiBao", "0/EWai/WuQiBao");
                _player.Skeleton.SetAttachment("SMainBoy0/s_nu", mobleName);
                _lastModelName = mobleName;
            }
            if (eShootDir != null)
            {
                SetBoneDir("SMainBoy0/YouDaBi", eShootDir.Value);
                _player.Animation.ClearTrack(1);
                _track = _player.SkeletonAnimation.state.SetAnimation(1, "RunHit", false);
                if (_track != null)
                {
                    _track.endTime = _duration;
                }
            }
        }

        protected void SetBoneDir(string boneName, EShootDirectionType eShootDir)
        {
            if (_bone == null)
            {
                _bone = _player.Skeleton.FindBone(boneName);
            }
            if (_bone == null) return;
            _dir = GetDir(eShootDir);
            _bone.SetControlRotate(_dir);
            //保证从前方旋转
            if (_dir < _bone.data.rotation)
            {
                _dir += 360;
            }
            _timer = _duration;

            //控制头的方向
            if (_headBone == null)
            {
                _headBone = _player.Skeleton.FindBone("SMainBoy0/TouBu");
            }
            if (_headBone == null) return;
            _headDir = GetHeadDir(eShootDir);
            _headBone.SetControlRotate(_headDir);
        }

        protected int GetDir(EShootDirectionType eShootDir)
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

        private int GetHeadDir(EShootDirectionType eShootDir)
        {
            int dir = 0;
            if (_player.ClimbState == EClimbState.Left || _player.ClimbState == EClimbState.Right || _player.ClimbState == EClimbState.Ladder)
            {
                switch (eShootDir)
                {
                    case EShootDirectionType.Up:
                        dir = 15;
                        break;
                    case EShootDirectionType.Right:
                        dir = 35;
                        break;
                    case EShootDirectionType.Down:
                        dir = -60;
                        break;
                    case EShootDirectionType.Left:
                        dir = -25;
                        break;
                    case EShootDirectionType.RightUp:
                        dir = 25;
                        break;
                    case EShootDirectionType.RightDown:
                        dir = -75;
                        break;
                    case EShootDirectionType.LeftDown:
                        dir = -45;
                        break;
                    case EShootDirectionType.LeftUp:
                        dir = 0;
                        break;
                }
            }
            else if (_player.ClimbState == EClimbState.Up)
            {
                switch (eShootDir)
                {
                    case EShootDirectionType.Up:
                        dir = -25;
                        break;
                    case EShootDirectionType.Right:
                        dir = 0;
                        break;
                    case EShootDirectionType.Down:
                        dir = 45;
                        break;
                    case EShootDirectionType.Left:
                        dir = 0;
                        break;
                    case EShootDirectionType.RightUp:
                        dir = -25;
                        break;
                    case EShootDirectionType.RightDown:
                        dir = 25;
                        break;
                    case EShootDirectionType.LeftDown:
                        dir = 25;
                        break;
                    case EShootDirectionType.LeftUp:
                        dir = -25;
                        break;
                }
            }
            else
            {
                switch (eShootDir)
                {
                    case EShootDirectionType.Up:
                        dir = 35;
                        break;
                    case EShootDirectionType.Right:
                        dir = 0;
                        break;
                    case EShootDirectionType.Down:
                        dir = -35;
                        break;
                    case EShootDirectionType.Left:
                        dir = 0;
                        break;
                    case EShootDirectionType.RightUp:
                        dir = 20;
                        break;
                    case EShootDirectionType.RightDown:
                        dir = -20;
                        break;
                    case EShootDirectionType.LeftDown:
                        dir = -20;
                        break;
                    case EShootDirectionType.LeftUp:
                        dir = 20;
                        break;
                }
            }
            return dir;
        }

        protected void Reset()
        {
            if (_bone != null)
            {
                _bone.ClearControlRotate();
            }
            _timer = -1;
            if (_headBone != null)
            {
                _headBone.ClearControlRotate();
            }
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
//            Debug.Log(_player.SkeletonAnimation.state.GetCurrent(0).time);
            if (_timer > 0)
            {
                _timer -= deltaTime;
                if (_timer < _mixDuration)
                {
                    if (_track != null)
                    {
                        _track.mix = _timer / _mixDuration;
                    }
                    if (_bone != null)
                    {
                        _bone.SetControlRotate(Mathf.Lerp(_dir, _bone.data.rotation, 1 - _timer / _mixDuration));
                    }
                    if (_headBone != null)
                    {
                        _headBone.SetControlRotate(Mathf.Lerp(_headDir, _headBone.data.rotation,
                            1 - _timer / _mixDuration));
                    }
                }
            }
            else if (_timer > -1)
            {
                Reset();
            }
        }
    }
}