using UnityEngine;
using System.Collections;
using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public interface ISituationAdventure
    {
        SituationAdventureParam GetLevelInfo();
    }

    public class SituationAdventureParam
    {
        public EAdventureProjectType ProjectType;
        public int Section;
        public int Level;
        public Game.Table_StandaloneLevel Table {
            get {
                var tableChapter = Game.TableManager.Instance.GetStandaloneChapter (Section);
                int levelId = 0;
                if (ProjectType == EAdventureProjectType.APT_Bonus) {
                    if ((Level - 1) < tableChapter.BonusLevels.Length) {
                        levelId = tableChapter.BonusLevels [Level - 1];
                    } else {
                        LogHelper.Error ("Find {0}'s bonus level of chapter {1} failed.", Level, Section);
                        return null;
                    }
                } else {
                    if ((Level - 1) < tableChapter.NormalLevels.Length) {
                        levelId = tableChapter.NormalLevels [Level - 1];
                    } else {
                        LogHelper.Error ("Find {0}'s normal level of chapter {1} failed.", Level, Section);
                        return null;
                    }
                }
                return Game.TableManager.Instance.GetStandaloneLevel (levelId);
            }
        }
    }
}

