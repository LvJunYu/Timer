/********************************************************************
** Filename : FakeEarth
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:43:48
** Summary : FakeEarth
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4002, Type = typeof(FakeEarth))]
    public class FakeEarth : Earth
    {
        protected bool _trigger = false;
        protected UnitBase _unit;
        protected SpriteRenderer _spriteRenderer;
        protected Sequence _editSequence;

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
            _spriteRenderer = _view.Trans.GetComponent<SpriteRenderer>();
            if (GameRun.Instance.IsEdit)
            {
                PlayAnimation();
            }
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_editSequence != null)
            {
                _editSequence.Rewind();
                _editSequence.Pause();
            }
        }

        internal override void OnEdit()
        {
            if (_editSequence != null)
            {
                _editSequence.Play();
            }
        }

        private void PlayAnimation()
        {
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f - Time.realtimeSinceStartup % 2f,
                () =>
                {
                    if (GameRun.Instance.IsEdit)
                    {
                        _editSequence = DOTween.Sequence();
                        _editSequence.Append(_spriteRenderer.DOFade(0, 0.5f));
                        _editSequence.Append(_spriteRenderer.DOFade(1, 0.8f));
                        _editSequence.AppendInterval(0.7f);
                        _editSequence.SetLoops(-1);
                    }
                }));
        }

        protected override void Clear()
        {
            _trigger = false;
            _unit = null;
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_editSequence != null)
            {
                DOTween.Kill(_trans);
                _editSequence.Kill();
                _editSequence = null;
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(other);
                }
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(other);
                }
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(other);
                }
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(other);
                }
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnTrigger(UnitBase other)
        {
            if (_trigger)
            {
                return;
            }
            _trigger = true;
            _unit = other;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOFade(0, 0.2f);
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_trigger && _unit != null)
            {
                if (!_colliderGrid.Intersects(_unit.ColliderGrid))
                {
                    _trigger = false;
                    _unit = null;
                    if (_spriteRenderer != null)
                    {
                        _spriteRenderer.DOFade(1, 0.5f);
                    }
                }
            }
        }
    }
}
