/********************************************************************
** Filename : PlaceHolder  
** Author : CWC
** Date : 12/29/2016 10:20:22 AM
** Summary : PlaceHolder  
***********************************************************************/

using System;
using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 65401, Type = typeof (PlaceHolder))]
    public class PlaceHolder : UnitBase
    {
        public override bool OnUpHit (UnitBase other, ref int y, bool checkOnly = false)
        {
            y = GetUpHitMin ();
            return true;
        }

        public override bool OnDownHit (UnitBase other, ref int y, bool checkOnly = false)
        {
            y = GetDownHitMin (other);
            return true;
        }

        public override bool OnLeftHit (UnitBase other, ref int x, bool checkOnly = false)
        {
            x = GetLeftHitMin (other);
            return true;
        }

        public override bool OnRightHit (UnitBase other, ref int x, bool checkOnly = false)
        {
            x = GetRightHitMin ();
            return true;
        }
    }
}