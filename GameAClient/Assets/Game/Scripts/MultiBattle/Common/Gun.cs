/********************************************************************
** Filename : Gun
** Author : Dong
** Date : 2017/6/6 星期二 下午 3:29:13
** Summary : Gun
***********************************************************************/

using SoyEngine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    public class Gun
    {
        [SerializeField] protected static IntVec2 _shooterEffectOffset = new IntVec2(320, 700);
        protected UnityNativeParticleItem _shooterEffect;
        protected PlayerBase _player;
        protected IntVec2 _curPos;
        protected string _lastModelName;
        protected EShootDirectionType _eShootDir;

        public Gun(PlayerBase player)
        {
            _player = player;
        }

        internal bool InstantiateView()
        {
            //初始化ShooterEffect
            _shooterEffect =
                GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.Shooter,
                    _player.Trans, ESortingOrder.Bullet);
            Play();
            return true;
        }

        public void ChangeView(string model)
        {
            if (_lastModelName == model)
            {
                return;
            }
            _lastModelName = model;
            if (_shooterEffect != null)
            {
                GameParticleManager.FreeParticleItem(_shooterEffect);
            }
            _shooterEffect =
                GameParticleManager.Instance.GetUnityNativeParticleItem(model, _player.Trans, ESortingOrder.Bullet);
            Play();
        }

        public void Play()
        {
            _curPos = GetDestPos();
            if (_shooterEffect != null)
            {
                _shooterEffect.Play();
                _shooterEffect.Trans.position = GM2DTools.TileToWorld(_curPos);
            }
        }

        public void Stop()
        {
            if (_shooterEffect != null)
            {
                _shooterEffect.Stop();
            }
            if (_player.Skeleton != null)
            {
                _player.Skeleton.SetAttachment("SMainBoy0/s_PenGuan", null);
                _player.Skeleton.SetAttachment("SMainBoy0/s_BeiBao", null);
                _player.Skeleton.SetAttachment("SMainBoy0/s_nu", null);
            }
        }

        internal void OnObjectDestroy()
        {
            if (_shooterEffect != null)
            {
                GameParticleManager.FreeParticleItem(_shooterEffect);
            }
            _shooterEffect = null;
        }

        public void UpdateView()
        {
            var destPos = GetDestPos();
            var deltaPos = (destPos - _curPos) / 6;
            if (deltaPos.x == 0)
            {
                deltaPos.x = destPos.x - _curPos.x;
            }
            if (deltaPos.y == 0)
            {
                deltaPos.y = destPos.y - _curPos.y;
            }
            _curPos += deltaPos;
            if (_shooterEffect != null)
            {
                _shooterEffect.Trans.position = GM2DTools.TileToWorld(_curPos);
            }
            SetBoneDir("SMainBoy0/PenGuan", _eShootDir);
            SetBoneDir("SMainBoy0/YouDaBi", _eShootDir);
        }

        private IntVec2 GetDestPos()
        {
            return _player.MoveDirection == EMoveDirection.Right
                ? _player.CenterDownPos + new IntVec2(-_shooterEffectOffset.x, _shooterEffectOffset.y)
                : _player.CenterDownPos + new IntVec2(_shooterEffectOffset.x, _shooterEffectOffset.y);
        }

        public void ChangeGun(Table_Equipment tableEquipment, EShootDirectionType? eShootDir)
        {
            if (_player.Skeleton != null)
            {
                if (UsePenGuan(tableEquipment.Id))
                {
                    _player.Skeleton.SetAttachment("SMainBoy0/s_BeiBao", "0/EWai/WuQiBao");
                    _player.Skeleton.SetAttachment("SMainBoy0/s_PenGuan", "0/EWai/WuQi");
                    if (eShootDir != null)
                    {
                        _eShootDir = eShootDir.Value;
                        SetBoneDir("SMainBoy0/PenGuan", eShootDir.Value);
                    }
                }
                else if (UseGun(tableEquipment.Id))
                {
                    _player.Skeleton.SetAttachment("SMainBoy0/s_nu", tableEquipment.ModelExtra);
                    if (eShootDir != null)
                    {
                        _eShootDir = eShootDir.Value;
                        SetBoneDir("SMainBoy0/YouDaBi", eShootDir.Value);
                    }
                }
            }
        }

        private void SetBoneDir(string boneName, EShootDirectionType eShootDir)
        {
            var bone = _player.Skeleton.FindBone(boneName);
            bone.rotation = (int) eShootDir;
            bone.UpdateWorldTransform();
        }

        private bool UsePenGuan(int id)
        {
            return id > 100 && id < 200;
        }

        private bool UseGun(int id)
        {
            return id > 200 && id < 300;
        }
    }
}