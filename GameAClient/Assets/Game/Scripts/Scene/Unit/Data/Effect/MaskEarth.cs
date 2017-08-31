/********************************************************************
** Filename : MaskEarth
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:43:48
** Summary : MaskEarth
***********************************************************************/

using System.ComponentModel;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 9010, Type = typeof(MaskEarth))]
    public class MaskEarth : Earth
    {
        protected bool _trigger;
        protected SpriteRenderer _spriteRenderer;
        protected int _edgeValue;
        protected bool _intersect;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _spriteRenderer = _view.Trans.GetComponent<SpriteRenderer>();
            if (GameRun.Instance.IsEdit)
            {
                _spriteRenderer.DOFade(0.5f, 0.5f);
            }
            CalculateEdge(true);
            return true;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            CalculateEdge(false);
            if (_spriteRenderer != null)
            {
                DOTween.Kill(_spriteRenderer);
            }
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOFade(1, 0f);
                _view.SetSortingOrder((int)ESortingOrder.Item);
            }
        }

        internal override void OnEdit()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOFade(0.5f, 0.5f);
                _view.SetSortingOrder((int)ESortingOrder.EffectItem);
            }
        }

        protected override void Clear()
        {
            _intersect = false;
            _trigger = false;
            SetAllCross(_trigger);
            base.Clear();
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                _intersect = true;
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(true);
                }
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(true);
                }
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(true);
                }
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            if (other.IsMain)
            {
                if (!checkOnly)
                {
                    OnTrigger(true);
                }
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public void OnTrigger(bool value)
        {
            if (_trigger == value)
            {
                return;
            }
            _trigger = value;
            SetAllCross(_trigger);
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOFade(_trigger ? 0 : 1, 0.5f);
            }
            SendMsgToAround();
//            LogHelper.Debug("OnTrigger {0} {1}", value, this);
        }
        
        public override void UpdateLogic()
        {
            if (_trigger && _intersect)
            {
                if (_edgeValue == 15)
                {
                    return;
                }
                var mainPlayer = PlayMode.Instance.MainPlayer;
                if (mainPlayer != null)
                {
                    if (!_colliderGrid.Intersects(mainPlayer.ColliderGrid))
                    {
                        _intersect = false;
                        if (IsEdgeEmpty(EDirectionType.Up))
                        {
                            if (mainPlayer.ColliderGrid.YMin > _colliderGrid.YMax)
                            {
                                OnTrigger(false);
                            }
                        }
                        if (IsEdgeEmpty(EDirectionType.Down))
                        {
                            if (mainPlayer.ColliderGrid.YMax < _colliderGrid.YMin)
                            {
                                OnTrigger(false);
                            }
                        }
                        if (IsEdgeEmpty(EDirectionType.Left))
                        {
                            if (mainPlayer.ColliderGrid.XMax < _colliderGrid.XMin)
                            {
                                OnTrigger(false);
                            }
                        }
                        if (IsEdgeEmpty(EDirectionType.Right))
                        {
                            if (mainPlayer.ColliderGrid.XMin > _colliderGrid.XMax)
                            {
                                OnTrigger(false);
                            }
                        }
                    }
                }
            }
        }

        private void SendMsgToAround()
        {
            if (_edgeValue == 0)
            {
                return;
            }
            if (!IsEdgeEmpty(EDirectionType.Up))
            {
                CheckPos(_unitDesc.GetUpPos(_unitDesc.Guid.z));
            }
            if (!IsEdgeEmpty(EDirectionType.Down))
            {
                CheckPos(_unitDesc.GetDownPos(_unitDesc.Guid.z));
            }
            if (!IsEdgeEmpty(EDirectionType.Left))
            {
                CheckPos(_unitDesc.GetLeftPos(_unitDesc.Guid.z));
            }
            if (!IsEdgeEmpty(EDirectionType.Right))
            {
                CheckPos(_unitDesc.GetRightPos(_unitDesc.Guid.z));
            }
        }

        private void CheckPos(IntVec3 pos)
        {
            UnitBase unit;
            if (ColliderScene2D.Instance.TryGetUnit(pos, out unit))
            {
                if (unit != null && unit.Id == Id)
                {
                    var maskEarth = unit as MaskEarth;
                    if (maskEarth != null)
                    {
                        maskEarth.OnTrigger(_trigger);
                    }
                }
            }
        }
        
        private bool IsEdgeEmpty(EDirectionType eDirectionType)
        {
            return (_edgeValue & (1 << (int) eDirectionType)) != 0;
        }
        
        private void CalculateEdge(bool add)
        {
            _edgeValue = 15;
            IntVec3 up=_unitDesc.GetUpPos(_unitDesc.Guid.z);
            IntVec3 down=_unitDesc.GetDownPos(_unitDesc.Guid.z);
            IntVec3 left=_unitDesc.GetLeftPos(_unitDesc.Guid.z);
            IntVec3 right=_unitDesc.GetRightPos(_unitDesc.Guid.z);
            if (CheckMaskEarth(up, EDirectionType.Down, add))
            {
                OnEdge(EDirectionType.Up, add);
            }
            if (CheckMaskEarth(down, EDirectionType.Up, add))
            {
                OnEdge(EDirectionType.Down, add);
            }
            if (CheckMaskEarth(left, EDirectionType.Right, add))
            {
                OnEdge(EDirectionType.Left, add);
            }
            if (CheckMaskEarth(right, EDirectionType.Left, add))
            {
                OnEdge(EDirectionType.Right, add);
            }
        }

        public void OnEdge(EDirectionType eDirectionType, bool add)
        {
            if (!add)
            {
                _edgeValue = (byte) (_edgeValue | (1 << (byte) eDirectionType));
            }
            else
            {
                _edgeValue = (byte) (_edgeValue & ~(1 << (byte) eDirectionType));
            }
            LogHelper.Debug("{0} {1}", _edgeValue, this);
        }

        private bool CheckMaskEarth(IntVec3 pos, EDirectionType eDirectionType, bool add)
        {
            UnitBase unit;
            if (ColliderScene2D.Instance.TryGetUnit(pos, out unit))
            {
                if (unit != null && unit.Id == Id)
                {
                    var maskEarth = unit as MaskEarth;
                    if (maskEarth != null)
                    {
                        maskEarth.OnEdge(eDirectionType, add);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
