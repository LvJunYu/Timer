using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;

namespace GameA
{
    public class ResManagedGuiManager : GUIManager
    {
        protected readonly Dictionary<EResScenary, List<UICtrlBase>> ResUICtrlDict =
            new Dictionary<EResScenary, List<UICtrlBase>>();

        protected override void InitUI(Type t)
        {
            base.InitUI(t);
            using (var itor = UITypeAttributeDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    var autoSetup = itor.Current.Value;
                    EResScenary resScenary = EResScenary.Default;
                    var resAutoSetup = autoSetup as UIResAutoSetupAttribute;
                    if (resAutoSetup != null)
                    {
                        resScenary = resAutoSetup.ResScenary;
                    }
                    List<UICtrlBase> uiCtrlBaseList;
                    if (!ResUICtrlDict.TryGetValue(resScenary, out uiCtrlBaseList))
                    {
                        uiCtrlBaseList = new List<UICtrlBase>();
                        ResUICtrlDict.Add(resScenary, uiCtrlBaseList);
                    }
                    var uictrl = UIRoot.GetUI(itor.Current.Key);
                    if (uictrl != null)
                    {
                        uiCtrlBaseList.Add(uictrl);
                    }
                }
            }
        }

        public void UnloadUI(EResScenary resScenary)
        {
#if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes) return;
#endif
            List<UICtrlBase> list;
            if (ResUICtrlDict.TryGetValue(resScenary, out list))
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].IsViewCreated)
                    {
                        list[j].Destroy();
                    }
                }
            }
            JoyResManager.Instance.UnloadScenary(resScenary);
        }

        public void LoadUI(EResScenary resScenary)
        {
#if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes) return;
#endif
            List<UICtrlBase> list;
            if (!ResUICtrlDict.TryGetValue(resScenary, out list))
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                var uictrl = list[i];
                if (uictrl.IsViewCreated)
                {
                    continue;
                }
                if (uictrl.IsOpen)
                {
                    CreateView(uictrl.GetType());
                    continue;
                }
                UIAutoSetupAttribute uiAutoSetupAttribute;
                if (UITypeAttributeDict.TryGetValue(uictrl.GetType(), out uiAutoSetupAttribute))
                {
                    if (uiAutoSetupAttribute.AutoSetupType != EUIAutoSetupType.Add)
                    {
                        CreateView(uictrl.GetType());
                    }
                }
            }
        }

        public void UnloadAtlas(EResScenary resScenary)
        {
#if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes) return;
#endif
            JoyResManager.Instance.UnloadScenary(resScenary, 1<<(int) EResType.Sprite);
        }

        public void LoadAtlas(EResScenary resScenary)
        {
#if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes) return;
#endif
            ((ResManagedUIRoot) UIRoot).RelinkSpriteReference(resScenary);
        }
    }
}