using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;

public class Board : MonoBehaviour
{
    [SerializeField] private SituationColorDictionary colorDictionary;
    [SerializeField] private Sprite NoneSprite;
    [SerializeField] private Sprite EnemySprite;
    [SerializeField] private Sprite PlayerSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Outline outline;
    private int _row;
    private int _col;

    public int Row => _row;
    public int Col => _col;
    public BoardColor currentBoardColor;
    public BoardObject currentBoardObject;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(BoardColor boardColor, int row, int col)
    {
        _row = row;
        _col = col;
        currentBoardColor = boardColor;
        SetBoardColor(currentBoardColor);
        SetHighlight(BoardSituation.None);
    }

    public void SetBoardColor(BoardColor boardColor)
    {
        switch(boardColor)
        {
            case BoardColor.None:
                spriteRenderer.sprite = NoneSprite;
            break;
            case BoardColor.Enemy:
                spriteRenderer.sprite = EnemySprite;
            break;
            case BoardColor.Player:
                spriteRenderer.sprite = PlayerSprite;
            break;
        }
    }

    public void SetHighlight(BoardSituation situation)
    {
        if(situation == BoardSituation.None)
            outline.enabled = false;
        else
        {
            outline.enabled = true;
            outline.color = (int)situation;
        }
    }
}
