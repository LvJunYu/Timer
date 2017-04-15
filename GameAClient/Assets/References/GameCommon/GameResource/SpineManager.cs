/********************************************************************

** Filename : SpineManager
** Author : ake
** Date : 2016/3/16 14:30:20
** Summary : SpineManager
***********************************************************************/

using System;
using GameA.Game;
using SoyEngine;
using Spine.Unity;
using UnityEngine;

public class SpineManager:MonoBehaviour
{
    #region

    private static SpineManager _instance;

    #endregion

    #region 属性

    public static SpineManager Instance
    {
        get
        {
            return _instance;
        }
    }

    #endregion


    private void Awake()
    {
        _instance = this;
    }

    #region

    //public SkeletonAnimation CreateSpineObjectByName(string spineAssetName)
    //{
    //    SkeletonDataAsset data;
    //    if (!GameResourceManager.Instance.TryGetSpineDataByName(spineAssetName, out data))
    //    {
    //        LogHelper.Error("CreateSpineObjectByName failed spineAssetName is invalid! {0}",spineAssetName);
    //        return null;
    //    }
    //    var go = new GameObject("SpineObject_"+ spineAssetName);
    //    CommonTools.SetParent(go.transform,SceneManager.TempParent);
    //    var res = go.AddComponent<SkeletonAnimation>();
    //    res.skeletonDataAsset = data;
    //    res.Initialize(true);
    //    Renderer render = res.GetComponent<Renderer>();
    //    if (render != null)
    //    {
    //        render.sortingOrder = (int)ESortingOrder.Hero;
    //    }
    //    return res;
    //}

    #endregion

}