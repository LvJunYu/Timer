/********************************************************************
** Filename : Earth
** Author : Dong
** Date : 2016/10/27 星期四 下午 3:42:11
** Summary : Earth
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GameA.Game
{
    [Unit(Id = 4001, Type = typeof(Earth))]
    public class Earth : BlockBase
    {
		/// <summary>
		/// 被涂装的边信息，有序从小到大 
		/// </summary>
        protected List<Edge> _edges = new List<Edge>();
        private static Comparison<Edge> _comparisonSkillType = SortEdge;
        private SplashController _leftEdgeSplash;
        private Mesh _paintMesh;

        public override bool CanPainted
        {
            get { return true; }
        }

        protected override void InitAssetPath()
        {
            _assetPath = string.Format("{0}_{1}", _tableUnit.Model, Random.Range(1, 3));
        }

        #region edge


		protected override void Clear ()
		{
			base.Clear ();
            _edges.Clear();
            if (_leftEdgeSplash != null)
            {
                Object.Destroy(_leftEdgeSplash.gameObject);
            }
		}

        private void InitSplash()
        {
            if (null == _leftEdgeSplash)
            {
                UnityNativeParticleItem particle =
                    GameParticleManager.Instance.GetUnityNativeParticleItem("Decal_Fire_L", _trans);
                particle.Trans.localPosition = new Vector3(0, 0, 0.1f);
                particle.Trans.localScale = Vector3.one*2;
                particle.Play();
                _leftEdgeSplash = particle.Trans.GetComponent<SplashController>();
            }
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
            int localStart = 0, localEnd = 0;
            GetLocalPos(start, end, ref localStart, ref localEnd, direction);
            if (localEnd < localStart)
            {
                return;
            }
            var edge = new Edge(localStart, localEnd, direction, eSkillType);
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
            if (_edges.Count > 0)
            {
                UpdateMesh();
            }
            //if (_leftEdgeSplash == null)
            //{
            //    InitSplash();
            //}
            //if (_leftEdgeSplash != null)
            //{
            //    var regions = new List<Vector2>();
            //    for (int i = 0; i < _edges.Count; i++)
            //    {
            //        if (_edges[i].Direction == EDirectionType.Left && direction == EDirectionType.Left)
            //        {
            //            regions.Add(new Vector2(_edges[i].Start * ConstDefineGM2D.ClientTileScale,
            //                (_edges[i].End + 1) * ConstDefineGM2D.ClientTileScale));
            //        }
            //    }
            //    _leftEdgeSplash.SetSplashRegion(regions);
            //}
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

        private void GetLocalPos(int start, int end, ref int localStart, ref int localEnd, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                case EDirectionType.Down:
                    localStart = Math.Max(_colliderGrid.XMin, start) - _colliderGrid.XMin;
                    localEnd = Math.Min(_colliderGrid.XMax, end) - _colliderGrid.XMin;
                    break;
                case EDirectionType.Left:
                case EDirectionType.Right:
                    localStart = Math.Max(_colliderGrid.YMin, start) - _colliderGrid.YMin;
                    localEnd = Math.Min(_colliderGrid.YMax, end) - _colliderGrid.YMin;
                    break;
            }
        }

        private void UpdateMesh()
        {
            return;
            if (_paintMesh == null)
            {
                _paintMesh = new Mesh();
            }
            for (int i = 0; i < _edges.Count; i++)
            {
                var edge = _edges[i];

                var mesh = new Mesh();

                var vertices = new Vector3[4];
                vertices[0] = new Vector3(-0.5f, -0.5f);
                vertices[1] = new Vector3(0.5f, -0.5f);
                vertices[2] = new Vector3(-0.5f, 0.5f);
                vertices[3] = new Vector3(0.5f, 0.5f);
                mesh.vertices = vertices;

                var colors32 = new Color32[4];
                colors32[0] = new Color32(100, 100, 100, 1);
                colors32[1] = new Color32(100, 100, 100, 1);
                colors32[2] = new Color32(100, 100, 100, 1);
                colors32[3] = new Color32(100, 100, 100, 1);
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
                uv[0 + i * 4] = new Vector2(0, 0);
                uv[1 + i * 4] = new Vector2(1, 0);
                uv[2 + i * 4] = new Vector2(0, 1);
                uv[3 + i * 4] = new Vector2(1, 1);
                mesh.uv = uv;

                switch (edge.ESkillType)
                {

                }
            }
        }

        #endregion

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

        private void CheckEdgeHit(UnitBase other, EDirectionType eDirectionType)
        {
            int localStart = 0, localEnd = 0;
            GetLocalPos(other.ColliderGrid.YMin, other.ColliderGrid.YMax, ref localStart, ref localEnd, eDirectionType);
            for (int i = 0; i < _edges.Count; i++)
            {
                if (_edges[i].Direction == eDirectionType && _edges[i].Intersect(localStart, localEnd))
                {
                    OnEdgeHit(other, _edges[i]);
                }
            }
        }

        private void OnEdgeHit(UnitBase other, Edge edge)
        {
            LogHelper.Debug("OnEdgeHit: {0}", edge);
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

        public override Edge GetUpEdge(UnitBase other)
        {
            int localStart = 0, localEnd = 0;
            GetLocalPos(other.ColliderGrid.YMin, other.ColliderGrid.YMax, ref localStart, ref localEnd, EDirectionType.Up);
            for (int i = 0; i < _edges.Count; i++)
            {
                if (_edges[i].Direction == EDirectionType.Up && _edges[i].Intersect(localStart, localEnd))
                {
                    //取靠近中间的？
                    return _edges[i];
                }
            }
            return base.GetUpEdge(other);
        }

        protected override bool CanEdgeClimbed(UnitBase other, EDirectionType eDirectionType)
        {
            int localStart = 0, localEnd = 0;
            GetLocalPos(other.ColliderGrid.YMin, other.ColliderGrid.YMax, ref localStart, ref localEnd, eDirectionType);
            for (int i = 0; i < _edges.Count; i++)
            {
                if (_edges[i].Direction == eDirectionType && _edges[i].Intersect(localStart, localEnd))
                {
                    return _edges[i].ESkillType == ESkillType.Clay;
                }
            }
            return base.CanEdgeClimbed(other, eDirectionType);
        }
    }

    public struct Edge
    {
        public static Edge zero = new Edge();
        public int Start;
        public int End;
        public EDirectionType Direction;
        public ESkillType ESkillType;

        public Edge(int start, int end, EDirectionType direction, ESkillType eSkillType)
        {
            Start = start;
            End = end;
            Direction = direction;
            ESkillType = eSkillType;
        }

        public override string ToString()
        {
            return string.Format("Start: {0}, End: {1}, Direction: {2}, ESkillType: {3}", Start, End, Direction, ESkillType);
        }

        private bool Intersect(ref Edge edge)
        {
            return edge.Start <= End && edge.End >= Start;
        }

        public bool Intersect(int start, int end)
        {
            return start <= End && end >= Start;
        }

        public bool Merge(ref Edge edge)
        {
            if (Intersect(ref edge))
            {
                Start = Math.Min(edge.Start, Start);
                End = Math.Max(edge.End, End);
                return true;
            }
            return false;
        }

        public void Cut(ref Edge edge, List<Edge> edges)
        {
            if (!Intersect(ref edge))
            {
                return;
            }
            edges.Remove(this);
            int cutStart = Math.Max(Start, edge.Start);
            int cutEnd = Math.Min(End, edge.End);
            if (cutStart > Start)
            {
                edges.Add(new Edge(Start, cutStart - 1, Direction, ESkillType));
            }
            if (End > cutEnd)
            {
                edges.Add(new Edge(cutEnd + 1, End, Direction, ESkillType));
            }
        }
    }
}