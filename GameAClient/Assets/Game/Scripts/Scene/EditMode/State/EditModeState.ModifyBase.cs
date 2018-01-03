using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public abstract class ModifyBase : Base
        {
            public class Data
            {
                public bool HasInited;
                public List<UnityNativeParticleItem> RedMaskEffectCache = new List<UnityNativeParticleItem>();
                public List<UnityNativeParticleItem> YellowMaskEffectCache = new List<UnityNativeParticleItem>();
                public float LastShowUnitPosEffectTime = 0;
                public UnitBase CurrentMovingUnitBase;
                public Transform MovingRoot;
                public Vector2 MouseActualPos;
                public Vector3 MouseObjectOffsetInWorld;
                /// <summary>
                /// 正在拖拽的地块的Extra
                /// </summary>
                public UnitExtra DragUnitExtra { get; set; }
            }

            private static readonly Vector2 MaskEffectOffset = new Vector2(0.35f, 0.4f);

            public List<ModifyData> RemovedUnits
            {
                get { return DataScene2D.CurScene.RemovedUnits; }
            }

            public List<ModifyData> ModifiedUnits
            {
                get { return DataScene2D.CurScene.ModifiedUnits; }
            }

            public List<ModifyData> AddedUnits
            {
                get { return DataScene2D.CurScene.AddedUnits; }
            }

            public override void Init()
            {
                var data = GetBlackBoard().GetStateData<Data>();
                if (data.HasInited)
                {
                    return;
                }
                data.HasInited = true;
            }

            public override void Dispose()
            {
                var data = GetBlackBoard().GetStateData<Data>();
                if (!data.HasInited)
                {
                    return;
                }

                for (int i = 0; i < data.RedMaskEffectCache.Count; i++)
                {
                    data.RedMaskEffectCache[i].DestroySelf();
                }
                data.RedMaskEffectCache.Clear();
                for (int i = 0; i < data.YellowMaskEffectCache.Count; i++)
                {
                    data.YellowMaskEffectCache[i].DestroySelf();
                }
                data.YellowMaskEffectCache.Clear();

                data.HasInited = false;
            }


            public int UsedModifyAddUnitCnt(int unitId)
            {
                int result = 0;
                for (int i = 0; i < DataScene2D.CurScene.AddedUnits.Count; i++)
                {
                    if (DataScene2D.CurScene.AddedUnits[i].ModifiedUnit.UnitDesc.Id == unitId)
                    {
                        result++;
                    }
                }
                return result;
            }

            public void ShowUnitPosEffect(IntVec3 guid)
            {
                var data = GetBlackBoard().GetStateData<Data>();
                if (Time.timeSinceLevelLoad - data.LastShowUnitPosEffectTime < 0.5f)
                    return;
                data.LastShowUnitPosEffectTime = Time.timeSinceLevelLoad;
                Vector3 pos = GM2DTools.TileToWorld(guid);
                pos.z = -60;
                pos.x += MaskEffectOffset.x;
                pos.y += MaskEffectOffset.y;
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.HereItIs, pos);
            }

            protected UnityNativeParticleItem GetUnusedYellowMask(int idx)
            {
                var data = GetBlackBoard().GetStateData<Data>();
                if (data.YellowMaskEffectCache.Count <= idx)
                {
                    UnityNativeParticleItem newYellowMask =
                        GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.YellowMask,
                            null);
                    if (null == newYellowMask)
                    {
                        LogHelper.Error("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.YellowMask);
                        return null;
                    }
                    data.YellowMaskEffectCache.Add(newYellowMask);
                }
                return data.YellowMaskEffectCache[idx];
            }

            protected UnityNativeParticleItem GetUnusedRedMask(int idx)
            {
                var data = GetBlackBoard().GetStateData<Data>();
                if (data.RedMaskEffectCache.Count <= idx)
                {
                    UnityNativeParticleItem newRedMask =
                        GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.RedMask,
                            null);
                    if (null == newRedMask)
                    {
                        LogHelper.Error("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.RedMask);
                        return null;
                    }
                    data.RedMaskEffectCache.Add(newRedMask);
                }
                return data.RedMaskEffectCache[idx];
            }

            protected void SetMaskEffectPos(UnityNativeParticleItem effect, IntVec3 guid)
            {
                Vector3 pos = GM2DTools.TileToWorld(guid);
                pos.z = -60f;
                pos.x += MaskEffectOffset.x;
                pos.y += MaskEffectOffset.y;
                effect.Trans.position = pos;
                effect.Play();
            }
        }

        public abstract class ModifyGenericBase<T> : ModifyBase where T : class, new()
        {
            private static T _internalInstance;

            public static T Instance
            {
                get { return _internalInstance ?? (_internalInstance = new T()); }
            }

            public static void Release()
            {
                _internalInstance = null;
            }
        }
    }
}