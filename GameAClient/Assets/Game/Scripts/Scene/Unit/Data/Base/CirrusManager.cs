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
            _cirrusDic.Add(_curDicIndex, new WholeCirrus(pos));
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
        private const string SpriteFormat = "M1Cirrus_{0}";
        private const int GrowSpeed = Cirrus.GrowSpeed;
        private const int CirrusId = UnitDefine.CirrusId;
        private readonly List<Cirrus> _cirrusJoints = new List<Cirrus>(Cirrus.MaxCirrusCount);
        private readonly IntVec2 _oriPos;
        private int _count;
        private int _curGrowValue;
        private IntVec2 _cirrusSize;
        private bool _growFinish;

        public bool IsInterest
        {
            get
            {
                for (int i = 0; i < _cirrusJoints.Count; i++)
                {
                    if (_cirrusJoints[i] != null && !_cirrusJoints[i].IsInterest)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public WholeCirrus(IntVec2 pos)
        {
            _cirrusSize = UnitManager.Instance.GetTableUnit(CirrusId).GetDataSize(0, Vector2.one);
            _oriPos = pos + IntVec2.down * _cirrusSize.y;
            _count = 0;
            GrowNewJoint();
        }

        private void GrowNewJoint()
        {
            var cirrus = PlayMode.Instance.CreateRuntimeUnit(CirrusId, _oriPos) as Cirrus;
            if (cirrus == null)
            {
                LogHelper.Error("Grow fail");
                return;
            }

            cirrus.SetJointIndex(_count);
            cirrus.OnPlay();
            _cirrusJoints.Add(cirrus);
            _curGrowValue = 0;
            _count++;
        }

        private bool Grow()
        {
            if (_growFinish)
            {
                return false;
            }

            if (_curGrowValue >= _cirrusSize.y)
            {
                if (_count < Cirrus.MaxCirrusCount)
                {
                    GrowNewJoint();
                }
                else
                {
                    _growFinish = true;
                    return false;
                }
            }

            var topCirrusPos = _cirrusJoints[0].CurPos;
            Grid2D checkGrid = new Grid2D(topCirrusPos.x, topCirrusPos.y + _cirrusSize.y,
                topCirrusPos.x + _cirrusSize.x - 1, topCirrusPos.y + _cirrusSize.y + GrowSpeed - 1);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].IsAlive && UnitDefine.CanHitCirrus(units[i]))
                {
                    return false;
                }
            }

            _curGrowValue += GrowSpeed;
            return true;
        }

        public void Clear()
        {
            _growFinish = false;
            _cirrusJoints.Clear();
        }

        public void UpdateLogic()
        {
            if (!IsInterest)
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
            if (!IsInterest)
            {
                return;
            }

            if (Grow())
            {
                for (int i = 0; i < _cirrusJoints.Count; i++)
                {
                    _cirrusJoints[i].Speed = _oriPos + ((_count - i - 1) * _cirrusSize.y + _curGrowValue) * IntVec2.up -
                                             _cirrusJoints[i].CurPos;
                    _cirrusJoints[i].UpdateView(deltaTime);
                }
            }
        }
    }
}