
using System;
using SoyEngine;
using UnityEngine;
using NewResourceSolution;
using Object = UnityEngine.Object;

namespace GameA
{
	public class ResManagedUIRoot: UIRoot
	{
		protected override UIViewBase InstanceView(Type uictrlType)
		{
			EResScenary resScenary = EResScenary.Default;
		    var autoSetup = GUIManager.GetUIAutoSetupAttribute(uictrlType) as UIResAutoSetupAttribute;
			if (autoSetup != null)
			{
				resScenary = autoSetup.ResScenary;
			}
			var path = uictrlType.Name;
            Object obj = JoyResManager.Instance.GetPrefab (EResType.UIPrefab, path, (int) resScenary);
            if (obj == null)
            {
                LogHelper.Error("Instantiate ui failed {0}", path);
                return null;
            }
            GameObject go = Instantiate(obj) as GameObject;
            if (go == null)
            {
                LogHelper.Error(path);
                return null;
            }
            var view = go.GetComponent<UIViewBase>();
            view.Init();
            view.Trans.SetParent(_trans, false);
            go.SetActive(false);
            return view;
        }
		
		public virtual UMViewBase InstanceItemView(string path, EResScenary resScenary)
		{
			Object obj = JoyResManager.Instance.GetPrefab (EResType.UIPrefab, path, (int) resScenary);
			if (obj == null)
			{
				LogHelper.Error(path);
				return null;
			}
			GameObject go = Instantiate(obj) as GameObject;
			if (go == null)
			{
				LogHelper.Error("prefab is null");
				return null;
			}
			return go.GetComponent<UMViewBase>();
		}
	}
}
