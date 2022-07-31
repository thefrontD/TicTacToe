using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Sprite NoneSprite;
    [SerializeField] private Sprite EnemySprite;
    [SerializeField] private Sprite PlayerSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
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
}
