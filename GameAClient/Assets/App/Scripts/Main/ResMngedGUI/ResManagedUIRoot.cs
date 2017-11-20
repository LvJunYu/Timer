
using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
	public class ResManagedUIRoot: UIRoot
	{
		private readonly Dictionary<Type, InstanceHolder> _viewInstanceCache = new Dictionary<Type, InstanceHolder>(128);

		private readonly Dictionary<EResScenary, List<InstanceHolder>> _resScenaryCache =
			new Dictionary<EResScenary, List<InstanceHolder>>(64);
		
		protected override UIViewBase InstanceView(Type uictrlType)
		{
			InstanceHolder holder;
			if (!_viewInstanceCache.TryGetValue(uictrlType, out holder))
			{
				EResScenary resScenary = EResScenary.Default;
				var autoSetup = GUIManager.GetUIAutoSetupAttribute(uictrlType) as UIResAutoSetupAttribute;
				if (autoSetup != null)
				{
					resScenary = autoSetup.ResScenary;
				}
				var path = uictrlType.Name;
				holder = new InstanceHolder(path, resScenary);
				_viewInstanceCache.Add(uictrlType, holder);
				List<InstanceHolder> list;
				if (!_resScenaryCache.TryGetValue(resScenary, out list))
				{
					list = new List<InstanceHolder>();
					_resScenaryCache.Add(resScenary, list);
				}
				list.Add(holder);
			}
			GameObject go = holder.GetViewInstance();
            if (go == null)
            {
                LogHelper.Error(uictrlType.Name);
                return null;
            }
            var view = go.GetComponent<UIViewBase>();
            view.Init();
            view.Trans.SetParent(_trans, false);
			UIViewResManagedBase uiViewResManagedBase = view as UIViewResManagedBase;
			if (uiViewResManagedBase != null)
			{
				uiViewResManagedBase.ResScenary = holder.ResScenary;
			}
            return view;
        }
		
		public UMViewBase InstanceItemView(Type umctrlType, string path, EResScenary resScenary)
		{
			InstanceHolder holder;
			if (!_viewInstanceCache.TryGetValue(umctrlType, out holder))
			{
				holder = new InstanceHolder(path, resScenary);
				_viewInstanceCache.Add(umctrlType, holder);
				List<InstanceHolder> list;
				if (!_resScenaryCache.TryGetValue(resScenary, out list))
				{
					list = new List<InstanceHolder>();
					_resScenaryCache.Add(resScenary, list);
				}
				list.Add(holder);
			}
			GameObject go = holder.GetViewInstance();
			if (go == null)
			{
				LogHelper.Error("prefab is null");
				return null;
			}
			var umViewBase = go.GetComponent<UMViewBase>();
			UMViewResManagedBase umViewResManagedBase = umViewBase as UMViewResManagedBase;
			if (umViewResManagedBase != null)
			{
				umViewResManagedBase.ResScenary = holder.ResScenary;
			}
			return umViewBase;
		}

		public void RelinkSpriteReference(EResScenary resScenary)
		{
			List<InstanceHolder> list;
			if (!_resScenaryCache.TryGetValue(resScenary, out list))
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				var holder = list[i];
				holder.RelinkSpriteReference();
			}
		}
		
		private class InstanceHolder
		{
			private string _resPath;
			private EResScenary _resScenary;
			private GameObject _rootGo;
			private IUIResManagedView _rootHolder;
			private HashSet<IUIResManagedView> _instanceHolderSet = new HashSet<IUIResManagedView>();

			public EResScenary ResScenary
			{
				get { return _resScenary; }
			}

			public InstanceHolder(string resPath, EResScenary resScenary)
			{
				_resPath = resPath;
				_resScenary = resScenary;
			}

			public GameObject GetViewInstance()
			{
				if (!_rootGo)
				{
					_rootGo = JoyResManager.Instance.GetPrefab (EResType.UIPrefab, _resPath, (int) _resScenary) as GameObject;
					if (_rootGo == null)
					{
						LogHelper.Error("Instantiate ui failed {0}", _resPath);
						return null;
					}
					_rootHolder = _rootGo.GetComponent<IUIResManagedView>();

					if (!Application.isEditor || RuntimeConfig.Instance.UseAssetBundleRes)
					{
						if (_rootHolder != null)
						{
							_rootHolder.CollectionSpriteReference();
						}
					}
				}
				GameObject go = Instantiate(_rootGo);
				if (_rootHolder != null)
				{
					var view = go.GetComponent<IUIResManagedView>();
					view.AddOnDestoryCallback(OnViewDestory);
					_instanceHolderSet.Add(view);
				}
				return go;
			}

			private void OnViewDestory(IUIResManagedView view)
			{
				_instanceHolderSet.Remove(view);
			}

			public void RelinkSpriteReference()
			{
				if (_rootHolder != null)
				{
					_rootHolder.RelinkSpriteReference();
				}
				using (var itor = _instanceHolderSet.GetEnumerator())
				{
					while (itor.MoveNext())
					{
						if (itor.Current != null)
						{
							itor.Current.RelinkSpriteReference();
						}
					}
				}
			}
		}
		
		public interface IUIResManagedView
		{
			void CollectionSpriteReference();
			void RelinkSpriteReference();
			void AddOnDestoryCallback(Action<IUIResManagedView> cb);
		}
	}
}
