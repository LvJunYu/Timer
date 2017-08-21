using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class RenderCameraManager
    {
        public static readonly RenderCameraManager Instance = new RenderCameraManager();
        private readonly Transform _root;
        private readonly Stack<RenderCamera> _pool = new Stack<RenderCamera>();

        private RenderCameraManager()
        {
            var go = new GameObject("RenderCameraManager");
            _root = go.transform;
            _root.SetParent(SocialApp.Instance.transform);
            CommonTools.SetAllLayerIncludeHideObj(_root, (int) ELayer.RenderUI);
        }

        public RenderCamera GetCamera(float modelSize, Transform targetTran, int targetTextureWidth,
            int targetTextureHeight)
        {
            var c = Get();
            c.Set(modelSize, targetTran, targetTextureWidth, targetTextureHeight);
            return c;
        }

        public void FreeCamera(RenderCamera renderCamera)
        {
            Free(renderCamera);
        }

        private RenderCamera Get()
        {
            RenderCamera renderCamera;
            if (_pool.Count > 0)
            {
                renderCamera = _pool.Pop();
                renderCamera.Enable();
            }
            else
            {
                renderCamera = new RenderCamera();
                renderCamera.Init(_root);
            }
            return renderCamera;
        }

        private void Free(RenderCamera renderCamera)
        {
            renderCamera.Root.SetParent(_root);
            renderCamera.Disable();
            _pool.Push(renderCamera);
        }
    }
}