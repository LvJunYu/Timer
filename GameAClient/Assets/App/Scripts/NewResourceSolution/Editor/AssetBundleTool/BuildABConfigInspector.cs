using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace NewResourceSolution.EditorTool
{
    [CustomEditor(typeof(BuildABConfig))]
    public class BuildABConfigInspector : Editor
    {
        private BuildABConfig _data;
		/// <summary>
		/// 是否在添加asset模式
		/// </summary>
		private bool _addAssetWhenSelect;

		/// <summary>
		/// 上一次添加操作所添加的asset
		/// </summary>
		private Object _lastTryAddObj;

        public void OnEnable()
        {
            _data = (BuildABConfig)target;
			_addAssetWhenSelect = false;
			_lastTryAddObj = null;
        }
        public override void OnInspectorGUI()
        {
			bool dirty = false;

			bool debug = EditorGUILayout.Toggle ("Debug build", _data.Debug);
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

            bool fullResPackage = EditorGUILayout.Toggle ("Full res package", _data.FullResPackage);
            if (fullResPackage != _data.FullResPackage)
            {
                _data.FullResPackage = fullResPackage;
                dirty = true;
            }

            if (!_data.FullResPackage)
            {
                // add button
                if (false == _addAssetWhenSelect) {
                    ToolUtility.SetGUIColorLightGreen ();
                    if (GUILayout.Button ("Add Asset")) {
                        _addAssetWhenSelect = true;
                        _lastTryAddObj = null;
                        ActiveEditorTracker.sharedTracker.isLocked = true;
                    }
                    ToolUtility.SetGUIColorDefault ();
                } else {
                    ToolUtility.SetGUIColorLightRed ();
                    if (GUILayout.Button ("End")) {
                        _addAssetWhenSelect = false;
                        Selection.activeObject = target;
                        ActiveEditorTracker.sharedTracker.isLocked = false;
                    } else {
                        // add selected asset
                        if (_lastTryAddObj != Selection.activeObject) {
                            AddAsset (AssetDatabase.GetAssetPath (Selection.activeObject), ref dirty);
                            _lastTryAddObj = Selection.activeObject;
                        }
                    }
                    ToolUtility.SetGUIColorDefault ();
                }

                EditorGUILayout.BeginHorizontal ();
                var allResList = _data.AllResLists;
                if (GUILayout.Button ("Expand all")) {
                    for (int i = 0; i < allResList.Count; i++)
                    {
                        allResList [i].FoldoutInEditorUI = true;
                    }
                    dirty = true;
                }
                ToolUtility.SetGUIColorLightRed ();
                if (GUILayout.Button ("Remove all")) {
                    if (EditorUtility.DisplayDialog (
                        "Warning",
                        "Do you want to remove all assets from in package assets list?",
                        "OK",
                        "Cancel"
                    )) {
                        Selection.activeObject = null;
                        _data.Clear ();
                    }
                }
                ToolUtility.SetGUIColorDefault ();
                EditorGUILayout.EndHorizontal ();

                for (int i = 0; i < allResList.Count; i++)
                {
                    EditorGUILayout.Separator ();
                    DisplayResList (ref allResList[i].FoldoutInEditorUI, allResList[i].ResType.ToString(), allResList[i].InPackageGuidList, true, ref dirty);
                }
            }
			if (dirty)
			{
            	EditorUtility.SetDirty(_data);
			}
        }

		private bool AddAsset (string path, ref bool dirty)
		{
			if (string.IsNullOrEmpty(path)) return false;
			EResType resType = ABUtility.GetRawResType(path);
            if (EResType.None != resType)
            {
                var allResList = _data.AllResLists;
                for (int i = 0; i < allResList.Count; i++)
                {
                    if (allResList [i].ResType == resType)
                    {
                        allResList [i].FoldoutInEditorUI = true;
                        if (allResList [i].IsFolderRes)
                        {
                            path = System.IO.Directory.GetParent (path).ToString ();
                        }
                        string guid = AssetDatabase.AssetPathToGUID (path);
                        if (!allResList [i].InPacakgeContains (guid))
                        {
                            allResList [i].AddInPackageAsset (guid);
                            dirty = true;
                            return true;
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
		private void DisplayResList (ref bool foldout, string title, List<string> assetGuidList, bool isfolderList, ref bool dirty)
		{
			bool newFoldout = EditorGUILayout.Foldout(foldout, title);
            if (newFoldout != foldout)
            {
                dirty = true;
                foldout = newFoldout;
            }
			if (!foldout) return;
			for (int i = assetGuidList.Count - 1; i >= 0 ; i--)
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
                            if (GUILayout.Button ("Add dependencies", GUILayout.Width (120)))
                            {
                                string[] dependencies = AssetDatabase.GetDependencies (assetPath, true);
                                for (int j = 0; j < dependencies.Length; j++)
                                {
                                    // if not self, add to
                                    if (string.Compare (dependencies [j], assetPath) != 0)
                                    {
                                        AddAsset (dependencies [j], ref dirty);
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
    }
}