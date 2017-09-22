using SoyEngine;

namespace GameA
{
    public abstract class UICtrlResManagedBase<T> : UICtrlGenericBase<T> where T : UIViewBase
    {
        public EResScenary ResScenary { get; private set; }

        protected override void OnAwake()
        {
            var autoSetup = GUIManager.GetUIAutoSetupAttribute(this.GetType());
            if (autoSetup == null)
            {
                ResScenary = EResScenary.Default;
            }
            else
            {
                var resAutoSetup = autoSetup as UIResAutoSetupAttribute;
                if (resAutoSetup == null)
                {
                    ResScenary = EResScenary.Default;
                }
                else
                {
                    ResScenary = resAutoSetup.ResScenary;
                }
            }
        }
    }
}