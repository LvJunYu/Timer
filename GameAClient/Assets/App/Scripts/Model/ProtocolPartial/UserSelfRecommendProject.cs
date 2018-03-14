//  | 用户自荐关卡列表

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserSelfRecommendProject
    {
        #region 字段

        public EUserSelfRecommendType Type;

        #endregion
    }

    public enum EUserSelfRecommendType
    {
        HaveProject,
        NoProject,
        UnLock
    }
}