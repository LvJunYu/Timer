using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class CirrusManager : IDisposable
    {
        private static CirrusManager _instance;
        private int _curDicIndex;
        private Dictionary<int, WholeCirrus> _cirrusDic = new Dictionary<int, WholeCirrus>();

        public static CirrusManager Instance
        {
            get { return _instance ?? (_instance = new CirrusManager()); }
        }

        public void Reset()
        {
            _curDicIndex = 0;
            foreach (var cirrus in _cirrusDic.Values)
            {
                cirrus.Clear();
            }

            _cirrusDic.Clear();
        }

        public void Dispose()
        {
            Reset();
            _instance = null;
        }

        public WholeCirrus GetWholeCirrus(int index)
        {
            WholeCirrus cirrus;
            if (_cirrusDic.TryGetValue(index, out cirrus))
            {
                return cirrus;
            }

            LogHelper.Error("GetWholeRope fail, index = {0}", index);
            return null;
        }

        public void CreateCirrus(IntVec2 pos)
        {
            _cirrusDic.Add(_curDicIndex, new WholeCirrus(pos, _curDicIndex));
            _curDicIndex++;
        }

        public void UpdateLogic()
        {
            foreach (var cirrus in _cirrusDic.Values)
            {
                cirrus.UpdateLogic();
            }
        }

        public void UpdateView(float deltaTime)
        {
            foreach (var cirrus in _cirrusDic.Values)
            {
                cirrus.UpdateView(deltaTime);
            }
        }
    }

    public class WholeCirrus
    {
        private const float GrowSpeed = 1;
        private const int CirrusId = UnitDefine.CirrusId;
        private List<Cirrus> _cirrusJoints = new List<Cirrus>(Cirrus.MaxCirrusCount);
        private IntVec2 _oriPos;
        private Cirrus _curGrowCirrus;
        private int _curGrowJointIndex;
        private float _curGrowValue;
        private IntVec2 _cirrusSize;
        private bool _growFinish;

        private bool _isInterest
        {
            get
            {
                for (int i = 0; i < _cirrusJoints.Count; i++)
                {
                    if (_cirrusJoints[i] == null || !_cirrusJoints[i].IsInterest)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public WholeCirrus(IntVec2 pos, int index)
        {
            _oriPos = pos;
            _cirrusSize = UnitManager.Instance.GetTableUnit(CirrusId).GetDataSize(0, Vector2.one);
            _curGrowJointIndex = -1;
            GrowNewJoint();
        }

        private void GrowNewJoint()
        {
            _curGrowJointIndex++;
            var pos = _oriPos + _curGrowJointIndex * IntVec2.up * _cirrusSize.y;
            _curGrowCirrus = PlayMode.Instance.CreateRuntimeUnit(CirrusId, pos) as Cirrus;
            if (_curGrowCirrus == null)
            {
                LogHelper.Error("Grow fail");
                return;
            }

            _cirrusJoints.Add(_curGrowCirrus);
            _curGrowValue = 0;
        }

        private void Grow(float deltaTime)
        {
            if (_curGrowValue >= 1)
            {
                if (_curGrowJointIndex < Cirrus.MaxCirrusCount)
                {
                    GrowNewJoint();
                }
                else
                {
                    _growFinish = true;
                }
            }
            
            var min = _curGrowCirrus.CurPos;
            var targetValue = _curGrowValue + deltaTime * GrowSpeed;
            Grid2D checkGrid = new Grid2D(min.x, min.y + (int) (_cirrusSize.y * _curGrowValue),
                min.x + _cirrusSize.x - 1, min.y + (int) (_cirrusSize.y * targetValue) - 1);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].IsAlive)
                {
                    return;
                }
            }

            _curGrowValue = targetValue;
            _curGrowCirrus.SetCurGrowValue(_curGrowValue);
        }

        public void Clear()
        {
            _growFinish = false;
            _curGrowCirrus = null;
            _cirrusJoints.Clear();
        }

        public void UpdateLogic()
        {
            if (!_isInterest)
            {
                return;
            }

            for (int i = 0; i < _cirrusJoints.Count; i++)
            {
                _cirrusJoints[i].UpdateLogic();
            }
        }

        public void UpdateView(float deltaTime)
        {
            if (!_isInterest)
            {
                return;
            }

            Grow(deltaTime);
            for (int i = 0; i < _cirrusJoints.Count; i++)
            {
                _cirrusJoints[i].UpdateView(deltaTime);
            }
        }
    }
}