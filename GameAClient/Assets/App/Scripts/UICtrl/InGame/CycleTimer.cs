using UnityEngine;

namespace GameA.Game
{
    public class CycleTimer : MonoBehaviour
    {
        private const string FillAmoutName = "_FillAmount";
        public SpriteRenderer Sprite;
        public SpriteRenderer GreySprite;

        private void Awake()
        {
            GreySprite.sortingOrder = Sprite.sortingOrder = (int) ESortingOrder.Item;
        }

        public void SetSprite(Sprite sprite)
        {
            Sprite.sprite = sprite;
            GreySprite.sprite = sprite;
        }

        public void SetValue(float value)
        {
            var sprite = GreySprite.sprite;
            if (sprite != null)
            {
                var val = 1 - Mathf.Lerp(sprite.textureRect.yMax, sprite.textureRect.y, value) / sprite.texture.height;
                GreySprite.material.SetFloat(FillAmoutName, val);
            }
        }
    }
}