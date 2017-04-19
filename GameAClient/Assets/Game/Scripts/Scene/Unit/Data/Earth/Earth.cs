﻿/********************************************************************
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
    public class Earth : BlockBase
    {
        protected List<Edge> _edges = new List<Edge>();
        private Comparison<Edge> _comparisonSkillType = SortEdge;

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

        public override void DoEdge(int start, int end, EDirectionType direction, ESkillType eSkillType)
        {
            switch (direction)
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
            var edge = new Edge(start, end, direction, eSkillType);
            LogHelper.Debug("DoEdge: {0}", edge);
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

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsHero)
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    if (_edges[i].Direction == EDirectionType.Up && _edges[i].Intersect(other.ColliderGrid.XMin, other.ColliderGrid.XMax))
                    {
                        OnUpEdgeHit(other, _edges[i]);
                    }
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsHero)
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    if (_edges[i].Direction == EDirectionType.Left && _edges[i].Intersect(other.ColliderGrid.YMin, other.ColliderGrid.YMax))
                    {
                        OnLeftEdgeHit(other, _edges[i]);
                    }
                }
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        protected void OnUpEdgeHit(UnitBase other, Edge edge)
        {
            LogHelper.Debug("OnUpEdgeHit: {0}", edge);
            switch (edge.ESkillType)
            {
                    case ESkillType.Fire:
                    break;
                    case ESkillType.Jelly:
                    break;
                    case ESkillType.Clay:
                    break;
                    case ESkillType.Ice:
                    break;
            }
        }

        protected void OnLeftEdgeHit(UnitBase other, Edge edge)
        {
            LogHelper.Debug("OnLeftEdgeHit: {0}", edge);
            switch (edge.ESkillType)
            {
                case ESkillType.Fire:
                    break;
                case ESkillType.Jelly:
                    break;
                case ESkillType.Clay:
                    break;
                case ESkillType.Ice:
                    break;
            }
        }
    }

    public struct Edge
    {
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

    public enum ESkillType
    {
        None,
        Fire,
        Jelly,
        Clay,
        Ice,
        Water,
    }
}