/********************************************************************
** Filename : UnitView
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:58:26
** Summary : UnitView
***********************************************************************/

using NewResourceSolution;
using SoyEngine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    public class UnitView : IPoolableObject
    {
        public static Color SelectedColor = Color.red;
        public static Color NormalColor = Color.white;

        protected Transform _trans;

        protected Transform _pairTrans;

        protected UnitBase _unit;
        protected AnimationSystem _animation;
        protected Skeleton _skeleton;
        protected SkeletonAnimation _skeletonAnimation;

        protected UnitPropertyViewWrapper _propertyViewWrapper;

        protected bool _isPart;

        public Transform Trans
        {
            get { return _trans; }
        }

        public AnimationSystem Animation
        {
            get { return _animation; }
        }

        public Skeleton Skeleton
        {
            get { return _skeleton; }
        }

        public SkeletonAnimation SkeletonAnimation
        {
            get { return _skeletonAnimation; }
        }

        public Transform PairTrans
        {
            get { return _pairTrans; }
        }

        public UnitView()
        {
            _trans = new GameObject(GetType().Name).transform;
            if (UnitManager.Instance != null)
            {
                _trans.parent = UnitManager.Instance.GetOriginParent();
            }
        }

        public virtual void OnGet()
        {
        }

        public bool Init(UnitBase unit, bool isPart)
        {
#if UNITY_EDITOR
            _trans.name = string.Format("{0}_{1}", unit.Id, unit.TableUnit.Name);
#endif
            _unit = unit;
            _isPart = isPart;
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

        public virtual void SetRendererColor(Color color)
        {
        }

        public virtual void SetSortingOrder(int sortingOrder)
        {
        }

        public virtual void SetDamageShaderValue(string name = null, float value = 1)
        {
        }

        public virtual void OnFree()
        {
#if UNITY_EDITOR
            _trans.name = GetType().Name;
#endif
            _unit = null;
            _isPart = false;
            _trans.SetActiveEx(true);
            _trans.position = UnitDefine.HidePos;
            _trans.localScale = Vector3.one;
            _trans.localRotation = Quaternion.identity;
            _trans.rotation = Quaternion.identity;
            _trans.parent = UnitManager.Instance.GetOriginParent();
            SetRendererEnabled(true);
            SetRendererColor(Color.white);
            if (_pairTrans != null)
            {
                Object.Destroy(_pairTrans.gameObject);
                _pairTrans = null;
            }
            if (_propertyViewWrapper != null)
            {
                _propertyViewWrapper.Hide();
                _propertyViewWrapper = null;
            }
        }

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
                _trans.localScale = new Vector3(_unit.UnitDesc.Scale.x, _unit.UnitDesc.Scale.y, 1);
//                _trans.localRotation = Quaternion.identity;
                _trans.parent = UnitManager.Instance.GetParent(_unit.TableUnit.EUnitType);
            }
            if (_pairTrans != null)
            {
                _pairTrans.SetActiveEx(true);
            }
            if (_propertyViewWrapper != null)
            {
                _propertyViewWrapper.Hide();
            }
            UpdateSign();
        }

        public virtual void OnPlay()
        {
            SetEditAssistActive(false);
        }

        public virtual void SetEditAssistActive(bool active)
        {
            if (_pairTrans != null)
            {
                _pairTrans.SetActiveEx(active);
            }
            if (_propertyViewWrapper != null)
            {
                if (active)
                {
                    var unitDesc = _unit.UnitDesc;
                    var unitExtra = DataScene2D.CurScene.GetUnitExtra(unitDesc.Guid);
                    _propertyViewWrapper.Show(ref unitDesc, ref unitExtra);
                }
                else
                {
                    _propertyViewWrapper.Hide();
                }
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
            if (_pairTrans != null)
            {
                _pairTrans.SetActiveEx(false);
            }
        }

        public void UpdateSign()
        {
            if (_isPart)
            {
                return;
            }
            if (_unit.Guid == IntVec3.zero)
            {
                return;
            }
            var tableUnit = _unit.TableUnit;
            if (tableUnit.EUnitType != EUnitType.Bullet)
            {
                if (GameRun.Instance.IsEdit)
                {
                    if (EditHelper.CheckCanEdit(tableUnit.Id))
                    {
                        if (_propertyViewWrapper == null)
                        {
                            _propertyViewWrapper = new UnitPropertyViewWrapper();
                        }
                        var unitDesc = _unit.UnitDesc;
                        var unitExtra = DataScene2D.CurScene.GetUnitExtra(unitDesc.Guid);
                        _propertyViewWrapper.Show(ref unitDesc, ref unitExtra);
                    }
                }
            }
            if (tableUnit.EPairType != EPairType.None)
            {
                SetPairRenderer();
            }
        }

        private void SetPairRenderer()
        {
            SpriteRenderer spriteRenderer;
            if (_pairTrans == null)
            {
                _pairTrans = new GameObject("Pair").transform;
                CommonTools.SetParent(_pairTrans, _trans);
                spriteRenderer = _pairTrans.gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = (int) ESortingOrder.AttTexture2;
                if (this is SpineUnit)
                {
                    var offset = GM2DTools.TileToWorld(_unit.GetDataSize()) * 0.5f;
                    offset.x = 0;
                    _pairTrans.localPosition = offset;
                }
            }
            spriteRenderer = _pairTrans.GetComponent<SpriteRenderer>();
            PairUnit pairUnit;
            if (!PairUnitManager.Instance.TryGetPairUnit(_unit.TableUnit.EPairType, _unit.UnitDesc, out pairUnit))
            {
                LogHelper.Debug("TryGetPairUnit Faield,{0}", _unit.UnitDesc);
                return;
            }
            Sprite arrowSprite;
            if (JoyResManager.Instance.TryGetSprite("Letter_" + pairUnit.Num, out arrowSprite))
            {
                spriteRenderer.sprite = arrowSprite;
            }
        }

        public Vector2 GetRotationPosOffset()
        {
            if (_unit.TableUnit.EGeneratedType != EGeneratedType.Spine)
            {
                return Vector2.zero;
            }
            Vector2 res = Vector2.zero;
            Vector2 size = GM2DTools.TileToWorld(_unit.GetDataSize() * 0.5f);
            switch ((EDirectionType) _unit.Rotation)
            {
                case EDirectionType.Right:
                    res.x = -size.x;
                    res.y = size.y;
                    break;
                case EDirectionType.Down:
                    res.y = size.y * 2;
                    break;
                case EDirectionType.Left:
                    res.x = size.x;
                    res.y = size.y;
                    break;
            }
            return res;
        }
    }
}