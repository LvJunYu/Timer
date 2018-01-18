/********************************************************************
** Filename : NodeTools
** Author : Dong
** Date : 2016/9/25 星期日 下午 6:35:05
** Summary : NodeTools
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public static class NodeFactory
    {
        public static SceneNode GetDataNode(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            return SoyEngine.NodeFactory.GetDataNode((ushort) tableUnit.Id,
                tableUnit.GetDataGrid(unitDesc.Guid.x, unitDesc.Guid.y, 0, unitDesc.Scale), unitDesc.Guid.z,
                unitDesc.Rotation, unitDesc.Scale, UnitManager.Instance.GetLayer(tableUnit));
        }

        public static SceneNode GetColliderNode(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            bool isDynamic = false;
            switch (tableUnit.EColliderType)
            {
                case EColliderType.Dynamic:
                    isDynamic = true;
                    break;
                case EColliderType.Static:
                    UnitExtraDynamic unitExtra;
                    if (DataScene2D.CurScene.TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                    {
                        isDynamic = unitExtra.IsDynamic();
                    }
                    break;
            }
            IntVec3 guid = tableUnit.RendererToCollider(ref unitDesc);
            Grid2D grid = tableUnit.GetColliderGrid(guid.x, guid.y, unitDesc.Rotation, unitDesc.Scale);
            return SoyEngine.NodeFactory.GetColliderNode(isDynamic, (ushort) tableUnit.Id, grid, guid.z,
                unitDesc.Rotation, unitDesc.Scale,
                UnitManager.Instance.GetLayer(tableUnit));
        }

        public static NodeData GetNodeData(MapRect2D mapRect2D, Table_Unit tableUnit)
        {
            var scale = new Vector2(mapRect2D.Scale== null ? 1 : mapRect2D.Scale.X,
                mapRect2D.Scale==null ? 1 : mapRect2D.Scale.Y);
            return new NodeData((ushort) mapRect2D.Id,
                new Grid2D(mapRect2D.XMin, mapRect2D.YMin, mapRect2D.XMax, mapRect2D.YMax),
                UnitManager.GetDepth(tableUnit),
                (byte)mapRect2D.Rotation, scale);
        }

        public static SceneNode GetBgNode(ushort id, Grid2D grid, int depth, Vector2 scale)
        {
            return SoyEngine.NodeFactory.GetColliderNode(true, id, grid, depth,0, scale, 0);
        }
        
        public static SceneNode GetColliderNode(ColliderDesc colliderDesc)
        {
            return SoyEngine.NodeFactory.GetColliderNode(colliderDesc.IsDynamic, 0, colliderDesc.Grid, colliderDesc.Depth,0, Vector2.one, colliderDesc.Layer);
        }
    }
}