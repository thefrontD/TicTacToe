using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;

namespace QuickOutline
{
    public class OutlineAnimation : MonoBehaviour
    {
        [SerializeField, Range(0, 2f)] private float flashingSpeed = 0.5f;
        [SerializeField, Range(0, 1f)] private float minAlpha = 0f;
        [SerializeField, Range(0, 1f)] private float maxAlpha = 1f;
        bool pingPong = false;
        private float _delta;

        // Use this for initialization
        void Start()
        {
            if(maxAlpha < minAlpha) maxAlpha = minAlpha;
            
            _delta = 0;
        }

        // Update is called once per frame
        void Update()
        {
            Color c0 = GetComponent<OutlineEffect>().lineColor0;
            Color c1 = GetComponent<OutlineEffect>().lineColor1;
            Color c2 = GetComponent<OutlineEffect>().lineColor2;

            if(pingPong)
            {
                _delta += Time.deltaTime * flashingSpeed;
                c0.a = c1.a = c2.a = _delta;

                if(_delta >= maxAlpha)
                    pingPong = false;
            }
            else
            {
                _delta -= Time.deltaTime * flashingSpeed;
                c0.a = c1.a = c2.a = _delta;

                if(_delta <= minAlpha)
                    pingPong = true;
            }

            c0.a = Mathf.Clamp01(c0.a);
            c1.a = Mathf.Clamp01(c1.a);
            c2.a = Mathf.Clamp01(c2.a);
            GetComponent<OutlineEffect>().lineColor0 = c0;
            GetComponent<OutlineEffect>().lineColor1 = c1;
            GetComponent<OutlineEffect>().lineColor2 = c2;
            GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();
        }
    }
}