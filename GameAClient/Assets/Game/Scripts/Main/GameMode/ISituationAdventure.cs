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
        public Record Record;
        public Game.Table_StandaloneLevel Table {
            get {
                return AppData.Instance.AdventureData.GetAdvLevelTable(Section, ProjectType, Level);
            }
        }
    }
}

