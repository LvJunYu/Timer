/********************************************************************
** Filename : GameUIExternal  
** Author : ake
** Date : 4/22/2016 6:50:43 PM
** Summary : GameUIExternal  
***********************************************************************/


using NewResourceSolution;
using UnityEngine;
using UnityEngine.UI;

namespace SoyEngine
{
    public static class GameUIExternal
    {
        public static void SetSpriteEx(this Image image, string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName) || image == null)
            {
                return;
            }
            Sprite sprite;
            if (JoyResManager.Instance.TryGetSprite (spriteName, out sprite))
            {
                image.sprite = sprite;
            }
        }
    }
}
