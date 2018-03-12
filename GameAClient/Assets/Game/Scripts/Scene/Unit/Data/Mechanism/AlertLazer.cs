/********************************************************************
** Filename : AlertLazer
** Author : Dong
** Date : 2017/1/7 星期六 上午 12:00:36
** Summary : AlertLazer
***********************************************************************/

using SoyEngine;
using UnityEngine;
using NewResourceSolution;

namespace GameA.Game
{
    [Unit(Id = 25011, Type = typeof(MoAlertLazer))]
    public class MoAlertLazer : AlertLazer
    {
    }

    [Unit(Id = 5011, Type = typeof(AlertLazer))]
    public class AlertLazer : BlockBase
    {
        protected GridCheck _gridCheck;

        protected UnityNativeParticleItem _lazerEffectWarning;
        protected UnityNativeParticleItem _lazerEffect;
        protected UnityNativeParticleItem _lazerEffectEnd;

        protected Vector3 _direction;
        protected int _distance;

        protected int _timer;
        protected bool _shoot = true;

        protected int _timeDelay;
        protected int _timeInterval;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _gridCheck = new GridCheck(this);
            return true;
        }

        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_withEffect != null)
            {
                _withEffect.Trans.position += Vector3.back * 0.1f;
            }
            _lazerEffectWarning = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectAlertLazer", _trans);
            if (_lazerEffectWarning != null)
            {
                _lazerEffectWarning.Trans.localEulerAngles = new Vector3(0, 0, -_angle);
            }
            _lazerEffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazer", _trans);
            if (_lazerEffect != null)
            {
                _lazerEffect.Trans.localEulerAngles = new Vector3(0, 0, -_angle);
            }
            _lazerEffectEnd =
                GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectAlertLazerStart", _trans);
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _direction = GM2DTools.GetDirection(_angle);
            _timer = 0;
            _shoot = true;
            _gridCheck.Clear();
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Stop();
            }
            if (_lazerEffect != null)
            {
                _lazerEffect.Stop();
            }
            if (_lazerEffectWarning != null)
            {
                _lazerEffectWarning.Stop();
            }
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _timeDelay = TableConvert.GetTime(unitExtra.TimeDelay);
            _timeInterval = TableConvert.GetTime(unitExtra.TimeInterval);
            return unitExtra;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_lazerEffectWarning);
            _lazerEffectWarning = null;
            FreeEffect(_lazerEffect);
            _lazerEffect = null;
            FreeEffect(_lazerEffectEnd);
            _lazerEffectEnd = null;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            _lazerEffectWarning.SetActiveStateEx(_eActiveState == EActiveState.Active);
            _lazerEffect.SetActiveStateEx(_eActiveState == EActiveState.Active);
            _lazerEffectEnd.SetActiveStateEx(_eActiveState == EActiveState.Active);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _gridCheck.Before();
            if (_eActiveState != EActiveState.Active)
            {
                _gridCheck.After();
                return;
            }
            //timeDelay
            if (_timeDelay > 0)
            {
                _timeDelay--;
                return;
            }
            _timer++;
            if (_shoot)
            {
                _distance = _tableUnit.ValidRange * ConstDefineGM2D.ServerTileScale;
                var hits = ColliderScene2D.RaycastAll(CenterPos, _direction, _distance, EnvManager.LazerShootLayer);
                if (hits.Count > 0)
                {
                    for (int i = 0; i < hits.Count; i++)
                    {
                        var hit = hits[i];
                        var tableUnit = UnitManager.Instance.GetTableUnit(hit.node.Id);
                        if (tableUnit != null && tableUnit.IsLazerBlock == 1)
                        {
                            if (UnitDefine.IsSameDirectionSwitchTrigger(hit.node, Rotation))
                            {
                                UnitBase switchTrigger;
                                if (ColliderScene2D.CurScene.TryGetUnit(hit.node, out switchTrigger))
                                {
                                    _gridCheck.Do((SwitchTriggerPress) switchTrigger);
                                    _distance = hit.distance + 80;
                                    break;
                                }
                            }
                            bool flag = false;
                            using (var units = ColliderScene2D.GetUnits(hit))
                            {
                                for (int j = 0; j < units.Count; j++)
                                {
                                    if (units[j] != this && units[j].IsAlive && !units[j].CanCross)
                                    {
                                        _distance = hit.distance;
                                        flag = true;
                                        break;
                                    }
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
                if (_timer < 100)
                {
                    UpdateEffect();
                    if (_timer >= 40)
                    {
                        if (hits.Count > 0)
                        {
                            for (int i = 0; i < hits.Count; i++)
                            {
                                var hit = hits[i];
                                if (hit.distance <= _distance && UnitDefine.IsLaserDamage(hit.node.Layer))
                                {
                                    UnitBase unit;
                                    if (ColliderScene2D.CurScene.TryGetUnit(hit.node, out unit))
                                    {
                                        if (unit != null && unit.IsAlive && unit.IsActor)
                                        {
                                            unit.InLazer();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (_timer == 100)
            {
                _shoot = false;
                if (_lazerEffect != null)
                {
                    _lazerEffect.Stop();
                }
                if (_lazerEffectEnd != null)
                {
                    _lazerEffectEnd.Stop();
                }
            }
            if (_timer == 100 + _timeInterval)
            {
                _shoot = true;
                _timer = 0;
            }
            _gridCheck.After();
        }

        private void UpdateEffect()
        {
            var distanceWorld = _distance * ConstDefineGM2D.ClientTileScale;
            if (_timer < 40)
            {
                if (_lazerEffectWarning != null)
                {
                    _lazerEffectWarning.Play();
                    _lazerEffectWarning.Trans.localScale = new Vector3(1, distanceWorld, 1);
                }
            }
            else
            {
                if (_lazerEffectWarning != null)
                {
                    _lazerEffectWarning.Stop();
                }
                if (_lazerEffect != null)
                {
                    _lazerEffect.Play();
                    _lazerEffect.Trans.localScale = new Vector3(1, distanceWorld, 1);
                    if (_lazerEffectEnd != null)
                    {
                        _lazerEffectEnd.Play();
                        _lazerEffectEnd.Trans.position =
                            GM2DTools.TileToWorld(CenterPos, _lazerEffect.Trans.position.z - 0.1f) +
                            distanceWorld * _direction;
                    }
                }
            }
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
            if (!JoyResManager.Instance.TryGetTexture(path, out texture))
            {
                LogHelper.Error("TryGetTextureByName Failed {0}", path);
                return;
            }
            var go = new GameObject("LazerEffect");
            _trans = go.transform;
            IntVec2 pointA = IntVec2.zero, pointB = IntVec2.zero;
            GM2DTools.GetBorderPoint(lazerUnit.ColliderGrid, (EDirectionType) lazerUnit.Rotation, ref pointA,
                ref pointB);
            _trans.position = GM2DTools.TileToWorld((pointA + pointB) / 2, lazerUnit.Trans.position.z - 0.1f);
            _trans.SetParent(lazerUnit.Trans);
            go.AddComponent<MeshFilter>().sharedMesh = Mesh;
            _renderer = go.AddComponent<MeshRenderer>();
            _renderer.sharedMaterial = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
            _renderer.sharedMaterial.mainTexture = texture;
            _renderer.sortingOrder = (int) ESortingOrder.Item;
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
                Object.Destroy(_trans.gameObject);
                _trans = null;
            }
            _renderer = null;
        }

        internal void Pause()
        {
            if (_renderer != null)
            {
                _renderer.enabled = false;
            }
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
            var transform = Matrix4x4.TRS(new Vector3(0f, 0.5f, 0), Quaternion.Euler(Vector3.forward * 90),
                Vector3.one);
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