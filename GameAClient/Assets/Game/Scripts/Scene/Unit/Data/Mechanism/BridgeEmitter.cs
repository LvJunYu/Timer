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
    public class BridgeEmitter : SwitchPress
    {
        private const int BridgeUnitId = 5009;
        protected Grid2D _checkGrid;
        protected Queue<UnitBase> _curCreatingQueue;
        protected List<Queue<UnitBase>> _waitingDestroyQueues = new List<Queue<UnitBase>>();
        private static List<Queue<UnitBase>> s_freeQueues = new List<Queue<UnitBase>>();
        protected Dictionary<IntVec3, List<GridCheck>> _gridChecks = new Dictionary<IntVec3, List<GridCheck>>();

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
            _gridChecks.Clear();
        }

        public override void OnTriggerStart(UnitBase other)
        {
            base.OnTriggerStart(other);
            _checkGrid = GM2DTools.CalculateFireColliderGrid(BridgeUnitId, _colliderGrid, _unitDesc.Rotation);
        }

        public override void UpdateLogic()
        {
            if (GameRun.Instance.LogicFrameCnt % 5 == 0)
            {
                if (!_trigger)
                {
                    if (_curCreatingQueue != null)
                    {
                        _waitingDestroyQueues.Add(_curCreatingQueue);
                        _curCreatingQueue = null;
                    }
                }
                else
                {
                    if (!DataScene2D.Instance.IsInTileMap(_checkGrid))
                    {
                        return;
                    }
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
                                var node = units[i];
                                if (node.Id == UnitDefine.SwitchTriggerId)
                                {
                                    List<GridCheck> lists;
                                    if (!_gridChecks.TryGetValue(unit.Guid, out lists))
                                    {
                                        lists = new List<GridCheck>();
                                        _gridChecks.Add(unit.Guid, lists);
                                    }
                                    var gridCheck = new GridCheck(unit);
                                    gridCheck.Do((SwitchTrigger)node);
                                    _gridChecks[unit.Guid].Add(gridCheck);
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
                        var unitDestroy = curQueue.Dequeue();
                        PlayMode.Instance.DestroyUnit(unitDestroy);
                        List<GridCheck> gridChecks;
                        if (_gridChecks.TryGetValue(unitDestroy.Guid, out gridChecks))
                        {
                            for (int j = 0; j < gridChecks.Count; j++)
                            {
                                gridChecks[j].OnDestroySelf(unitDestroy);
                            }
                        }
                        if (curQueue.Count == 0)
                        {
                            FreeQueue(curQueue);
                            _waitingDestroyQueues.Remove(curQueue);
                        }
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
