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
            //这样背景会好看很多
            if (GM2DGame.Instance.CurrentMode == EMode.Play || GM2DGame.Instance.CurrentMode == EMode.PlayRecord)
            {
                var fov = CameraManager.Instance.RendererCamera.orthographicSize * 2;
                var ratio = GM2DGame.Instance.GameScreenWidth / (float)GM2DGame.Instance.GameScreenHeight;
                var height = fov * ConstDefineGM2D.ServerTileScale;
                var width = fov * ratio * ConstDefineGM2D.ServerTileScale;
                IntVec2 size = new IntVec2(_tableBg.Width, _tableBg.Height) * 20;
                var x = width / size.x * 0.5f;
                var y = height / size.y * 0.5f;
                _trans.localScale = new Vector3(x, y, 1);
            }
            else
            {
                IntVec2 size = new IntVec2(_tableBg.Width, _tableBg.Height) * 20;
                var followRect = BgScene2D.Instance.FollowRect;
                int x = (followRect.XMax + 1 - followRect.XMin) / size.x;
                int y = (followRect.YMax + 1 - followRect.YMin) / size.y;
                var scale = Mathf.Max(x, y);
                _trans.localScale = new Vector3(scale, scale, 1);
            }
            _curPos = CameraManager.Instance.RendererCamaraTrans.position;
            _trans.position = _curPos;
            return true;
        }

        public override void Update(IntVec2 delPos)
        {
            if (GM2DGame.Instance.CurrentMode == EMode.Play || GM2DGame.Instance.CurrentMode == EMode.PlayRecord)
            {
                if (_trans == null)
                {
                    return;
                }
                _curPos = CameraManager.Instance.RendererCamaraTrans.position;
                _trans.position = _curPos;
            }
            else
            {
                base.Update(delPos);
            }
        }
    }
}
