using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Diagnostics;
using Debug=UnityEngine.Debug;

namespace JoyEditorTools {
    public class JoyToolUtility {
        public static void RunBash(string shellPath) {
            ProcessStartInfo psi = new ProcessStartInfo ();
            psi.FileName = "bash";
            psi.Arguments = shellPath;

            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Process p = new Process ();
            p.StartInfo = psi;
            try {
                p.OutputDataReceived += (o, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data)) {
                        Debug.Log (e.Data);
                    }
                };
                p.Start();
                p.BeginOutputReadLine();
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
            finally {
                // 好像并没有用
                AssetDatabase.Refresh ();
            }
        }
    }
}