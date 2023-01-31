using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    private int idx;
    public int Idx { get => idx; set => idx = (value >= 0) ? value : 0; }
    
    public Card Card;
    [SerializeField] private float animationDuration;
    [SerializeField] private Vector3 MouseOnPos;
    [SerializeField] private float MouseOnScale;
    private Vector3 originPos;
    private Vector3 originRot;
    private Vector3 originScale;
    [SerializeField] private SpriteRenderer CardBackground;
    [SerializeField] private SpriteRenderer CardManaImage;
    [SerializeField] private SpriteRenderer CardImage;
    [SerializeField] private TextMeshPro CardCostText;
    [SerializeField] private TextMeshPro CardNameText;
    [SerializeField] private TextMeshPro CardDescText;
    //[SerializeField] private TextMeshPro CardEffectExplanation;
    [SerializeField] private ParticleSystem HightLightParticle;
    [SerializeField] private Sprite CardBackImage;
    private bool isDrag = false;
    private int _originIdx = 0;
    public bool isHand = false;

    void Start()
    {
        originScale = transform.localScale;
        setPos();
    }

    void Update()
    {
        if (isDrag)
        {
            transform.position = Utils.CardMousePos;
        }
    }

    public void init(Card card)
    {
        this.Card = card;
        
        CardBackground.sprite = Resources.Load<Sprite>($"Images/Cards/{card.CardType}/BackGround");
        string cardPathName = card.CardName.Replace(':', '-').Replace('/', '_');
        print(cardPathName);
        CardImage.sprite = Resources.Load<Sprite>($"Images/Cards/{card.CardType}/{cardPathName}");

        SetBackOrder(0);

        CardNameText.text = Card.CardName;
        CardDescText.text = Card.CardDesc;
        CardCostText.text = Card.CardCost.ToString();
    }

    private void OnMouseEnter()
    {
        if(isHand && !isDrag && !CardManager.Instance.isDrag)
        {
            DOTween.Kill(this.transform);
            
            transform.DOMove(originPos + MouseOnPos, animationDuration).SetEase(Ease.OutQuart);
            transform.DOScale(originScale * MouseOnScale, animationDuration).SetEase(Ease.OutQuart);
            transform.DORotate(new Vector3(0, 0, 0), animationDuration).SetEase(Ease.OutQuart);
            CardManager.Instance.mouseEnterAnimation(Idx);
            SetSortingOrder(50);
        }
    }
    
    private void OnMouseExit()
    {
        if(isHand && !isDrag && !CardManager.Instance.isDrag)
        {
            transform.DOMove(originPos, animationDuration).SetEase(Ease.OutQuart);
            transform.DORotate(originRot, animationDuration).SetEase(Ease.OutQuart);
            transform.DOScale(originScale, animationDuration).SetEase(Ease.OutQuart);
            CardManager.Instance.mouseExitAnimation(Idx);
            SetSortingOrder(_originIdx);
        }
    }
    
    void OnMouseDown()
    {
        if(PlayerManager.Instance.state.GetType() == typeof(NormalState) && isHand && PlayerManager.Instance.Clickable)
        {
            isDrag = true;
            CardManager.Instance.isDrag = true;
            transform.DOScale(originScale, animationDuration).SetEase(Ease.OutQuart);
        }
    }
    void OnMouseUp()
    {
        LayerMask mask = LayerMask.GetMask("CardUsable");
        if(isDrag && Physics2D.Raycast(Utils.MousePos, Vector3.forward, 50, mask))
        {
            if(Card.usingCard())
            {
                isDrag = false;
                CardManager.Instance.isDrag = false;
                SoundManager.Instance.PlaySE("UsingCard");
                HightLightCard(false);
                CardManager.Instance.HandtoGrave(idx);
            }
            else if(PlayerManager.Instance.TutorialPhase == 16)
            {
                isDrag = false;
                CardManager.Instance.isDrag = false;
                transform.DOMove(originPos, animationDuration).SetEase(Ease.OutQuart);
                transform.DORotate(originRot, animationDuration).SetEase(Ease.OutQuart);
                TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
            }
            else
            {
                isDrag = false;
                CardManager.Instance.isDrag = false;
                transform.DOMove(originPos, animationDuration).SetEase(Ease.OutQuart);
                transform.DORotate(originRot, animationDuration).SetEase(Ease.OutQuart);
            }
        }
        else
        {
            isDrag = false;
            CardManager.Instance.isDrag = false;
            transform.DOMove(originPos, animationDuration).SetEase(Ease.OutQuart);
            transform.DORotate(originRot, animationDuration).SetEase(Ease.OutQuart);
        }
    }

    public void setPos()
    {
        originScale = Utils.cardScaleOnHand;
        originPos = transform.position;
        originRot = transform.rotation.eulerAngles;
    }
    
    public void setPos(Vector3 pos, Vector3 rot, int idx)
    {
        originPos = pos;
        originRot = rot;
        SetSortingOrder(idx);
        _originIdx = idx;
    }

    public void SetSortingOrder(int idx)
    {
        CardBackground.sprite = Resources.Load<Sprite>($"Images/Cards/"+ Card.CardType.ToString() +"/BackGround");
        CardManaImage.gameObject.SetActive(true);
        CardBackground.sortingOrder = idx;
        CardManaImage.sortingOrder = idx+1;
        CardImage.sortingOrder = idx+1;
        CardCostText.sortingOrder = idx+1;
        CardNameText.sortingOrder = idx+1;
        CardDescText.sortingOrder = idx+1;
    }
    public void SetBackOrder(int idx){
        CardManaImage.gameObject.SetActive(false);
        CardBackground.sortingOrder = idx+1;
        CardManaImage.sortingOrder = idx;
        CardBackground.sprite = CardBackImage;
        CardImage.sortingOrder = idx;
        CardCostText.sortingOrder = idx;
        CardNameText.sortingOrder = idx;
        CardDescText.sortingOrder = idx;
    }
    
    public void ToGrave()
    {
        SetBackOrder(0);
        _originIdx = idx;
    }

    public void HightLightCard(bool isOn)
    {
        if(isOn)
        {
            HightLightParticle.gameObject.SetActive(true);
            HightLightParticle.Play();
        }
        else
        {
            HightLightParticle.Clear();
            HightLightParticle.gameObject.SetActive(false);
        }
    }

    /*
    private void OnMouseDrag()
    {
        if(isHand)
        {
            Vector3 trackPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(trackPosition.x, trackPosition.y, 0.5f);
        }
    }
    
    void OnMouseUp()
    {
        if(isHand)
        {
            isDrag = false;
            transform.DOMove(originPos, animationDuration);
            transform.DOScale(originScale, animationDuration);
            this.Card.usingCard();
        }
    }*/
}