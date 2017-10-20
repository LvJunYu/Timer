/********************************************************************
** Filename : MainPlayer
** Author : Dong
** Date : 2017/3/3 星期五 下午 8:28:03
** Summary : MainPlayer
***********************************************************************/

using System;

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

        public override void UpdateView(float deltaTime)
        {
            base.UpdateView(deltaTime);
            if (GM2DGame.Instance.GameMode is GameModeWorldPlay)
            {
                ((GameModeWorldPlay)GM2DGame.Instance.GameMode).ShadowData.RecordPos(_curPos);
            }
        }
    }
}
