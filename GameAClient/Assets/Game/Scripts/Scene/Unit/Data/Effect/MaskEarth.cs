/********************************************************************
** Filename : MaskEarth
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:43:48
** Summary : MaskEarth
***********************************************************************/

using System.ComponentModel;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 9002, Type = typeof(MaskEarth))]
    public class MaskEarth : Earth
    {
        protected bool _trigger;
        protected SpriteRenderer _spriteRenderer;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _spriteRenderer = _view.Trans.GetComponent<SpriteRenderer>();
            if (GameRun.Instance.IsEdit)
            {
                _spriteRenderer.DOFade(0.5f, 0.5f);
            }
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _spriteRenderer.DOFade(1, 0f);
        }

        internal override void OnEdit()
        {
            _spriteRenderer.DOFade(0.5f, 0.5f);
        }

        protected override void Clear()
        {
            _trigger = false;
            base.Clear();
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
                _spriteRenderer.DOFade(0, 0.5f);
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
                    var maskEarth = unit as MaskEarth;
                    if (maskEarth != null)
                    {
                        maskEarth.OnTrigger();
                    }
                }
            }
        }
    }
}
