using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Diagnostics;
using Debug=UnityEngine.Debug;

namespace JoyEditorTools {
    public class JoyTableTool {
        /// <summary>
        /// 重新生成表代码和编辑器用的json数据
        /// </summary>
        [MenuItem("JoyTools/TableTools/BuildEditorTables")]
        public static void BuildEditorTables() {
            JoyToolUtility.RunBash (Application.dataPath + Define.BuildTableClientShellPath);
        }
        /// <summary>
        /// 重新生成编辑器用的json数据
        /// </summary>
        [MenuItem("JoyTools/TableTools/BuildEditorTables (data only)")]
        public static void BuildEditorTablesData() {
            JoyToolUtility.RunBash (Application.dataPath + Define.BuildTableClientDataShellPath);
        }
    }
}