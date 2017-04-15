/********************************************************************
** Filename : Trigger  
** Author : CWC
** Date : 12/29/2016 10:20:22 AM
** Summary : Trigger  
***********************************************************************/

using System;
using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 65400, Type = typeof (Trigger))]
    public class Trigger : UnitBase
    {
        private Callback _callback;
        public override void OnHit(UnitBase other)
        {
            if (other.IsMain)
            {
                OnTrigger();
                PlayMode.Instance.DestroyUnit(this);
            }
        }

        protected void OnTrigger()
        {
            if (_callback != null) {
                _callback();
            }
        }

        public void SetCallback (Callback cb) {
            _callback = cb;
        }
    }
}