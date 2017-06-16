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
    public class Earth : PaintBlock
    {
        protected override void InitAssetPath()
        {
            _assetPath = string.Format("{0}_{1}", _tableUnit.Model, Random.Range(1, 3));
        }

        public override Edge GetUpEdge(UnitBase other)
        {
            int start, end;
            if (GetPos(other, EDirectionType.Up, out start, out end))
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

        protected override bool CanEdgeClimbed(UnitBase other, EDirectionType eDirectionType)
        {
            int start, end;
            if (GetPos(other, eDirectionType, out start, out end))
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    if (_edges[i].Direction == eDirectionType && _edges[i].Intersect(start, end))
                    {
                        return _edges[i].ESkillType == ESkillType.Clay;
                    }
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
            if (cutStart - 1 >= Start + Earth.MinEdgeLength)
            {
                edges.Add(new Edge(Start, cutStart - 1, Direction, ESkillType));
            }
            if (End >= cutEnd + 1 + +Earth.MinEdgeLength)
            {
                edges.Add(new Edge(cutEnd + 1, End, Direction, ESkillType));
            }
        }
    }
}