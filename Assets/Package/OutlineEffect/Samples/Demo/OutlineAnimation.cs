using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;

namespace QuickOutline
{
    public class OutlineAnimation : MonoBehaviour
    {
        [SerializeField] private List<int> flashingColorIndex;
        [SerializeField, Range(0, 2f)] private float flashingSpeed = 0.5f;
        [SerializeField, Range(0, 1f)] private float minAlpha = 0f;
        [SerializeField, Range(0, 1f)] private float maxAlpha = 1f;
        bool pingPong = false;
        private float _delta;
        private List<Color> flashingColor;

        // Use this for initialization
        void Start()
        {
            if(maxAlpha < minAlpha) maxAlpha = minAlpha;

            _delta = 0;
        }

        void Update()
        {
            if(pingPong)
            {
                _delta += Time.deltaTime * flashingSpeed;
                foreach (int idx in flashingColorIndex)
                    GetComponent<OutlineEffect>().SetAlpha(idx, Mathf.Clamp01(_delta));

                if(_delta >= maxAlpha)
                    pingPong = false;
            }
            else
            {
                _delta -= Time.deltaTime * flashingSpeed;
                foreach (int idx in flashingColorIndex)
                    GetComponent<OutlineEffect>().SetAlpha(idx, Mathf.Clamp01(_delta));

                if(_delta <= minAlpha)
                    pingPong = true;
            }

            GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();
        }
    }
}