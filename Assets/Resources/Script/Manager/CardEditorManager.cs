using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEditorManager : MonoBehaviour
{
    [SerializeField] private GameObject TotalCardContent;
    [SerializeField] private GameObject PlayerCardContent;
    [SerializeField] private GameObject PuzzleCardContent;

    [SerializeField] private GameObject TotalPanelMemberPrefab;
    [SerializeField] private GameObject PlayerPanelMemberPrefab;
    [SerializeField] private GameObject PuzzlePanelMemberPrefab;

    private List<Card> TotalCardList = new List<Card>();
    private List<Card> PlayerCardList = new List<Card>();
    private List<Card> PuzzleCardList = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        TotalCardList = CardData.Instance._load("TotalCardData");
        PlayerCardList = CardData.Instance._load("PlayerCard");
        //PuzzleCardList = CardData.Instance._load("PuzzleCardList");//TODO
        for(int i = 0; i< TotalCardList.Count; i++)
        {
            GameObject newTotalPanelMember = Instantiate(TotalPanelMemberPrefab);
            newTotalPanelMember.transform.SetParent(TotalCardContent.transform);

        }
    }

    void UpdateCardList()
    {
        //TODO
        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
