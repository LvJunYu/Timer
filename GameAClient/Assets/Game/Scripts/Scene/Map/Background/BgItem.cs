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

namespace GameA.Game
{
    [Poolable(MinPoolSize = 30, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class BgItem : IPoolableObject
    {
        protected Table_Background _tableBg;
        protected Transform _trans;
        protected Vector2 _curPos;
        protected SpriteRenderer _spriteRenderer;
        protected SceneNode _node;
        private bool _isShow;

        public virtual bool Init(Table_Background table, SceneNode node)
        {
            _isShow = true;
            _tableBg = table;
            _node = node;
            var size = new IntVec2(_node.Grid.XMax - _node.Grid.XMin + 1, _node.Grid.YMax - _node.Grid.YMin + 1);
            _curPos = GM2DTools.TileToWorld(new IntVec2(_node.Guid.x, _node.Guid.y) + size / 2);
            GameObject go;
            if (!TryCreateObject(out go))
            {
                return false;
            }
            _trans = go.transform;
            _trans.position = _curPos;
            _trans.localScale = new Vector3(_node.Rotation * 0.1f, _node.Rotation * 0.1f, 0);
            if (_tableBg.RotateSpeed != 0)
            {
                var r = _trans.gameObject.AddComponent<BgItemRotate>();
                r.Init(_tableBg);
            }
            return true;
        }

        public virtual void Update(IntVec2 deltaPos)
        {
            if (_trans == null)
            {
                return;
            }
            UpdateMove();
            UpdateFollow(GM2DTools.TileToWorld(deltaPos));
            UpdateRenderer();
            _trans.position = _curPos;
        }

        private void UpdateRenderer()
        {
            //云在下面不显示
            if (_tableBg.Depth == (int)EBgDepth.Far || _tableBg.Depth == (int)EBgDepth.Near)
            {
                if (_curPos.y <= 17f)
                {
                    if (_isShow)
                    {
                        _spriteRenderer.enabled = false;
                        _isShow = false;
                    }
                }
                else
                {
                    if (!_isShow)
                    {
                        _spriteRenderer.enabled = true;
                        _isShow = true;
                    }
                }
            }
        }

        private void UpdateFollow(Vector2 deltaPos)
        {
            _curPos += deltaPos*(1 - BgScene2D.Instance.GetMoveRatio(_tableBg.Depth));
        }

        protected virtual  void UpdateMove()
        {
            if (_tableBg.MoveSpeedX != 0 || _tableBg.MoveSpeedY != 0)
            {
                _curPos += new Vector2(_tableBg.MoveSpeedX, _tableBg.MoveSpeedY) * ConstDefineGM2D.FixedDeltaTime * BgScene2D.Instance.GetMoveRatio(_tableBg.Depth);
            }
            var followRect = BgScene2D.Instance.GetRect(_tableBg.Depth);
            var tilePos = GM2DTools.WorldToTile(_curPos);
            if (tilePos.x <= followRect.XMin)
            {
                tilePos.x += followRect.XMax - followRect.XMin + 1;
            }
            else if (tilePos.x > followRect.XMax)
            {
                tilePos.x -= followRect.XMax - followRect.XMin + 1;
            }
            if (_tableBg.Depth != (int) EBgDepth.Nearest)
            {
                if (tilePos.y <= followRect.YMin)
                {
                    tilePos.y += followRect.YMax - followRect.YMin + 1;
                }
                else if (tilePos.y > followRect.YMax)
                {
                    tilePos.y -= followRect.YMax - followRect.YMin + 1;
                }
            }
            _curPos = GM2DTools.TileToWorld(tilePos);
        }

        private bool TryCreateObject(out GameObject go)
        {
            go = null;
            Sprite sprite;
            if (!GameResourceManager.Instance.TryGetSpriteByName(_tableBg.Model, out sprite))
            {
                LogHelper.Error("TryGetSpriteByName failed,{0}", _tableBg.Model);
                return false;
            }
            go = new GameObject();
            _spriteRenderer = go.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = _tableBg.SortingLayer;
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
