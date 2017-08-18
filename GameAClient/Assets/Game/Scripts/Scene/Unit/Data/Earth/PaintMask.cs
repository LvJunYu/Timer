using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class PaintMask : MonoBehaviour
    {
        private const int TextureSize = 64;
        [SerializeField]
        private Texture2D _mainMaskTexture;
        [SerializeField]
        private Texture2D[] _maskTextures = new Texture2D[8];
        [SerializeField]
        private Texture2D[] _maskWaterTextures = new Texture2D[4];

        private Color[][] _mainMaskColors;
        private List<Color[][]> _maskTextureColors = new List<Color[][]>();
        private List<Color[][]> _maskWaterTextureColors = new List<Color[][]>();

        public static PaintMask Instance;
        
        private static Color[] CacheColors = new Color[TextureSize * TextureSize];
        private static Color[] CacheMainColors = new Color[TextureSize * TextureSize];

        private void Awake()
        {
            Instance = this;
        }

        private Color[][] GetColors(Color[] colors, int width = TextureSize, int height = TextureSize)
        {
            var retColors = new Color[height][];
            for (int i = 0; i < height; i++)
            {
                retColors[i] = new Color[width];
                for (int j = 0; j < width; j++)
                {
                    retColors[i][j] = colors[i * width + j];
                }
            }
            return retColors;
        }

        private Color[] GetColors(Color[][] colors, int x, int y, int width, int height)
        {
            int count = 0;
            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    CacheColors[count] = colors[j][i];
                    count++;
                }
            }
            return CacheColors;
        }
        
        private Color[] GetMainColors(Color[][] colors, int x, int y, int width, int height)
        {
            int count = 0;
            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    CacheMainColors[count] = colors[j][i];
                    count++;
                }
            }
            return CacheMainColors;
        }

        public IEnumerator Init()
        {
            _mainMaskColors = GetColors(_mainMaskTexture.GetPixels());
            for (int i = 0; i < _maskTextures.Length; i++)
            {
                var colors = _maskTextures[i].GetPixels();
                _maskTextureColors.Add(GetColors(colors));
                yield return null;
            }
            for (int i = 0; i < _maskWaterTextures.Length; i++)
            {
                var colors = _maskWaterTextures[i].GetPixels();
                _maskWaterTextureColors.Add(GetColors(colors));
                yield return null;
            }
            yield return null;
        }
        
        public Color[] GetMainMaskColors(int x, int y, int width, int height)
        {
            return GetMainColors(_mainMaskColors, x, y, width, height);
        }

        public Color[] GetMaskColors(int edge, int num, int x, int y, int width, int height)
        {
            int i = 2 * edge + num;
            return GetColors(_maskTextureColors[i], x, y, width, height);
        }
        
        public Color[] GetWaterMaskColors(int edge, int num, int x, int y, int width, int height)
        {
            int i = edge;
            return GetColors(_maskWaterTextureColors[i], x, y, width, height);
        }
    }
}