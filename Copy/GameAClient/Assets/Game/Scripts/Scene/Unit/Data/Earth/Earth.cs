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

namespace GameA.Game
{
    [Unit(Id = 4001, Type = typeof(Earth))]
    public class Earth : PaintBlock
    {
        protected override void InitAssetPath()
        {
            var v = DataScene2D.CurScene.GetUnitExtra(_guid).ChildId;
            _assetPath = string.Format("{0}_{1}", _tableUnit.Model,  (byte) Mathf.Clamp(v, 1, 2));
        }

        public override Edge GetUpEdge(UnitBase other)
        {
            int start, end;
            if (GetPos(other.ColliderGrid, EDirectionType.Up, out start, out end))
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    if (_edges[i].Direction == EDirectionType.Up && _edges[i].Intersect(start, end))
                    {
                        //取靠近中间的？
                        return _edges[i];
                    }
                }
            }
            return base.GetUpEdge(other);
        }

        protected override bool CanEdgeClimbed(UnitBase other, Grid2D checkGrid, EDirectionType eDirectionType)
        {
            int start, end;
            if (GetPos(checkGrid, eDirectionType, out start, out end))
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    if (_edges[i].Direction == eDirectionType && _edges[i].Intersect(start, end))
                    {
                        return _edges[i].EPaintType == EPaintType.Clay;
                    }
                }
            }
            return base.CanEdgeClimbed(other, checkGrid, eDirectionType);
        }
    }

    public struct Edge : IEquatable<Edge>
    {
        public static Edge zero = new Edge();
        public int Start;
        public int End;
        public EDirectionType Direction;
        public EPaintType EPaintType;

        public Edge(int start, int end, EDirectionType direction, EPaintType ePaintType)
        {
            Start = start;
            End = end;
            Direction = direction;
            EPaintType = ePaintType;
        }

        public override string ToString()
        {
            return string.Format("Start: {0}, End: {1}, Direction: {2}, ESkillType: {3}", Start, End, Direction, EPaintType);
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
            if (cutStart - 1 >= Start + PaintBlock.MinEdgeLength)
            {
                edges.Add(new Edge(Start, cutStart - 1, Direction, EPaintType));
            }
            if (End >= cutEnd + 1 + +PaintBlock.MinEdgeLength)
            {
                edges.Add(new Edge(cutEnd + 1, End, Direction, EPaintType));
            }
        }

        public bool Equals(Edge other)
        {
            return Start == other.Start && End == other.End && Direction == other.Direction && EPaintType == other.EPaintType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Edge && Equals((Edge) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start;
                hashCode = (hashCode * 397) ^ End;
                hashCode = (hashCode * 397) ^ (int) Direction;
                hashCode = (hashCode * 397) ^ (int) EPaintType;
                return hashCode;
            }
        }
    }
}