/********************************************************************
** Filename : MainPlayer
** Author : Dong
** Date : 2017/3/3 星期五 下午 8:28:03
** Summary : MainPlayer
***********************************************************************/

using System;
using SoyEngine;
using Random = UnityEngine.Random;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 1002, Type = typeof(MainPlayer))]
    public class MainPlayer : PlayerBase
    {
        public override bool IsMain
        {
            get { return true; }
        }

        protected override void OnJump()
        {
            if (!GameAudioManager.Instance.IsPlaying(AudioNameConstDefineGM2D.Sping))
            {
                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Jump);
            }
            base.OnJump();
        }

        protected override void OnStep()
        {
            base.OnStep();
            int randomValue = Random.Range(0, 3);
            switch (randomValue)
            {
                case 0:
                    GameAudioManager.Instance.PlaySoundsEffects("AudioWalkWood01");
                    break;
                case 1:
                    GameAudioManager.Instance.PlaySoundsEffects("AudioWalkWood02");
                    break;
                case 2:
                    GameAudioManager.Instance.PlaySoundsEffects("AudioWalkWood03");
                    break;
            }
        }
    }
}
