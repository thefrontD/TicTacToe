using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewWorldMapManager : MonoBehaviour
{
    [SerializeField] int _tileSize = 3;
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private GameObject WallPrefab;
    [SerializeField] private GameObject BushPrefab;
    [SerializeField] private GameObject TileLocation;
    [SerializeField] private List<List<GameObject>> TileObjects = new List<List<GameObject>>();
    [SerializeField] private int PlayerX = 1; // 0, 1, 2
    [SerializeField] private int PlayerY = 1; // 0, 1, 2

    private List<List<Tile>> _tiles = new List<List<Tile>>();
    public List<List<Tile>> Tiles => _tiles;

    private List<List<Border>> _borders = new List<List<Border>>();
    public List<List<Border>> Borders => _borders;

    private List<List<Border>> _horizontalBorders = new List<List<Border>>();
    public List<List<Border>> HorizontalBorders => _horizontalBorders;

    private List<List<Border>> _verticalBorders = new List<List<Border>>();
    public List<List<Border>> VerticalBorders => _verticalBorders;



    // Start is called before the first frame update
    void Start()
    {
        TileLoading();
        BorderLoading();
        ShowTilesAroundPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TileLoading()
    {
        for(int i = 0; i<_tileSize; i++)
        {
            _tiles.Add(new List<Tile>());
            TileObjects.Add(new List<GameObject>());
            for(int j = 0; j<_tileSize; j++)
            {
                GameObject newTile = Instantiate(TilePrefab, new Vector3(-1f + i, 0, -1f + j), Utils.QS);
                TileObjects[i].Add(newTile);
                newTile.SetActive(false);
                _tiles[i].Add(newTile.GetComponent<Tile>());
                //_tiles[i][j].SetType(1);//monster TODO: get info from map data
            }
        }
    }

    public void BorderLoading()
    {
        for(int i = 0; i<2*_tileSize-1; i++)
        {
            Borders.Add(new List<Border>());
            for (int j = 0; j < 2 * _tileSize - 1; j++)
            {
                Borders[i].Add(Border.None);
                if ((i + j) % 2 == 1)
                {
                    GameObject newBorder = Instantiate(WallPrefab, new Vector3(-1f + 0.5f * j, 0, -1f + 0.5f * i), Utils.QS);
                }
            }
        }
    }

    public void ShowTilesAroundPlayer()
    {
        TileObjects[PlayerX][PlayerY].SetActive(true);
        if (PlayerX > 0 && Borders[PlayerX * 2 - 1][PlayerY * 2] == Border.None)
            TileObjects[PlayerX - 1][PlayerY].SetActive(true);
        if (PlayerX < _tileSize - 1 && Borders[PlayerX * 2 + 1][PlayerY * 2] == Border.None)
            TileObjects[PlayerX + 1][PlayerY].SetActive(true);
        if (PlayerY > 0 && Borders[PlayerX * 2][PlayerY * 2 - 1] == Border.None)
            TileObjects[PlayerX][PlayerY - 1].SetActive(true);
        if (PlayerY < _tileSize - 1 && Borders[PlayerX * 2][PlayerY * 2 + 1] == Border.None)
            TileObjects[PlayerX][PlayerY + 1].SetActive(true);
    }

    public void MovePlayer()
    {
        
    }
}
