/********************************************************************
** Filename : Gun
** Author : Dong
** Date : 2017/6/6 星期二 下午 3:29:13
** Summary : Gun
***********************************************************************/

using SoyEngine;
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
        protected JetGun _jetGuan;
        protected ProjectileGun _projectileGun;

        public Gun(PlayerBase player)
        {
            _player = player;
            _jetGuan = new JetGun(player);
            _projectileGun = new ProjectileGun(player);
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

        public void Revive()
        {
            _jetGuan.Revive();
            _projectileGun.Revive();
        }

        public void Stop()
        {
            if (_shooterEffect != null)
            {
                _shooterEffect.Stop();
            }
            _jetGuan.Stop();
            _projectileGun.Stop();
        }

        internal void OnObjectDestroy()
        {
            _jetGuan.OnObjectDestroy();
            _projectileGun.OnObjectDestroy();
            if (_shooterEffect != null)
            {
                GameParticleManager.FreeParticleItem(_shooterEffect);
            }
            _shooterEffect = null;
        }

        public void UpdateView(float deltaTime)
        {
            _jetGuan.UpdateView(deltaTime);
            _projectileGun.UpdateView(deltaTime);
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
                    _jetGuan.SetGun(tableEquipment, eShootDir);
                }
                else if (UseGun(tableEquipment.Id))
                {
                    _projectileGun.SetGun(tableEquipment, eShootDir);
                }
            }
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