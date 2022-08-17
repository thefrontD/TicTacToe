using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardIntention : MonoBehaviour
{
    [SerializeField] private float moveTime;
    [SerializeField] private float tweenDelay;
    [SerializeField] private int maxHeight;
    [SerializeField] private int minHeight;
    [SerializeField] private BoardSituationSpriteDictionary spriteDictionary;
    private Sequence _sequence;
    
    void Start()
    {
        _sequence = DOTween.Sequence();

        _sequence
            .Append(transform.DOLocalMoveZ(maxHeight, moveTime, false).SetEase(Ease.InOutQuad))
            .AppendInterval(tweenDelay)
            .Append(transform.DOLocalMoveZ(minHeight, moveTime, false).SetEase(Ease.InOutQuad))
            .AppendInterval(tweenDelay)
            .SetLoops(-1, LoopType.Restart);
        
        _sequence.Play();
    }

    public void SetSprite(BoardSituation boardSituation)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteDictionary[boardSituation];
    }

    private void OnDisable()
    {
        _sequence.Pause();
    }

    void OnEnable()
    { 
        _sequence.Play();
    }
}
