/********************************************************************
** Filename : Screenshot
** Author : Dong
** Date : 2015/11/26 星期四 下午 12:54:06
** Summary : Screenshot
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace SoyEngine
{
    public class Screenshot : MonoBehaviour
    {
        public void GetWholeScreenshot(Action<Texture2D> screenshotTexture)
        {
            StartCoroutine(GrabScreenshot(new Rect(0, 0, Screen.width, Screen.height), screenshotTexture));
        }

        public void GetScreenshot(Rect screenArea, Action<Texture2D> screenshotTexture)
        {
            if (screenArea.width == 0 && screenArea.height == 0)
            {
                LogHelper.Warning("screenArea is zero");
                return;
            }
            StartCoroutine(GrabScreenshot(screenArea, screenshotTexture));
        }

        private IEnumerator GrabScreenshot(Rect screenArea, Action<Texture2D> screenshotTexture)
        {
            yield return new WaitForEndOfFrame();
            var texture = new Texture2D((int)screenArea.width, (int)screenArea.height, TextureFormat.RGB24, false);
            texture.ReadPixels(screenArea, 0, 0);
            texture.Apply();
            if (screenshotTexture != null)
            {
                screenshotTexture.Invoke(texture);
            }
        }
    }
}
