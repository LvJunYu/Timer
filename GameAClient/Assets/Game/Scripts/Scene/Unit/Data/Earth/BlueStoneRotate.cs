/********************************************************************
** Filename : BlueStoneRotate
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:35:30
** Summary : BlueStoneRotate
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4103, Type = typeof(BlueStoneRotate))]
    public class BlueStoneRotate : Magic
    {
        protected override void InitAssetPath()
        {
            _assetPath = _tableUnit.Model;
        }

        public override void UpdateExtraData()
        {
            base.UpdateExtraData();
            if (_view != null)
            {
                _trans.localEulerAngles = new Vector3(0, 0, GetRotation(_unitDesc.Rotation));
                Vector3 offset = GetRotationPosOffset();
                _trans.localPosition = offset + GetTransPos();
            }
        }
    }
}
