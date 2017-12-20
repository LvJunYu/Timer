/********************************************************************
** Filename : Gun
** Author : Dong
** Date : 2017/6/6 星期二 下午 3:29:13
** Summary : Gun
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using Spine;
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
            ClearAnimation();
        }

        internal void OnObjectDestroy()
        {
            ClearAnimation();
            if (_shooterEffect != null)
            {
                GameParticleManager.FreeParticleItem(_shooterEffect);
            }
            _shooterEffect = null;
        }

        public void UpdateView(float deltaTime)
        {
            var buffer = new List<string>(_timerDic.Keys);
            foreach (var boneName in buffer)
            {
                if (_timerDic[boneName] > 0)
                {
                    _timerDic[boneName] -= deltaTime;
                    if (_timerDic[boneName] < _duration / 2)
                    {
                        _boneDic[boneName].data.rotation = Mathf.Lerp(_boneDic[boneName].data.rotation,
                            _originalRotateDic[boneName], 0.1f);
                    }
                }
                else if (_timerDic[boneName] > -1)
                {
                    ClearBoneDir(boneName);
                }
            }
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
//            SetBoneDir("SMainBoy0/PenGuan", _eShootDir);
//            SetBoneDir("SMainBoy0/YouDaBi", _eShootDir);
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
                        SetBoneDir("SMainBoy0/PenGuan", eShootDir.Value);
                    }
                }
                else if (UseGun(tableEquipment.Id))
                {
                    _player.Skeleton.SetAttachment("SMainBoy0/s_nu", tableEquipment.ModelExtra);
                    if (eShootDir != null)
                    {
                        SetBoneDir("SMainBoy0/YouDaBi", eShootDir.Value);
                    }
                }
            }
        }

        private IkConstraint _penguanTransformConstraint;

        private void SetBoneDir(string boneName, EShootDirectionType eShootDir)
        {
            var bone = _player.Skeleton.FindBone(boneName);
            if (bone == null) return;
            if (!_boneDic.ContainsKey(boneName))
            {
                _boneDic.Add(boneName, bone);
            }
            if (!_originalRotateDic.ContainsKey(boneName))
            {
                _originalRotateDic.Add(boneName, bone.data.rotation);
            }
            bone.data.rotation = GetDir(eShootDir, _player.Trans.localEulerAngles.y == 180);
            _timerDic.AddOrReplace(boneName, _duration);
        }

        private int GetDir(EShootDirectionType eShootDir, bool faceLeft)
        {
            if (faceLeft)
            {
                return (int) eShootDir;
            }
            int dir = 0;
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
            return dir;
        }

        private void ClearBoneDir(string boneName)
        {
            var bone = _player.Skeleton.FindBone(boneName);
            if (bone != null && _originalRotateDic.ContainsKey(boneName))
            {
                bone.data.rotation = _originalRotateDic[boneName];
            }
            if (_timerDic.ContainsKey(boneName))
            {
                _timerDic[boneName] = -1;
            }
        }

        private void ClearAnimation()
        {
            var buffer = new List<string>(_timerDic.Keys);
            foreach (var boneName in buffer)
            {
                ClearBoneDir(boneName);
            }
            _boneDic.Clear();
        }

        private const float _duration = 1f;
        private Dictionary<string, Bone> _boneDic = new Dictionary<string, Bone>(2);
        private Dictionary<string, float> _timerDic = new Dictionary<string, float>(2);
        private Dictionary<string, float> _originalRotateDic = new Dictionary<string, float>(2);

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