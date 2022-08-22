using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleTweening : MonoBehaviour
{
    [SerializeField] private float maxScale;
    [SerializeField] private float tweenTime; 
    [SerializeField] private float tweenDelay;
    private Sequence _sequence;


    void Start()
    {
       _sequence = DOTween.Sequence();

       _sequence
            .Append(transform.DOScale(Vector3.one * maxScale, tweenTime))
            .AppendInterval(tweenDelay)
            .Append(transform.DOScale(Vector3.one, tweenTime))
            .AppendInterval(tweenDelay)
            .SetLoops(-1, LoopType.Restart);
        
        _sequence.Play();
    }
}
