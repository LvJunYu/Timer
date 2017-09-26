using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using SoyEngine;

namespace NewResourceSolution.EditorTool
{
    [CustomEditor(typeof(BuildABConfig))]
    public class BuildABConfigInspector : Editor
    {
    	private BuildABConfig _data;
    	/// <summary>
    	/// 是否在添加asset模式
    	/// </summary>
    	private int _addAssetMode;

    	/// <summary>
    	/// 上一次添加操作所添加的asset
    	/// </summary>
    	private Object _lastTryAddObj;

    	public void OnEnable()
    	{
    		_data = (BuildABConfig)target;
    		_addAssetMode = 0;
    		_lastTryAddObj = null;
    	}
    	public override void OnInspectorGUI()
    	{
    		bool dirty = false;

    		bool debug = EditorGUILayout.Toggle("Debug build", _data.Debug);
    		if (debug != _data.Debug)
    		{
    			_data.Debug = debug;
    			dirty = true;
    		}

    		string version = EditorGUILayout.TextField("Version", _data.Version);
    		if (string.Compare(version, _data.Version) != 0)
    		{
    			_data.Version = version;
    			dirty = true;
    		}
    		ELocale includeLocales = (ELocale)EditorGUILayout.EnumMaskPopup("Include locales", (ELocale)_data.IncludeLocales);
    		if ((int)includeLocales != _data.IncludeLocales)
    		{
    			_data.IncludeLocales = (int)includeLocales;
    			dirty = true;
    		}
    		string outputPath = EditorGUILayout.TextField("Output path", _data.OutputPath);
    		if (string.Compare(outputPath, _data.OutputPath) != 0)
    		{
    			_data.OutputPath = outputPath;
    			dirty = true;
    		}
    		EditorGUILayout.Separator();

    		bool fullResPackage = EditorGUILayout.Toggle("Full res package", _data.FullResPackage);
    		if (fullResPackage != _data.FullResPackage)
    		{
    			_data.FullResPackage = fullResPackage;
    			dirty = true;
    		}

    		if (!_data.FullResPackage)
    		{
    			if (2 != _addAssetMode)
    			{
    				// add button
    				if (0 == _addAssetMode)
    				{
    					ToolUtility.SetGUIColorLightGreen();
    					if (GUILayout.Button("Add buildin asset"))
    					{
    						_addAssetMode = 1;
    						_lastTryAddObj = null;
    						ActiveEditorTracker.sharedTracker.isLocked = true;
    					}
    					ToolUtility.SetGUIColorDefault();
    				}
    				else
    				{
    					ToolUtility.SetGUIColorLightRed();
    					if (GUILayout.Button("End"))
    					{
    						_addAssetMode = 0;
    						Selection.activeObject = target;
    						ActiveEditorTracker.sharedTracker.isLocked = false;
    					}
    					else
    					{
    						// add selected asset
    						if (_lastTryAddObj != Selection.activeObject)
    						{
                                dirty = AddAsset (AssetDatabase.GetAssetPath (Selection.activeObject)) || dirty;
    							_lastTryAddObj = Selection.activeObject;
    						}
    					}
    					ToolUtility.SetGUIColorDefault();
    				}

    				EditorGUILayout.BeginHorizontal();
    				var allResList = _data.AllResLists;
    				if (GUILayout.Button("Expand all"))
    				{
    					for (int i = 0; i < allResList.Count; i++)
    					{
    						allResList[i].FoldoutInEditorUI = true;
    					}
    					dirty = true;
    				}
    				ToolUtility.SetGUIColorLightRed();
    				if (GUILayout.Button("Remove all"))
    				{
    					if (EditorUtility.DisplayDialog(
    						"Warning",
    						"Do you want to remove all assets from in package assets list?",
    						"OK",
    						"Cancel"
    					))
    					{
    						Selection.activeObject = null;
    						_data.Clear();
    					}
    				}
    				ToolUtility.SetGUIColorDefault();
    				EditorGUILayout.EndHorizontal();

    				for (int i = 0; i < allResList.Count; i++)
    				{
    					EditorGUILayout.Separator();
    					DisplayResList(
                            ref allResList[i].FoldoutInEditorUI,
                            allResList[i].ResType.ToString(),
                            allResList[i].InPackageGuidList,
                            allResList[i].IsFolderRes,
                            AddAsset,
                            ref dirty);
    				}
    			}
    		}

    		// adam res part
    		if (1 != _addAssetMode)
    		{
    			if (0 == _addAssetMode)
    			{
    				ToolUtility.SetGUIColorLightGreen();
    				if (GUILayout.Button("Add adam asset"))
    				{
    					_addAssetMode = 2;
    					_lastTryAddObj = null;
    					ActiveEditorTracker.sharedTracker.isLocked = true;
    				}
    				ToolUtility.SetGUIColorDefault();
    			}
    			else
    			{
    				ToolUtility.SetGUIColorLightRed();
    				if (GUILayout.Button("End"))
    				{
    					_addAssetMode = 0;
    					Selection.activeObject = target;
    					ActiveEditorTracker.sharedTracker.isLocked = false;
    				}
    				else
    				{
    					// add selected asset
    					if (_lastTryAddObj != Selection.activeObject)
    					{
                            dirty = AddAdamAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) || dirty;
    						_lastTryAddObj = Selection.activeObject;
    					}
    				}
    				ToolUtility.SetGUIColorDefault();
    			}

    			bool alwaysFoldout = true;
                DisplayResList(ref alwaysFoldout, "Single adam res", _data.SingleAdamResList, false, AddAdamAsset, ref dirty);
    			EditorGUILayout.Separator();
                DisplayResList(ref alwaysFoldout, "Folder adam res", _data.FolderAdamResList, true, AddAdamAsset, ref dirty);
    		}

    		if (dirty)
    		{
    			EditorUtility.SetDirty(_data);
    		}
    	}

    	private bool AddAsset(string path)
    	{
    		if (string.IsNullOrEmpty(path)) return false;
    		EResType resType = ABUtility.GetRawResType(path);
    		if (EResType.None != resType)
    		{
    			var allResList = _data.AllResLists;
    			for (int i = 0; i < allResList.Count; i++)
    			{
    				if (allResList[i].ResType == resType)
    				{
    					allResList[i].FoldoutInEditorUI = true;
    					if (allResList[i].IsFolderRes)
    					{
    						path = System.IO.Directory.GetParent(path).ToString();
    					}
    					string guid = AssetDatabase.AssetPathToGUID(path);
    					if (!allResList[i].InPacakgeContains(guid))
    					{
    						return allResList[i].AddInPackageAsset(guid);
    					}
    				}
    			}
    		}
    		return false;
    	}

    	private bool AddAdamAsset(string path)
    	{
    		if (string.IsNullOrEmpty(path)) return false;
    		EResType resType = ABUtility.GetRawResType(path);
    		if (EResType.None != resType)
    		{
    			var allResList = _data.AllResLists;
    			for (int i = 0; i < allResList.Count; i++)
    			{
    				if (allResList[i].ResType == resType)
    				{
    					if (allResList[i].IsFolderRes)
    					{
    						path = System.IO.Directory.GetParent(path).ToString();
                            return !string.IsNullOrEmpty (_data.AddFolderAdamRes (path));
    					}
    					else
    					{
                            return !string.IsNullOrEmpty (_data.AddSingleAdamRes(path));
    					}
    				}
    			}
    		}
    		return false;
    	}

    	/// <summary>
    	/// 显示单一打包资源列表
    	/// </summary>
    	/// <param name="foldout">Foldout.</param>
    	/// <param name="title">Title.</param>
    	/// <param name="assetGuidList">Asset GUID list.</param>
    	/// <param name="dirty">Dirty.</param>
        private void DisplayResList(
            ref bool foldout,
            string title,
            List<string> assetGuidList,
            bool isfolderList,
            AddAssetDelegate addDependencDelegate,
            ref bool dirty)
    	{
    		bool newFoldout = EditorGUILayout.Foldout(foldout, title);
    		if (newFoldout != foldout)
    		{
    			dirty = true;
    			foldout = newFoldout;
    		}
    		if (!foldout) return;
    		for (int i = assetGuidList.Count - 1; i >= 0; i--)
    		{
    			string guid = assetGuidList[i];
    			GUILayout.BeginHorizontal();
    			if (GUILayout.Button("Remove", GUILayout.Width(60)))
    			{
    				if (assetGuidList[i] == AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.activeObject)))
    				{
    					Selection.activeObject = null;
    				}
    				assetGuidList.RemoveAt(i);
    				dirty = true;
    			}
    			GUILayout.Space(6);
    			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
    			if (string.IsNullOrEmpty(assetPath))
    			{
    				ToolUtility.SetGUIColorLightRed();
    				EditorGUILayout.LabelField("Missing");
    				ToolUtility.SetGUIColorDefault();
    			}
    			else
    			{
    				System.Type targetAssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
    				var assetObj = AssetDatabase.LoadAssetAtPath(assetPath, targetAssetType);
    				if (null != assetObj)
    				{
    					EditorGUILayout.ObjectField(assetObj, targetAssetType, false);
    					if (!isfolderList)
    					{
    						// add dependence
    						if (GUILayout.Button("Add dependencies", GUILayout.Width(120)))
    						{
    							string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);
    							for (int j = 0; j < dependencies.Length; j++)
    							{
    								// if not self, add to
                                    if (string.Compare (dependencies [j], assetPath) != 0)
                                    {
                                        dirty = addDependencDelegate (dependencies [j]) || dirty;
                                    }
    							}
    						}
    					}
    				}
    				else
    				{
    					ToolUtility.SetGUIColorLightRed();
    					EditorGUILayout.LabelField(assetPath);
    					ToolUtility.SetGUIColorDefault();
    				}
    			}
    			GUILayout.EndVertical();
    		}
    	}

        private delegate bool AddAssetDelegate (string newAsset); 
    }
}