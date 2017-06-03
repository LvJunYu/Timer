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
        public const int MinEdgeLength = 64;
        public const int OffsetEdgeLength = 10;
        /// <summary>
        /// 被涂装的边信息，有序从小到大 
        /// </summary>
        protected List<Edge> _edges = new List<Edge>();
        private static Comparison<Edge> _comparisonSkillType = SortEdge;
        private Mesh _paintMesh;
        private MeshFilter _paintMeshFilter;
        private static List<CombineInstance> _combineInstances = new List<CombineInstance>();

        private Texture2D _paintTexture;
        private Texture2D _maskTexture;
        private static Color32[] EmptyPixels;
        private const int Ratio = 8;
        private static Color CleanColor = new Color(1f, 1f, 1f, 0f);

        public override bool CanPainted
        {
            get { return true; }
        }

        protected override void Clear()
        {
            base.Clear();
            _edges.Clear();
            if (_paintMesh != null)
            {
                Object.Destroy(_paintMesh);
                _paintMesh = null;
            }
            if (_paintMeshFilter != null)
            {
                Object.Destroy(_paintMeshFilter.gameObject);
                _paintMeshFilter = null;
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

        public override void DoPaint(int start, int end, EDirectionType direction, ESkillType eSkillType)
        {
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
            UpdateMesh();
            UpdateTexture(ref edge);
            if (_paintMeshFilter == null)
            {
                _paintMeshFilter = new GameObject("Paint").gameObject.AddComponent<MeshFilter>();
                _paintMeshFilter.gameObject.transform.localPosition = Vector3.forward * (_trans.localPosition.z - 0.01f);
                _paintMeshFilter.sharedMesh = new Mesh();
                var mr = _paintMeshFilter.gameObject.AddComponent<MeshRenderer>();
                //mr.sharedMaterial = new Material(Shader.Find("Difu"));
            }
            _paintMeshFilter.sharedMesh.CombineMeshes(_combineInstances.ToArray());
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

        private void UpdateTexture(ref Edge edge)
        {
            if (_paintTexture == null)
            {
                if (_view != null)
                {
                    var sr = _view.Trans.GetComponent<SpriteRenderer>();
                    int width = (int)sr.sprite.rect.width / Ratio;
                    int height = (int)sr.sprite.rect.height / Ratio;
                    if (EmptyPixels == null)
                    {
                        var count = width*height;
                        EmptyPixels = new Color32[count];
                        for (int i = 0; i < count; i++)
                        {
                            EmptyPixels[i] = CleanColor;
                        }
                    }
                    _paintTexture = new Texture2D(width, height);
                    _paintTexture.wrapMode = TextureWrapMode.Clamp;
                    _paintTexture.SetPixels32(EmptyPixels);
                    _paintTexture.Apply();

                    _maskTexture = new Texture2D(width, height);
                    _maskTexture.SetPixels32(EmptyPixels);
                    _maskTexture.Apply();

                    var paintObject = new GameObject("Paint");
                    CommonTools.SetParent(paintObject.transform, _view.Trans);
                    paintObject.transform.localPosition = Vector3.forward * (_trans.localPosition.z - 0.01f);
                    var paintRenderer = paintObject.AddComponent<SpriteRenderer>();
                    paintRenderer.sortingOrder = sr.sortingOrder;
                    paintRenderer.sprite = Sprite.Create(_paintTexture,
                        new Rect(0f, 0f, _paintTexture.width, _paintTexture.height), new Vector2(0.5f, 0.5f), 16, 0,
                        SpriteMeshType.FullRect);
                }
            }
            Vector2 v1, v2, v3;
            Vector2 v0 = v1 = v2 = v3 = Vector2.zero;
            float start = edge.Start * ConstDefineGM2D.ClientTileScale;
            float end = edge.End * ConstDefineGM2D.ClientTileScale;

            switch (edge.Direction)
            {
                case EDirectionType.Up:
                    {
                        float y = _colliderGrid.YMax * ConstDefineGM2D.ClientTileScale;
                        v0 = new Vector2(start - 0.15f, y - 0.1f);
                        v3 = new Vector2(end + 0.15f, y + 0.1f);
                        var offset = (start + end)/2 - _trans.position.x;
                        int pixelX = (int)((offset + v0.x - (_colliderGrid.XMin - 160) * ConstDefineGM2D.ClientTileScale) * 16);
                        int pixelY = (int) ((v0.y - (_colliderGrid.YMin - 160) * ConstDefineGM2D.ClientTileScale) * 16);
                        pixelX = Math.Max(0, pixelX);
                        pixelY = Math.Max(0, pixelY);
                        int width = (int) ((v3.x - v0.x)*16);
                        int height = (int) ((v3.y - v0.y)*16);
                        var paintedColor = _paintTexture.GetPixels(pixelX, pixelY, width, height);
                        var maskedColor = _maskTexture.GetPixels(pixelX, pixelY, width, height);
                        var paintingColor = _paintTexture.GetPixels(pixelX, pixelY, width, height);
                        var maskingColor = _paintTexture.GetPixels(pixelX, pixelY, width, height);
                        for (int i = 0; i < paintingColor.Length; i++)
                        {
                            if (maskedColor[i].a == 0f)
                            {
                                maskedColor[i] = maskingColor[i];
                            }
                            if (maskedColor[i].a == 0f)
                            {
                                continue;
                            }
                            if (paintingColor[i].a == 0f || paintingColor[i] == paintedColor[i])
                            {
                                continue;
                            }
                            paintedColor[i] = paintingColor[i];
                        }
                        LogHelper.Debug("{0} | {1} | {2} | {3} | {4}", pixelX, pixelY, width, height, offset);
                    }
                    break;
                case EDirectionType.Down:
                    break;
                case EDirectionType.Left:
                    break;
                case EDirectionType.Right:
                    break;
            }
        }

        private void UpdateMesh()
        {
            _combineInstances.Clear();
            if (_paintMesh == null)
            {
                _paintMesh = new Mesh();
            }
            for (int i = 0; i < _edges.Count; i++)
            {
                var edge = _edges[i];
                _combineInstances.Add(new CombineInstance { mesh = CreateMesh(ref edge), transform = Matrix4x4.identity });
            }
        }

        private Mesh CreateMesh(ref Edge edge)
        {
            var mesh = new Mesh();
            Vector2 v1, v2, v3;
            Vector2 v0 = v1 = v2 = v3 = Vector2.zero;
            float start = (edge.Start - OffsetEdgeLength) * ConstDefineGM2D.ClientTileScale;
            float end = (edge.End + OffsetEdgeLength) * ConstDefineGM2D.ClientTileScale;
            switch (edge.Direction)
            {
                case EDirectionType.Up:
                    {
                        float y = _colliderGrid.YMax * ConstDefineGM2D.ClientTileScale;
                        v0 = new Vector2(start - 0.15f, y - 0.1f);
                        v1 = new Vector2(end - 0.15f, y - 0.1f);
                        v2 = new Vector2(start + 0.15f, y + 0.1f);
                        v3 = new Vector2(end + 0.15f, y + 0.1f);
                    }
                    break;
                case EDirectionType.Down:
                    {
                        float y = _colliderGrid.YMin * ConstDefineGM2D.ClientTileScale;
                        v0 = new Vector2(start - 0.15f, y - 0.1f);
                        v1 = new Vector2(end - 0.15f, y - 0.1f);
                        v2 = new Vector2(start - 0.15f, y + 0.1f);
                        v3 = new Vector2(end - 0.15f, y + 0.1f);
                    }
                    break;
                case EDirectionType.Left:
                    {
                        float x = _colliderGrid.XMin * ConstDefineGM2D.ClientTileScale;
                        v0 = new Vector2(x - 0.15f, start - 0.1f);
                        v1 = new Vector2(x + 0.15f, start - 0.1f);
                        v2 = new Vector2(x - 0.15f, end - 0.1f);
                        v3 = new Vector2(x + 0.15f, end - 0.1f);
                    }
                    break;
                case EDirectionType.Right:
                    {
                        float x = _colliderGrid.XMax * ConstDefineGM2D.ClientTileScale;
                        v0 = new Vector2(x - 0.15f, start - 0.1f);
                        v1 = new Vector2(x + 0.15f, start + 0.1f);
                        v2 = new Vector2(x - 0.15f, end - 0.1f);
                        v3 = new Vector2(x + 0.15f, end + 0.1f);
                    }
                    break;
            }
            var vertices = new Vector3[4];
            vertices[0] = v0;
            vertices[1] = v1;
            vertices[2] = v2;
            vertices[3] = v3;
            mesh.vertices = vertices;

            var colors32 = new Color32[4];
            colors32[0] = Color.red;
            colors32[1] = Color.red;
            colors32[2] = Color.red;
            colors32[3] = Color.red;
            mesh.colors32 = colors32;

            var tri = new int[6];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
            mesh.triangles = tri;

            var normals = new Vector3[4];
            normals[0] = Vector3.back;
            normals[1] = Vector3.back;
            normals[2] = Vector3.back;
            normals[3] = Vector3.back;
            mesh.normals = normals;

            var uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;
            mesh.RecalculateBounds();
            return mesh;
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
    }
}
