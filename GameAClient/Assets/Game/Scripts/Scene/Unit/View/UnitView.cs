/********************************************************************
** Filename : UnitView
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:58:26
** Summary : UnitView
***********************************************************************/

using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using DG.Tweening;
using SoyEngine;
using UnityEngine;
using NewResourceSolution;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class UnitView : IPoolableObject
    {
        public static Color SelectedColor = Color.red;
        public static Color NormalColor = Color.white;
        
        protected Transform _trans;

		protected Transform _dirTrans;
        protected Transform _dirTrans2;

        protected UnitBase _unit;
        protected WingView _wingLeft;
        protected WingView _wingRight;
        protected AnimationSystem _animation;

        protected StatusBar _statusBar;

        protected bool _isPart;

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

        public StatusBar StatusBar
        {
            get
            {
                if (null == _statusBar)
                {
                    InitStatusBar();
                }
                return _statusBar;
            }
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

        public virtual void SetRendererColor (Color color)
        {
        }

        public virtual void SetSortingOrder(int sortingOrder)
        {
            
        }

        public virtual void SetMatShader(Shader shader,string name = null,float value = 1)
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
            _trans.parent = UnitManager.Instance.GetOriginParent();
            SetRendererEnabled(true);
            SetRendererColor(Color.white);
			if (_dirTrans != null)
			{
				Object.Destroy(_dirTrans.gameObject);
			    _dirTrans = null;
			}
            if (_dirTrans2 != null)
            {
                Object.Destroy(_dirTrans2.gameObject);
                _dirTrans2 = null;
            }
            if (_wingLeft != null)
            {
                PoolFactory<WingView>.Free(_wingLeft);
                _wingLeft = null;
            }
            if (_wingRight != null)
            {
                PoolFactory<WingView>.Free(_wingRight);
                _wingRight = null;
            }
        }
        
        private void GenerateWing()
        {
            if (_unit.MoveDirection != EMoveDirection.None && !_unit.IsActor)
            {
                if (_wingLeft == null)
                {
                    _wingLeft = PoolFactory<WingView>.Get();
                    _wingLeft.Init("M1WingLeft");
                    SetWingTrans(_wingLeft, 0.01f);
                }
                if (_wingRight == null)
                {
                    _wingRight = PoolFactory<WingView>.Get();
                    _wingRight.Init("M1WingRight");
                    SetWingTrans(_wingRight, -0.01f);
                }
            }
        }

        private void SetWingTrans(WingView wing, float z)
        {
            wing.Trans.parent = _trans;
            wing.Trans.localPosition = Vector3.forward * z;
            if (_unit.TableUnit.EGeneratedType == EGeneratedType.Spine)
            {
                wing.Trans.localPosition += Vector3.up * GM2DTools.TileToWorld(_unit.GetColliderSize().y / 2);
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
                _trans.localRotation = Quaternion.identity;
                _trans.parent = UnitManager.Instance.GetParent(_unit.TableUnit.EUnitType);
            }
            if (_dirTrans != null)
            {
                _dirTrans.SetActiveEx(true);
            }
            if (_dirTrans2 != null)
            {
                _dirTrans2.SetActiveEx(true);
            }
            if (_wingLeft != null)
            {
                _wingLeft.Trans.SetActiveEx(true);
            }
            if (_wingRight != null)
            {
                _wingRight.Trans.SetActiveEx(true);
            }
            if (_statusBar != null)
            {
                Object.Destroy(_statusBar.gameObject);
                _statusBar = null;
            }
            UpdateSign();
        }

        public virtual void OnPlay()
        {
            SetEditAssistActive(false);
        }

        public virtual void SetEditAssistActive(bool active)
        {
            if (_dirTrans != null)
            {
                _dirTrans.SetActiveEx(active);
            }
            if (_dirTrans2 != null)
            {
                _dirTrans2.SetActiveEx(active);
            }
            if (_wingLeft != null)
            {
                _wingLeft.Trans.SetActiveEx(active);
            }
            if (_wingRight != null)
            {
                _wingRight.Trans.SetActiveEx(active);
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
            if (_dirTrans2 != null)
            {
                _dirTrans2.SetActiveEx(false);
            }
        }

        private void CreateDirTrans(string attName)
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
            Sprite arrowSprite;
            if (ResourcesManager.Instance.TryGetSprite(attName, out arrowSprite))
            {
                meshRenderer.sprite = arrowSprite;
            }
        }

        public void UpdateSign()
        {
            if (_isPart)
            {
                return;
            }
            var tableUnit = _unit.TableUnit;
            if (tableUnit.EUnitType != EUnitType.Bullet)
            {
                if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
                {
                    if (_unit.MoveDirection != EMoveDirection.None)
                    {
                        GenerateWing();
                        CreateDirTrans("M1Move");
                    }
                    else if (UnitDefine.IsEditClick(tableUnit.Id))
                    {
                        CreateDirTrans("M1Click");
                    }
                    else if (tableUnit.CanEdit(EEditType.Direction) || tableUnit.Id == UnitDefine.RollerId)
                    {
                        CreateDirTrans("M1Move");
                    }
                }
            }
            if (tableUnit.EPairType != EPairType.None)
            {
                SetPairRenderer();
            }
            if (_dirTrans != null)
            {
                if (tableUnit.CanEdit(EEditType.Direction))
                {
                    //Vector3 offset = GetRotationPosOffset();
                    _dirTrans.localEulerAngles = new Vector3(0, 0, GetRotation(_unit.UnitDesc.Rotation));
                    //_dirTrans.localPosition = offset + _unit.GetTransPos();
                }

                if (_unit.MoveDirection != EMoveDirection.None || tableUnit.Id == UnitDefine.RollerId)
                {
                    if (tableUnit.Id == UnitDefine.RollerId)
                    {
                        var rollerUnit = _unit as Roller;
                        if (rollerUnit != null)
                        {
                            _dirTrans.localEulerAngles = new Vector3(0, 0, GetRotation((byte)(rollerUnit.RollerDirection - 1)));
                        }
                    } 
                    else
                    {
                        _dirTrans.localEulerAngles = new Vector3(0, 0, GetRotation((byte)(_unit.MoveDirection - 1)));
                    }
                    //角色单独处理
                    if (UnitDefine.IsHero(tableUnit.Id))
                    {
                        _dirTrans.localEulerAngles = new Vector3(0, 0, GetRotation((byte)(EMoveDirection.Right - 1)));
                    }
                }
            }
	    }

        private void SetPairRenderer()
        {
            SpriteRenderer spriteRenderer;
            if (_dirTrans2 == null)
            {
                _dirTrans2 = new GameObject("Pair").transform;
                CommonTools.SetParent(_dirTrans2, _trans);
                spriteRenderer = _dirTrans2.gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = (int)ESortingOrder.AttTexture2;
                if (this is SpineUnit)
                {
                    var offset = GM2DTools.TileToWorld(_unit.GetDataSize()) * 0.5f;
                    offset.x = 0;
                    _dirTrans2.localPosition = offset;
                }
            }
            spriteRenderer = _dirTrans2.GetComponent<SpriteRenderer>();
            PairUnit pairUnit;
            if (!PairUnitManager.Instance.TryGetPairUnit(_unit.TableUnit.EPairType, _unit.UnitDesc, out pairUnit))
            {
                LogHelper.Debug("TryGetPairUnit Faield,{0}", _unit.UnitDesc);
                return;
            }
            Sprite arrowSprite;
            if (ResourcesManager.Instance.TryGetSprite("Letter_" + pairUnit.Num, out arrowSprite))
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
            switch ((EDirectionType)_unit.Rotation)
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

        private int GetRotation(byte rotation)
        {
            return -90 * rotation;
        }
        
        
        private void InitStatusBar()
        {
            if (null != _statusBar) return;
            GameObject statusBarObj = Object.Instantiate(ResourcesManager.Instance.GetPrefab(EResType.ParticlePrefab, "StatusBar", 1)) as GameObject;
            if (null != statusBarObj)
            {
                _statusBar = statusBarObj.GetComponent<StatusBar>();
                CommonTools.SetParent(statusBarObj.transform, _trans);
            }
        }
    }
}
