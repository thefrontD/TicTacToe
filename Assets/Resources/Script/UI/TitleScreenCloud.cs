using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleScreenCloud : MonoBehaviour
{
    [SerializeField] private float startDelay;
    [SerializeField] private float moveTime;
    [SerializeField] private int endPoint;
    private Sequence _cloudSequence;
    private float _time;
    private bool _isrun = false;
    private Vector3 originPos;

    void Start()
    {
        _cloudSequence = DOTween.Sequence();
        originPos = transform.position;

        _cloudSequence
            .Append(transform.DOLocalMoveX(endPoint, moveTime, false))
            .SetLoops(-1, LoopType.Restart);
    }

    void Update()
    {
        if(_time < startDelay)
            _time += Time.deltaTime;
        else if(!_isrun)
        {
            _cloudSequence.Play();
            _isrun = true;
        }
    }
}
