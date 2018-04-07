using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Runtime
{
    public class BehaviorTreePool : IDisposable
    {
        private static BehaviorTreePool _instance;

        public static BehaviorTreePool Instance
        {
            get { return _instance ?? (_instance = new BehaviorTreePool()); }
        }

        private Stack<BehaviorTree> _freeBehaviorTrees = new Stack<BehaviorTree>();
        private Dictionary<string, ExternalBehaviorTree> _cache = new Dictionary<string, ExternalBehaviorTree>();
        private Transform _parent;

        private void CreateParent()
        {
            _parent = new GameObject("BehaviorTreePool").transform;
            _parent.position = new Vector3(-100000, -100000, 0);
        }

        private BehaviorTree CreateBehaviorTree()
        {
            if (_parent == null)
            {
                CreateParent();
            }

            BehaviorTree behaviorTree = new GameObject("Behavior").AddComponent<BehaviorTree>();
            behaviorTree.transform.SetParent(_parent, false);
            behaviorTree.StartWhenEnabled = false;
            behaviorTree.PauseWhenDisabled = true;
            behaviorTree.RestartWhenComplete = true;
            behaviorTree.ResetValuesOnRestart = false;
            behaviorTree.SaveResetValues();
            return behaviorTree;
        }

        private void CreateBehaviorManager()
        {
            Behavior.CreateBehaviorManager();
            BehaviorManager.instance.UpdateInterval = UpdateIntervalType.Manual;
        }

        public void Dispose()
        {
            foreach (var behaviorTree in _freeBehaviorTrees)
            {
                Object.Destroy(behaviorTree.gameObject);
            }

            _freeBehaviorTrees.Clear();
            _cache.Clear();
            if (_parent != null)
            {
                Object.Destroy(_parent.gameObject);
                _parent = null;
            }

            if (BehaviorManager.instance != null)
            {
                Object.Destroy(BehaviorManager.instance.gameObject);
            }

            _instance = null;
        }

        public BehaviorTree Get(string behaviorName)
        {
            ExternalBehaviorTree behavior;
            if (!_cache.TryGetValue(behaviorName, out behavior))
            {
                behavior = JoyResManager.Instance.GetBehavior(behaviorName);
                if (behavior == null)
                {
                    LogHelper.Error("GetBehavior fail, behaviorName = {0}", behaviorName);
                    return null;
                }

                _cache.Add(behaviorName, behavior);
            }

            if (null == BehaviorManager.instance)
            {
                CreateBehaviorManager();
            }

            BehaviorTree behaviorTree;
            if (_freeBehaviorTrees.Count > 0)
            {
                behaviorTree = _freeBehaviorTrees.Pop();
            }
            else
            {
                behaviorTree = CreateBehaviorTree();
            }

            behaviorTree.ExternalBehavior = behavior;
            return behaviorTree;
        }

        public void Free(BehaviorTree behaviour)
        {
            behaviour.ExternalBehavior = null;
            BehaviorManager.instance.DestroyBehavior(behaviour);
            _freeBehaviorTrees.Push(behaviour);
        }
    }
}