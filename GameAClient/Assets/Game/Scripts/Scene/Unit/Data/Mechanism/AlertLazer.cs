/********************************************************************
** Filename : AlertLazer
** Author : Dong
** Date : 2017/1/7 星期六 上午 12:00:36
** Summary : AlertLazer
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5011, Type = typeof(AlertLazer))]
    public class AlertLazer : BlockBase
    {
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;

        protected int _timerMagic;
        protected bool _shoot = true;
        protected UnityNativeParticleItem _effect;
        protected LazerEffect _lazerEffect1;
        protected LazerEffect _lazerEffect2;
        protected GridCheck _gridCheck;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            GM2DTools.GetBorderPoint(_colliderGrid, (EDirectionType)Rotation, ref _pointA, ref _pointB);
            _gridCheck = new GridCheck(this);
            return true;
        }

        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }

        protected override void Clear()
        {
            base.Clear();
            _timerMagic = 0;
            _shoot = true;
            if (_effect != null)
            {
                _effect.Stop();
            }
            if (_lazerEffect1 != null)
            {
                _lazerEffect1.Reset();
            }
            if (_lazerEffect2 != null)
            {
                _lazerEffect2.Reset();
            }
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_effect != null)
            {
                _effect.DestroySelf();
                _effect = null;
            }
            if (_lazerEffect1 != null)
            {
                _lazerEffect1.OnObjectDestroy();
                _lazerEffect1 = null;
            }
            if (_lazerEffect2 != null)
            {
                _lazerEffect2.OnObjectDestroy();
                _lazerEffect2 = null;
            }
        }

        public override void UpdateLogic()
        {
            _timerMagic++;
            _gridCheck.Before();
            if (_shoot)
            {
                if (_effect != null)
                {
                    _effect.StopEmit();
                }
                var distance = GM2DTools.GetDistanceToBorder(_pointA, Rotation);
                var hits = ColliderScene2D.GridCastAll(_pointA, _pointB, Rotation, distance, EnvManager.LazerShootLayer);
                if (hits.Count > 0)
                {
                    var grid = SceneQuery2D.GetGrid(_pointA, _pointB, Rotation, distance);
                    for (int i = 0; i < hits.Count; i++)
                    {
                        var hit = hits[i];
                        if (IsBlock(hit.node.Layer))
                        {
                            if (IsSameDirectionSwitchTrigger(hit.node))
                            {
                                UnitBase switchTrigger;
                                if (ColliderScene2D.Instance.TryGetUnit(hit.node, out switchTrigger))
                                {
                                    _gridCheck.Do((SwitchTrigger)switchTrigger);
                                    distance = hit.distance + 80;
                                    break;
                                }
                            }
                            bool flag = false;
                            List<UnitBase> units = ColliderScene2D.GetUnits(hit, grid);
                            for (int j = 0; j < units.Count; j++)
                            {
                                UnitBase unit = units[j];
                                if (unit != null && unit.IsAlive && !(unit is TransparentEarth) && GM2DTools.OnDirectionHit(unit, PlayMode.Instance.MainUnit, (EMoveDirection)(Rotation+1)))
                                {
                                    distance = hit.distance;
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                break;
                            }
                        }
                    }
                }
                //显示警告
                if (_timerMagic < 100)
                {
                    if (_lazerEffect1 == null)
                    {
                        _lazerEffect1 = new LazerEffect(this, ConstDefineGM2D.M1LazerEffect1);
                    }
                    _lazerEffect1.Update((float)distance / ConstDefineGM2D.ServerTileScale);
                    if (_timerMagic >= 30)
                    {
                        if (_lazerEffect2 == null)
                        {
                            _lazerEffect2 = new LazerEffect(this, ConstDefineGM2D.M1LazerEffect3);
                        }
                        _lazerEffect2.Update((float)distance / ConstDefineGM2D.ServerTileScale);
                        if (hits.Count > 0)
                        {
                            for (int i = 0; i < hits.Count; i++)
                            {
                                var hit = hits[i];
                                if (IsDamage(hit.node.Layer) && hit.distance <= distance)
                                {
                                    UnitBase unit;
                                    if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
                                    {
                                        if (unit != null && unit.IsAlive)
                                        {
                                            if (unit.IsHero)
                                            {
                                                unit.OnLight();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (_timerMagic == 100)
            {
                _shoot = false;
                if (_lazerEffect1 != null)
                {
                    _lazerEffect1.Stop();
                }
                if (_lazerEffect2 != null)
                {
                    _lazerEffect2.Stop();
                }
                if (_effect == null)
                {
                    _effect = GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.MarioLazerParticle, _trans);
                }
                _effect.Play();
            }
            else if (_timerMagic == 200)
            {
                _shoot = true;
                _timerMagic = 0;
            }
            _gridCheck.After();
        }

        public bool IsSameDirectionSwitchTrigger(SceneNode node)
        {
            return node.Id == UnitDefine.SwitchTriggerId &&
                   (node.Rotation + Rotation == 2 || node.Rotation + Rotation == 4);
        }

        private bool IsBlock(int layer)
        {
            return ((1 << layer) & EnvManager.LazerBlockLayer) != 0;
        }

        private bool IsDamage(int layer)
        {
            return ((1 << layer) & (EnvManager.HeroLayer | EnvManager.MainPlayerLayer)) != 0;
        }
    }

    public class LazerEffect
    {
        protected static Mesh Mesh;
        protected Transform _trans;
        protected MeshRenderer _renderer;
        protected float _scrollSpeed = 0.1f;

        public LazerEffect(UnitBase lazerUnit, string path)
        {
            CreateMesh();
            Texture texture;
            if (!GameResourceManager.Instance.TryGetTextureByName(path, out texture))
            {
                LogHelper.Error("TryGetTextureByName Failed {0}", path);
                return;
            }
            var go = new GameObject("LazerEffect");
            _trans = go.transform;
            IntVec2 pointA = IntVec2.zero, pointB = IntVec2.zero;
            GM2DTools.GetBorderPoint(lazerUnit.ColliderGrid, (EDirectionType) lazerUnit.Rotation, ref pointA, ref pointB);
            _trans.position = GM2DTools.TileToWorld((pointA + pointB) / 2);
            _trans.SetParent(lazerUnit.Trans);
            go.AddComponent<MeshFilter>().sharedMesh = Mesh;
            _renderer = go.AddComponent<MeshRenderer>();
            _renderer.sharedMaterial = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
            _renderer.sharedMaterial.mainTexture = texture;
            _renderer.sortingOrder = (int)ESortingOrder.LazerEffect;
            _trans.eulerAngles = new Vector3(0, 0, lazerUnit.Rotation * -90);
        }

        public void Update(float scale)
        {
            if (_renderer != null && _renderer.sharedMaterial != null)
            {
                _trans.localScale = new Vector3(1, scale, 1);
                _renderer.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(-Time.frameCount * _scrollSpeed, 0));
                _renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(scale * 0.5f, 1));
                _renderer.enabled = true;
            }
        }

        internal void OnObjectDestroy()
        {
            if (_trans != null)
            {
                UnityEngine.Object.Destroy(_trans.gameObject);
                _trans = null;
            }
            _renderer = null;
        }

        internal void Stop()
        {
            if (_renderer != null)
            {
                _renderer.enabled = false;
            }
        }

        internal void Reset()
        {
            Stop();
        }

        private void CreateMesh()
        {
            if (Mesh != null)
            {
                return;
            }
            Mesh = new Mesh();
            var transform = Matrix4x4.TRS(new Vector3(0f, 0.5f, 0), Quaternion.Euler(Vector3.forward * 90), Vector3.one);
            var vertices = new Vector3[4];
            vertices[0] = transform.MultiplyPoint(new Vector3(-0.5f, -0.5f));
            vertices[1] = transform.MultiplyPoint(new Vector3(0.5f, -0.5f));
            vertices[2] = transform.MultiplyPoint(new Vector3(-0.5f, 0.5f));
            vertices[3] = transform.MultiplyPoint(new Vector3(0.5f, 0.5f));
            Mesh.vertices = vertices;

            var tri = new int[6];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
            Mesh.triangles = tri;

            var normals = new Vector3[4];
            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;
            Mesh.normals = normals;

            var uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            Mesh.uv = uv;

            Mesh.RecalculateBounds();
        }
    }
}
