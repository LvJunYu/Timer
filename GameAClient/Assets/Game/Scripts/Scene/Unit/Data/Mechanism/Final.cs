/********************************************************************
** Filename : Final
** Author : Dong
** Date : 2016/10/20 星期四 下午 1:59:03
** Summary : Final
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5001, Type = typeof(Final))]
    public class Final : BlockBase
    {
        protected static Final _instance;
        protected UnityNativeParticleItem _efffect;

        public static Vector3 Position
        {
            get
            {
                if (_instance != null)
                {
                    return _instance.Trans.position;
                }
                return PlayMode.Instance.MainUnit.Trans.position;
            }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _instance = this;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _efffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectFinal", _trans);
            if (_efffect != null)
            {
                SetUpTrans(_efffect.Trans);
                _efffect.Play();
            }
            return true;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_efffect);
            _efffect = null;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsMain)
            {
                //播放动画
                PlayMode.Instance.SceneState.Arrived = true;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }
    }
}