using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class USViewChapter : USViewBase
    {
		public RectTransform[] NormalLevelPos;
		public RectTransform[] BonusLevelPos;
		public RectTransform LevelRoot;
		public Image[] BonusLevelBlockImages1;
	    public Image[] BonusLevelBlockImages2;
	    public Image[] BonusLevelBlockImages3;
	    public GameObject[] BonusLevelBlockGroup;
		public GameObject BackgroundImage;
		public GameObject ForgroundImage;
		public GameObject NormalLevelPrefab;
		public GameObject BonusLevelPrefab;
    }
}
