using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private SituationColorDictionary colorDictionary;
    [SerializeField] private Sprite NoneSprite;
    [SerializeField] private Sprite EnemySprite;
    [SerializeField] private Sprite PlayerSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoardIntention boardIntention;
    [SerializeField] private List<Color> colors;
    [SerializeField] private ParticleSystem bingoEffect;
    [SerializeField] private ParticleSystem bingoEnemyEffect;
    [SerializeField] private ParticleSystem colorEffect;
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
        SetBoardColor(currentBoardColor);
        SetHighlight(BoardSituation.None);
    }

    public void SetBoardColor(BoardColor boardColor)
    {
        currentBoardColor = boardColor;
        switch (boardColor)
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

        BoardManager.Instance.BoardColors[Row][Col] = boardColor;
    }

    public void SetHighlight(BoardSituation situation)
    {
        if(situation == BoardSituation.None)
        {
            GetComponent<Outlinable>().enabled = false;
            boardIntention.gameObject.SetActive(false);
        }
        else
        {
            boardIntention.gameObject.SetActive(true);
            boardIntention.SetSprite(situation);
            GetComponent<Outlinable>().enabled = true;
            GetComponent<Outlinable>().OutlineParameters.Color = colors[(int) situation];
        }
    }

    public void ActivateBingoEffect(bool select, BoardColor color = BoardColor.None){
        if(select == true){
            //Debug.Log("ActivateBingoEffect true Color: " + color.ToString());
            if(color == BoardColor.Player)
                bingoEffect.gameObject.SetActive(true);
            else
                bingoEnemyEffect.gameObject.SetActive(true);

        }
        else{
            //Debug.Log("ActivateBingoEffect false");
            bingoEffect.gameObject.SetActive(false);
            bingoEnemyEffect.gameObject.SetActive(false);
        }
    }

    public void ActivateColorEffect()
    {
        colorEffect.gameObject.SetActive(true);
    }
}
