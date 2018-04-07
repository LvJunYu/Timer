  /********************************************************************
  ** Filename : UMViewPersonalProjectCard.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMViewPersonalProjectCard.cs
  ***********************************************************************/


using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UMViewPublishedProjectCard : UMViewResManagedBase
    {
        public Texture DefaultCoverTexture;
        public RawImage Cover;
        public Text Title;
        public Text PublishTime;
        public Text PlayerCnt;
        public Text PassRate;
        public Text LikedCnt;
        public Text CommentCnt;
        public Image UnsetectMark;
        public Image SeletedMark;
        public GameObject InfoDock;
        public GameObject EmptyDock;
        public Button CardBtn;
    }
}
