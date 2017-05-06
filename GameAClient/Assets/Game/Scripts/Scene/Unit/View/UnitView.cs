/********************************************************************
** Filename : UnitView
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:58:26
** Summary : UnitView
***********************************************************************/

using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class UnitView : IPoolableObject
    {
        protected static Vector2 _hidePos = Vector3.one * -5;

        protected Transform _trans;

		protected Transform _dirTrans;

        protected UnitBase _unit;

        protected AnimationSystem _animation;

		public Transform Trans
        {
            get { return _trans; }
        }

	    public Transform DirTrans
	    {
		    get { return _dirTrans; }
	    }

        public AnimationSystem Animation
        {
            get { return _animation; }
        }

        public UnitView()
        {
            _trans = new GameObject(GetType().Name).transform;
			if (UnitManager.Instance != null) 
            {
				_trans.parent = UnitManager.Instance.GetOriginParent ();
			}
        }

        public virtual void OnGet()
        {
        }

        public bool Init(UnitBase unit)
        {
#if UNITY_EDITOR
            _trans.name = string.Format("{0}_{1}", unit.Id, unit.TableUnit.Name);
#endif
            _unit = unit;
            if (!OnInit())
            {
                return false;
            }
            Reset();
            return true;
        }

        public virtual void InitMorphId(byte morphId)
        {
        }

        protected virtual bool OnInit()
        {
            return false;
        }

        public virtual void SetRendererEnabled(bool value)
        {

        }

        public virtual void SetRendererColor (Color color)
        {
        }

        public virtual void SetSortingOrder(int sortingOrder)
        {
            
        }

        public virtual void OnFree()
        {
#if UNITY_EDITOR
            _trans.name = GetType().Name;
#endif
            _unit = null;
            _trans.SetActiveEx(true);
            _trans.position = _hidePos;
            _trans.localScale = Vector3.one;
            _trans.localRotation = Quaternion.identity;
            _trans.parent = UnitManager.Instance.GetOriginParent();
			if (_dirTrans != null)
			{
				Object.Destroy(_dirTrans.gameObject);
			    _dirTrans = null;
			}
		}

	    public static Color SelectedColor = Color.red;
	    public static Color NormalColor = Color.white;

	    public virtual void OnSelect()
	    {
		    
	    }

	    public virtual void OnCancelSelect()
	    {
		    
	    }

        public virtual void OnDestroyObject()
        {
        }

        public virtual void Reset()
        {
            if (_unit == null)
            {
                return;
            }
            if (_animation != null)
            {
                _animation.Reset();
            }
            if (_trans != null)
            {
                _trans.position = _unit.GetTransPos();
                _trans.localScale = new Vector3(_unit.UnitDesc.Scale.x, _unit.UnitDesc.Scale.y, 1);
                _trans.rotation = Quaternion.identity;
                _trans.parent = UnitManager.Instance.GetParent(_unit.TableUnit.EUnitType);
            }
            if (_dirTrans != null)
            {
                _dirTrans.SetActiveEx(true);
            }
            UpdateSign();
        }

        public virtual void OnPlay()
        {
            if (_dirTrans != null)
            {
                _dirTrans.SetActiveEx(false);
            }
        }

        public virtual void ChangeView(string assetPath)
        {
            
        }

        public virtual void OnNeighborDirChanged(ENeighborDir neighborDir, bool add)
        {
        }

        public void OnIsChild()
        {
            if (_dirTrans != null)
            {
                _dirTrans.SetActiveEx(false);
            }
        }

        private void CreateDirTrans()
        {
            if (_dirTrans != null)
            {
                return;
            }
            _dirTrans = new GameObject("AttTexture").transform;
            CommonTools.SetParent(_dirTrans, _trans);
            if (this is SpineUnit)
            {
                var offset = GM2DTools.TileToWorld(_unit.GetDataSize()) * 0.5f;
                offset.x = 0;
                _dirTrans.localPosition = offset;
            }
            var meshRenderer = _dirTrans.gameObject.AddComponent<SpriteRenderer>();
            meshRenderer.sortingOrder = (int)ESortingOrder.AttTexture;
            var tweener = _dirTrans.DOScale(0.7f, 0.5f);
            tweener.SetLoops(-1, LoopType.Yoyo);
            Sprite arrowTexture;
            if (GameResourceManager.Instance.TryGetSpriteByName(ConstDefineGM2D.DirectionTextureName, out arrowTexture))
            {
                meshRenderer.sprite = arrowTexture;
            }
        }

        public void UpdateSign()
        {
            var tableUnit = _unit.TableUnit;
            //当不可动但却能动，说明有蓝石
            if (!tableUnit.CanMove && _unit.MoveDirection != EMoveDirection.None)
            {
                //生成蓝石
            }
            if (tableUnit.EUnitType != EUnitType.Bullet)
            {
                if (GM2DGame.Instance.CurrentMode == EMode.Edit || 
					GM2DGame.Instance.CurrentMode == EMode.EditTest ||
					GM2DGame.Instance.CurrentMode == EMode.ModifyEdit
				)
                {
                    //生成方向标志
                    if (tableUnit.CanRotate || _unit.MoveDirection != EMoveDirection.None || tableUnit.Id == ConstDefineGM2D.RollerId)
                    {
                        CreateDirTrans();
                    }
                }
            }

            if (_dirTrans != null)
            {
                if (tableUnit.CanRotate)
                {
                    //Vector3 offset = GetRotationPosOffset();
                    _dirTrans.localEulerAngles = new Vector3(0, 0, GetRotation(_unit.UnitDesc.Rotation));
                    //_dirTrans.localPosition = offset + _unit.GetTransPos();
                }

                if (_unit.MoveDirection != EMoveDirection.None || tableUnit.Id == ConstDefineGM2D.RollerId)
                {
                    float y = 0;
                    if (_unit.IsMonster)
                    {
                        y = _unit.MoveDirection != EMoveDirection.Right ? 180 : 0;
                    }
                    if (tableUnit.Id == ConstDefineGM2D.RollerId)
                    {
                        var rollerUnit = _unit as Roller;
                        if (rollerUnit != null)
                        {
                            _dirTrans.localEulerAngles = new Vector3(0, y, GetRotation((byte)(rollerUnit.RollerDirection - 1)));
                        }
                    }
                    else
                    {
                        _dirTrans.localEulerAngles = new Vector3(0, y, GetRotation((byte)(_unit.MoveDirection - 1)));
                    }
                }
            }
	    }

        private void SetPairRenderer()
        {
            //var meshRenderer = _dirTrans2.gameObject.AddComponent<SpriteRenderer>();
            //meshRenderer.sortingOrder = (int)ESortingOrder.AttTexture2;
            //PairUnit pairUnit;
            //if (!PairUnitManager.Instance.TryGetPairUnit(_unit.TableUnit.EPairType, _unit.UnitDesc,out pairUnit))
            //{
            //    LogHelper.Debug("TryGetPairUnit Faield,{0}",_unit.UnitDesc);
            //    return;
            //}
            //Sprite arrowTexture;
            //if (GameResourceManager.Instance.TryGetSpriteByName("Letter_"+pairUnit.Num, out arrowTexture))
            //{
            //    meshRenderer.sprite = arrowTexture;
            //}
        }

        public Vector2 GetRotationPosOffset()
        {
            if (_unit.TableUnit.EGeneratedType != EGeneratedType.Spine)
            {
                return Vector2.zero;
            }
            Vector2 res = Vector2.zero;
            Vector2 size = GM2DTools.TileToWorld(_unit.GetDataSize() * 0.5f);
            switch ((ERotationType)_unit.Rotation)
            {
                case ERotationType.Right:
                    res.x = -size.x;
                    res.y = size.y;
                    break;
                case ERotationType.Down:
                    res.y = size.y * 2;
                    break;
                case ERotationType.Left:
                    res.x = size.x;
                    res.y = size.y;
                    break;
            }
            return res;
        }

        private int GetRotation(byte rotation)
        {
            return -90 * rotation;
        }
	}
}
