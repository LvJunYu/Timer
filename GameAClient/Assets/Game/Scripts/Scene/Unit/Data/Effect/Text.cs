using NewResourceSolution;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 9001, Type = typeof(Text))]
    public class Text : UnitBase
    {
        internal override bool InstantiateView()
        {
//            var go = UnityEngine.Object.Instantiate (ResourcesManager.Instance.GetPrefab(
//                EResType.ModelPrefab, 
//                ConstDefineGM2D.TextBillboardPrefabName)
//            ) as GameObject;
            return base.InstantiateView();
        }
    }
}