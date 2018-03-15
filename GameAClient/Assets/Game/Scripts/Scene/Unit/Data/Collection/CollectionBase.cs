/********************************************************************
** Filename : CollectionBase
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:45:23
** Summary : CollectionBase
***********************************************************************/

using DG.Tweening;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class CollectionBase : Magic
    {
        protected CycleTimer _cycleTimer;
        protected Tweener _tweener;
        protected int _timer;
        protected bool _isCycle;
        protected int _cycleTimerMax;

        protected virtual string _cycleTimerSpriteName
        {
            get { return _tableUnit.Model; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground();
            return true;
        }

        internal override void OnObjectDestroy()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }

            if (_cycleTimer != null)
            {
                Object.Destroy(_cycleTimer.gameObject);
                _cycleTimer = null;
            }

            base.OnObjectDestroy();
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            CreateCycleTimer();
            return true;
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _isCycle = unitExtra.TimerCirculation;
            _cycleTimerMax = unitExtra.CycleInterval * ConstDefineGM2D.FixedFrameCount;
            return unitExtra;
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            SetCycleTimerActive(false);
        }

        public override void OnIntersect(UnitBase other)
        {
            if (_isAlive && other.IsPlayer)
            {
                if (_isCycle)
                {
                    if (_timer != 0)
                    {
                        return;
                    }

                    OnTrigger(other);
                    _timer = _cycleTimerMax;
                    SetCycleTimerActive(true);
                }
                else
                {
                    OnLastTrigger();
                    OnTrigger(other);
                }
            }
        }

        protected virtual void OnLastTrigger()
        {
            OnDead();
            PlayMode.Instance.DestroyUnit(this);
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.AddColltion(Id);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
                if (_cycleTimer != null)
                {
                    if (_timer > 0)
                    {
                        _cycleTimer.SetValue(_timer / (float) _cycleTimerMax);
                    }
                    else
                    {
                        SetCycleTimerActive(false);
                    }
                }
            }
        }

        protected virtual void CreateCycleTimer()
        {
            if (null != _cycleTimer)
            {
                return;
            }

            GameObject cycleTimerObj =
                Object.Instantiate(JoyResManager.Instance.GetPrefab(EResType.ParticlePrefab, "CycleTimer")) as
                    GameObject;
            if (null != cycleTimerObj)
            {
                _cycleTimer = cycleTimerObj.GetComponent<CycleTimer>();
                CommonTools.SetParent(cycleTimerObj.transform, _trans);
                cycleTimerObj.SetActive(false);
                if (_cycleTimer != null)
                {
                    _cycleTimer.SetSprite(JoyResManager.Instance.GetSprite(_cycleTimerSpriteName));
                }
            }
        }

        private void SetCycleTimerActive(bool value)
        {
            if (_view == null || _cycleTimer == null)
            {
                return;
            }

            _cycleTimer.SetActiveEx(value);
            if (value)
            {
                if (_tweener != null)
                {
                    _tweener.Rewind();
                }
                _view.SetRendererColor(Color.clear);
            }
            else
            {
                if (_tweener != null)
                {
                    _tweener.PlayForward();
                }
                _view.SetRendererColor(Color.white);
            }
        }

        public void StopTwenner()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }
        }
    }
}