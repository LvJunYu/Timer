/********************************************************************
** Filename : BridgeEmitter
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:36:28
** Summary : BridgeEmitter
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5008, Type = typeof(BridgeEmitter))]
    public class BridgeEmitter : SwitchUnit
    {
        private const int BridgeUnitId = 5009;
        protected Grid2D _checkGrid;
        protected Queue<UnitBase> _curCreatingQueue;
        protected List<Queue<UnitBase>> _waitingDestroyQueues = new List<Queue<UnitBase>>();
        private static List<Queue<UnitBase>> s_freeQueues = new List<Queue<UnitBase>>();

        protected override bool OnInit()
        {
            _triggerReverse = true;
            if (!base.OnInit())
            {
                return false;
            }
            return true;
        }

        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }

        protected override void Clear()
        {
            base.Clear();
            _curCreatingQueue = null;
            _waitingDestroyQueues.Clear();
            s_freeQueues.Clear();
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (_eActiveState == EActiveState.Active)
            {
                _checkGrid = GM2DTools.CalculateFireColliderGrid(BridgeUnitId, _colliderGrid, _unitDesc.Rotation);
            }
        }

        public override void UpdateLogic()
        {
            if (GameRun.Instance.LogicFrameCnt % 5 != 0)
            {
                return;
            }
            if (_switchTrigger==null || _switchTrigger.EActiveState != EActiveState.Active)
            {
                if (_curCreatingQueue != null)
                {
                    _waitingDestroyQueues.Add(_curCreatingQueue);
                    _curCreatingQueue = null;
                }
            }
            else
            {
                bool blocked = false;
                var units = ColliderScene2D.GridCastAllReturnUnits(_checkGrid, EnvManager.BridgeBlockLayer);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (!unit.CanBridgeCross)
                    {
                        blocked = true;
                        break;
                    }
                }
                if (!blocked)
                {
                    var unit = PlayMode.Instance.CreateRuntimeUnit(BridgeUnitId, new IntVec2(_checkGrid.XMin, _checkGrid.YMin));
                    if (unit != null)
                    {
                        if (_curCreatingQueue == null)
                        {
                            _curCreatingQueue = AllocQueue();
                        }
                        _curCreatingQueue.Enqueue(unit);
                        _checkGrid = GM2DTools.CalculateFireColliderGrid(BridgeUnitId, _checkGrid, _unitDesc.Rotation);

                        for (int i = 0; i < units.Count; i++)
                        {
                            if (UnitDefine.IsSwitchTrigger(units[i].Id))
                            {
                                units[i].OnIntersect(unit);
                            }
                        }
                    }
                }
            }
            if (_waitingDestroyQueues.Count > 0)
            {
                for (int i = _waitingDestroyQueues.Count - 1; i >= 0; i--)
                {
                    var curQueue = _waitingDestroyQueues[i];
                    PlayMode.Instance.DestroyUnit(curQueue.Dequeue());
                    if (curQueue.Count == 0)
                    {
                        FreeQueue(curQueue);
                        _waitingDestroyQueues.Remove(curQueue);
                    }
                }
            }
        }

        private static void FreeQueue(Queue<UnitBase> queue)
        {
            s_freeQueues.Add(queue);
        }

        /// <summary>
        /// 循环利用队列
        /// </summary>
        /// <returns></returns>
        private Queue<UnitBase> AllocQueue()
        {
            if (s_freeQueues.Count == 0)
            {
                return new Queue<UnitBase>();
            }
            var queue = s_freeQueues[0];
            s_freeQueues.RemoveAt(0);
            return queue;
        }
    }
}
