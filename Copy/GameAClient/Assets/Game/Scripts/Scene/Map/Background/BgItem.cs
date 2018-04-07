/********************************************************************
** Filename : BgItem
** Author : Dong
** Date : 2016/11/29 星期二 上午 11:17:51
** Summary : BgItem
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 30, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class BgItem : IPoolableObject
    {
        protected GameObject _go;
        protected Table_Background _tableBg;
        protected Transform _trans;
        protected Vector3 _basePos;
        protected Vector3 _baseFollowPos;
        protected Vector3 _deltaMove;
        protected Vector3 _curPos;
        protected SpriteRenderer _spriteRenderer;
        protected SceneNode _node;
        protected float _sizeX;
        protected float _halfSizeX;
        
        public Transform Trans
        {
            get { return _trans; }
        }

        public int Depth
        {
            get { return _tableBg.Depth; }
        }

        public SceneNode Node
        {
            get { return _node; }
        }

        public Table_Background TableBg
        {
            get { return _tableBg; }
        }

        public virtual bool Init(Table_Background table, SceneNode node, bool beforeScene = false, bool setCenter = false)
        {
            _tableBg = table;
            _node = node;
            var size = new IntVec2(_node.Grid.XMax - _node.Grid.XMin + 1, _node.Grid.YMax - _node.Grid.YMin + 1);
            int zDepth = table.Depth + UnitDefine.ZOffsetBackground;
            //最前显示
            if (beforeScene)
            {
                zDepth -= 850;
            }
            _basePos = GM2DTools.TileToWorld(new IntVec2(_node.Guid.x, _node.Guid.y) + size / 2, zDepth);
            //居中
            if (setCenter)
            {
                int offset = size.x / ConstDefineGM2D.ServerTileScale -
                             (int) BgScene2D.Instance.GetRect(table.Depth).width;
                _basePos += new Vector3(-offset / 2, -21, 0);
            }
            _curPos = _basePos;
            if (!TryCreateObject())
            {
                return false;
            }

            _trans = _go.transform;
            _trans.localPosition = _curPos;
            _trans.localScale = new Vector3(_node.Scale.x, _node.Scale.y, 1);

            _sizeX = (_node.Grid.XMax - _node.Grid.XMin + 1) * ConstDefineGM2D.ClientTileScale;
            _halfSizeX = _sizeX / 2;
            _deltaMove = Vector3.zero;
            return true;
        }

        public virtual void SetBaseFollowPos(Vector2 baseFollowPos)
        {
            _baseFollowPos = baseFollowPos;
        }

        public virtual void ResetPos()
        {
            _deltaMove = Vector3.zero;
            Update(_baseFollowPos);
        }

        public virtual void SetBasePos(Vector3 basePos)
        {
            _basePos = new Vector3(basePos.x, basePos.y, _basePos.z);
        }

        public virtual void Update(Vector3 followPos)
        {
            if (_trans == null)
            {
                return;
            }

            UpdateMove(followPos);
            _trans.position = _curPos;
        }

        protected virtual bool UpdateMove(Vector3 followPos)
        {
            if (Math.Abs(_tableBg.MoveSpeedX) > 0)
            {
                _deltaMove += new Vector3(_tableBg.MoveSpeedX, 0) * ConstDefineGM2D.FixedDeltaTime;
            }

            _curPos = _basePos + (followPos - _baseFollowPos) * (1 - BgScene2D.Instance.GetMoveRatio(_tableBg.Depth)) +
                      _deltaMove * BgScene2D.Instance.GetMoveRatio(_tableBg.Depth);
            var followRect = BgScene2D.Instance.GetRect(_tableBg.Depth);
            float w = followRect.width + _sizeX;
            float modX = (_curPos.x - followRect.xMin + _halfSizeX) % w;
            if (modX < 0)
            {
                modX += w;
            }

            _curPos.x = modX + followRect.xMin - _halfSizeX;
            return true;
        }

        private bool TryCreateObject()
        {
            Sprite sprite = BgScene2D.Instance.GetModelSprite(_tableBg.Model);
            if (sprite == null)
            {
                LogHelper.Error("TryGetSpriteByName failed,{0}", _tableBg.Model);
                return false;
            }
            if (_go == null)
            {
                _go = new GameObject();
                _spriteRenderer = _go.AddComponent<SpriteRenderer>();
            }

            _spriteRenderer.sprite = sprite;
            _spriteRenderer.material.color = new Color(1, 1, 1, _tableBg.Alpha);
            _spriteRenderer.sortingOrder = (int) ESortingOrder.Item;
            _go.transform.parent = BgScene2D.Instance.GetParent(_tableBg.Depth);
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
                        Object.Destroy(scripts[i]);
                    }
                }

                _trans.position = Vector3.zero;
                _trans.rotation = Quaternion.identity;
                _trans.localScale = Vector3.one;
            }
        }

        public virtual void OnDestroyObject()
        {
            if (_go != null)
            {
                Object.Destroy(_go);
                _trans = null;
            }
        }
    }
}