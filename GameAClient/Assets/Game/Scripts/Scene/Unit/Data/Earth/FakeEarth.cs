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
        protected bool _trigger;
        protected SpriteRenderer _spriteRenderer;
        protected Sequence _editSequence;
        private Coroutine _coroutine;

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
            _coroutine = CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(2f - Time.realtimeSinceStartup % 2f,
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
                    _coroutine = null;
                }));
        }

        protected override void Clear()
        {
            _trigger = false;
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_coroutine != null)
            {
                CoroutineProxy.Instance.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            if (_editSequence != null)
            {
                _editSequence.Rewind();
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
                    OnTrigger();
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
                    OnTrigger();
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
                    OnTrigger();
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
                    OnTrigger();
                }
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public void OnTrigger()
        {
            if (_trigger)
            {
                return;
            }
            _trigger = true;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOFade(0, 0.2f);
            }
            SendMsgToAround();
        }

        private void SendMsgToAround()
        {
            CheckPos(_unitDesc.GetUpPos(_unitDesc.Guid.z));
            CheckPos(_unitDesc.GetDownPos(_unitDesc.Guid.z));
            CheckPos(_unitDesc.GetLeftPos(_unitDesc.Guid.z));
            CheckPos(_unitDesc.GetRightPos(_unitDesc.Guid.z));
        }

        private void CheckPos(IntVec3 pos)
        {
            UnitBase unit;
            if (ColliderScene2D.Instance.TryGetUnit(pos, out unit))
            {
                if (unit != null && unit.Id == Id)
                {
                    var fakeEarth = unit as FakeEarth;
                    if (fakeEarth != null)
                    {
                        fakeEarth.OnTrigger();
                    }
                }
            }
        }
    }
}
