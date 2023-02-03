using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMapBoard : MonoBehaviour
{
    [SerializeField] private int _col;
    [SerializeField] private int _row;
    [SerializeField] private Sprite _clearImage;
    [SerializeField] private SpriteRenderer _clearSprite;

    public int Col => _col;
    public int Row => _row;

    void Start()
    {
        if(GameManager.Instance.CurrentCol == _col && GameManager.Instance.CurrentRow == _row){
            SetClear();
        }
    }

    void Update()
    {
        
    }

    public void init(int col, int row) {
        _col = col;
        _row = row;
    }

    public void SetClear() {
        _clearSprite.sprite  = _clearImage;
    }

    private void OnMouseDown() {
        WorldMapManager.Instance.MovePlayer(_col, _row);
    }
}
