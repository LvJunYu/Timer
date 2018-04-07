using UnityEngine;

namespace SoyEngine
{
	public static class TransformExtension 
    {
        public static void DestroyChildren(this Transform trans)
        {
            foreach (Transform child in trans)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static Transform AddChildFromPrefab(this Transform trans, Transform prefab, string name = null)
        {
            Transform childTrans = Object.Instantiate(prefab.gameObject).GetComponent<Transform>();
            childTrans.SetParent(trans, false);
            if (name != null)
	        {
                childTrans.gameObject.name = name;
	        }
            return childTrans;
        }
	}
}
