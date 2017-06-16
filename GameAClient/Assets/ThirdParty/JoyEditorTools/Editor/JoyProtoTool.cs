using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Diagnostics;
using Debug=UnityEngine.Debug;

namespace JoyEditorTools {
    public class JoyProtoTool {
        /// <summary>
        /// 重新生成表代码和编辑器用的json数据
        /// </summary>
        [MenuItem("JoyTools/ProtoTools/BuildProtocols")]
        public static void BuildProtocols() {
            JoyToolUtility.RunBash (Application.dataPath + Define.BuildProtoShellPath);
            JoyToolUtility.RunBash (Application.dataPath + Define.BuildProtoMsgShellPath);
        }
    }
}