using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;

namespace NewResourceSolution
{
    public class UnityTools
    {
        public static IEnumerator WWWRequestWithTimeout (WWW request, int timeoutInMilliSeconds = 8000)
        {
            long startTick = DateTimeUtil.GetNowTicks ();
            bool isTimeout = false;
            while (!request.isDone)
            {
                if (DateTimeUtil.GetNowTicks() - startTick > timeoutInMilliSeconds * 10000)
                {
                    isTimeout = true;
                    break;
                }
                yield return null;
            }
            if (isTimeout)
            {
                LogHelper.Warning ("WWW request timeout, path: {0}", request.url);
//                request.Dispose();
                yield break;
            }
            if (null != request.error)
            {
                LogHelper.Error ("WWW request error, info: {0}, path: {1}", request.error, request.url);
//                request.Dispose();
                yield break;
            }
        }

        public static IEnumerator GetObjectFromServer<T> (string path, System.Action<T> setObjCB, int timeoutInMilliSeconds = 8000)
		{
            WWW request = new WWW (path);
            yield return WWWRequestWithTimeout (request, timeoutInMilliSeconds);
            if (!request.isDone || null != request.error || string.IsNullOrEmpty (request.text))
            {
                yield break;
            }
//            LogHelper.Info ("request.text: {0}", request.text);
            T obj = JsonTools.DeserializeObject<T> (request.text);
            if (null != setObjCB) {
                setObjCB.Invoke (obj);
            }
            request.Dispose ();
		}

        public static bool TryGetObjectFromLocal<T> (string fileName, out T obj)
        {
			string fullPath = StringUtil.Format (StringFormat.TwoLevelPath, ResPath.PersistentDataPath, fileName);
            return TryGetObjectFromStorage<T> (fullPath, out obj);
        }

        public static bool TryGetObjectFromStreamingAssets<T> (string fileName, out T obj)
        {
			string fullPath = StringUtil.Format (StringFormat.TwoLevelPath, ResPath.StreamingAssetsPath, fileName);
            return TryGetObjectFromStorage<T> (fullPath, out obj);
        }

		public static bool TryGetObjectFromTemporaryCache<T> (string fileName, out T obj)
		{
			string fullPath = StringUtil.Format (StringFormat.ThreeLevelPath, ResPath.PersistentDataPath, ResPath.TempCache, fileName);
			return TryGetObjectFromStorage<T> (fullPath, out obj);
		}

		public static bool TrySaveObjectToLocal<T> (T obj, string fileName, bool overwrite = true, bool debug = false)
        {
			string fullPath = StringUtil.Format (StringFormat.TwoLevelPath, ResPath.PersistentDataPath, fileName);
			return TrySaveObjectToStorage<T> (obj, fullPath, overwrite, debug);
        }

        private static bool TryGetObjectFromStorage<T> (string fullPath, out T obj)
        {
            obj = default(T);
            string s;
            if (FileTools.TryReadFileToString (fullPath, out s))
            {
                if (string.IsNullOrEmpty (s))
                {
                    return false;
                }
                obj = JsonTools.DeserializeObject<T> (s);
                return true;
            }
            else
            {
                return false;
            }
        }

		private static bool TrySaveObjectToStorage<T> (T obj, string fullPath, bool overwrite = true, bool debug = false)
        {
			#if UNITY_EDITOR
			debug = true;
			#endif
			string str = JsonTools.SerializeObject(obj, debug);
            return FileTools.WriteStringToFile (str, fullPath, overwrite);
        }
    }
}