using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 运动数据
    ///</summary>
    public class Locomotion
    {
        /// <summary>
        /// 质量
        /// </summary>
        public float Mass = 1;

        /// <summary>
        /// 操控矢量
        /// </summary>
        public Vector3 SteeringVector;

        /// <summary>
        /// 默认移动速度
        /// </summary>
        public float MoveSpeed = 5;

        /// <summary>
        /// 最大移动速度
        /// </summary>
        public float MaxMoveSpeed = 30;

        /// <summary>
        /// 最大合力值
        /// </summary>
        public const float MaxForceValue = 10;

        /// <summary>
        /// 旋转速度
        /// </summary>
        public float RotateSpeed = 5f;

        /// <summary>
        /// 加速度 
        /// (若物体加速过快可能发生抖动，需保证加速度*计算频率小于1)
        /// (或者修改最大速度不超过默认速度)
        /// </summary>
        [Range(0.1f, 30f)] protected float _acceleration = 1f;

        /// <summary>
        /// 计算加速度的频率
        /// </summary>
        protected float _caculateFrequency = 0.1f;

        /// <summary>
        /// 是否平面运动
        /// </summary>
        public bool IsPlane = true;

        /// <summary>
        /// 操控算法数组
        /// </summary>
        [HideInInspector] public SteeringBase[] Steerings;

        /// <summary>
        /// 当前速度向量
        /// </summary>
        [HideInInspector] public Vector3 CurrentSpeed;

        /// <summary>
        /// 合力
        /// </summary>
        [HideInInspector] public Vector3 SumForce;

        protected bool _run;

        /// <summary>
        /// 计算最终操控矢量
        /// </summary>
        public void CaculateSteeringVector()
        {
            //根据多个操控方式计算合力
            SumForce = Vector3.zero;
            for (int i = 0; i < Steerings.Length; i++)
            {
                if (Steerings[i].isActiveAndEnabled)
                {
                    SumForce += Steerings[i].GetSteeringForce();
                }
            }

            //限制合力大小
            SumForce = Vector3.ClampMagnitude(SumForce, MaxForceValue);
            //计算操控矢量
            SteeringVector = SumForce / Mass;
            //判断是否平面
            if (IsPlane)
            {
                SteeringVector.z = 0;
            }

            //若合力为0，物体停止运动（用来实现物体停止，但需立即重新计算操控力，防止真的出现不合理的停顿）
            if (SteeringVector == Vector3.zero)
            {
                CurrentSpeed = Vector3.zero;
            }
        }

        public void SetEnable(bool value)
        {
            _run = value;
            //计算频率不能低于速度增幅，否则再下一次计算时会超过预期速度，导致物体来回震动
            if (_run)
            {
                if (_caculateFrequency * _acceleration > 1)
                {
                    _caculateFrequency = 1f / _acceleration;
                }
                Steerings = GetSteerings();
            }
        }

        private SteeringBase[] GetSteerings()
        {
            throw new System.NotImplementedException();
        }
    }
}