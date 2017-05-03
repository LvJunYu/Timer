using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SoyEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class JoySceneManager
    {
        internal static readonly JoySceneManager Instance = new JoySceneManager();
        private int[] _emptySceneArray = new int[] { 1, 2 };

        private JoySceneManager()
        {
        }

        internal void Init()
        {
            Messenger.AddListener(EMessengerType.LoadEmptyScene, LoadEmptyScene);
        }        

        internal void LoadEmptyScene()
        {
			SocialApp.Instance.EventSystem.SetActiveEx(true);
			int curInx = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            for (int i = 0; i < _emptySceneArray.Length; i++)
            {
                if (_emptySceneArray[i] != curInx)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(_emptySceneArray[i]);
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                    break;
                }
            }
        }
    }
}

