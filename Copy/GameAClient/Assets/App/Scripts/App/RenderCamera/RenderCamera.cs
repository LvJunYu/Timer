using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class RenderCamera
    {
        private Transform _root;
        private Camera _camera;
        private RenderTexture _renderTexture;

        public Transform Root
        {
            get { return _root; }
        }

        public Texture Texture
        {
            get { return _renderTexture; }
        }

        public void Init(Transform parent)
        {
            var go = new GameObject("RenderCamera");
            _root = go.transform;
            _camera = _root.gameObject.AddComponent<Camera>();
            _camera.orthographic = true;
            _camera.farClipPlane = 100;
            _camera.nearClipPlane = -100;
            _camera.cullingMask = 1 << (int) ELayer.RenderUI;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.clear;
            CommonTools.SetAllLayerIncludeHideObj(_root, (int) ELayer.RenderUI);
        }

        public void Set(float modelSize, Transform targetTran, int targetTextureWidth,
            int targetTextureHeight)
        {
            _camera.orthographicSize = modelSize * 0.5f;
            if (_renderTexture != null)
            {
                ReleaseRenderTexture();
            }

            _renderTexture =
                RenderTexture.GetTemporary(targetTextureWidth, targetTextureHeight, 0, RenderTextureFormat.ARGB32);
            _camera.targetTexture = _renderTexture;
            _root.SetParent(targetTran.parent);
            CommonTools.ResetTransform(_root);
            CommonTools.SetAllLayerIncludeHideObj(targetTran, (int) ELayer.RenderUI);
        }

        public void Enable()
        {
            _root.SetActiveEx(true);
        }

        public void Disable()
        {
            _root.SetActiveEx(false);
        }

        private void ReleaseRenderTexture()
        {
            if (_camera.targetTexture != null)
            {
                _camera.targetTexture = null;
            }

            RenderTexture.ReleaseTemporary(_renderTexture);
            _renderTexture = null;
        }

        public void SetOffsetPos(Vector3 offesetpos)
        {
            _root.localPosition = offesetpos;
        }
    }
}