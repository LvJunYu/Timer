/********************************************************************
** Filename : PaintBlock
** Author : Dong
** Date : 2017/5/24 星期三 下午 3:48:11
** Summary : PaintBlock
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class PaintBlock : BlockBase
    {
        public const int MinEdgeLength = 0;
        public const int OffsetEdgeLength = 10;
        /// <summary>
        /// 被涂装的边信息，有序从小到大 
        /// </summary>
        protected List<Edge> _edges = new List<Edge>();
        private static Comparison<Edge> _comparisonSkillType = SortEdge;
        protected GameObject _paintObject;
        private Texture2D _paintTexture;
        private Texture2D _maskTexture;

        private static Color32[] EmptyPixels;
        private const int Ratio = 1;
        private const int PixelsPerUnit = 256 / (Ratio * 2);
        private const float PixelsPerTile = 256f / (640 * 2);
        private static Color CleanColor = new Color(1f, 1f, 1f, 0f);
        private static Color EdgeColor = new Color32(111, 47, 11, 255);

        private static Color[] PaintUpColor = new Color[4]
        {
            new Color32(255, 128, 36, 255),
            new Color32(197, 245, 246, 255), 
            new Color32(241, 213, 74, 255),
            new Color32(141, 254, 184, 255)
        };
        private static Color[] PaintRightColor = new Color[4]
        {
            new Color32(214, 32, 31, 255),
            new Color32(109, 202, 207, 255),
            new Color32(181, 123, 35, 255),
            new Color32(42, 185, 170, 255),
        };

        private static Color[] PaintFrontColor = new Color[4]
        {
            new Color32(238, 91, 47, 255),
            new Color32(142, 231, 246, 255),
            new Color32(221, 183, 53, 255),
            new Color32(40, 221, 152, 255),
        };
        
        private static Color[] PaintEdgeColor = new Color[4]
        {
            new Color32(255, 209, 111, 255),
            new Color32(240, 245, 246, 255),
            new Color32(250, 234, 104, 255),
            new Color32(210, 255, 226, 255),
        };

        private static int TileOffsetX = (int)(22f / PixelsPerTile);
        private static int TileOffsetY = (int)(14 / PixelsPerTile);
        public static int TileOffsetHeight = (int)(45 / PixelsPerTile);

        public override bool CanPainted
        {
            get { return true; }
        }

        protected override void Clear()
        {
            base.Clear();
            _edges.Clear();
            if (_paintTexture != null)
            {
                Object.Destroy(_paintTexture);
                _paintTexture = null;
            }
            if (_maskTexture != null)
            {
                Object.Destroy(_maskTexture);
                _maskTexture = null;
            }
            if (_paintObject != null)
            {
                Object.Destroy(_paintObject);
                _paintObject = null;
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsHero && !checkOnly)
            {
                CheckEdgeHit(other, EDirectionType.Up);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsHero && !checkOnly)
            {
                CheckEdgeHit(other, EDirectionType.Down);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsHero && !checkOnly)
            {
                CheckEdgeHit(other, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsHero && !checkOnly)
            {
                CheckEdgeHit(other, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        /// <summary>
        /// 倒排
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int SortEdge(Edge x, Edge y)
        {
            return y.ESkillType.CompareTo(x.ESkillType);
        }

        public override void DoPaint(int start, int end, EDirectionType direction, ESkillType eSkillType, bool draw = true)
        {
            var center = (start + end) * 0.5f * ConstDefineGM2D.ClientTileScale;
            if (!GetPos(ref start, ref end, direction))
            {
                return;
            }
            var edge = new Edge(start, end, direction, eSkillType);
            if (eSkillType == ESkillType.Water)
            {
                Cut(ref edge);
            }
            else
            {
                Merge(ref edge);
                Cut(ref edge);
                _edges.Add(edge);
            }
            //排序下
            if (_edges.Count > 1)
            {
                _edges.Sort(_comparisonSkillType);
            }
            if (draw)
            {
                UpdateTexture(ref edge, center);
            }
        }

        private void Merge(ref Edge edge)
        {
            while (true)
            {
                bool hasMerge = false;
                for (int i = _edges.Count - 1; i >= 0; i--)
                {
                    var current = _edges[i];
                    if (edge.Direction == current.Direction && edge.ESkillType == current.ESkillType)
                    {
                        if (edge.Merge(ref current))
                        {
                            hasMerge = true;
                            _edges.RemoveAt(i);
                        }
                    }
                }
                if (!hasMerge)
                {
                    break;
                }
            }
        }

        private void Cut(ref Edge edge)
        {
            for (int i = _edges.Count - 1; i >= 0; i--)
            {
                //判断同面
                if (_edges[i].Direction == edge.Direction)
                {
                    //不同类切割添加
                    if (_edges[i].ESkillType != edge.ESkillType)
                    {
                        _edges[i].Cut(ref edge, _edges);
                    }
                }
            }
        }

        protected bool GetPos(UnitBase other, EDirectionType eDirectionType, out int start, out int end)
        {
            start = 0;
            end = 0;
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                case EDirectionType.Down:
                    start = Math.Max(_colliderGrid.XMin, other.ColliderGrid.XMin);
                    end = Math.Min(_colliderGrid.XMax, other.ColliderGrid.XMax);
                    break;
                case EDirectionType.Left:
                case EDirectionType.Right:
                    start = Math.Max(_colliderGrid.YMin, other.ColliderGrid.YMin);
                    end = Math.Min(_colliderGrid.YMax, other.ColliderGrid.YMax);
                    break;
            }
            return end >= start + MinEdgeLength;
        }

        protected bool GetPos(ref int start, ref int end, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                case EDirectionType.Down:
                    start = Math.Max(_colliderGrid.XMin, start);
                    end = Math.Min(_colliderGrid.XMax, end);
                    break;
                case EDirectionType.Left:
                case EDirectionType.Right:
                    start = Math.Max(_colliderGrid.YMin, start);
                    end = Math.Min(_colliderGrid.YMax, end);
                    break;
            }
            return end >= start + MinEdgeLength;
        }

        private void UpdateTexture(ref Edge edge, float center)
        {
            if (_paintObject == null)
            {
                CreatePaintObject();
            }
            int pixelX = 0, pixelY = 0, offsetX = 0, offsetY = 0, width = 0, height = 0;
            Texture maskingTexture = null;
            switch (edge.Direction)
            {
                case EDirectionType.Up:
                    {
                        offsetX = (int) ((center - _trans.position.x) * PixelsPerUnit);
                        pixelX = (int)((edge.Start - TileOffsetX - (_colliderGrid.XMin - 320)) * PixelsPerTile);
                        pixelY = (int)((_colliderGrid.YMax - TileOffsetY - TileOffsetHeight - (_colliderGrid.YMin - 320)) * PixelsPerTile);
                        width = (int)((edge.End + TileOffsetX - (edge.Start - TileOffsetX)) * PixelsPerTile);
                        height = (int)((TileOffsetY + TileOffsetY + TileOffsetHeight) * PixelsPerTile);
                    }
                    break;
                case EDirectionType.Down:
                    {
                        offsetX = (int)((center - _trans.position.x) * PixelsPerUnit);
                        pixelX = (int)((edge.Start - TileOffsetX - (_colliderGrid.XMin - 320)) * PixelsPerTile);
                        pixelY = (int)((_colliderGrid.YMin - TileOffsetY - (_colliderGrid.YMin - 320)) * PixelsPerTile);
                        width = (int)((edge.End + TileOffsetX - (edge.Start - TileOffsetX)) * PixelsPerTile);
                        height = (int)((TileOffsetY + TileOffsetY + TileOffsetHeight) * PixelsPerTile);
                    }
                    break;
                case EDirectionType.Left:
                    {
                        offsetY = (int)((center - _trans.position.y) * PixelsPerUnit);
                        pixelX = (int)((_colliderGrid.XMin - TileOffsetX - (_colliderGrid.XMin - 320)) * PixelsPerTile);
                        pixelY = (int)((edge.Start - TileOffsetY - (_colliderGrid.YMin - 320)) * PixelsPerTile);
                        width = (int)((TileOffsetX + TileOffsetX + TileOffsetHeight) * PixelsPerTile);
                        height = (int)((edge.End + TileOffsetY - (edge.Start - TileOffsetY)) * PixelsPerTile);
                    }
                    break;
                case EDirectionType.Right:
                    {
                        offsetY = (int)((center - _trans.position.y) * PixelsPerUnit);
                        pixelX = (int)((_colliderGrid.XMax - TileOffsetX - TileOffsetHeight - (_colliderGrid.XMin - 320)) * PixelsPerTile);
                        pixelY = (int)((edge.Start - TileOffsetY - (_colliderGrid.YMin - 320)) * PixelsPerTile);
                        width = (int)((TileOffsetX + TileOffsetX + TileOffsetHeight) * PixelsPerTile);
                        height = (int)((edge.End + TileOffsetY - (edge.Start - TileOffsetY)) * PixelsPerTile);
                    }
                    break;
            }
            if (!GameResourceManager.Instance.TryGetTextureByName(string.Format("Mask_{0}_{1}", (int)edge.Direction,UnityEngine.Random.Range(0,2)), out maskingTexture))
            {
                LogHelper.Error("TryGetSpriteByName Failed");
                return;
            }
            if (maskingTexture == null)
            {
                return;
            }
            if (edge.ESkillType == ESkillType.Water)
            {
                edge.ESkillType++;
                //return;
            }
            pixelX = Mathf.Clamp(pixelX, 0, 255);
            pixelY = Mathf.Clamp(pixelY, 0, 255);
            var paintedColor = _paintTexture.GetPixels(pixelX, pixelY, width, height);
            var maskedColor = _maskTexture.GetPixels(pixelX, pixelY, width, height);
            Color[] maskingColor = ((Texture2D)maskingTexture).GetPixels(Mathf.Clamp(pixelX - offsetX, 0, 255), Mathf.Clamp(pixelY - offsetY, 0, 255), width, height);
            var maskBaseColor = PlayMode.Instance.MaskBaseTexture.GetPixels(pixelX, pixelY, width, height);
            for (int i = 0; i < paintedColor.Length; i++)
            {
                if (maskBaseColor[i].a == 0f)
                {
                    continue;
                }
                //叠加mask
                if (maskedColor[i].a == 0f || (maskedColor[i].maxColorComponent < 0.2f && maskingColor[i].maxColorComponent >= 0.2f))
                {
                    maskedColor[i] = maskingColor[i];
                }
                //直接不显示
                if (maskedColor[i].a == 0f)
                {
                    continue;
                }
                if (maskedColor[i].maxColorComponent < 0.2f)
                {
                    paintedColor[i] = EdgeColor;
                }
                else
                {
                    if (maskBaseColor[i].r == 1f)
                    {
                        paintedColor[i] = PaintUpColor[(int)edge.ESkillType];
                    }
                    else if (maskBaseColor[i].g == 1f)
                    {
                        paintedColor[i] = PaintRightColor[(int)edge.ESkillType];
                    }
                    else if (maskBaseColor[i].b == 1f)
                    {
                        paintedColor[i] = PaintFrontColor[(int)edge.ESkillType];
                    }
                    else
                    {
                        paintedColor[i] = PaintEdgeColor[(int)edge.ESkillType];
                    }
                }
            }
            _paintTexture.SetPixels(pixelX, pixelY, width, height, paintedColor);
            _paintTexture.Apply();
            _maskTexture.SetPixels(pixelX, pixelY, width, height, maskedColor);
            _maskTexture.Apply();
            LogHelper.Debug("{0} | {1} | {2} | {3} | {4}", pixelX, pixelY, width, height, offsetX);
        }

        private void CreatePaintObject()
        {
            if (_view != null)
            {
                var sr = _view.Trans.GetComponent<SpriteRenderer>();
                int textureWidth = (int)sr.sprite.rect.width / Ratio;
                int textureHeight = (int)sr.sprite.rect.height / Ratio;
                if (EmptyPixels == null)
                {
                    var count = textureWidth * textureHeight;
                    EmptyPixels = new Color32[count];
                    for (int i = 0; i < count; i++)
                    {
                        EmptyPixels[i] = CleanColor;
                    }
                }
                _paintTexture = new Texture2D(textureWidth, textureHeight);
                _paintTexture.wrapMode = TextureWrapMode.Clamp;
                _paintTexture.filterMode = FilterMode.Point;
                _paintTexture.SetPixels32(EmptyPixels);
                _paintTexture.Apply();

                _maskTexture = new Texture2D(textureWidth, textureHeight);
                _maskTexture.filterMode = FilterMode.Point;
                _maskTexture.SetPixels32(EmptyPixels);
                _maskTexture.Apply();

                _paintObject = new GameObject("Paint");
                CommonTools.SetParent(_paintObject.transform, _view.Trans);
                _paintObject.transform.localPosition = Vector3.back * 0.01f;
                var paintRenderer = _paintObject.AddComponent<SpriteRenderer>();
                paintRenderer.sortingOrder = sr.sortingOrder;
                paintRenderer.sprite = Sprite.Create(_paintTexture, new Rect(0f, 0f, _paintTexture.width, _paintTexture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0,
                    SpriteMeshType.FullRect);
            }
        }

        protected void CheckEdgeHit(UnitBase other, EDirectionType eDirectionType)
        {
            int start, end;
            if (GetPos(other, eDirectionType, out start, out end))
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    if (_edges[i].Direction == eDirectionType && _edges[i].Intersect(start, end))
                    {
                        OnEdgeHit(other, _edges[i]);
                    }
                }
            }
        }

        protected void OnEdgeHit(UnitBase other, Edge edge)
        {
            //LogHelper.Debug("OnEdgeHit: {0}", edge);
            switch (edge.ESkillType)
            {
                case ESkillType.Fire:
                    Fire.OnEffect(other);
                    break;
                case ESkillType.Jelly:
                    Jelly.OnEffect(other, edge.Direction);
                    break;
                case ESkillType.Clay:
                    break;
                //搞定 不用管
                case ESkillType.Ice:
                    break;
            }
        }

        //private void UpdateMesh()
        //{
        //    _combineInstances.Clear();
        //    if (_paintMesh == null)
        //    {
        //        _paintMesh = new Mesh();
        //    }
        //    for (int i = 0; i < _edges.Count; i++)
        //    {
        //        var edge = _edges[i];
        //        _combineInstances.Add(new CombineInstance { mesh = CreateMesh(ref edge), transform = Matrix4x4.identity });
        //    }
        //}

        //private Mesh CreateMesh(ref Edge edge)
        //{
        //    var mesh = new Mesh();
        //    Vector2 v1, v2, v3;
        //    Vector2 v0 = v1 = v2 = v3 = Vector2.zero;
        //    float start = (edge.Start - OffsetEdgeLength) * ConstDefineGM2D.ClientTileScale;
        //    float end = (edge.End + OffsetEdgeLength) * ConstDefineGM2D.ClientTileScale;
        //    switch (edge.Direction)
        //    {
        //        case EDirectionType.Up:
        //            {
        //                float y = _colliderGrid.YMax * ConstDefineGM2D.ClientTileScale;
        //                v0 = new Vector2(start - 0.15f, y - 0.1f);
        //                v1 = new Vector2(end - 0.15f, y - 0.1f);
        //                v2 = new Vector2(start + 0.15f, y + 0.1f);
        //                v3 = new Vector2(end + 0.15f, y + 0.1f);
        //            }
        //            break;
        //        case EDirectionType.Down:
        //            {
        //                float y = _colliderGrid.YMin * ConstDefineGM2D.ClientTileScale;
        //                v0 = new Vector2(start - 0.15f, y - 0.1f);
        //                v1 = new Vector2(end - 0.15f, y - 0.1f);
        //                v2 = new Vector2(start - 0.15f, y + 0.1f);
        //                v3 = new Vector2(end - 0.15f, y + 0.1f);
        //            }
        //            break;
        //        case EDirectionType.Left:
        //            {
        //                float x = _colliderGrid.XMin * ConstDefineGM2D.ClientTileScale;
        //                v0 = new Vector2(x - 0.15f, start - 0.1f);
        //                v1 = new Vector2(x + 0.15f, start - 0.1f);
        //                v2 = new Vector2(x - 0.15f, end - 0.1f);
        //                v3 = new Vector2(x + 0.15f, end - 0.1f);
        //            }
        //            break;
        //        case EDirectionType.Right:
        //            {
        //                float x = _colliderGrid.XMax * ConstDefineGM2D.ClientTileScale;
        //                v0 = new Vector2(x - 0.15f, start - 0.1f);
        //                v1 = new Vector2(x + 0.15f, start + 0.1f);
        //                v2 = new Vector2(x - 0.15f, end - 0.1f);
        //                v3 = new Vector2(x + 0.15f, end + 0.1f);
        //            }
        //            break;
        //    }
        //    var vertices = new Vector3[4];
        //    vertices[0] = v0;
        //    vertices[1] = v1;
        //    vertices[2] = v2;
        //    vertices[3] = v3;
        //    mesh.vertices = vertices;

        //    var colors32 = new Color32[4];
        //    colors32[0] = Color.red;
        //    colors32[1] = Color.red;
        //    colors32[2] = Color.red;
        //    colors32[3] = Color.red;
        //    mesh.colors32 = colors32;

        //    var tri = new int[6];
        //    tri[0] = 0;
        //    tri[1] = 2;
        //    tri[2] = 1;
        //    tri[3] = 2;
        //    tri[4] = 3;
        //    tri[5] = 1;
        //    mesh.triangles = tri;

        //    var normals = new Vector3[4];
        //    normals[0] = Vector3.back;
        //    normals[1] = Vector3.back;
        //    normals[2] = Vector3.back;
        //    normals[3] = Vector3.back;
        //    mesh.normals = normals;

        //    var uv = new Vector2[4];
        //    uv[0] = new Vector2(0, 0);
        //    uv[1] = new Vector2(1, 0);
        //    uv[2] = new Vector2(0, 1);
        //    uv[3] = new Vector2(1, 1);
        //    mesh.uv = uv;
        //    mesh.RecalculateBounds();
        //    return mesh;
        //}
    }
}
