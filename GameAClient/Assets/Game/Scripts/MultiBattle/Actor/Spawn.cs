/********************************************************************
** Filename : Spawn
** Author : Dong
** Date : 2017/6/24 星期六 下午 6:54:08
** Summary : Spawn
***********************************************************************/

namespace GameA.Game
{
    [Unit(Id = 1001, Type = typeof(Spawn))]
    public class Spawn : BlockBase
    {
        protected override void InitAssetPath()
        {
            _assetPath = _tableUnit.Model.Replace("0", GetUnitExtra().TeamId.ToString());
        }
    }
}