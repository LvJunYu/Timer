/********************************************************************
** Filename : UIButtonExInject  
** Author : ake
** Date : 4/29/2016 1:41:41 PM
** Summary : UIButtonExInject  
***********************************************************************/


using UnityEngine.UI;

namespace SoyEngine
{
    public partial class UIButtonExInject:BaseInjectHandler
    {
        public override bool InjectDependsAsset(InjectItem item)
        {
            if (!base.InjectDependsAsset(item))
            {
                LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
                return false;
            }
            if (item.AssetNameArray.Length != 3)
            {
                LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0},item.AssetNameArray.Length != 3", item.ToString());
                return false;
            }
            ButtonEx button = item.InjectObject as ButtonEx;
            if (button == null)
            {
                LogHelper.Error("item.InjectObject as Button is null! item is {0}", item.ToString());
                return false;
            }
            SpriteState state = new SpriteState();
            state.highlightedSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[0]);
            state.pressedSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[1]);
            state.disabledSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[2]);

            button.State = state;
            return true;
        }
    }
}
