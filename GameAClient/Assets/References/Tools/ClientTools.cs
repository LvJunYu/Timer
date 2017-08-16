/********************************************************************
** Filename : CommonTools
** Author : Dong
** Date : 2014/11/24 16:49:16
** Summary : CommonTools
***********************************************************************/

using System;
using DG.Tweening;
using GameA.Game;
using Spine;
using UnityEngine;
using Animation = Spine.Animation;
using Object = UnityEngine.Object;

namespace SoyEngine
{
    public static class ClientTools
    {
        public static GameObject InstantiateObject(Object asset)
        {
            return (GameObject) Object.Instantiate(asset);
        }

        public static void SetAllLayerIncludeHideObj(Transform root, int layer)
        {
            if (root == null)
            {
                LogHelper.Info("SetAllLayerIncludeHideObj, Root is Null!");
                return;
            }
            root.gameObject.layer = layer;
            int count = root.childCount;
            Transform tmpObj;
            for (int i = 0; i < count; i++)
            {
                tmpObj = root.GetChild(i);
                SetAllLayerIncludeHideObj(tmpObj, layer);
            }
        }

        /// <summary>
        ///     初始化Transform
        /// </summary>
        public static void SetParent(Transform trans, Transform parent)
        {
            if (trans != null && parent != null && trans.parent != parent)
            {
                trans.SetParent(parent);
                ResetTransform(trans);
            }
        }

        /// <summary>
        ///     重置Transform
        /// </summary>
        /// <param name="trans"></param>
        public static void ResetTransform(Transform trans)
        {
            if (trans == null)
            {
                return;
            }
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        public static void SetActive(GameObject go, bool value)
        {
            if (go == null)
            {
                return;
            }
            go.SetActive(value);
        }

        public static Transform FindChildDeep(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }
            for (int i = 0; i < root.childCount; i++)
            {
                Transform trans = root.GetChild(i);
                if (trans.name == name)
                {
                    return trans;
                }
                Transform result = FindChildDeep(trans, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static void SetTweener(Tweener tweener, Ease ease,Action finisCallback = null)
        {
            tweener.SetUpdate(true);
            tweener.SetEase(ease);
	        if (finisCallback == null)
	        {
				tweener.OnComplete(null);
			}
	        else
	        {
				tweener.OnComplete(new TweenCallback(finisCallback));
			}
        }

        /// <summary>  
        /// 对相机截图。   
        /// </summary>  
        /// <returns>The screenshot2.</returns>  
        /// <param name="camera">Camera.要被截屏的相机</param>  
        /// <param name="rect">Rect.截屏的区域</param>  
        public static Texture2D CaptureCamera(Camera camera, Vector2 screenSize, Rect rect)   
        {  
            // 创建一个RenderTexture对象
            RenderTexture rt = new RenderTexture((int)screenSize.x, (int)screenSize.y, 0);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
            camera.targetTexture = rt;  
            camera.Render();  
            //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
            //ps: camera2.targetTexture = rt;  
            //ps: camera2.Render();  
            //ps: -------------------------------------------------------------------  

            // 激活这个rt, 并从中中读取像素。  
            RenderTexture.active = rt; 
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);
            if(rect.x < 0)
            {
                screenShot.ReadPixels(new Rect(0, rect.y, rect.width, rect.height), -(int)rect.x, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
                Color blackColor = Color.black;
                for(int x=0; x<-rect.x; x++)
                {
                    for(int y=0; y<rect.height; y++)
                    {
                        screenShot.SetPixel(x, y, blackColor);
                        screenShot.SetPixel((int)(rect.width + rect.x + x), y, blackColor);
                    }
                }
            }
            else
            {
                screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
            }
            screenShot.Apply();  

            // 重置相关参数，以使用camera继续在屏幕上显示  
            camera.targetTexture = null;  
            //ps: camera2.targetTexture = null;  
            RenderTexture.active = null; // JC: added to avoid errors  
            GameObject.Destroy(rt);
            return screenShot;
        }
    }
}