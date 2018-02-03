using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game.AI
{
    public static class AIConfigHelper
    {
        private static Dictionary<int, Dictionary<EFSMStateType, List<TransformState>>> _cache =
            new Dictionary<int, Dictionary<EFSMStateType, List<TransformState>>>();

        private static bool _hasInited;
        private static List<TransformState> _allTransformCache; //所有状态都会进行的转换

        public static void Init()
        {
            _cache.Clear();
            var tables = TableManager.Instance.Table_AIConfigDic;
            foreach (var table in tables.Values)
            {
                if (!_cache.ContainsKey(table.AiType))
                {
                    _cache.Add(table.AiType, new Dictionary<EFSMStateType, List<TransformState>>());
                    _allTransformCache = null;
                }

                var dic = _cache[table.AiType];
                List<TransformState> transformList;
                //PreState为空是所有状态都要判断的转换
                if (string.IsNullOrEmpty(table.PreState))
                {
                    if (_allTransformCache == null)
                    {
                        _allTransformCache = new List<TransformState>();
                    }

                    transformList = _allTransformCache;
                }
                else
                {
                    var preStateType = (EFSMStateType) Enum.Parse(typeof(EFSMStateType), table.PreState, true);
                    if (!dic.ContainsKey(preStateType))
                    {
                        dic.Add(preStateType, new List<TransformState>());
                        //新的前置状态要添加
                        if (_allTransformCache != null)
                        {
                            dic[preStateType].AddRange(_allTransformCache);
                        }
                    }

                    transformList = dic[preStateType];
                }

                var targetStateType = (EFSMStateType) Enum.Parse(typeof(EFSMStateType), table.TargetState, true);
                EFSMConditionType[] conditions = new EFSMConditionType[2];
                float[][] conditionParas = new float[2][];
                if (!string.IsNullOrEmpty(table.Constion_1))
                {
                    conditions[0] = (EFSMConditionType) Enum.Parse(typeof(EFSMConditionType), table.Constion_1, true);
                    conditionParas[0] = table.ConstionValue_1;
                }

                if (!string.IsNullOrEmpty(table.Constion_2))
                {
                    conditions[1] = (EFSMConditionType) Enum.Parse(typeof(EFSMConditionType), table.Constion_2, true);
                    conditionParas[1] = table.ConstionValue_2;
                }

                transformList.Add(new TransformState(targetStateType, conditions, conditionParas));
            }

            _hasInited = true;
        }

        public static Dictionary<EFSMStateType, List<TransformState>> GetAIConfig(int aiType)
        {
            if (!_hasInited)
            {
                Init();
            }
            if (_cache.ContainsKey(aiType))
            {
                return _cache[aiType];
            }
            LogHelper.Error("GetAIConfig fail, aiType = {0}", aiType);
            return null;
        }

        public static void Clear()
        {
            _cache.Clear();
        }
    }

    public class TransformState
    {
        public EFSMStateType TargetStateType;
        public FSMConditionBase[] Constions;
        public float[][] ConditionParas;

        public TransformState(EFSMStateType targetStateType, EFSMConditionType[] constions, float[][] conditionParas)
        {
            TargetStateType = targetStateType;
            Constions = new FSMConditionBase[constions.Length];
            for (int i = 0; i < Constions.Length; i++)
            {
                Constions[i] = FSMFactory.GetFsmConditionObj(constions[i]);
            }

            ConditionParas = conditionParas;
        }
    }
}