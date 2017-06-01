/********************************************************************
** Filename : BgRoot
** Author : Dong
** Date : 2016/11/29 星期二 下午 7:42:38
** Summary : BgRoot
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 1, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class BgRoot : BgItem
    {
        public override bool Init(Table_Background table, SceneNode node)
        {
            if (!base.Init(table, node))
            {
                return false;
            }
            IntVec2 size = new IntVec2(_tableBg.Width, _tableBg.Height) * 20;
            var validMapRect = DataScene2D.Instance.ValidMapRect;
            int x = (validMapRect.Max.x + 1 - validMapRect.Min.x) / size.x;
            int y = (validMapRect.Max.y + 1 - validMapRect.Min.y) / size.y;
            _trans.localScale = new Vector3(x, y, 1);
            _trans.position = _curPos;
            return true;
        }
    }
}
