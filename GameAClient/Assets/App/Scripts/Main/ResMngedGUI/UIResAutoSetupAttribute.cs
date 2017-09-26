using System;
using SoyEngine;

namespace GameA
{
    /// <summary>
    ///     UI自动创建
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UIResAutoSetupAttribute : UIAutoSetupAttribute
    {
        #region 变量
        private readonly EResScenary _resScenary;
        #endregion

        #region 访问器
        public EResScenary ResScenary
        {
            get { return _resScenary; }
        }
        #endregion

        public UIResAutoSetupAttribute(EResScenary resScenary = EResScenary.Default,
            EUIAutoSetupType autoSetupType = EUIAutoSetupType.Add) : base(autoSetupType)
        {
            _resScenary = resScenary;
        }
    }
}