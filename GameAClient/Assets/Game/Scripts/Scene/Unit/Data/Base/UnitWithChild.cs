/********************************************************************
** Filename : UnitWithChild
** Author : Dong
** Date : 2016/10/27 星期四 下午 5:14:39
** Summary : UnitWithChild
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class UnitWithChild : BlockBase
    {
        protected UnitChild _unitChild;
        protected UnitBase _childUnit;
        protected bool _trigger;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _unitChild = DataScene2D.Instance.GetUnitExtra(_guid).Child;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_unitChild.Id > 0)
            {
                _childUnit = PlayMode.Instance.CreateRuntimeUnit(_childUnit.Id, _curPos, _childUnit.Rotation, Vector2.one);
                _childUnit.SetFacingDir(_unitChild.MoveDirection);
                PlayMode.Instance.Freeze(_childUnit);
            }
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
        }

        internal override void OnObjectDestroy()
        {
            if (PlayMode.Instance != null)
            {
                PlayMode.Instance.DestroyUnit(_childUnit);
            }
        }

        protected virtual void PushChild()
        {
            if (_childUnit != null)
            {
                PlayMode.Instance.UnFreeze(_childUnit);
            }
        }
    }
}
