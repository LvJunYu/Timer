using SoyEngine.Proto;

namespace GameA
{
    public partial class AdventureSection
    {
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            for (int i = 0; i < _normalProjectList.Count; i++)
            {
                var p = _normalProjectList[i];
                p.AdventureProjectType = EAdventureProjectType.APT_Normal;
                p.SectionId = _section;
                p.LevelId = i + 1;
            }
            for (int i = 0; i < _bonusProjectList.Count; i++)
            {
                var p = _bonusProjectList[i];
                p.AdventureProjectType = EAdventureProjectType.APT_Bonus;
                p.SectionId = _section;
                p.LevelId = i + 1;
            }
        }
    }
}