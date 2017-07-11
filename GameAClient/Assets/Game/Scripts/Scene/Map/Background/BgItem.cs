/********************************************************************
** Filename : BgItem
** Author : Dong
** Date : 2016/11/29 星期二 上午 11:17:51
** Summary : BgItem
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;
using UnityEngine;
using NewResourceSolution;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 30, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class BgItem : IPoolableObject
    {
        protected Table_Background _tableBg;
        protected Transform _trans;
        protected Vector3 _curPos;
        protected SpriteRenderer _spriteRenderer;
        protected SceneNode _node;

        public Transform Trans
        {
            get { return _trans; }
        }

        public virtual bool Init(Table_Background table, SceneNode node)
        {
            _tableBg = table;
            _node = node;
            var size = new IntVec2(_node.Grid.XMax - _node.Grid.XMin + 1, _node.Grid.YMax - _node.Grid.YMin + 1);
            _curPos = GM2DTools.TileToWorld(new IntVec2(_node.Guid.x, _node.Guid.y) + size / 2, table.Depth + 10);
            GameObject go;
            if (!TryCreateObject(out go))
            {
                return false;
            }
            _trans = go.transform;
            _trans.localPosition = _curPos;
            _trans.localScale = new Vector3(_node.Scale.x, _node.Scale.y, 1);
            return true;
        }

        public virtual void Update(IntVec2 deltaPos)
        {
            if (_trans == null)
            {
                return;
            }
            UpdateFollow(GM2DTools.TileToWorld(deltaPos));
            UpdateMove();
            _trans.position = _curPos;
        }

        protected virtual bool UpdateMove()
        {
            if (_tableBg.MoveSpeedX != 0)
            {
                _curPos += new Vector3(_tableBg.MoveSpeedX, 0) * ConstDefineGM2D.FixedDeltaTime * BgScene2D.Instance.GetMoveRatio(_tableBg.Depth);
            }
            var followRect = BgScene2D.Instance.GetRect(_tableBg.Depth);
            var tilePos = GM2DTools.WorldToTile(_curPos);
            var sizeX = _node.Grid.XMax - _node.Grid.XMin + 1;
            if (tilePos.x + sizeX / 2 <= followRect.XMin)
            {
                tilePos.x += followRect.XMax - followRect.XMin + 1 + sizeX;
            }
            _curPos = GM2DTools.TileToWorld(tilePos, _curPos.z);
            return true;
        }

        private void UpdateFollow(Vector3 deltaPos)
        {
            _curPos += deltaPos * (1 - BgScene2D.Instance.GetMoveRatio(_tableBg.Depth));
        }

        private bool TryCreateObject(out GameObject go)
        {
            go = null;
            Sprite sprite = null;
            if (!ResourcesManager.Instance.TryGetSprite(_tableBg.Model, out sprite))
            {
                LogHelper.Error("TryGetSpriteByName failed,{0}", _tableBg.Model);
                return false;
            }
            go = new GameObject();
            _spriteRenderer = go.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;
            if (_tableBg.Alpha < 1)
            {
                _spriteRenderer.material.color = new Color(1, 1, 1, _tableBg.Alpha);
            }
            _spriteRenderer.sortingOrder = (int)ESortingOrder.Item;
            go.transform.parent = BgScene2D.Instance.GetParent(_tableBg.Depth);
            return true;
        }

        public void OnGet()
        {
        }

        public virtual void OnFree()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = null;
            }
            _tableBg = null;
            _curPos = Vector2.zero;
            if (_trans != null)
            {
                var scripts = _trans.GetComponents<MonoBehaviour>();
                for (int i = scripts.Length - 1; i >= 0; i--)
                {
                    if (scripts[i] != null)
                    {
                        UnityEngine.Object.Destroy(scripts[i]);
                    }
                }
                _trans.position = Vector3.zero;
                _trans.rotation = Quaternion.identity;
                _trans.localScale = Vector3.one;
                _trans = null;
            }
        }

        public virtual void OnDestroyObject()
        {
        }
    }
}
