using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// 解决项目中一样的资源（名字或者路径不同）存在两份的问题 （多人做UI出现的问题，或者美术没有管理好资源）
/// </summary>
public class ReplaceRef : EditorWindow
{
    private static ReplaceRef _window;
    private Object _sourceOld;
    private Object _sourceNew;
    private string _oldGuid;
    private string _newGuid;
    private bool isContainScene;
    private bool isContainPrefab = true;
    private bool isContainMat;
    private bool isContainAsset;
    private const string _searchRoot = "Assets/JoyGameArt/Locale/WW/Prefabs/UI";
    private List<string> withoutExtensions = new List<string>();
    private List<string> _list = new List<string>();

    [MenuItem("Tools/ReplaceRefGUID")]
    public static void ReplaceRefGUID()
    {
        _window = (ReplaceRef) GetWindowWithRect(typeof(ReplaceRef), new Rect(0, 0, 600, 600), true, "引用替换 (●'◡'●)");
        _window.Show();
    }

    void OnGUI()
    {
        // 要被替换的（需要移除的）
        GUILayout.Space(20);
        _sourceOld = EditorGUILayout.ObjectField("旧的资源", _sourceOld, typeof(Object), true);
        _sourceNew = EditorGUILayout.ObjectField("新的资源", _sourceNew, typeof(Object), true);

        // 在那些类型中查找（.unity\.prefab\.mat）
        GUILayout.Space(20);
        GUILayout.Label("要在哪些类型中查找替换：");
        EditorGUILayout.BeginHorizontal();
        isContainScene = GUILayout.Toggle(isContainScene, ".unity");
        isContainPrefab = GUILayout.Toggle(isContainPrefab, ".prefab");
        isContainMat = GUILayout.Toggle(isContainMat, ".mat");
        isContainAsset = GUILayout.Toggle(isContainAsset, ".asset");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
        if (GUILayout.Button("查找引用!"))
        {
            Prepare(false);
        }
        GUILayout.Space(20);
        if (GUILayout.Button("直接替换!"))
        {
            Prepare(true);
        }
        GUILayout.Space(20);
        for (int i = 0; i < _list.Count; i++)
        {
            GUILayout.Label(_list[i]);
        }
    }

    private void Prepare(bool replace)
    {
        if (EditorSettings.serializationMode != SerializationMode.ForceText)
        {
            Debug.LogError("需要设置序列化模式为 SerializationMode.ForceText");
            ShowNotification(new GUIContent("需要设置序列化模式为 SerializationMode.ForceText"));
        }
        else if (_sourceNew == null || _sourceOld == null)
        {
            Debug.LogError("不能为空！");
            ShowNotification(new GUIContent("不能为空！"));
        }
        else if (_sourceNew.GetType() != _sourceOld.GetType())
        {
            Debug.LogError("两种资源类型不一致！");
            ShowNotification(new GUIContent("两种资源类型不一致！"));
        }
        else if (!isContainScene && !isContainPrefab && !isContainMat && !isContainAsset)
        {
            Debug.LogError("要选择一种 查找替换的类型");
            ShowNotification(new GUIContent("要选择一种 查找替换的类型"));
        }
        else
        {
            _oldGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_sourceOld));
            _newGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_sourceNew));
            Debug.Log("oldGUID = " + _oldGuid + "  " + "_newGuid = " + _newGuid);
            withoutExtensions = new List<string>();
            if (isContainScene)
            {
                withoutExtensions.Add(".unity");
            }
            if (isContainPrefab)
            {
                withoutExtensions.Add(".prefab");
            }
            if (isContainMat)
            {
                withoutExtensions.Add(".mat");
            }
            if (isContainAsset)
            {
                withoutExtensions.Add(".asset");
            }
            FindAndReplace(replace);
        }
    }

    private void FindAndReplace(bool replace)
    {
        if (withoutExtensions == null || withoutExtensions.Count == 0)
        {
            withoutExtensions = new List<string> {".prefab", ".unity", ".mat", ".asset"};
        }
        string[] files = Directory.GetFiles(_searchRoot, "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        int startIndex = 0;
        if (files == null || files.Length == 0)
        {
            Debug.Log("没有找到文件");
            return;
        }
        _list.Clear();
        EditorApplication.update = delegate
        {
            string file = files[startIndex];
            bool isCancel =
                EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, startIndex / (float) files.Length);
            var content = File.ReadAllText(file);
            if (Regex.IsMatch(content, _oldGuid))
            {
                Debug.Log(file);
                _list.Add(file);
                if (replace)
                {
                    content = content.Replace(_oldGuid, _newGuid);
                    File.WriteAllText(file, content);
                }
            }
            startIndex++;
            if (isCancel || startIndex >= files.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                AssetDatabase.Refresh();
                if (replace)
                {
                    Debug.Log("替换结束");
                    _list.Add("替换结束");
                }
                else
                {
                    Debug.Log("查找结束");
                    _list.Add("查找结束");
                }
            }
        };
    }

    private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}