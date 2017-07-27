using System.Collections;
using System;
using SoyEngine;
using SoyEngine.Proto;
using System.Collections.Generic;
using UnityEngine;

namespace GameA.Game
{

    public class StatusBarPre:MonoBehaviour
    {
        public Sprite Blood;
        public Sprite Energy;


        public float speed = 5.0f;

        public float bloodnow;     
        public float bloodbefore;
        public float bloodmax;

        public float Energynow;
        public float Energybefore;
        public float Energymax;

        void Start()
        {
            bloodbefore = bloodmax;
        }

        void Update()
        {
            if (bloodbefore != bloodnow)
                bloodbefore = bloodnow;
            StartCoroutine(Changeblood(bloodnow / bloodmax));
        }

        IEnumerator Changeblood(float px)
        {
            float timeSum = 0.0f;
            if (px < 0)
                px = 0;
            else if (px > 1)
                px = 1;

            if (this.transform.localScale.x >= px + 0.005 || this.transform.localScale.x <= px - 0.005)
            {
                yield return null;
                this.transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(px, this.transform.localScale.y, this.transform.localScale.z), speed * Time.deltaTime / 3f);
            }
        }

    }
}
