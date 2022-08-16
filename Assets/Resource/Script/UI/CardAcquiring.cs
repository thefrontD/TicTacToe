using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAcquiring : MonoBehaviour, IPointerClickHandler
{
    Card card;
    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Card Selected");
        Debug.Log(card);
        PlayerManager.Instance.PlayerCard.Add(card);
        transform.parent.GetComponent<CardAcquiringPanel>().EndCardAcquiring();
    }

    public void SetCard(Card card){
        this.card = card;
    }

    void Update()
    {
        
    }


}
