/********************************************************************
** Filename : GuideBoard
** Author : Dong
** Date : 2016/11/6 星期日 下午 2:25:30
** Summary : GuideBoard
***********************************************************************/

using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 7101, Type = typeof (GuideBoard))]
    public class GuideBoard : DecorationBase
    {
        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }
    }
}