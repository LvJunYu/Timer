using UnityEngine;
using System.Collections;
using System;
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
    }
}

