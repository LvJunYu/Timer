using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class RopeManager : IDisposable
    {
        private static RopeManager _instance;
        [SerializeField] private Dictionary<int, List<RopeJoint>> _ropes = new Dictionary<int, List<RopeJoint>>();
        private int _curDicIndex;
        private Dictionary<IntVec3, Rope> _ropeUnits = new Dictionary<IntVec3, Rope>();

        public static RopeManager Instance
        {
            get { return _instance ?? (_instance = new RopeManager()); }
        }

        public Dictionary<IntVec3, Rope> RopeUnits
        {
            get { return _ropeUnits; }
        }

        public void AddRope(Rope rope)
        {
            if (!RopeUnits.ContainsKey(rope.Guid))
            {
                RopeUnits.Add(rope.Guid, rope);
            }
        }

        public void RemoveRope(Rope rope)
        {
            if (RopeUnits.ContainsKey(rope.Guid))
            {
                RopeUnits.Remove(rope.Guid);
            }
        }

        public void Reset()
        {
            _curDicIndex = 0;
            _ropes.Clear();
        }

        public void Dispose()
        {
            _curDicIndex = 0;
            _ropeUnits.Clear();
            _ropes.Clear();
            _instance = null;
        }

        public void AddRopeJoint(int ropeIndex)
        {
            _ropes[ropeIndex].Capacity += Rope.JointCount;
            for (int j = 0; j < Rope.JointCount; j++)
            {
                _ropes[ropeIndex].Add(null);
            }
        }

        public int AddNewRope()
        {
            _curDicIndex++;
            _ropes.Add(_curDicIndex, new List<RopeJoint>(Rope.JointCount));
            for (int j = 0; j < Rope.JointCount; j++)
            {
                _ropes[_curDicIndex].Add(null);
            }
            return _curDicIndex;
        }

        public void SetRopeJoint(int ropeIndex, int segmentIndex, RopeJoint[] ropeJoints)
        {
            for (int i = 0; i < ropeJoints.Length; i++)
            {
                _ropes[ropeIndex][segmentIndex * Rope.JointCount + i] = ropeJoints[i];
            }
        }

        public void RemoveRopeJoint(int ropeIndex, int segmentIndex)
        {
            for (int i = _ropes[ropeIndex].Count - 1; i >= segmentIndex * Rope.JointCount; i--)
            {
                _ropes[ropeIndex].RemoveAt(i);
            }
            if (_ropes[ropeIndex].Count == 0)
            {
                _ropes.Remove(ropeIndex);
            }
        }

        public bool CheckTieUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            IntVec2 dataSize = tableUnit.GetDataSize(ref unitDesc);
            IntVec2 centerUpFloorPos = new IntVec2(unitDesc.Guid.x + dataSize.x / 2, unitDesc.Guid.y + dataSize.y + 1);
            Grid2D checkGrid = new Grid2D(centerUpFloorPos, centerUpFloorPos);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].CanRope)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckTieRope(UnitDesc unitDesc, out Rope rope)
        {
            var tableUnit = TableManager.Instance.GetUnit(unitDesc.Id);
            IntVec2 dataSize = tableUnit.GetDataSize(ref unitDesc);
            IntVec2 centerDownFloorPos = new IntVec2(unitDesc.Guid.x + dataSize.x / 2, unitDesc.Guid.y - 1);
            Grid2D checkGrid = new Grid2D(centerDownFloorPos, centerDownFloorPos);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Id == UnitDefine.RopeId)
                {
                    rope = units[i] as Rope;
                    return true;
                }
            }
            rope = null;
            return false;
        }

        public void OnPlay()
        {
            foreach (var joints in _ropes.Values)
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    if (joints[i] == null)
                    {
                        LogHelper.Error("joint == null");
                        return;
                    }
                    if (i == 0)
                    {
                    }
                    else
                    {
                        joints[i].SetPreJoint(joints[i - 1]);
                        joints[i - 1].SetnNextJoint(joints[i]);
                    }
                }
            }
        }

        public void CheckAllRopeTie()
        {
            foreach (var rope in _ropeUnits.Values)
            {
                if (!rope.Tied && !rope.CheckNeighbor())
                {
                    LogHelper.Error("rope can not tie!");
                }
            }
        }

        public void Transmit(IntVec2 acc, int ropeIndex)
        {
            var joints = _ropes[ropeIndex];
            for (int i = ropeIndex + 1; i < joints.Count; i++)
            {
                joints[i].Transmit(acc);
            }
            for (int i = ropeIndex - 1; i >= 0; i--)
            {
                joints[i].Transmit(acc);
            }
        }

        public void UpdateView(float deltaTime)
        {
            foreach (var joints in _ropes.Values)
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    if (joints[i].IsInterest)
                    {
                        joints[i].UpdateView(deltaTime);
                    }
                }
            }
        }

        public int GetMaxHeight(int ropeIndex)
        {
            var joints = _ropes[ropeIndex];
            return joints[joints.Count - 1].CenterPos.y;
        }
        
        public int GetMinHeight(int ropeIndex)
        {
            var joints = _ropes[ropeIndex];
            return joints[0].CenterPos.y;
        }
    }
}